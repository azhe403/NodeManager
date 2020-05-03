using System;
using System.IO;
using NodeManager.Models;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class SerilogHelper
    {
        public static void ConfigureSerilog()
        {
            var filePath = Path.Combine(AppConfig.LogsPath, "App-.log");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    retainedFileCountLimit: 14)
                .CreateLogger();
        }
    }
}