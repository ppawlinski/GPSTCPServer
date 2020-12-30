using GPSTCPClient.Helpers;
using GPSTCPClient.Models;
using GPSTCPClient.View;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GPSTCPClient.ViewModel
{
    public class NavigationVM
    {
        public UserLocationsSource Locations { get; set; }

        public string AddingLocationName { get; set; }

        public Address SelectedAddingLocation { get; set; }
        

        public AddressesSource AddingLocationsList { get; set; }

        public UserLocation SelectedFavLocation { get; set; }
        private string selectedAddingLocationText;
        public string SelectedAddingLocationText
        {
            get
            {
                return selectedAddingLocationText;
            }
            set
            {
                selectedAddingLocationText = value;

                if (selectedAddingLocationText.Length >= 3 && !AddingLocationsList.Addresses.Any(p => p.DisplayName == value))
                {
                    FindAddressForFav(value);
                }
                else
                {
                    AddingLocationsList.IsDropDownOpen = false;
                }
            }
        }
        public NavigationVM()
        {
            Locations = new UserLocationsSource(Client.GetMyAddresses(), false);
            AddingLocationsList = new AddressesSource();
            SelectedAddingLocation = new Address();
            AddLocationCommand = new Command(sender => AddLocation());
            DelLocationCommand = new Command(sender => DelLocation());
        }
        public void FindAddressForFav(string val)
        {
            Task.Run(async () =>
                {
                    await Task.Delay(500);
                    if (SelectedAddingLocationText == val)
                    {
                        AddingLocationsList.FillAddressess(Client.GetAddress(SelectedAddingLocationText));
                        AddingLocationsList.IsDropDownOpen = true;
                    }
                }
            );
        }
        public ICommand AddLocationCommand { get; set; }
        public ICommand DelLocationCommand { get; set; }

        private async void AddLocation()
        {
            if (await Client.AddAddress(SelectedAddingLocation, AddingLocationName))
                Locations.Add(new UserLocation(AddingLocationName, SelectedAddingLocation));
        }

        private async void DelLocation()
        {
            if(await Client.DeleteAddress(SelectedFavLocation.Name))
            {
                Locations.Remove(SelectedFavLocation);
            }
        }
    }
}
