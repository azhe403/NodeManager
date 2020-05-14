using Prism.Regions;
using System.Linq;

namespace NodeManager.Helpers
{
    internal static class PrismHelper
    {
        public static IRegionManager RegionManager { get; set; }

        public static void Navigate(string regionName, string navigatePath,
            NavigationParameters navigationParameters = null)
        {
            if (RegionManager == null)
            {
                DialogHelper.ErrorDialog("RegionManager property must be initialized.");
                return;
            }

            var regionCount = RegionManager.Regions.Count();
            if (regionCount == 0)
            {
                DialogHelper.WarnDialog("No Regions registered");
                return;
            }

            var regionTarget = RegionManager.Regions.Where(region => region.Name == regionName);
            if (!regionTarget.Any())
            {
                DialogHelper.WarnDialog($"No region with name '{regionName}'");
                return;
            }

            RegionManager.Regions[regionName].RemoveAll();
            RegionManager.RequestNavigate(regionName, navigatePath, navigationParameters);
        }

        public static void ClearRegion(string regionName)
        {
            RegionManager.Regions[regionName].RemoveAll();
        }
    }
}