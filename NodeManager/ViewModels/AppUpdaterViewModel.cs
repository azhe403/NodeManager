using NodeManager.Helpers;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using NodeManager.Models;
using Prism.Commands;

namespace NodeManager.ViewModels
{
    public class AppUpdaterViewModel : BindableBase
    {
        private ObservableCollection<AppUpdate> _listAppUpdates;

        public ObservableCollection<AppUpdate> ListAppUpdatesCollection
        {
            get => _listAppUpdates;
            set => SetProperty(ref _listAppUpdates, value);
        }

        public DelegateCommand RefreshListUpdatesCommand { get; set; }

        public AppUpdaterViewModel()
        {
            RefreshListUpdatesCommand = new DelegateCommand(RefreshListUpdates);
            ListAppUpdatesCollection = AppHelper.GetListAppUpdates();
        }

        private void RefreshListUpdates()
        {
            AppHelper.GetListAppUpdates();
        }
    }
}