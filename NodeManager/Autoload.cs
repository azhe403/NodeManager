using NodeManager.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace NodeManager
{
    internal class AutoLoad : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<VersionManager>();
            containerRegistry.RegisterForNavigation<PackageManager>();
            containerRegistry.RegisterForNavigation<NodeDownloader>();
            containerRegistry.RegisterForNavigation<Settings>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RequestNavigate("MainRegion", "VersionManager");
        }
    }
}