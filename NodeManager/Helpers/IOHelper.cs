using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Serilog;
using SymbolicLinkSupport;

namespace NodeManager.Helpers
{
    internal static class IOHelper
    {
        public static string EnsureDirectory(this string dirPath)
        {
            var path = Path.GetDirectoryName(dirPath);

            Log.Information($"Ensuring Dir: {path}");
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

        public static void DeleteFile(this string path)
        {
            if (File.Exists(path))
            {
                Log.Information($"Deleting {path}");
                File.Delete(path);
            }
        }

        public static void DeleteAllFiles(this string path)
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
        }

        public static string SearchPath(string targetDir, string filter)
        {
            var dirs = Directory.GetDirectories(targetDir);
            var filteredDir = dirs.FirstOrDefault(str => str.Contains(filter));

            return filteredDir;
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

        public static double DirSize3(this string p)
        {
            try
            {
                // 1.
                // Get array of all file names.
                string[] a = Directory.GetFiles(p, "*.*", SearchOption.AllDirectories);

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
            catch
            {
                return -1;
            }
        }

        public static bool IsFileExist(this string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool IsDirExist(this string filePath)
        {
            return Directory.Exists(filePath);
        }

        public static void FastUnzip(this string filePath, string destinationPath, bool createEmptyDir = false)
        {
            Log.Information($"Extracting file {filePath}");
            var fastZip = new FastZip
            {
                CreateEmptyDirectories = createEmptyDir
            };

            // Will always overwrite if target filenames already exist
            fastZip.ExtractZip(filePath, destinationPath, null);
            Log.Information($"Extracted to {destinationPath}");
        }

        public static void CreateSymlink(this string dirPath, string targetPath)
        {
            Log.Information($"Map Symlink {dirPath} to {targetPath}");

            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.CreateSymbolicLink(targetPath);
        }

        public static bool IsSymlink(this string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            return dirInfo.IsSymbolicLinkValid();
        }
    }
}