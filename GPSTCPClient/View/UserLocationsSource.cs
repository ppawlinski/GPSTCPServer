using GPSTCPClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.View
{
    public class UserLocationsSource : INotifyPropertyChanged
    {

        public UserLocationsSource(Task<List<UserLocation>> loader, bool search)
        {
            Searching = search;
            Locations = new ObservableCollection<UserLocation>() { new UserLocation("WCZYTYWANIE...", null) };
            Task.Run(async () =>
            {
                Locations = new ObservableCollection<UserLocation>(await loader);
            });
        }
        public UserLocationsSource(Task<UserLocation[]> loader, bool search)
        {
            Searching = search;
            Locations = new ObservableCollection<UserLocation>() { new UserLocation("WCZYTYWANIE...", null) };
            Task.Run(async () =>
            {
                Locations = new ObservableCollection<UserLocation>(await loader);
            });
        }
        public UserLocationsSource(Task<Address[]> loader, bool search)
        {
            Searching = search;
            Locations = new ObservableCollection<UserLocation>();
            List<UserLocation> ulList = new List<UserLocation>();
            Locations.Add(new UserLocation() { Address = new Address() { DisplayName = "WCZYTYWANIE..." } });
            Task.Run(async () =>
            {
                var addresses = await loader;
                foreach (var address in addresses)
                {
                    ulList.Add(new UserLocation("", address));
                }
                Locations = new ObservableCollection<UserLocation>(ulList);
            });
        }
        private bool searching;

        public bool Searching
        {
            get
            {
                return searching;
            }
            set
            {
                searching = value;
            }
        }

        public UserLocationsSource(List<UserLocation> loader, bool search)
        {
            Searching = search;
            Locations = new ObservableCollection<UserLocation>(loader);
        }

        public UserLocationsSource(bool search)
        {
            Searching = search;
            Locations = new ObservableCollection<UserLocation>();
        }

        private ObservableCollection<UserLocation> storedLocations;
        private ObservableCollection<UserLocation> searchingLocations;

        public ObservableCollection<UserLocation> Locations
        {
            get
            {
                if (Searching) return searchingLocations;
                else return storedLocations;
            }
            set
            {
                if (Searching) searchingLocations = value;
                else storedLocations = value;
                OnPropertyChanged(nameof(Locations));
            }
        }



        public async void Find(string input)
        {
            List<UserLocation> ul = new List<UserLocation>();
            var found = await Client.GetAddress(input);
            foreach(var addr in found)
            {
                ul.Add(new UserLocation() { Name = "", Address = addr });
            }
            searchingLocations = new ObservableCollection<UserLocation>(ul);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public void Add(UserLocation ul)
        {
            Locations.Add(ul);
        }

        public void Remove(UserLocation ul)
        {
            Locations.Remove(ul);
        }
    }
}
