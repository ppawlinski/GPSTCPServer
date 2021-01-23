using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System.Linq;
using System.Windows.Input;
using PolylineEncoder.Net.Utility.Decoders;
using System.Collections.ObjectModel;
using System;

namespace GPSTCPClient.ViewModel
{
    public class NavigationVM : ViewModelBase
    {
        public FavouritesVM FavVM { get; set; }
        public NavigationVM(ViewModelBase favVM_)
        {
            FromToggle = true;
            Pins = new ObservableCollection<Pushpin>();
            FavVM = favVM_ as FavouritesVM;
            ToAddressessSearch = new MixedSearch();
            ToAddressessSearch.OnSelectedAction += ToAddressessSearch_OnSelectedAction;
            FromAddressessSearch = new MixedSearch();
            FromAddressessSearch.OnSelectedAction += FromAddressessSearch_OnSelectedAction;
            MainMap = new MapVM();
            FindRouteCommand = new Command(arg => FindRoute());
            CenterOnRouteCommand = new Command(arg => CenterOnRoute(arg));
            CenterOnUserLocationCommand = new Command(arg => CenterOnUserLocation(arg));
            SwapAddressesCommand = new Command(arg => SwapAddresses());
            SearchLocationEnterClick = new Command(arg => FindRoute());
            MapDoubleClickCommand = new Command(arg => MapDoubleClick(arg));
            FromAddressessSearch.StoredLocations = FavVM.Locations;
            ToAddressessSearch.StoredLocations = FavVM.Locations;

            FavVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Locations")
                {
                    if (FavVM.Locations.Count > 0)
                    {
                        MainMap.Center = MapVM.GetLocation(FavVM.Locations.First().Address);
                    }
                    else
                    {
                        MainMap.Center = new Location(52.0, 19.0);
                        MainMap.ZoomLevel = 5;
                    }
                }
            };
        }


        private bool toToggle;
        public bool ToToggle
        {
            get
            {
                return toToggle;
            }
            set
            {
                toToggle = value;
                if (FromToggle && value) FromToggle = false;
                if (!FromToggle && !value) FromToggle = true;
                OnPropertyChanged(nameof(toToggle));
            }
        }

        private bool fromToggle;
        public bool FromToggle
        {
            get
            {
                return fromToggle;
            }
            set
            {
                fromToggle = value;
                if (ToToggle && value) ToToggle = false;
                if (!ToToggle && !value) ToToggle = true;
                OnPropertyChanged(nameof(FromToggle));
            }
        }
        private MixedSearch toAddressessSearch;
        public MixedSearch ToAddressessSearch
        {
            get
            {
                return toAddressessSearch;
            }
            set
            {
                toAddressessSearch = value;
            }
        }

        private MixedSearch fromAddressessSearch;
        public MixedSearch FromAddressessSearch
        {
            get
            {
                return fromAddressessSearch;
            }
            set
            {
                fromAddressessSearch = value;
            }
        }

        private StepModel[] routeInstructions;
        public StepModel[] RouteInstrucions
        {
            get
            {
                return routeInstructions;
            }
            set
            {
                routeInstructions = value;
                OnPropertyChanged(nameof(RouteInstrucions));
            }
        }

        private MapVM mainMap;
        public MapVM MainMap
        {
            get
            {
                return mainMap;
            }
            set
            {
                mainMap = value;
            }
        }

        private ObservableCollection<Pushpin> pins;
        public ObservableCollection<Pushpin> Pins
        {
            get
            {
                return pins;
            }
            set
            {
                pins = value;
                OnPropertyChanged(nameof(Pins));
            }
        }

        

        private async void MapDoubleClick(object arg)
        {
            if(arg is Location loc)
            {
                var described = await Client.DescribeAddress(loc.Latitude, loc.Longitude);
                if(described.Lat != 0  && described.Lon != 0)
                {
                    var describedLoaction = new Location(described.Lat, described.Lon);
                     
                    if (FromToggle)
                    {
                        var zPin = Pins.FirstOrDefault(p => p.Content == "Z");
                        if (zPin != null) Pins.Remove(zPin);
                        var newPin = new Pushpin() { Location = describedLoaction, Content = "Z" };
                        Pins.Add(newPin);
                        if (!String.IsNullOrEmpty(FromAddressessSearch?.SelectedLocation?.Name))
                        {
                            FromAddressessSearch.SearchingLocations.Clear();
                            FromAddressessSearch.SearchingLocations.Add(new UserLocation(described));
                            FromAddressessSearch.SelectedLocationText = described.DisplayName;
                            FromAddressessSearch.SelectedLocation = new UserLocation(described);
                        }
                        FromAddressessSearch.SearchingLocations.Clear();
                        FromAddressessSearch.SearchingLocations.Add(new UserLocation(described));
                        FromAddressessSearch.SelectedLocation = new UserLocation(described);
                        FromAddressessSearch.SelectedLocationText = described.DisplayName;

                    }
                    else if (ToToggle)
                    {
                        var doPin = Pins.FirstOrDefault(p => p.Content == "Do");
                        if (doPin != null) Pins.Remove(doPin);
                        Pins.Add(new Pushpin() { Location = describedLoaction, Content = "Do" });
                        if (!String.IsNullOrEmpty(ToAddressessSearch?.SelectedLocation?.Name))
                        {
                            ToAddressessSearch.SearchingLocations.Clear();
                            ToAddressessSearch.SearchingLocations.Add(new UserLocation(described));
                            ToAddressessSearch.SelectedLocationText = described.DisplayName;
                            ToAddressessSearch.SelectedLocation = new UserLocation(described);
                        }
                        ToAddressessSearch.SearchingLocations.Clear();
                        ToAddressessSearch.SearchingLocations.Add(new UserLocation(described));
                        ToAddressessSearch.SelectedLocation = new UserLocation(described);
                        ToAddressessSearch.SelectedLocationText = described.DisplayName;
                    }
                }
                
            }
        }

