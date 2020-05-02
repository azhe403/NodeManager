using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;

namespace NodeManager.Helpers
{
    internal static class RaygunHelper
    {
        private static readonly RaygunClient Client = new RaygunClient("itdxm0Xb7rlB34MWtNw3g");

        public static void Send(Exception ex)
        {
            Client.Send(ex);
        }

        public static void SendMessage(string message)
        {
        }
    }
}