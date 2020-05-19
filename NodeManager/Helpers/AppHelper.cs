using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Linq;
using NodeManager.Models;
using Serilog;

namespace NodeManager.Helpers
{
    internal static class AppHelper
    {
        public static ObservableCollection<AppUpdate> GetListAppUpdates()
        {
            var listUpdate = new List<AppUpdate>();
            try
            {
                var urlSource = "https://autodeploy.node-manager.azhe.info/";

                Log.Information("Reading online updates..");
                var doc = new HtmlWeb().Load(urlSource);
                var listHref = doc.DocumentNode.SelectNodes("//a[@href]");

                var listUrl = listHref.Select(href => href.GetAttributeValue("href", string.Empty))
                    .Where(hrefVal => hrefVal.StartsWith("/"))
                    .Select(hrefVal => urlSource + hrefVal.TrimStart('/'));

                foreach (var url in listUrl)
                {
                    var partUrl = url.Split('/');
                    var names = partUrl[3];
                    var partName = names.Split('-');
                    var name = partName[0];
                    var version = partName[1];
                    var commit = partName[2];

                    listUpdate.Add(new AppUpdate()
                    {
                        Url = url,
                        Name = name,
                        Version = version,
                        NameVersion = name + ' ' + version
                    });
                }

                Log.Information($"Loaded {listUpdate.Count} item(s)");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Load AppUpdates");
            }

            listUpdate.Reverse();
            return listUpdate.ToObservableCollection();
        }
    }
}