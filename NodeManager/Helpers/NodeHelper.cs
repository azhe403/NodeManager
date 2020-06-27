using NodeManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class NodeHelper
    {
        public static async Task<List<NodeJs>> LoadCloudNodeJs()
        {
            var fileStamp = DateTime.Now.ToString("yyyy-MM-dd");
            var localJson = Path.Combine(AppConfig.CachesPath, $"node-registry_{fileStamp}.json");

            var url = "https://nodejs.org/dist/index.json";
            Log.Information($"Loading NodeJs registry");

            if (!localJson.IsFileExist())
            {
                Log.Information("Updating registry Cache");
                await url.WithTimeout(10)
                    .DownloadFileAsync(Path.GetDirectoryName(localJson), Path.GetFileName(localJson))
                    .ConfigureAwait(false);
            }

            var json = File.ReadAllText(localJson);
            var nodeModels = NodeJs.FromJson(json);

            return nodeModels.ToList();
        }

        public static string GetDistUrl(this NodeJs nodeJs)
        {
            var distUrl = $"https://nodejs.org/dist/{nodeJs.NodeVersion}/node-{nodeJs.NodeVersion}-win-x64.zip";
            return distUrl;
        }

        public static NodeActive GetActiveNode()
        {
            var symlinkTarget = AppConfig.SymlinkPath.GetSymlinkTarget();
            if (symlinkTarget == null)
            {
                return null;
            }

            var nodeFolder = symlinkTarget.GetDirectoryName();
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
                var rawSize = await installationPath.DirSize3Async(false)
                    .ConfigureAwait(false);
                size = rawSize.SizeFormat();
            }

            return size;
        }
    }
}