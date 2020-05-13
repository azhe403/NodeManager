using Serilog;
using System.Diagnostics;

namespace NodeManager.Helpers
{
    internal static class ProcessHelper
    {
        public static Process RunCommandAsAdmin(this string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    Verb = "runas"
                }
            };

            process.OutputDataReceived += (sender, args) => Log.Debug($"ShExec: {sender} {args.Data}");
            process.ErrorDataReceived += (sender, args) => Log.Error($"ShExec: {sender} - {args}");
            process.Exited += (sender, args) => Log.Debug($"ShExec: {sender} - {args}");

            process.Start();
            process.WaitForExit();

            return process;
        }

        public static Process RunCommand(this string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                }
            };

            process.OutputDataReceived += (sender, args) => Log.Debug($"ShExec: {sender} {args.Data}");
            process.ErrorDataReceived += (sender, args) => Log.Error($"ShExec: {sender} - {args}");
            process.Exited += (sender, args) => Log.Debug($"ShExec: {sender} - {args}");

            process.Start();
            process.WaitForExit();

            return process;
        }
    }
}