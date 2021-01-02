﻿using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    public class NavigationVM : ViewModelBase
    {
        private AddressesSearch favAddressSearch;
        public AddressesSearch FavAddressSearch
        {
            get
            {
                return favAddressSearch;
            }
            set
            {
                favAddressSearch = value;
                OnPropertyChanged(nameof(FavAddressSearch));
            }
        }

        private ObservableCollection<UserLocation> locations;
        public ObservableCollection<UserLocation> Locations
        {
            get
            { return locations; }
            set
            {
                locations = value;
                ToAddressessSearch.StoredLocations = value;
                FromAddressessSearch.StoredLocations = value;
                OnPropertyChanged(nameof(Locations));
            }
        }

        private string addingLocationName;
        public string AddingLocationName
        {
            get
            {
                return addingLocationName;
            }
            set
            {
                addingLocationName = value;
                OnPropertyChanged(nameof(AddingLocationName));
            }
        }

        private ObservableCollection<Address> addingLocationsList;
        public ObservableCollection<Address> AddingLocationsList
        {
            get
            {
                return addingLocationsList;
            }
            set
            {
                addingLocationsList = value;
                OnPropertyChanged(nameof(AddingLocationsList));
            }
        }

        private UserLocation selectedFavLocation;
        public UserLocation SelectedFavLocation
        {
            get
            {
                return selectedFavLocation;
            }
            set
            {
                selectedFavLocation = value;
                OnPropertyChanged(nameof(SelectedFavLocation));
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

        private RouteModel[] routeInstructions;
        public RouteModel[] RouteInstrucions
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

        private MapView mainMap;
        public MapView MainMap
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

        public NavigationVM()
        {

            ToAddressessSearch = new MixedSearch();
            ToAddressessSearch.OnSelectedAction += ToAddressessSearch_OnSelectedAction;
            FromAddressessSearch = new MixedSearch();
            FromAddressessSearch.OnSelectedAction += FromAddressessSearch_OnSelectedAction;
            MainMap = new MapView();
            Locations = new ObservableCollection<UserLocation>(new UserLocation[] { new UserLocation("WCZYTYWANIE...") });
            Task.Run(async () =>
            {
                Locations = new ObservableCollection<UserLocation>(await Client.GetMyAddresses());

                if (Locations.Count > 0)
                {
                    MainMap.Center = MapView.GetLocation(Locations.First().Address);
                    MainMap.MainLoc = new Pin(Locations.First());
                }
                else MainMap.MainLoc = new Pin();
                
                //MainMap.FillWithFavs(Locations);
            });
            AddingLocationsList = new ObservableCollection<Address>();
            AddLocationCommand = new Command(sender => AddLocation());
            DelLocationCommand = new Command(sender => DelLocation());
            FindRouteCommand = new Command(sender => FindRoute());
            CenterOnRouteCommand = new Command(sender => CenterOnRoute(sender));
            CenterOnUserLocationCommand = new Command(sender => CenterOnUserLocation(sender));
            SwapAddressesCommand = new Command(sender => SwapAddresses());
            SearchLocationEnterClick = new Command(sender => FindRoute());
            AddLocationEnterClick = new Command(sender => AddLocation());
            FavAddressSearch = new AddressesSearch();
            MainMap.FromPin = new Pin();
            MainMap.ToPin = new Pin();
        }

        private void FromAddressessSearch_OnSelectedAction(object sender, System.EventArgs e)
        {
            if (sender is MixedSearch ms)
            {
                if (!String.IsNullOrEmpty(ms.SelectedLocation?.Address.Lat))
                {
                    MainMap.Center = MapView.GetLocation(ms.SelectedLocation.Address);
                    MainMap.FromPin = new Pin(ms.SelectedLocation.Address);
                }
            }
        }

        private void ToAddressessSearch_OnSelectedAction(object sender, System.EventArgs e)
        {
            if(sender is MixedSearch ms)
            {
                if (!String.IsNullOrEmpty(ms.SelectedLocation.Address.Lat))
                {
                    MainMap.Center = MapView.GetLocation(ms.SelectedLocation.Address);
                    MainMap.ToPin = new Pin(ms.SelectedLocation.Address);
                }
            }
        }

        public ICommand AddLocationCommand { get; set; }
        public ICommand DelLocationCommand { get; set; }
        public ICommand FindRouteCommand { get; set; }
        public ICommand CenterOnRouteCommand { get; set; }
        public ICommand SwapAddressesCommand { get; set; }
        public ICommand CenterOnUserLocationCommand { get; set; }
        public ICommand SearchLocationEnterClick { get; set; }
        public ICommand AddLocationEnterClick { get; set; }

        private async void AddLocation()
        {
            if (await Client.AddAddress(FavAddressSearch.SelectedAddress, AddingLocationName))
            {
                Locations.Add(new UserLocation(AddingLocationName, FavAddressSearch.SelectedAddress));
                AddingLocationName = "";
                FavAddressSearch = new AddressesSearch();
                if(Locations.Count == 1)
                {
                    MainMap.MainLoc = new Pin(Locations.First());
                }
            }
        }

        private async void DelLocation()
        {
            if (SelectedFavLocation == null) return;
            if (await Client.DeleteAddress(SelectedFavLocation.Name))
            {
                Locations.Remove(SelectedFavLocation);
                if(Locations.Count == 0)
                {
                    MainMap.MainLoc = new Pin();
                }
            }
        }

        private async void FindRoute()
        {
            RouteInstrucions = new RouteModel[]
            {
                new RouteModel()
                {
                    Description = "WCZYTYWANIE..."
                }
            };
            var rm = await Client.GetRoute(FromAddressessSearch.SelectedLocation.Address, ToAddressessSearch.SelectedLocation.Address);

            RouteString[] ri = new RouteString[rm.Length];
            MainMap.PolylineLocations = new LocationCollection();
            for (int i = 0; i < rm.Length; i++)
            {
                ri[i] = new RouteString(rm[i].Description);
                foreach (var tupl in rm[i].Intersections)
                {
                    MainMap.PolylineLocations.Add(new Location(tupl.Item1, tupl.Item2));
                }
            }
            MainMap.Center = MainMap.PolylineLocations.First();
            MainMap.ZoomLevel = 15;
            RouteInstrucions = rm;
            MainMap.FromPin = new Pin("Początek", MainMap.PolylineLocations.First());
            MainMap.ToPin = new Pin("Koniec", MainMap.PolylineLocations.Last());
        }

        private void CenterOnRoute(object sender)
        {
            if (sender is RouteModel rm)
            {
                MainMap.Center = new Location(rm.Maneuver.Item1, rm.Maneuver.Item2);
                //TODO ustawić zoom w zależności od dystansu (przedziały metodą prób i błędów)
            }
        }

        private void CenterOnUserLocation(object sender)
        {
            if (sender is UserLocation ul)
            {
                MainMap.Center = MapView.GetLocation(ul.Address);
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
