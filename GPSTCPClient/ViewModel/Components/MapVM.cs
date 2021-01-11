using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.Generic;
using System.Globalization;

namespace GPSTCPClient.ViewModel.Components
{
    public class MapVM : ViewModelBase
    {
        public MapVM()
        {
            Center = new Location();
            MapMarkers = new List<Pushpin>();
            ZoomLevel = 10;
            MainLoc = new Pin();
            FromPin = new Pin();
            ToPin = new Pin();
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
        private Pin mainLoc;
        public Pin MainLoc
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
        public Pin FromPin
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
        public Pin ToPin
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
        private Pin fromPin;
        private Pin toPin;

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
            foreach (var ul in userLocations)
            {
                Pushpin pin = new Pushpin();
                pin.Location = GetLocation(ul.Address);
                pin.Name = ul.Name;
                MapMarkers.Add(pin);
            }
        }

        public static Location GetLocation(Address address)
        {
            // return new Location(double.Parse(address.Lat, CultureInfo.InvariantCulture), double.Parse(address.Lon, CultureInfo.InvariantCulture));
            return new Location(address.Lat, address.Lon);
        }

    }
}
