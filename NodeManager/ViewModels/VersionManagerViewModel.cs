using Flurl.Http;
using NodeManager.Helpers;
using NodeManager.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace NodeManager.ViewModels
{
    public class VersionManagerViewModel : BindableBase
    {
        private string _installCaption;
        private string _refreshCaption;
        private string _searchBox;
        private bool _isIdle;
        private bool _isEnableNodeJs;
        private bool _isOpenDownloadPane;
        private DataTable _listNodeVersionVersions;
        private NodeJsRow _selectedNodeJs;
        private ObservableCollection<NodeJsRow> _nodeJsDataCollection;
        private ObservableCollection<NodeJsRow> _nodeJsDataFilter;
        private IRegionManager RegionManager;

        public string InstallCaption
        {
            get => _installCaption;
            set => SetProperty(ref _installCaption, value);
        }

        public string RefreshCaption
        {
            get => _refreshCaption;
            set => SetProperty(ref _refreshCaption, value);
        }

        public string SearchBox
        {
            get => _searchBox;
            set
            {
                SetProperty(ref _searchBox, value);
                OnSearchBoxChanged();
            }
        }

        public bool IsIdle
        {
            get => _isIdle;
            set => SetProperty(ref _isIdle, value);
        }

        public bool IsEnableNodeJs
        {
            get => _isEnableNodeJs;
            set
            {
                SetProperty(ref _isEnableNodeJs, value);
                EnsureEnvironmentVar(value);
            }
        }

        public bool IsOpenDownloadPane
        {
            get => _isOpenDownloadPane;
            set => SetProperty(ref _isOpenDownloadPane, value);
        }

        public DataTable ListNodeVersions
        {
            get => _listNodeVersionVersions;
            set => SetProperty(ref _listNodeVersionVersions, value);
        }

        public NodeJsRow SelectedNodeJs
        {
            get => _selectedNodeJs;
            set
            {
                SetProperty(ref _selectedNodeJs, value);
                OnSelectionChanged();
            }
        }

        public ObservableCollection<NodeJsRow> NodeJsCollection
        {
            get => _nodeJsDataCollection;
            set => SetProperty(ref _nodeJsDataCollection, value);
        }

        public ObservableCollection<NodeJsRow> NodeJsFilter
        {
            get => _nodeJsDataFilter;
            set => SetProperty(ref _nodeJsDataFilter, value);
        }

        public DelegateCommand RefreshVersionCommand { get; set; }
        public DelegateCommand InstallSelectedCommand { get; set; }
        public DelegateCommand CancelInstallCommand { get; set; }

        public VersionManagerViewModel()
        {
            RefreshVersionCommand = new DelegateCommand(async () => await LoadVersionAsync());
            InstallSelectedCommand = new DelegateCommand(async () => await InstallSelectedAsync());
            CancelInstallCommand = new DelegateCommand(CancelInstall);

            ListNodeVersions = new DataTable();
            ListNodeVersions.Columns.Add("NodeVersion");
            ListNodeVersions.Columns.Add("NpmVersion");
            ListNodeVersions.Columns.Add("IsLts");
            ListNodeVersions.Columns.Add("DistUrl");
            ListNodeVersions.Columns.Add("IsInstalled");

            InstallCaption = "Install";
            RefreshCaption = "Refresh";
            SearchBox = "";

            IsEnableNodeJs = AppConfig.IsPathEnvSet;

            NodeJsCollection = new ObservableCollection<NodeJsRow>();

            // Parallel.Invoke(async () => await LoadVersionAsync());
            Task.WhenAll(LoadVersionAsync());

            // EnsureEnvironmentVar();
        }

        private async Task LoadVersionAsync()
        {
            if (EnvHelper.InDesignMode)
                return;

            try
            {
                RefreshCaption = "Refreshing..";
                IsIdle = false;

                var installDir = AppConfig.NodePath;
                var fileStamp = DateTime.Now.ToString("yyyy-MM-dd");
                var localJson = Path.Combine(AppConfig.CachesPath, $"node-registry_{fileStamp}.json");

                var url = "https://nodejs.org/dist/index.json";
                Log.Information($"Loading NodeJs registry");

                if (!localJson.IsFileExist())
                {
                    Log.Information("Updating registry Cache");
                    await url.WithTimeout(5)
                        .DownloadFileAsync(Path.GetDirectoryName(localJson), Path.GetFileName(localJson));
                }

                var json = File.ReadAllText(localJson);
                var nodeModels = NodeJs.FromJson(json);

                Log.Information("Preparing to View");
                ListNodeVersions.Clear();
                foreach (NodeJs nodeJs in nodeModels)
                {
                    var nodeVersion = nodeJs.NodeVersion;
                    var npmVersion = nodeJs.NpmVersion;
                    var isLts = nodeJs.Lts.Bool ?? true;
                    var ltsName = nodeJs.Lts.Enum.ToString() ?? "Non-LTS";
                    var distUrl = $"https://nodejs.org/dist/{nodeJs.NodeVersion}/node-{nodeJs.NodeVersion}-win-x64.zip";

                    var row = ListNodeVersions.NewRow();
                    row["NodeVersion"] = nodeJs.NodeVersion;
                    row["NpmVersion"] = nodeJs.NpmVersion;
                    row["IsLts"] = nodeJs.Lts.Bool ?? true;
                    row["DistUrl"] = distUrl;

                    var dirs = Directory.GetDirectories(installDir);
                    var filteredDir = dirs.Where(str => str.Contains(nodeVersion));
                    var isInstalled = filteredDir.Any();

                    ListNodeVersions.Rows.Add(row);

                    NodeJsCollection.Add(new NodeJsRow()
                    {
                        NodeVersion = nodeVersion,
                        NpmVersion = npmVersion,
                        ReleaseDate = nodeJs.Date.ToString("yyyy-MM-dd"),
                        IsLts = isLts,
                        IsInstalled = isInstalled,
                        LtsName = ltsName,
                        DistUrl = distUrl
                    });

                    NodeJsFilter = NodeJsCollection;
                }

                Log.Information($"NodeJs loaded {ListNodeVersions.Rows.Count} items");
            }
            catch (Exception ex)
            {
                const string errorMsg = "Error when loading from Registry. Please check your internet connection.";
                var dialogResult = DialogHelper.ErrorDialog(errorMsg + " Are you want to refresh?",
                    messageBoxButton: MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes) await LoadVersionAsync();

                Log.Error(ex, "Error fetch from Registry");
                Log.Information(errorMsg);
            }

            RefreshCaption = "Refresh";
            IsIdle = true;
        }

        private async Task InstallSelectedAsync()
        {
            if (SelectedNodeJs == null)
            {
                DialogHelper.WarnDialog("Please select NodeJS below");
                return;
            }

            var version = SelectedNodeJs.NodeVersion;
            var distUrl = SelectedNodeJs.DistUrl;
            var isInstalled = SelectedNodeJs.IsInstalled;
            var fileName = Path.GetFileName(distUrl);
            var localFile = Path.Combine(AppConfig.TempPath, fileName);
            var installDir = AppConfig.NodePath;
            var dirs = Directory.GetDirectories(installDir);
            var installNode = dirs.FirstOrDefault(str => str.Contains(version));

            Log.Information($"Is Node {version} installed {isInstalled}");

            IsIdle = false;
            if (isInstalled)
            {
                InstallCaption = "Uninstalling..";

                await installNode.DeleteAllFilesAsync(true);

                await LoadVersionAsync();

                Log.Information($"Uninstalling Node {version} complete.");

                InstallCaption = "Install";
                IsIdle = true;
            }
            else
            {
                try
                {
                    IsOpenDownloadPane = true;
                    //var navigationParams = new NavigationParameters();
                    //navigationParams.Add("SelectedNode", SelectedNodeJs);
                    //navigationParams.Add("TempPath", Path.GetDirectoryName(localFile));
                    //PrismHelper.Navigate("VersionRegion", "NodeDownloader", navigationParams);

                    InstallCaption = "Installing..";

                    if (!await localFile.IsZipValidAsync())
                    {
                        Log.Information($"Downloading file {distUrl}");
                        //await distUrl.DownloadFileAsync("/", localFile);
                        DownloadFileAsync(distUrl, localFile);
                        //downloadWorker.RunWorkerAsync();
                        // Log.Information($"Saved to {localFile}");

                        //Log.Information("Extracting zip file");
                        //localFile.FastUnzip(installDir);

                        //Log.Information("Creating Symlink");
                        //installNode.CreateSymlink(symLinkTarget);
                    }
                    else
                    {
                        Log.Information("Previous temp file is available, skip downloading");
                        await OnDownloadCompleteAsync();
                    }

                    // Log.Information($"Installing Node {version} complete.");
                    // InstallCaption = "Uninstall";
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error Installing Node");
                    DialogHelper.ErrorDialog(ex.Message);
                    InstallCaption = "Install";
                }

                // IsOpenDownloadPane = false;
            }
        }

        private void CancelInstall()
        {
            webClient.CancelAsync();
            InstallCaption = "Install";
            IsOpenDownloadPane = false;
            IsIdle = true;
        }

        private void OnSelectionChanged()
        {
            var isInstalled = SelectedNodeJs.IsInstalled;
            InstallCaption = isInstalled ? "Uninstall" : "Install";

            // EnsureEnvironmentVar();
        }

        private void OnSearchBoxChanged()
        {
            if (!string.IsNullOrEmpty(SearchBox))
            {
                NodeJsFilter = NodeJsCollection.Where(node =>
                        node.NodeVersion.Contains(SearchBox) ||
                        node.LtsName.Contains(SearchBox))
                    .ToObservableCollection();
            }
            else
            {
                NodeJsFilter = NodeJsCollection;
            }
        }

        private async Task OnDownloadCompleteAsync()
        {
            var version = SelectedNodeJs.NodeVersion;
            var distUrl = SelectedNodeJs.DistUrl;
            var isInstalled = SelectedNodeJs.IsInstalled;
            var fileName = Path.GetFileName(distUrl);
            var localFile = Path.Combine(AppConfig.TempPath, fileName);
            var installDir = AppConfig.NodePath;
            var symLinkTarget = AppConfig.SymlinkPath;

            if (!await localFile.IsZipValidAsync())
            {
                const string corruptMsg = "Zip file may corrupt. Please re-download";
                Log.Information(corruptMsg);
                DialogHelper.WarnDialog(corruptMsg);

                IsOpenDownloadPane = false;
                IsIdle = true;
                return;
            }

            DownloadTitle = $"Installing {version}";
            DownloadDetail = "";

            Log.Information("Extracting zip file");
            await localFile.FastUnzipAsync(installDir);

            // var dirs = Directory.GetDirectories(installDir);
            // var installNode = dirs.FirstOrDefault(str => str.Contains(version));
            var installNode = installDir.FindPath(version);
            Log.Information("Creating Symlink");
            installNode.CreateSymlink(symLinkTarget);

            Log.Information($"Installing Node {version} complete.");
            InstallCaption = "Uninstall";
            IsOpenDownloadPane = false;
            IsIdle = true;
        }

        private static void EnsureEnvironmentVar(bool installNode)
        {
            var installDir = AppConfig.NodePath;
            var newEnvPath = "";
            var oldEnvPath = AppConfig.ListEnvPath;

            if (installNode)
            {
                Log.Information("Enabling NodeJS..");
                oldEnvPath = oldEnvPath.Where(path => !path
                    .Replace("\\", @"\")
                    .Contains(AppConfig.SymlinkPath)).ToList();
                oldEnvPath.Add(AppConfig.SymlinkPath);
                newEnvPath = oldEnvPath.ToArray().Join(";");
                Log.Information("NodeJS enabled successfully.");
            }
            else
            {
                Log.Information("Disabling NodeJS");
                oldEnvPath = oldEnvPath.Where(path => !path
                        .Replace("\\", @"\")
                        .Contains(AppConfig.SymlinkPath)).ToList();
                newEnvPath = oldEnvPath.ToArray().Join(";");
                Log.Information("NodeJS disabled successfully.");
            }

            Environment.SetEnvironmentVariable("PATH", newEnvPath, EnvironmentVariableTarget.User);
        }

        #region Downloader

        private string _downloadSpeed;
        private string _downloadProgress;
        private string _downloadSize;
        private string _downloadTitle;
        private string _downloadDetail;
        private int _downloadPercentage;

        private WebClient webClient;               // Our WebClient that will be doing the downloading for us
        private Stopwatch sw = new Stopwatch();

        public string DownloadSpeed
        {
            get => _downloadSpeed;
            set => SetProperty(ref _downloadSpeed, value);
        }

        public string DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }

        public string DownloadSize
        {
            get => _downloadSize;
            set => SetProperty(ref _downloadSize, value);
        }

        public string DownloadTitle
        {
            get => _downloadTitle;
            set => SetProperty(ref _downloadTitle, value);
        }

        public string DownloadDetail
        {
            get => _downloadDetail;
            set => SetProperty(ref _downloadDetail, value);
        }

        public int ProgressPercentage
        {
            get => _downloadPercentage;
            set => SetProperty(ref _downloadPercentage, value);
        }

        public void DownloadFileAsync(string urlAddress, string location)
        {
            DownloadTitle = $"Initializing..";
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += Completed;
                webClient.DownloadProgressChanged += ProgressChanged;

                // The variable that will be holding the url address (making sure it starts with http://)
                //Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri("http://" + urlAddress);
                Uri URL = new Uri(urlAddress);

                // Start the stopwatch which we will be using to calculate the download speed
                sw.Start();

                try
                {
                    // Start downloading the file
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error download file");
                }
            }
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Calculate download speed and output it to labelSpeed.
            //DownloadSpeed = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));
            DownloadSpeed = (e.BytesReceived / sw.Elapsed.TotalSeconds).SizeFormat("/s");

            // Update the progressbar percentage only when the value is not the same.
            ProgressPercentage = e.ProgressPercentage;

            // Show the percentage on our label.
            //labelPerc.Text = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            //labelDownloaded.Text = string.Format("{0} MB's / {1} MB's",
            //    (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
            //    (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

            DownloadTitle = $"Downloading Node {SelectedNodeJs.NodeVersion}";
            DownloadProgress = e.BytesReceived.ToDouble().SizeFormat();
            DownloadSize = e.TotalBytesToReceive.ToDouble().SizeFormat();

            DownloadDetail = $"Speed: {DownloadSpeed}. Size {DownloadProgress}/{DownloadSize} ({ProgressPercentage}%).";

            // Thread.Sleep(20);
        }

        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // Reset the stopwatch.
            sw.Reset();

            if (e.Cancelled == true)
            {
                Log.Information("Download has been canceled.");
            }
            else
            {
                Log.Information("Download completed!");
                OnDownloadCompleteAsync();
            }
        }

        #endregion Downloader
    }
}