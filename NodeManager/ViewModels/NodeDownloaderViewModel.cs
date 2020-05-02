using NodeManager.Helpers;
using NodeManager.Models;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NodeManager.ViewModels
{
    internal class NodeDownloaderViewModel : BindableBase, INavigationAware
    {
        private string _downloadSpeed;
        private string _downloadProgress;
        private string _downloadSize;
        private int _downloadPercentage;

        private WebClient webClient;               // Our WebClient that will be doing the downloading for us
        private Stopwatch sw = new Stopwatch();

        public string DownloadSpeed
        {
            get => _downloadSpeed;
            set => SetProperty(ref _downloadSpeed, value);
        }

        public string DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }

        public string DownloadSize
        {
            get => _downloadSize;
            set => SetProperty(ref _downloadSize, value);
        }

        public int ProgressPercentage
        {
            get => _downloadPercentage;
            set => SetProperty(ref _downloadPercentage, value);
        }

        public NodeDownloaderViewModel()
        {
        }

        public void DownloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                // The variable that will be holding the url address (making sure it starts with http://)
                Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri("http://" + urlAddress);

                // Start the stopwatch which we will be using to calculate the download speed
                sw.Start();

                try
                {
                    // Start downloading the file
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error download file");
                }
            }
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Calculate download speed and output it to labelSpeed.
            DownloadSpeed = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Update the progressbar percentage only when the value is not the same.
            ProgressPercentage = e.ProgressPercentage;

            // Show the percentage on our label.
            //labelPerc.Text = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            //labelDownloaded.Text = string.Format("{0} MB's / {1} MB's",
            //    (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
            //    (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

            DownloadProgress = e.BytesReceived.ToDouble().SizeFormat();
            DownloadSize = e.TotalBytesToReceive.ToDouble().SizeFormat();
        }

        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // Reset the stopwatch.
            sw.Reset();

            if (e.Cancelled == true)
            {
                Log.Information("Download has been canceled.");
            }
            else
            {
                Log.Information("Download completed!");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var navigationParams = navigationContext.Parameters;
            var TempPath = navigationParams["TempPath"];
            var selectedNode = navigationParams["SelectedNode"] as NodeJsRow;
            var distUrl = selectedNode.DistUrl;
            var fileName = System.IO.Path.GetFileName(distUrl);

            DownloadFile(distUrl, TempPath + @"\" + fileName);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}