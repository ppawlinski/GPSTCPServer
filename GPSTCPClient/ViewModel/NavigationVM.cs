using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PolylineEncoder.Net.Utility.Decoders;
using System.Windows.Threading;
using System.Windows.Data;
using System.Diagnostics;

namespace GPSTCPClient.ViewModel
{
    public class NavigationVM : ViewModelBase
    {

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
        public FavouritesVM FavVM { get; set; }
        public NavigationVM(ViewModelBase favVM_)
        {
            FavVM = favVM_ as FavouritesVM;
            ToAddressessSearch = new MixedSearch();
            ToAddressessSearch.OnSelectedAction += ToAddressessSearch_OnSelectedAction;
            FromAddressessSearch = new MixedSearch();
            FromAddressessSearch.OnSelectedAction += FromAddressessSearch_OnSelectedAction;
            MainMap = new MapVM();
            FindRouteCommand = new Command(sender => FindRoute());
            CenterOnRouteCommand = new Command(sender => CenterOnRoute(sender));
            CenterOnUserLocationCommand = new Command(sender => CenterOnUserLocation(sender));
            SwapAddressesCommand = new Command(sender => SwapAddresses());
            SearchLocationEnterClick = new Command(sender => FindRoute());
            FromAddressessSearch.StoredLocations = FavVM.Locations;
            ToAddressessSearch.StoredLocations = FavVM.Locations;
            FavVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Locations")
                {
                    if(FavVM.Locations.Count > 0)
                    {
                        MainMap.MainLoc = new Pin(FavVM.Locations.First());
                        MainMap.Center = MapVM.GetLocation(FavVM.Locations.First().Address);
                    }
                }
            };
        }

        

        private void FromAddressessSearch_OnSelectedAction(object sender, System.EventArgs e)
        {
            if (sender is MixedSearch ms)
            {
                if (!double.IsNaN(ms.SelectedLocation?.Address.Lat ?? double.NaN))
                {
                    MainMap.Center = MapVM.GetLocation(ms.SelectedLocation.Address);
                    MainMap.FromPin = new Pin(ms.SelectedLocation.Address);
                }
            }
        }

        private void ToAddressessSearch_OnSelectedAction(object sender, System.EventArgs e)
        {
            if(sender is MixedSearch ms)
            {
                if (!double.IsNaN(ms.SelectedLocation?.Address.Lat ?? double.NaN))
                {
                    MainMap.Center = MapVM.GetLocation(ms.SelectedLocation.Address);
                    MainMap.ToPin = new Pin(ms.SelectedLocation.Address);
                }
            }
        }

        public ICommand FindRouteCommand { get; set; }
        public ICommand CenterOnRouteCommand { get; set; }
        public ICommand SwapAddressesCommand { get; set; }
        public ICommand CenterOnUserLocationCommand { get; set; }
        public ICommand SearchLocationEnterClick { get; set; }


        private async void FindRoute()
        {
            FavVM.MainVM.Loading = true;
            RouteInstrucions = new StepModel[]
            {
                new StepModel()
                {
                    Description = "WCZYTYWANIE..."
                }
            };
            var rm = await Client.GetRoute(FromAddressessSearch.SelectedLocation.Address, ToAddressessSearch.SelectedLocation.Address);

            RouteString[] ri = new RouteString[rm.Steps.Length];
            MainMap.PolylineLocations = new LocationCollection();
            for (int i = 0; i < rm.Steps.Length; i++)
            {
                ri[i] = new RouteString(rm.Steps[i].Description);
                var polyLineLocations = new Decoder().Decode(rm.Steps[i].Polyline);
                foreach (var geoCoordinate in polyLineLocations)
                {
                    MainMap.PolylineLocations.Add(new Location(geoCoordinate.Latitude,geoCoordinate.Longitude));
                }
            }
            MainMap.Center = MainMap.PolylineLocations.First();
            MainMap.ZoomLevel = 15;
            RouteInstrucions = rm.Steps;
            MainMap.FromPin = new Pin("Początek", MainMap.PolylineLocations.First());
            MainMap.ToPin = new Pin("Koniec", MainMap.PolylineLocations.Last());
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
