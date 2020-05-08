using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Serilog;
using SymbolicLinkSupport;

namespace NodeManager.Helpers
{
    internal static class IOHelper
    {
        #region Directory

        public static string EnsureDirectory(this string dirPath)
        {
            var path = Path.GetDirectoryName(dirPath);

            Log.Information($"Ensuring dDir {path}");
            if (Directory.Exists(path))
            {
                return dirPath;
            }

            if (!Directory.Exists(path))
            {
                var dir = Directory.CreateDirectory(path);
            }

            return dirPath;
        }

        public static string DeleteDirectory(this string dirPath)
        {
            var path = Path.GetDirectoryName(dirPath);

            Log.Information($"Deleting dir {path}");
            if (!Directory.Exists(path))
            {
                return dirPath;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }

            return dirPath;
        }

        public static double DirSize(this string path)
        {
            var d = new DirectoryInfo(path);
            double size = 0;

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(path);
            }

            return size;
        }

        public static double DirSize2(this string path)
        {
            if (!path.IsDirExist())
            {
                Log.Information($"Path {path} is not exist");
                return 0;
            }

            var dir = new DirectoryInfo(path);
            return dir.GetFiles().Sum(fi => fi.Length) +
                   dir.GetDirectories().Sum(di => DirSize2(path));
        }

        public static double DirSize3(this string path, bool logging = true)
        {
            try
            {
                if (logging) Log.Information($"Getting size of directory {path}");

                // 1.
                // Get array of all file names.
                string[] a = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                // 2.
                // Calculate total bytes of all files in a loop.
                double b = 0;
                foreach (string name in a)
                {
                    // 3.
                    // Use FileInfo to get length of each file.
                    FileInfo info = new FileInfo(name);
                    b += info.Length;
                }

                // 4.
                // Return total size
                return b;
            }
            catch (Exception ex)
            {
                if (logging) Log.Error(ex, "Error calculating directory size - 3. Return will -1");
                return -1;
            }
        }

        public static async Task<double> DirSize3Async(this string path, bool logging = true)
        {
            double dirSize = 0;
            await Task.Run(() => { dirSize = DirSize3(path, logging); });

            return dirSize;
        }

        public static bool IsDirExist(this string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        public static string FindPath(this string dirPath, string filter, bool logging = true)
        {
            if (logging) Log.Information($"Find path contains '{filter}' in {dirPath}");
            var dirs = Directory.GetDirectories(dirPath);
            var installNode = dirs.FirstOrDefault(str => str.Contains(filter));

            if (!installNode.IsNullOrEmpty())
                if (logging)
                    Log.Information($"Found {installNode}");

            return installNode;
        }

        #endregion Directory

        #region File

        public static string FindFile(this string dirPath, string filter)
        {
            var dirs = Directory.GetFiles(dirPath);
            var filteredDir = dirs.FirstOrDefault(str => str.Contains(filter));

            return filteredDir;
        }

        public static bool IsFileExist(this string filePath)
        {
            return File.Exists(filePath);
        }

        public static void DeleteFile(this string path)
        {
            if (File.Exists(path))
            {
                Log.Information($"Deleting {path}");
                File.Delete(path);
            }
        }

        public static void DeleteAllFiles(this string path, bool andThis = false)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            Log.Information($"Cleaning {path}");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            if (andThis)
            {
                Log.Information($"Deleting {path}");
                Directory.Delete(path);
            }
        }

        public static async Task DeleteAllFilesAsync(this string path, bool andThis)
        {
            await Task.Run(() => { DeleteAllFiles(path, andThis); });
        }

        #endregion File

        #region Zip Archiver

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
            await Task.Run(() => { FastUnzip(filePath, destinationPath, createEmptyDir); });
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

            await Task.Run(() => { isValid = IsZipValid(filePath); });

            return isValid;
        }

        #endregion Zip Archiver

        #region Symlink

        public static void CreateSymlink(this string dirPath, string targetPath)
        {
            Log.Information($"Map Symlink {dirPath} to {targetPath}");

            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.CreateSymbolicLink(targetPath);

            var isSymlink = dirInfo.IsSymbolicLinkValid();
            Log.Information($"Is created: {isSymlink}");
        }

        public static bool IsSymlink(this string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            return dirInfo.IsSymbolicLinkValid();
        }

        #endregion Symlink
    }
}