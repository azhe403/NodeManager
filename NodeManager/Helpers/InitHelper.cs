using NodeManager.Models;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class InitHelper
    {
        public static void PrepareAll()
        {
            PrepareDirectory();
            LoadConfig();

            Log.Information("App is Ready!");
        }

        public static void PrepareDirectory()
        {
            AppConfig.LogsPath.EnsureDirectory();
            AppConfig.CachesPath.EnsureDirectory();
            AppConfig.TempPath.EnsureDirectory();
            AppConfig.NodePath.EnsureDirectory();
        }

            //     AppCenter.Start("abf25fa8-61ab-4f91-a577-5db1df1318b3", typeof(Analytics), typeof(Crashes));
        public static void LoadConfig()
        {
            SerilogHelper.ConfigureSerilog();
        }
    }
}