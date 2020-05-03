using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeManager.Helpers
{
    internal class DialogHelper
    {
        public static MessageBoxResult ErrorDialog(string message, string caption = null,
            MessageBoxButton messageBoxButton = MessageBoxButton.OK)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Application.Current.MainWindow.Title;
            }

            return MessageBox.Show(message, caption, messageBoxButton, MessageBoxImage.Error);
        }

        public static MessageBoxResult WarnDialog(string message, string caption = null,
           MessageBoxButton messageBoxButton = MessageBoxButton.OK)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Application.Current.MainWindow != null
                    ? Application.Current.MainWindow.Title
                    : "NodeManager";
            }

            return MessageBox.Show(message, caption, messageBoxButton, MessageBoxImage.Warning);
        }
    }
}