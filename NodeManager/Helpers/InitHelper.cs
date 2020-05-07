using System.Windows.Forms;
using NodeManager.Models;
using NodeManager.Properties;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class InitHelper
    {
        public static void PrepareAll()
        {
            PrepareDirectory();
            LoadConfig();
            PrepareInterfaces();

            NotifyHelper.Info("App is starting, please wait..");
            Log.Information("App is Ready!");
        }

        public static void PrepareDirectory()
        {
            AppConfig.LogsPath.EnsureDirectory();
            AppConfig.CachesPath.EnsureDirectory();
            AppConfig.TempPath.EnsureDirectory();
            AppConfig.NodePath.EnsureDirectory();
        }

        public static void PrepareInterfaces()
        {
            AppModel.GeInstance();

            // Init NotifyIcon
            AppModel.AppTrayIcon = new NotifyIcon()
            {
                Icon = Resources.icons8_nodejs,
                Visible = true,
                Text = "NodeJS Manager"
            };

            //     AppCenter.Start("abf25fa8-61ab-4f91-a577-5db1df1318b3", typeof(Analytics), typeof(Crashes));
        }

        public static void LoadConfig()
        {
            SerilogHelper.ConfigureSerilog();
        }
    }
}