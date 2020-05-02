using System;
using Serilog;

namespace NodeManager.Helpers
{
    internal class SerilogHelper
    {
        public static void ConfigureSerilog()
        {
            var filePath = $"Storage/Logs/App-.log";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    retainedFileCountLimit: 14)
                .CreateLogger();
        }
    }
}