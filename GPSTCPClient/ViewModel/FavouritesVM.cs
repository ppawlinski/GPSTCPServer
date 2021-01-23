using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using MaterialDesignThemes.Wpf;
using GPSTCPClient.Views;
using System.Collections.Generic;
using System;

namespace GPSTCPClient.ViewModel
{
    public class FavouritesVM : ViewModelBase
    {
        public FavouritesVM(MainVM mainVM_)
        {
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
            Pins = new ObservableCollection<Pushpin>();
            FavAddressSearch.OnSelectedAction += FavAddressSearch_OnSelectedAction;
            LoadData();
        }

        private void FavAddressSearch_OnSelectedAction(object sender, EventArgs e)
        {
            if (sender is AddressesSearch asS)
            {
                if (asS.SelectedAddress == null) return;
                var currPin = Pins?.FirstOrDefault();
                if (currPin != null)
                {
                    Pins.Clear();
                    currPin.Location = new Location(asS.SelectedAddress.Lat, asS.SelectedAddress.Lon);
                    Pins.Add(currPin);
                }
                else
                {
                    if (!Locations.Any(p => p.Address == asS.SelectedAddress))
                    {
                        Pins.Clear();
                        Pins.Add(new Pushpin() { Location = new Location(asS.SelectedAddress.Lat, asS.SelectedAddress.Lon), Content = Tools.CreateIcon((Editing == null) ? PackIconKind.Plus : PackIconKind.Edit) });
                    }
                }
                if (!(currPin != null && (Math.Abs(currPin.Location.Latitude - asS.SelectedAddress.Lat) < 1 || Math.Abs(currPin.Location.Longitude - asS.SelectedAddress.Lon) < 1))) FavMap.Center = new Location(asS.SelectedAddress.Lat, asS.SelectedAddress.Lon);
            }
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
                if (Locations.Count > 0)
                {
                    FavMap.Center = MapVM.GetLocation(Locations.First().Address);
                }
                else
                {
                    FavMap.Center = new Location(52.0, 19.0);
                    FavMap.ZoomLevel = 5;
                }
                OnPropertyChanged(nameof(Locations));
                MainVM.Loading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private UserLocation editing;
        public UserLocation Editing
        {
            get
            {
                return editing;
            }
            set
            {
                editing = value;
                var currPin = Pins?.FirstOrDefault();
                if (currPin != null)
                {
                    Pins.Clear();
                    if(value == null) currPin.Content = Tools.CreateIcon(PackIconKind.Plus);
                    else currPin.Content = Tools.CreateIcon(PackIconKind.Edit);
                    Pins.Add(currPin);
                }

            }
        }
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
        

        public string DialogContent
        {
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

        private async void AddLocation()
        {
            MainVM.Loading = true;
            if (AddingLocationName != "" && !double.IsNaN(FavAddressSearch.SelectedAddress?.Lat ?? double.NaN))
            {
                if (Editing == null)
                {
                    if (await Client.AddAddress(FavAddressSearch.SelectedAddress, AddingLocationName))
                    {
                        Locations.Add(new UserLocation(AddingLocationName, FavAddressSearch.SelectedAddress));
                        AddingLocationName = "";
                        FavAddressSearch.SelectedAddress = null;
                        FavAddressSearch.SelectedAddressText = "";
                        FavAddressSearch.Addresses.Clear();
                    }
                }
                else
                {
                    if (Editing.Address == FavAddressSearch.SelectedAddress && Editing.Name == AddingLocationName)
                    {
                        Editing = null;
                        AddingLocationName = "";
                        FavAddressSearch.SelectedAddress = null;
                        FavAddressSearch.SelectedAddressText = "";
                        FavAddressSearch.Addresses.Clear();
                    }
                    else
                    {
                        if (await Client.EditAddress(Editing.Name, AddingLocationName, FavAddressSearch.SelectedAddress))
                        {
                            Editing = null;
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
                Editing = null;
                FavAddressSearch.Addresses.Clear();
            }
            MainVM.Loading = false;

        }

        private async void DeleteLocation(object selected)
        {
            if (selected != null && selected is UserLocation ul)
            {
                DialogContent = "Czy na pewno chcesz usunąć zapisany adres?";
                string result = (string)await DialogHost.Show(new OkCancelDialog(), "DeleteFavDialog");
                if (result == "Accept")
                {
                    if (await Client.DeleteAddress(ul.Name))
                    {
                        Locations.Remove(ul);
                        var currPin = Pins.FirstOrDefault();
                        Pins.Clear();
                        if (currPin != null) Pins.Add(currPin);
                    }
                }
            }
        }
        private async void MapDoubleClick(object arg)
        {
            if (arg is Location point)
            {
                var described = await Client.DescribeAddress(point.Latitude, point.Longitude);
                
                Pins.Clear();
                if(Editing == null)
                {
                    if (described.Lat != 0 && described.Lon != 0)
                    {
                        Pins.Add(new Pushpin() { Location = new Location(described.Lat, described.Lon), Content = Tools.CreateIcon(PackIconKind.Plus) });
                    }
                } else
                {
                    if (described.Lat != 0 && described.Lon != 0)
                    {
                        Pins.Add(new Pushpin() { Location = new Location(described.Lat, described.Lon), ToolTip = this.AddingLocationName, Content = Tools.CreateIcon(PackIconKind.Edit)});
                    }
                }
                FavAddressSearch.Addresses.Clear();
                FavAddressSearch.Addresses.Add(described);
                FavAddressSearch.SelectedAddress = described;
                FavAddressSearch.SelectedAddressText = described.DisplayName;

            }
        }


        private void EditLocation(object selected)
        {
            if (selected != null && selected is UserLocation ul)
            {
                Editing = ul;
                AddingLocationName = Editing.Name;
                FavAddressSearch.Addresses.Clear();
                FavAddressSearch.Addresses.Add(Editing.Address);
                FavAddressSearch.SelectedAddress = Editing.Address;
                FavAddressSearch.SelectedAddressText = Editing.Address?.DisplayName ?? "";
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
            Editing = null;
            AddingLocationName = "";
            FavAddressSearch.Addresses.Clear();
            FavAddressSearch.SelectedAddress = null;
            FavAddressSearch.SelectedAddressText = "";
            Pins.Clear();
        }
    }
}
