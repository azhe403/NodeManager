using System;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class StrHelper
    {
        public static string Join(this string[] arr, string delimiter)
        {
            return string.Join(delimiter, arr);
        }

        public static string[] Lines(this string source)
        {
            return source.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string ResolveVariable(this string input, object parameters)
        {
            Log.Information("Resolving variable..");
            var type = parameters.GetType();
            Regex regex = new Regex("\\{(.*?)\\}");
            var sb = new StringBuilder();
            var pos = 0;

            if (input == null) return input;

            foreach (Match toReplace in regex.Matches(input))
            {
                var capture = toReplace.Groups[0];
                var paramName = toReplace.Groups[toReplace.Groups.Count - 1].Value;
                var property = type.GetProperty(paramName);

                if (property == null) continue;
                sb.Append(input.Substring(pos, capture.Index - pos));
                sb.Append(property.GetValue(parameters, null));
                pos = capture.Index + capture.Length;
            }

            if (input.Length > pos + 1) sb.Append(input.Substring(pos));

            return sb.ToString();
        }
    }
}