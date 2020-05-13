using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Serilog;
using SymbolicLinkSupport;

namespace NodeManager.Helpers
{
    internal static class SymlinkHelper
    {
        private static void CreateWinSymlink(this string dirSource, string dirSymlink)
        {
            if (dirSymlink.IsDirExist())
            {
                throw new IOException($"Directory {dirSymlink} is exist, is not safe for replace");
            }

            Log.Information($"Creating symlink via cmd {dirSource} => {dirSymlink}");
            var cmd = $"mklink /D {dirSymlink} {dirSource}".RunCommandAsAdmin();
        }

        public static void CreateSymlink(this string dirPath, string targetPath)
        {
            Log.Information($"Map Symlink {dirPath} to {targetPath}");

            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.CreateSymbolicLink(targetPath);

            var isSymlink = dirInfo.IsSymbolicLinkValid();
            Log.Information($"Is created: {isSymlink}");
        }

        public static string GetSymlinkTarget(this string symlinkPath)
        {
            if (!IsSymlink(symlinkPath))
            {
                Log.Warning($"Path {symlinkPath} is not Symlink");
                return symlinkPath;
            }

            var dirInfo = new DirectoryInfo(symlinkPath);
            var dirTarget = dirInfo.GetSymbolicLinkTarget();

            Log.Information($"Symlink {symlinkPath} target => {dirTarget}");
            return dirTarget;
        }

        public static bool IsSymlink(this string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            return dirInfo.IsSymbolicLink();
        }

        public static IEnumerable<Symlink> GetAllSymLinks(string workingDir)
        {
            var converter = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/c dir /Al")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir
                }
            };

            string output = "";
            converter.OutputDataReceived += (sender, e) =>
            {
                output += e.Data + "\r\n";
            };
            converter.Start();
            converter.BeginOutputReadLine();
            converter.WaitForExit();

            Regex regex = new Regex(@"\n.*\<SYMLINKD\>\s(.*)\s\[(.*)\]\r");

            var matches = regex.Matches(output);
            foreach (Match match in matches)
            {
                var name = match.Groups[1].Value.Trim();
                var target = match.Groups[2].Value.Trim();
                Console.WriteLine($@"Symlink: {name} --> {target}");

                yield return new Symlink() { Name = name, Target = target };
            }
        }
    }

    internal class Symlink
    {
        public string Name { get; set; }
        public string Target { get; set; }
    }
}