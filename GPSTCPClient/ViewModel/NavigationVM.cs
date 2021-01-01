using GPSTCPClient.Models;
using GPSTCPClient.View;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
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

        private ObservableCollection<UserLocation> locations;

        private string addingLocationName;

        private ObservableCollection<Address> addingLocationsList;

        private UserLocation selectedFavLocation;


        private MixedSearch toAddressessSearch;
        public MixedSearch ToAddressessSearch {
            get
            {
                return toAddressessSearch;
            }
            set
            {
                toAddressessSearch = value;
                OnPropertyChanged(nameof(ToAddressessSearch));
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
                OnPropertyChanged(nameof(FromAddressessSearch));
            }
        }

        private RouteString[] routeInstructions;
        public RouteString[] RouteInstrucions
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

        public NavigationVM()
        {
            ToAddressessSearch = new MixedSearch();
            FromAddressessSearch = new MixedSearch();
            Locations = new ObservableCollection<UserLocation>();
            Task.Run(async () => Locations = new ObservableCollection<UserLocation>(await Client.GetMyAddresses()));
            AddingLocationsList = new ObservableCollection<Address>();
            AddLocationCommand = new Command(sender => AddLocation());
            DelLocationCommand = new Command(sender => DelLocation());
            FindRouteCommand = new Command(sender => FindRoute());
            FavAddressSearch = new AddressesSearch();
        }

        public ICommand AddLocationCommand { get; set; }

        public ICommand DelLocationCommand { get; set; }
        public ICommand FindRouteCommand { get; set; }

        private async void AddLocation()
        {
            if (await Client.AddAddress(FavAddressSearch.SelectedAddress, AddingLocationName))
            {
                Locations.Add(new UserLocation(AddingLocationName, FavAddressSearch.SelectedAddress));
                AddingLocationName = "";
                FavAddressSearch = new AddressesSearch();
            }
        }

        private async void DelLocation()
        {
            if (await Client.DeleteAddress(SelectedFavLocation.Name))
            {
                Locations.Remove(SelectedFavLocation);
            }
        }

        private async void FindRoute()
        {
            RouteInstrucions = new RouteString[] { new RouteString("WCZYTYWANIE...") };
            var strings = await Client.GetRoute(FromAddressessSearch.SelectedLocation.Address, ToAddressessSearch.SelectedLocation.Address);
            
            RouteString[] ri = new RouteString[strings.Length];
            for(int i=0; i<strings.Length; i++)
            {
                ri[i] = new RouteString(strings[i]);
            }
            RouteInstrucions = ri;
        }
    }
}
