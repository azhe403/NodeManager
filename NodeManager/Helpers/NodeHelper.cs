using NodeManager.Models;
using System;
using System.Threading.Tasks;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class NodeHelper
    {
        public static NodeActive GetActiveNode()
        {
            var nodeFolder = AppConfig.SymlinkPath.GetSymlinkTarget().GetDirectoryName();
            var splitFolder = nodeFolder.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitFolder.Length != 4)
            {
                Log.Error("The Node installation path may changed, Node version can't get");
                return new NodeActive();
            }

            var version = splitFolder[1];
            var os = splitFolder[2];
            var arch = splitFolder[3];
            var platform = os + arch.Substring(1);

            return new NodeActive()
            {
                NodeVersion = version,
                ArchShort = arch,
                OperatingSystem = os,
                Platform = platform
            };
        }

        public static async Task<string> CalculateInstalledSize(string installationPath)
        {
            var size = "0 B";
            if (installationPath.IsDirExist())
            {
                size = (await installationPath.DirSize3Async(false)).SizeFormat();
            }

            return size;
        }
    }
}