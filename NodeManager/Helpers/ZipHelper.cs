using ICSharpCode.SharpZipLib.Zip;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NodeManager.Helpers
{
    internal static class ZipHelper
    {
        public static void FastUnzip(this string filePath, string destinationPath, bool createEmptyDir = false)
        {
            Log.Information($"Extracting file {filePath}");
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = createEmptyDir
            };

            // Will always overwrite if target filename s already exist
            fastZip.ExtractZip(filePath, destinationPath, null);
            Log.Information($"Extracted to {destinationPath}");
        }

        public static async Task FastUnzipAsync(this string filePath, string destinationPath,
            bool createEmptyDir = false)
        {
            await Task.Run(() =>
                {
                    Log.Debug("Async FastUnzip");
                    FastUnzip(filePath, destinationPath, createEmptyDir);
                })
                .ConfigureAwait(false);
        }

        public static bool IsZipValid(this string filePath)
        {
            bool isValid;

            if (!filePath.IsFileExist())
            {
                Log.Information($"Zip Validation skipped because file {filePath} is not exist.");
                return false;
            }

            try
            {
                Log.Information($"Validating {filePath}");
                var zipFile = new ZipFile(filePath);
                isValid = zipFile.TestArchive(true, TestStrategy.FindFirstError, null);
                zipFile.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error validating Zip");
                isValid = false;
            }

            Log.Information($"Is {filePath} valid: {isValid}");
            return isValid;
        }

        public static async Task<bool> IsZipValidAsync(this string filePath)
        {
            var isValid = false;

            await Task.Run(() => { isValid = IsZipValid(filePath); })
                .ConfigureAwait(false);

            return isValid;
        }
    }
}