using System.Windows.Forms;
using Prism.Mvvm;

namespace NodeManager.Models
{
    internal class AppModel : BindableBase
    {
        private static AppModel _appModel;
        private bool _isAppIdle;

        public static NotifyIcon AppTrayIcon { get; set; }

        public bool IsAppIdle
        {
            get => _isAppIdle;
            set => SetProperty(ref _isAppIdle, value);
        }

        public AppModel()
        {
        }

        public static AppModel GeInstance()
        {
            if (_appModel == null)
            {
                _appModel = new AppModel();
            }

            return _appModel;
        }
    }
}