using System.Diagnostics;
using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;
using myoddweb.directorywatcher;
using myoddweb.directorywatcher.interfaces;
using NodeManager.Helpers;
using NodeManager.Models;

namespace NodeManager.ViewModels
{
    public class StorageViewerViewModel : BindableBase
    {
        private string _tempSize;
        private string _logsSize;
        private string _nodeSize;
        private string _cacheSize;
        private Stopwatch _stopwatch;

        public string TempSize
        {
            get => _tempSize;
            set => SetProperty(ref _tempSize, value);
        }

        public string LogsSize
        {
            get => _logsSize;
            set => SetProperty(ref _logsSize, value);
        }

        public string NodeSize
        {
            get => _nodeSize;
            set => SetProperty(ref _nodeSize, value);
        }

        public string CacheSize
        {
            get => _cacheSize;
            set => SetProperty(ref _cacheSize, value);
        }

        public StorageViewerViewModel()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            CalculateTemp();
            StorageWatcher();
        }

        private void StorageWatcher()
        {
            // create Watcher
            var watch = new Watcher();

            // Add a request.
            watch.Add(new Request(AppConfig.BaseLocalAppData, true));

            watch.OnTouchedAsync += WatchOnChangedAsync;

            // start watching
            watch.Start();
        }

        private Task WatchOnChangedAsync(IFileSystemEvent e, CancellationToken token)
        {
            // await Task.Run(() =>
            // {
            //Log.Information("Storage changed..");

            if (_stopwatch.Elapsed.Seconds >= 1)
            {
                _stopwatch.Reset();
                _stopwatch.Stop();
                CalculateTemp();
                _stopwatch.Start();
            }

            // }, token);

            return Task.CompletedTask;
        }

        private void CalculateTemp()
        {
            if (EnvHelper.InDesignMode) return;

            TempSize = AppConfig.TempPath.DirSize2().SizeFormat();
            LogsSize = AppConfig.LogsPath.DirSize2().SizeFormat();
            NodeSize = AppConfig.NodePath.DirSize3().SizeFormat();
            CacheSize = AppConfig.CachesPath.DirSize2().SizeFormat();
        }
    }
}