using GPSTCPClient.Models;
using GPSTCPClient.View;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GPSTCPClient.ViewModel
{
    public class NavigationVM
    {
        private UserLocationsSource locations;

        public UserLocationsSource Locations
        {
            get
            {
                return locations;
            }
            set
            {
                locations = value;
            }
        }

        private UserLocation selectedAddingLocation;
        public UserLocation SelectedAddingLocation
        {
            get
            {
                return selectedAddingLocation;
            }
            set
            {
                selectedAddingLocation = value;
            }
        }

        public string SelectedAddingLocationText
        {
            get
            {
                return selectedAddingLocation?.Address?.DisplayName ?? "";
            }
            set
            {
                selectedAddingLocation.Address.DisplayName = value;
                if (selectedAddingLocation.Address.DisplayName.Length > 3) FindAddress();
            }
        }
        private UserLocationsSource addingLocationsList;
        public UserLocationsSource AddingLocationsList
        {
            get
            {
                return addingLocationsList;
            }
            set
            {
                addingLocationsList = value;
            }
        }

        public NavigationVM()
        {
            Locations = new UserLocationsSource(Client.GetMyAddresses(), false);
            AddingLocationsList = new UserLocationsSource(true);
            SelectedAddingLocation = new UserLocation();
        }
        public void FindAddress()
        {
            Task.Run(() =>
            {
                AddingLocationsList = new UserLocationsSource(Client.GetAddress(SelectedAddingLocationText), true);
            });
        }

    }
}
