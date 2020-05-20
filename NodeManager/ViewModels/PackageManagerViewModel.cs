using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using NodeManager.Helpers;
using NodeManager.Models;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using WK.Libraries.BetterFolderBrowserNS;

namespace NodeManager.ViewModels
{
    public class PackageManagerViewModel : BindableBase
    {
        private DataTable _listNpmPackages;

        public DataTable ListNpmPackages
        {
            get => _listNpmPackages;
            set => SetProperty(ref _listNpmPackages, value);
        }

        public DelegateCommand LoadJsonCommand { get; set; }

        public PackageManagerViewModel()
        {
            LoadJsonCommand = new DelegateCommand(async () => await LoadJson());

            ListNpmPackages = new DataTable();
            ListNpmPackages.Columns.Add("PackageName");
            ListNpmPackages.Columns.Add("NodeVersion");
        }

        private async Task LoadJson()
        {
            Log.Information("Preparing open Dialog");
            var openDir = new BetterFolderBrowser
            {
                RootFolder = Environment.CurrentDirectory
            };

            Log.Information("Opening dialog");
            if (openDir.ShowDialog() != DialogResult.OK) return;

            await Task.Run(() =>
            {
                var selectedDir = openDir.SelectedPath;
                Log.Information($"SelectedDir: {selectedDir}");

                var packageJson = $"{selectedDir}/package.json";
                if (packageJson.IsFileExist())
                {
                    Log.Information("Reading package.json");
                    var jsonFile = File.ReadAllText(packageJson);
                    var jsonModel = PackageJson.FromJson(jsonFile);

                    Log.Information("Loading to View");
                    ListNpmPackages.Clear();

                    var deps = jsonModel.Dependencies;
                    var mapDeps = JsonHelper.DynamicToDictionary(deps);
                    foreach (var mapDep in mapDeps)
                    {
                        string packageName = mapDep.Key.ToString();
                        string packageVersion = mapDep.Value.ToString();
                        // var search = await packageName.Substring(1).GetPackage(packageVersion.Substring(1));

                        // Log.Information($"Search: {search}");

                        ListNpmPackages.Rows.Add(packageName, packageVersion);
                    }

                    var devDeps = jsonModel.DevDependencies;
                    var mapDevDeps = JsonHelper.DynamicToDictionary(devDeps);
                    foreach (var mapDep in mapDevDeps)
                    {
                        ListNpmPackages.Rows.Add(mapDep.Key, mapDep.Value);
                    }

                    Log.Information($"Loaded {ListNpmPackages.Rows.Count} packages");
                    Log.Information($"Load package.json finish");
                }
                else
                {
                    Log.Information($"package.json in {selectedDir} is not exist.");
                }
            });
        }
    }
}