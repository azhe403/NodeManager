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

        public static void LoadConfig()
        {
            SerilogHelper.ConfigureSerilog();
        }
    }
}