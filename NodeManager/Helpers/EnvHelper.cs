using System.Windows;

namespace NodeManager.Helpers
{
    internal class EnvHelper
    {
        public static bool InDesignMode => !(Application.Current is App);
    }
}