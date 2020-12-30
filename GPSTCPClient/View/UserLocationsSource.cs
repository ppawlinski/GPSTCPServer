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

        public UserLocationsSource(Task<List<UserLocation>> loader)
        {
            Locations = new ObservableCollection<UserLocation>() { new UserLocation("WCZYTYWANIE...", null) };
            Task.Run(async () =>
            {
                Locations = new ObservableCollection<UserLocation>(await loader);
            });
        }
        public UserLocationsSource(Task<UserLocation[]> loader)
        {
            Locations = new ObservableCollection<UserLocation>() { new UserLocation("WCZYTYWANIE...", null) };
            Task.Run(async () =>
            {
                Locations = new ObservableCollection<UserLocation>(await loader);
            });
        }
        public UserLocationsSource(Task<Address[]> loader)
        {
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
        public UserLocationsSource(AddressesSource loader)
        {
            Locations = new ObservableCollection<UserLocation>();
            foreach (var addr in loader.Addresses)
            {
                Locations.Add(new UserLocation("", addr));
            }
            
        }

        public UserLocationsSource(List<UserLocation> loader)
        {
            Locations = new ObservableCollection<UserLocation>(loader);
        }

        public UserLocationsSource()
        {
            Locations = new ObservableCollection<UserLocation>();
        }

        private ObservableCollection<UserLocation> locations;
        public ObservableCollection<UserLocation> Locations
        {
            get {
                return locations;
            }
            set
            {
                locations = value;
                OnPropertyChanged(nameof(Locations));
            }

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
