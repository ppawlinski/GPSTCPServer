using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GPSTCPClient.ViewModel.Components
{
    public class Pin : ViewModelBase
    {
        public Pin(UserLocation ul)
        {
            Location = GetLocation(ul.Address);
            Name = ul.Name;
            Visibility = Visibility.Visible;
        }

        public Pin(Address address)
        {
            Location = GetLocation(address);
            Name = "";
            Visibility = Visibility.Visible;
        }

        public Pin(string name_, Location location_)
        {
            Location = location_;
            Name = name_;
            Visibility = Visibility.Visible;
        }

        public Pin()
        {
            Location = new Location();
            Name = "";
            Visibility = Visibility.Hidden; 
        }

        private Visibility visibility;
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        private Location location;
        public Location Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private Location GetLocation(Address address)
        {
            return new Location(double.Parse(address.Lat, CultureInfo.InvariantCulture), double.Parse(address.Lon, CultureInfo.InvariantCulture));
        }
    }
}
