using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel.Components
{
    public class MapVM : ViewModelBase
    {
        public MapVM()
        {
            Center = new Location();
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
            foreach (var ul in userLocations)
            {
                Pushpin pin = new Pushpin();
                pin.Location = GetLocation(ul.Address);
                pin.Name = ul.Name;
            }
        }

        public static Location GetLocation(Address address)
        {
            // return new Location(double.Parse(address.Lat, CultureInfo.InvariantCulture), double.Parse(address.Lon, CultureInfo.InvariantCulture));
            return new Location(address.Lat, address.Lon);
        }

    }
}
