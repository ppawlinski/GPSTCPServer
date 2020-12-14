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
            storedLocations = new ObservableCollection<UserLocation>() { new UserLocation() { Name = "WCZYTYWANIE..." } };
            Task.Run(async () =>
            {
                storedLocations = new ObservableCollection<UserLocation>(await loader);
            });
            OnPropertyChanged(null);
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
                OnPropertyChanged(nameof(Searching));
            }
        }

        public UserLocationsSource(List<UserLocation> loader)
        {
            storedLocations = new ObservableCollection<UserLocation>(loader);
            OnPropertyChanged(null);
        }

        public UserLocationsSource()
        {
            storedLocations = new ObservableCollection<UserLocation>();
            OnPropertyChanged(null);
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
    }
}
