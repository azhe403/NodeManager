using System.Threading.Tasks;
using Flurl.Http;
using Serilog;

namespace NodeManager.Helpers
{
    public static class NpmRegistryHelper
    {
        public static async Task<string> GetPackage(this string packageName, string version = null)
        {
            var url = $"https://registry.npmjs.org/{packageName}";

            if (!string.IsNullOrEmpty(version))
            {
                url += $"/{version}";
            }

            Log.Information($"Getting Json {url}");
            var jsonMeta = await url.GetStringAsync().ConfigureAwait(false);
            return jsonMeta;
        }

        public static async Task<string> GetPackageTags(this string packageName)
        {
            var url = $"https://registry.npmjs.org/-/package/{packageName}/dist-tags";
            Log.Information($"Getting Json {url}");
            var jsonMeta = await url.GetStringAsync().ConfigureAwait(false);
            return jsonMeta;
        }

        public static async Task<string> Search(this string packageName, int limit)
        {
            var url = $"https://registry.npmjs.com/-/v1/search?text={packageName}&limit={limit}";

            Log.Information($"Getting Json {url}");
            var json = await url.GetStringAsync().ConfigureAwait(false);
            return json;
        }
    }
}