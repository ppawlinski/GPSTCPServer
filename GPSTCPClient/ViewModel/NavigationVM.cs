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

        public Address SelectedFromAddress { get; set; }
        public string selectedFromAddressText;
        public string SelectedFromAddressText
        {
            get
            {
                return selectedFromAddressText;
            }
            set
            {
                selectedFromAddressText = value;
                if(selectedFromAddressText.Length >= 3 && !FromAddressesContainer.Locations.Any(p => p.Address.DisplayName == value))
                {
                    FindAddressFrom(value);
                }
                else
                {
                    FromAddressesSearch.IsDropDownOpen = false;
                }
            }
        }
        public AddressesSource FromAddressesSearch { get; set; }
        public UserLocationsSource FromAddressesContainer
        {
            get
            {
                if (selectedFromAddressText?.Length == 0) return Locations;
                else
                {
                    return new UserLocationsSource(FromAddressesSearch);
                }
            }
        }
        public NavigationVM()
        {
            Locations = new UserLocationsSource(Client.GetMyAddresses());
            AddingLocationsList = new AddressesSource();
            SelectedAddingLocation = new Address();
            FromAddressesSearch = new AddressesSource();
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
                        AddingLocationsList.FillAddressess(Client.GetAddress(val));
                        AddingLocationsList.IsDropDownOpen = true;
                    }
                }
            );
        }
        public void FindAddressFrom(string val)
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                if (SelectedFromAddressText == val)
                {
                    FromAddressesSearch.FillAddressess(Client.GetAddress(val));
                    FromAddressesSearch.IsDropDownOpen = true;
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
