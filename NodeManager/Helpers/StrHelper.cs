using System;

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
            return source.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}