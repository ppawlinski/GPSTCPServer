using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    public class FavouritesVM : ViewModelBase
    {
        public FavouritesVM(ViewModelBase nvm_)  {
            Nvm = nvm_ as NavigationVM;
            DeleteCommand = new Command(arg => DeleteLocation(arg));
            EditCommand = new Command(arg => EditLocation(arg));
        }

        public NavigationVM Nvm { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        private async void DeleteLocation(object selected)
        {
            if(selected != null && selected is UserLocation ul)
            {
                if (await Client.DeleteAddress(ul.Name))
                {
                    Nvm.Locations.Remove(ul);
                    if (Nvm.Locations.Count == 0)
                    {
                        Nvm.MainMap.MainLoc = new Pin();
                    }
                }
            }
        }

        private void EditLocation(object selected)
        {
            if(selected != null && selected is UserLocation ul)
            {
                throw new NotImplementedException();
            }
        }
    }
}
