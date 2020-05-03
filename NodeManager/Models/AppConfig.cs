using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodeManager.Models
{
    internal static class AppConfig
    {
        public static string RoamingAppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string BaseLocalAppData => Path.Combine(LocalAppData, @"WinTenDev\NodeManager");
        public static string LogsPath => Path.Combine(BaseLocalAppData, @"Logs\");
        public static string CachesPath => Path.Combine(BaseLocalAppData, @"Caches\");
        public static string TempPath => Path.Combine(BaseLocalAppData, @"Temp\");
        public static string NodePath => Path.Combine(BaseLocalAppData, @"Nodes\");
        public static string SymlinkPath => @"C:\Program Files\nodejs";

        public static List<string> ListEnvPath
            => Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Split(';').ToList();

        public static bool IsPathEnvSet => ListEnvPath.Contains(SymlinkPath);
    }
}