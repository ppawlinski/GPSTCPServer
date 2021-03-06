﻿using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System.Globalization;
using MaterialDesignThemes.Wpf;

namespace GPSTCPClient.ViewModel.Components
{
    public class UserLocation : ViewModelBase
    {
        private Address address;
        private string name;
        public UserLocation()
        {
            Address = new Address();
            Name = "";
        }
        public UserLocation(string name_)
        {
            Address = new Address();
            Name = name_;
        }
        public UserLocation(Address address_)
        {
            Address = address_;
            Name = "";
        }
        public UserLocation(string name_, Address address_)
        {
            Name = name_;
            Address = address_;
        }
        public Address Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public Pushpin Pin => new Pushpin() { Location = new Location(address.Lat, address.Lon), ToolTip = Name, Content = new PackIcon() { Kind = PackIconKind.Star } };

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

        public string Cords
        {
            get
            {
                return Address?.Lat.ToString("0.###", CultureInfo.InvariantCulture) + " : " + Address?.Lon.ToString("0.###", CultureInfo.InvariantCulture);
            }
        }

        public override string ToString()
        {
            if (Name != "") return $"[{Name}] {Address?.DisplayName ?? ""}";
            else return Address.DisplayName;

        }
    }
}
