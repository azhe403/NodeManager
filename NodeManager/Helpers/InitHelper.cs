using Serilog;

namespace NodeManager.Helpers
{
    internal class InitHelper
    {
        public static void PrepareAll()
        {
            PrepareDirectory();
            LoadConfig();

            Log.Information("App is Ready!");
        }

        public static void PrepareDirectory()
        {
            @"Storage\Logs\".EnsureDirectory();
            @"Storage\Caches\".EnsureDirectory();
            @"Storage\Temp\".EnsureDirectory();
            @"Storage\Nodes\".EnsureDirectory();
        }

        public static void LoadConfig()
        {
            SerilogHelper.ConfigureSerilog();
        }
    }
}