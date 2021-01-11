using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GPSTCPClient.ViewModel.Components
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
                OnSelected(new PropertyChangedEventArgs(nameof(value)));
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
                if (String.IsNullOrEmpty(selectedLocationText) || storedLocations.Contains(selectedLocation))
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
                if (value != SelectedLocation?.ToString())
                {
                    selectedLocation = null;
                    SearchingLocations.Clear();
                    selectedLocationText = value;
                    IsDropDownOpen = false;
                    OnPropertyChanged(nameof(Locations));
                }
                else selectedLocationText = value;

                if (selectedLocationText.Length >= 3 && !Locations.Any(p => p.ToString() == value))
                {
                    FindAddress(value);
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
                if (selectedLocationText == val)
                {
                    return convertAddressesToUl(await Client.GetAddress(val));
                }
                return null;
            }
            ).ContinueWith(task =>
            {
                if (task.Result != null && task.Result.Length > 0)
                {
                    SearchingLocations.Clear();
                    SearchingLocations.AddRange(task.Result);
                    OnPropertyChanged(nameof(Locations));
                    if (SearchingLocations.Count > 0) IsDropDownOpen = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private UserLocation[] convertAddressesToUl(Address[] addresses)
        {
            UserLocation[] userLocations = new UserLocation[addresses.Length];
            for (int i = 0; i < addresses.Length; i++)
            {
                userLocations[i] = new UserLocation("", addresses[i]);
            }
            return userLocations;
        }

        public event EventHandler OnSelectedAction;
        protected virtual void OnSelected(EventArgs e)
        {
            EventHandler handler = OnSelectedAction;
            handler?.Invoke(this, e);
        }
    }
}
