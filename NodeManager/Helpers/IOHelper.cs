using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NodeManager.Helpers
{
    internal static class IOHelper
    {
        #region Directory

        /// <summary>Ensures the directory of file name is created.</summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns></returns>
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

        /// <summary>Safe deletes the directory if exist.</summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get name of directory from given path.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string GetDirectoryName(this string dirPath)
        {
            // return Path.GetDirectoryName(dirPath);
            return new DirectoryInfo(dirPath).Name;
        }

        /// <summary>
        /// Calculating directory size. Method 1
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculating directory size. Method 2
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculating directory size. Method 3
        /// </summary>
        /// <param name="path"></param>
        /// <param name="logging"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculating directory size with async. Method 3
        /// </summary>
        /// <param name="path"></param>
        /// <param name="logging"></param>
        /// <returns></returns>
        public static async Task<double> DirSize3Async(this string path, bool logging = true)
        {
            double dirSize = 0;
            await Task.Run(() => { dirSize = DirSize3(path, logging); })
                .ConfigureAwait(false);

            return dirSize;
        }

        /// <summary>
        /// Check if directory exist from given path
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool IsDirExist(this string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        /// <summary>
        /// Find directory name contains given filter.
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="filter">Filter string</param>
        /// <param name="logging">Log when finding path</param>
        /// <returns></returns>
        public static string FindPath(this string dirPath, string filter, bool logging = true)
        {
            if (logging) Log.Information($"Find path contains '{filter}' in {dirPath}");
            var dirs = Directory.GetDirectories(dirPath);
            var foundDir = dirs.FirstOrDefault(str => str.Contains(filter));

            if (!foundDir.IsNullOrEmpty())
                if (logging)
                    Log.Information($"Found {foundDir}");

            return foundDir;
        }

        #endregion Directory

        #region File

        /// <summary>
        /// Get file name from given full file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(this string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Find file name contains filter string.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string FindFile(this string dirPath, string filter)
        {
            var dirs = Directory.GetFiles(dirPath);
            var filteredDir = dirs.FirstOrDefault(str => str.Contains(filter));

            return filteredDir;
        }

        /// <summary>
        /// Check file if exist from given file path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileExist(this string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// Safe delete file with check file if exist before delete.
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(this string path)
        {
            if (File.Exists(path))
            {
                Log.Information($"Deleting {path}");
                File.Delete(path);
            }
        }

        /// <summary>
        /// Delete all file on given directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="andThis"></param>
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

        /// <summary>
        /// Delete all file on given directory with async.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="andThis"></param>
        /// <returns></returns>
        public static async Task DeleteAllFilesAsync(this string path, bool andThis)
        {
            await Task.Run(() => { DeleteAllFiles(path, andThis); })
                .ConfigureAwait(false);
        }

        #endregion File
    }
}