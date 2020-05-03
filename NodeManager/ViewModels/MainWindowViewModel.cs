using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using myoddweb.directorywatcher;
using myoddweb.directorywatcher.interfaces;
using NodeManager.Helpers;
using NodeManager.Views;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeManager.Models;

namespace NodeManager.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Node Version Manager";
        private List<string> listLogs;
        private int _selectedLine;
        private HamburgerMenuItemCollection _hamburgerMenuItemCollection;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public HamburgerMenuItemCollection HamburgerMenuItemCollection
        {
            get => _hamburgerMenuItemCollection;
            set => SetProperty(ref _hamburgerMenuItemCollection, value);
        }

        public int SelectedLine
        {
            get => _selectedLine;
            set => SetProperty(ref _selectedLine, value);
        }

        public List<string> ListLogs
        {
            get => listLogs;
            set => SetProperty(ref listLogs, value);
        }

        //public MainWindowViewModel()
        //{
        //}

        public MainWindowViewModel(IRegionManager regionManager)
        {
            BuildMenuItems();

            if (EnvHelper.IsAdministrator())
            {
                Title += " (Administrator)";
            }

            PrismHelper.RegionManager = regionManager;

            Parallel.Invoke(async () => await LoadLogsAsync());

            LogsWatcher();
        }

        private void BuildMenuItems()
        {
            HamburgerMenuItemCollection = new HamburgerMenuItemCollection()
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterialLight(){Kind = PackIconMaterialLightKind.Download},
                    Label = "Version Manager",
                    ToolTip = "Version Manager",
                    Tag = new VersionManager()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterialLight(){Kind = PackIconMaterialLightKind.Repeat},
                    Label = "Package Manager",
                    ToolTip = "Package Manager",
                    Tag = new PackageManager()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconFeatherIcons(){Kind =  PackIconFeatherIconsKind.Settings},
                    Label = "Settings",
                    ToolTip = "Settings",
                    Tag = new Settings()
                }
            };
        }

        private void LogsWatcher()
        {
            // create Watcher
            var watch = new Watcher();

            // Add a request.
            watch.Add(new Request(@"Storage\Logs", true));

            watch.OnTouchedAsync += Watch_OnTouchedAsync; ;

            // start watching
            watch.Start();
        }

        private async Task Watch_OnTouchedAsync(IFileSystemEvent e, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                await LoadLogsAsync();
            });
        }

        private async Task LoadLogsAsync()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var logPath = Path.Combine(AppConfig.LogsPath, $"App-{date}.log");

            ListLogs = new List<string>();
            using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                var logs = await sr.ReadToEndAsync();
                ListLogs.AddRange(logs.Lines().Reverse());
            }

            ListLogs = ListLogs.Where(x => x.Contains("[INF]")).ToList();

            SelectedLine = 0;
        }
    }
}