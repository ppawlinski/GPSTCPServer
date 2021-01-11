using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System;
using MaterialDesignThemes.Wpf;
using GPSTCPClient.Views;

namespace GPSTCPClient.ViewModel
{
    public class FavouritesVM : ViewModelBase
    {
        public FavouritesVM(MainVM mainVM_)  {
            MainVM = mainVM_;
            FavMap = new MapVM();
            Locations = new ObservableCollection<UserLocation>();
            FavAddressSearch = new AddressesSearch();
            DeleteCommand = new Command(arg => DeleteLocation(arg));
            EditCommand = new Command(arg => EditLocation(arg));
            MapDoubleClickCommand = new Command(arg => MapDoubleClick(arg));
            CenterOnUserLocationCommand = new Command(arg => CenterOnUserLocation(arg));
            AddLocationCommand = new Command(arg => AddLocation());
            ClearAddingCommand = new Command(arg => ClearAdding());
            LoadData();
        }
        private void LoadData()
        {
            Task.Run(async () =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => MainVM.Loading = true);
                return await Client.GetMyAddresses();
            }).ContinueWith(task =>
            {
                Locations.Clear();
                Locations.AddRange(task.Result);
                if(Locations.Count > 0)
                {
                    FavMap.MainLoc = new Pin(Locations.First());
                    FavMap.Center = MapVM.GetLocation(Locations.First().Address);
                }
                OnPropertyChanged(nameof(Locations));

                //if (Locations.Count > 0)
                //{
                //    MainMap.Center = MapVM.GetLocation(Locations.First().Address);
                //    MainMap.MainLoc = new Pin(Locations.First());
                //}
                //else MainMap.MainLoc = new Pin();
                MainVM.Loading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private UserLocation editing;
        public MainVM MainVM { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand MapDoubleClickCommand { get; set; }
        public ICommand AddLocationCommand { get; set; }
        public ICommand CenterOnUserLocationCommand { get; set; }
        public ICommand ClearAddingCommand { get; set; }
        private MapVM favMap;
        public MapVM FavMap
        {
            get
            {
                return favMap;
            }
            set
            {
                favMap = value;
                OnPropertyChanged(nameof(FavMap));
            }
        }

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

        private string dialogContent;
        public string DialogContent {
            get
            {
                return dialogContent;
            }
            set
            {
                dialogContent = value;
                OnPropertyChanged(nameof(DialogContent));
            }
        }

        private async void AddLocation()
        {
            MainVM.Loading = true;
            if (AddingLocationName != "" && !double.IsNaN(FavAddressSearch.SelectedAddress?.Lat ?? double.NaN))
            {
                if(editing == null)
                {
                    if (await Client.AddAddress(FavAddressSearch.SelectedAddress, AddingLocationName))
                    {
                        Locations.Add(new UserLocation(AddingLocationName, FavAddressSearch.SelectedAddress));
                        AddingLocationName = "";
                        FavAddressSearch = new AddressesSearch();
                    }
                }
                else
                {
                    if(editing.Address == FavAddressSearch.SelectedAddress && editing.Name == AddingLocationName)
                    {
                        editing = null;
                        AddingLocationName = "";
                        FavAddressSearch.SelectedAddress = null;
                        FavAddressSearch.SelectedAddressText = "";
                        FavAddressSearch.Addresses.Clear();
                    }
                    else
                    {
                        if(await Client.EditAddress(editing.Name, AddingLocationName, FavAddressSearch.SelectedAddress))
                        {
                            editing = null;
                            AddingLocationName = "";
                            FavAddressSearch.SelectedAddress = null;
                            FavAddressSearch.SelectedAddressText = "";
                            FavAddressSearch.Addresses.Clear();
                            await Task.Run(async () =>
                            {
                                return await Client.GetMyAddresses();
                            }).ContinueWith(task =>
                            {
                                Locations.Clear();
                                Locations.AddRange(task.Result);
                                MainVM.Loading = false;
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                        }
                    }
                }
            }
            else
            {
                editing = null;
                FavAddressSearch.Addresses.Clear();
            }
            MainVM.Loading = false;
            
        }

        private async void DeleteLocation(object selected)
        {
            if(selected != null && selected is UserLocation ul)
            {
                DialogContent = "Czy na pewno chcesz usunąć zapisany adres?";
                string result = (string)await DialogHost.Show(new OkCancelDialog(), "DeleteFavDialog");
                if(result == "Accept")
                {
                    if (await Client.DeleteAddress(ul.Name))
                    {
                        Locations.Remove(ul);
                    }
                }
            }
        }
        private async void MapDoubleClick(object arg)
        {
            if(arg is Location point)
            {
                var described = await Client.DescribeAddress(point.Latitude, point.Longitude);
                FavAddressSearch.Addresses.Clear();
                FavAddressSearch.Addresses.Add(described);
                FavAddressSearch.SelectedAddress = described;
                FavAddressSearch.SelectedAddressText = described.DisplayName;
                FavMap.MainLoc = new Pin(described);
            }
        }

        private void EditLocation(object selected)
        {
            if(selected != null && selected is UserLocation ul)
            {
                editing = ul;
                AddingLocationName = editing.Name;
                FavAddressSearch.Addresses.Clear();
                FavAddressSearch.Addresses.Add(editing.Address);
                FavAddressSearch.SelectedAddress = editing.Address;
                FavAddressSearch.SelectedAddressText = editing.Address?.DisplayName ?? "";
            }
        }
        private void CenterOnUserLocation(object arg)
        {
            if (arg is UserLocation ul)
            {
                FavMap.Center = MapVM.GetLocation(ul.Address);
                //TODO W zależności od ul.Address.Type można dostosować zoom
            }
        }

        private void ClearAdding()
        {
            editing = null;
            AddingLocationName = "";
            FavAddressSearch.Addresses.Clear();
            FavAddressSearch.SelectedAddress = null;
            FavAddressSearch.SelectedAddressText = "";
        }
    }
}
