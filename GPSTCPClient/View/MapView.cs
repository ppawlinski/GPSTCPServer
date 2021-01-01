﻿using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GPSTCPClient.View
{
    public class MapView : ViewModelBase
    {
        public MapView()
        {
            Center = new Location();
            MapMarkers = new List<Pushpin>();
            MainLoc = new Location();
            FromPin = new Location();
            ToPin = new Location();
            ZoomLevel = 10;
        }
        private Location center;
        public Location Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
                OnPropertyChanged(nameof(Center));
            }
        }

        public void SetCenter(string lat, string lon)
        {
            double lat_ = double.Parse(lat, CultureInfo.InvariantCulture);
            double lon_ = double.Parse(lon, CultureInfo.InvariantCulture);
            Center = new Location(lat_, lon_);
        }
        private Location mainLoc;
        public Location MainLoc
        {
            get
            {
                return mainLoc;
            }
            set
            {
                mainLoc = value;
                OnPropertyChanged(nameof(MainLoc));
            }
        }
        private Location fromPin;
        public Location FromPin
        {
            get
            {
                return fromPin;
            }
            set
            {
                fromPin = value;
                OnPropertyChanged(nameof(FromPin));
            }
        }
        private Location toPin;
        public Location ToPin
        {
            get
            {
                return toPin;
            }
            set
            {
                toPin = value;
                OnPropertyChanged(nameof(ToPin));
            }
        }

        private List<Pushpin> mapMarkers;
        public List<Pushpin> MapMarkers
        {
            get
            {
                return mapMarkers;
            }
            set
            {
                mapMarkers = value;
                OnPropertyChanged(nameof(MapMarkers));
            }
        }

        private LocationCollection polylineLocations;
        public LocationCollection PolylineLocations
        {
            get
            {
                return polylineLocations;
            }
            set
            {
                polylineLocations = value;
                OnPropertyChanged(nameof(PolylineLocations));
            }
        }

        private int zoomLevel;
        public int ZoomLevel
        {
            get
            {
                return zoomLevel;
            }
            set
            {
                zoomLevel = value;
                OnPropertyChanged(nameof(ZoomLevel));
            }
        }

        public void FillWithFavs(IEnumerable<UserLocation> userLocations)
        {
            foreach(var ul in userLocations)
            {
                Pushpin pin = new Pushpin();
                pin.Location = new Location(double.Parse(ul.Address.Lat, CultureInfo.InvariantCulture), double.Parse(ul.Address.Lon, CultureInfo.InvariantCulture));
                pin.Name = ul.Name;
                MapMarkers.Add(pin);
            }
        }

    }
}
