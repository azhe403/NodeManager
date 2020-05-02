using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;
using myoddweb.directorywatcher;
using myoddweb.directorywatcher.interfaces;
using NodeManager.Helpers;

namespace NodeManager.ViewModels
{
    public class StorageViewerViewModel : BindableBase
    {
        private string _tempSize;
        private string _logsSize;
        private string _nodeSize;
        private string _cacheSize;

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
            CalculateTemp();
            StorageWatcher();
        }

        private void StorageWatcher()
        {
            // create Watcher
            var watch = new Watcher();

            // Add a request.
            watch.Add(new Request("Storage", true));

            watch.OnTouchedAsync += WatchOnChangedAsync;

            // start watching
            watch.Start();
        }

        private async Task WatchOnChangedAsync(IFileSystemEvent e, CancellationToken token)
        {
            await Task.Run(() =>
            {
                //Log.Information("Storage changed..");

                CalculateTemp();
            }, token);
        }

        private void CalculateTemp()
        {
            if (EnvHelper.InDesignMode) return;

            TempSize = @"Storage\Temp".DirSize2().SizeFormat();
            LogsSize = @"Storage\Logs".DirSize2().SizeFormat();
            NodeSize = @"Storage\Nodes".DirSize3().SizeFormat();
            CacheSize = @"Storage\Caches".DirSize2().SizeFormat();
        }
    }
}