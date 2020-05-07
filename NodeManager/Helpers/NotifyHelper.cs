using NodeManager.Models;
using System.Windows.Forms;

namespace NodeManager.Helpers
{
    internal static class NotifyHelper
    {
        public static NotifyIcon Info(string message)
        {
            var trayIcon = AppModel.AppTrayIcon;
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.BalloonTipText = message;
            trayIcon.BalloonTipTitle = "NodeJS Manager";
            trayIcon.ShowBalloonTip(3000);
            return trayIcon;
        }

        public static NotifyIcon Warn(string message)
        {
            var trayIcon = AppModel.AppTrayIcon;
            trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            trayIcon.BalloonTipText = message;
            trayIcon.BalloonTipTitle = "NodeJS Manager";
            trayIcon.ShowBalloonTip(3000);
            return trayIcon;
        }

        public static NotifyIcon Error(string message)
        {
            var trayIcon = AppModel.AppTrayIcon;
            trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            trayIcon.BalloonTipText = message;
            trayIcon.BalloonTipTitle = "NodeJS Manager";
            trayIcon.ShowBalloonTip(3000);
            return trayIcon;
        }
    }
}