        private void FromAddressessSearch_OnSelectedAction(object sender, System.EventArgs e)
        {
            if (sender is MixedSearch ms)
            {
                if (!double.IsNaN(ms.SelectedLocation?.Address.Lat ?? double.NaN))
                {
                    var pin = Pins.FirstOrDefault(p => p.Content == "Z");
                    if (pin != null)
                    {
                        if (Math.Abs(pin.Location.Latitude - ms.SelectedLocation.Address.Lat) > 1 && Math.Abs(pin.Location.Longitude - ms.SelectedLocation.Address.Lon) > 1)
                        {
                            MainMap.Center = MapVM.GetLocation(ms.SelectedLocation.Address);
                        }
                        Pushpin newPin = new Pushpin() { Content = "Z", Location = MapVM.GetLocation(ms.SelectedLocation.Address) };
                        if (pin != null) Pins.Remove(pin);
                        Pins.Add(newPin);
                    }
                    else
                    {
                        Pushpin newPin = new Pushpin() { Content = "Z", Location = MapVM.GetLocation(ms.SelectedLocation.Address) };
                        if (pin != null) Pins.Remove(pin);
                        Pins.Add(newPin);
                        MainMap.Center = MapVM.GetLocation(ms.SelectedLocation.Address);

                    }
                }
            }
        }

        private void ToAddressessSearch_OnSelectedAction(object sender, System.EventArgs e)
        {
            if (sender is MixedSearch ms)
            {
                if (!double.IsNaN(ms.SelectedLocation?.Address.Lat ?? double.NaN))
                {
                    var pin = Pins.FirstOrDefault(p => p.Content == "Do");
                    if(pin != null)
                    {
                        if(Math.Abs(pin.Location.Latitude - ms.SelectedLocation.Address.Lat) > 1 && Math.Abs(pin.Location.Longitude - ms.SelectedLocation.Address.Lon) > 1)
                        {
                            MainMap.Center = MapVM.GetLocation(ms.SelectedLocation.Address);
                        }
                        Pushpin newPin = new Pushpin() { Content = "Do", Location = MapVM.GetLocation(ms.SelectedLocation.Address) };
                        if (pin != null) Pins.Remove(pin);
                        Pins.Add(newPin);
                    }
                    else
                    {
                        Pushpin newPin = new Pushpin() { Content = "Do", Location = MapVM.GetLocation(ms.SelectedLocation.Address) };
                        if (pin != null) Pins.Remove(pin);
                        Pins.Add(newPin);
                        MainMap.Center = MapVM.GetLocation(ms.SelectedLocation.Address);

                    }
                }
            }
        }

        public ICommand FindRouteCommand { get; set; }
        public ICommand CenterOnRouteCommand { get; set; }
        public ICommand SwapAddressesCommand { get; set; }
        public ICommand CenterOnUserLocationCommand { get; set; }
        public ICommand SearchLocationEnterClick { get; set; }
        public ICommand MapDoubleClickCommand { get; set; }

        private async void FindRoute()
        {
            if (FromAddressessSearch.SelectedLocation == null || ToAddressessSearch.SelectedLocation == null) return;
            FavVM.MainVM.Loading = true;
            RouteInstrucions = new StepModel[]
            {
                new StepModel()
                {
                    Description = "WCZYTYWANIE..."
                }
            };
            var rm = await Client.GetRoute(FromAddressessSearch.SelectedLocation.Address, ToAddressessSearch.SelectedLocation.Address);
            if (rm != null)
            {
                RouteString[] ri = new RouteString[rm.Steps.Length];
                MainMap.PolylineLocations = new LocationCollection();
                for (int i = 0; i < rm.Steps.Length; i++)
                {
                    ri[i] = new RouteString(rm.Steps[i].Description);
                    var polyLineLocations = new Decoder().Decode(rm.Steps[i].Polyline);
                    foreach (var geoCoordinate in polyLineLocations)
                    {
                        MainMap.PolylineLocations.Add(new Location(geoCoordinate.Latitude, geoCoordinate.Longitude));
                    }
                }
                //MainMap.Center = MainMap.PolylineLocations.First();
                MainMap.ZoomLevel = 15;
                RouteInstrucions = rm.Steps;
            }
            else
            {

                RouteInstrucions = new StepModel[]
                {
                new StepModel()
                {
                    Description = "Nie znaleziono trasy dla podanych adresów."
                }
                };
            }
            FavVM.MainVM.Loading = false;
        }

        private void CenterOnRoute(object sender)
        {
            if (sender is StepModel rm)
            {
                MainMap.Center = new Location(rm.Maneuver.Item1, rm.Maneuver.Item2);
                //TODO ustawić zoom w zależności od dystansu (przedziały metodą prób i błędów)
            }
        }

        private void CenterOnUserLocation(object sender)
        {
            if (sender is UserLocation ul)
            {
                MainMap.Center = MapVM.GetLocation(ul.Address);
                //TODO W zależności od ul.Address.Type można dostosować zoom
            }
        }

        private void SwapAddresses()
        {
            var from = FromAddressessSearch.SelectedLocation;
            FromAddressessSearch.SelectedLocation = ToAddressessSearch.SelectedLocation;
            ToAddressessSearch.SelectedLocation = from;
        }

    }
}
