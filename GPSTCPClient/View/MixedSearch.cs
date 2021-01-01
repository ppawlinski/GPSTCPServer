using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GPSTCPClient.View
{
    public class MixedSearch : ViewModelBase
    {
        public MixedSearch()
        {
            SelectedLocation = new UserLocation();
            storedLocations = new ObservableCollection<UserLocation>();
            searchingLocations = new ObservableCollection<UserLocation>();
            SelectedLocationText = "";
        }

        private UserLocation selectedLocation;

        public UserLocation SelectedLocation
        {
            get
            {
                return selectedLocation;
            }
            set
            {
                selectedLocation = value;
                OnPropertyChanged(nameof(SelectedLocation));
            }
        }

        private ObservableCollection<UserLocation> storedLocations;
        public ObservableCollection<UserLocation> StoredLocations
        {
            get
            {
                return storedLocations;
            }
            set
            {
                storedLocations = value;
                OnPropertyChanged(nameof(StoredLocations));
                OnPropertyChanged(nameof(Locations));
            }
        }

        private ObservableCollection<UserLocation> searchingLocations;
        public ObservableCollection<UserLocation> SearchingLocations
        {
            get
            {
                return searchingLocations;
            }
            set
            {
                searchingLocations = value;
                OnPropertyChanged(nameof(SearchingLocations));
                OnPropertyChanged(nameof(Locations));
            }
        }
        public ObservableCollection<UserLocation> Locations
        {
            get
            {
                if (String.IsNullOrEmpty(selectedLocationText) || StoredLocations.Contains(SelectedLocation))
                {
                    return StoredLocations;
                }
                else return SearchingLocations;
            }
        }

        private string selectedLocationText;
        public string SelectedLocationText
        {
            get
            {
                return selectedLocationText;
            }
            set
            {
                selectedLocationText = value;
                OnPropertyChanged(nameof(Locations));
                if (selectedLocationText.Length >= 3 && !Locations.Any(p => p.Address.DisplayName == value))
                {
                    FindAddress(value);
                    IsDropDownOpen = true;
                }
                else
                {
                    IsDropDownOpen = false;
                }
                OnPropertyChanged(nameof(SelectedLocationText));
            }
        }

        private bool isDropDownOpen;
        public bool IsDropDownOpen
        {
            get
            {
                return isDropDownOpen;
            }
            set
            {
                isDropDownOpen = value;
                OnPropertyChanged(nameof(IsDropDownOpen));
            }
        }

        private void FindAddress(string val)
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                if (SelectedLocationText == val)
                {
                    SearchingLocations = new ObservableCollection<UserLocation>(convertAddressesToUl(await Client.GetAddress(val)));
                }
            }
            );
        }

        private UserLocation[] convertAddressesToUl(Address[] addresses)
        {
            UserLocation[] userLocations = new UserLocation[addresses.Length];
            for(int i=0; i<addresses.Length; i++)
            {
                userLocations[i] = new UserLocation("", addresses[i]);
            }
            return userLocations;
        }
    }
}
