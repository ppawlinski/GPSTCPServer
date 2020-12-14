using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BruTile.Predefined;
using GPSTCPClient.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets.ScaleBar;
using Newtonsoft.Json;

namespace GPSTCPClient.Pages
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Navigation : Page
    {
        private NavigationService navService;
        private Page prevPage;
        //private UserLocation[] locations;
        //private List<UserLocation> history;
        //public UserLocation[] UserLocations { 
        //    get { 
        //        return locations; }  }

        //private Map CreateMap()
        //{
        //    var map = new Map()
        //    {
        //        CRS = "EPSG:3857",
        //        Transformation = new MinimalTransformation()
        //    };

        //    map.Layers.Add(OpenStreetMap.CreateTileLayer());
        //    map.Layers.Add(CreatePinLayer());
        //    map.Home = n => n.NavigateTo(map.Layers[1].Envelope.Centroid, map.Resolutions[5]);
        //    map.Widgets.Add(new ScaleBarWidget(map) { TextAlignment = Mapsui.Widgets.Alignment.Center, HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Center, VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Top });
        //    map.Widgets.Add(new Mapsui.Widgets.Zoom.ZoomInOutWidget { MarginX = 20, MarginY = 40 });
        //    map.Widgets.Add(new Mapsui.Widgets.Hyperlink() { MarginX = 5, MarginY = 1 });

        //    return map;
        //}

        //private MemoryLayer CreatePinLayer()
        //{
        //    return new MemoryLayer
        //    {
        //        Name = "Points",
        //        IsMapInfoLayer = true,
        //        DataSource = new MemoryProvider(GetCitiesFromEmbeddedResource()),
        //        Style = CreateBitmapStyle()
        //    };
        //}

        public Navigation(NavigationService nav, Page prevP)
        {

            InitializeComponent();

            //MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create(KnownTileSource.OpenStreetMap,userAgent: "TCPServerProject v1.0")));


            navService = nav;
            prevPage = prevP;
            //Task.Run(async () =>
            //{
            //    await Client.GetMyAddresses().ContinueWith((t) =>
            //    {
            //        locations = (t.Result as List<UserLocation>).ToArray();
            //        Dispatcher.Invoke(() =>
            //        {

            //            FromAddressCB.ItemsSource = t.Result as List<UserLocation>;
            //            ToAddressCB.ItemsSource = t.Result as List<UserLocation>;
            //            MyAddressesDG.ItemsSource = t.Result as List<UserLocation>;

            //        });

            //    });

            //}).ContinueWith((t) =>
            //{
            //    MapControl.Map = CreateMap();
            //});

            //history = new List<UserLocation>();

        }


        //private void Logout_Click(object sender, RoutedEventArgs e)
        //{
        //    Task.Run(async () =>
        //    {
        //        await Client.Logout();
        //        Client.Disconnect();
        //    }).ContinueWith((t) =>
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            if (prevPage is Login l)
        //            {
        //                l.LoginErrors.Text = String.Empty;
        //            }
        //            navService.GoBack();
        //        });
        //    });
        //}

        //private void Szukaj_Click(object sender, RoutedEventArgs e)
        //{


        //    if (FromAddressCB.SelectedItem == null || ToAddressCB.SelectedItem == null)
        //    {

        //    }
        //    else
        //    {
        //        Address fromAddres = ((UserLocation)FromAddressCB.SelectedItem).Address;
        //        Address toAddres = ((UserLocation)ToAddressCB.SelectedItem).Address;
        //        string[] instructions = null;
        //        Task.Run(async () =>
        //        {
        //            instructions = await Client.GetRoute(fromAddres, toAddres);
        //            List<RouteString> routeStrings = new List<RouteString>();
        //            foreach (string instr in instructions)
        //            {
        //                routeStrings.Add(new RouteString(instr));
        //            }
        //            Dispatcher.Invoke(() => Routes.ItemsSource = routeStrings);
        //        });
        //    }
        //}

        //private void FromAddressCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (sender is not null && sender is ComboBox cb)
        //    {
        //        if (cb.SelectedItem is UserLocation ul)
        //        {
        //            if(!locations.Contains(ul)) history.Add(ul);
        //        }

        //    }
        //    FromAddressCB.ItemsSource = locations.Union(history);
        //}

        //private void ToAddressCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (sender is not null && sender is ComboBox cb)
        //    {
        //        if (cb.SelectedItem is UserLocation ul)
        //        {
        //            history.Add(ul);
        //        }

        //    }
        //    ToAddressCB.ItemsSource = locations.Union(history).Distinct();
        //}

        //private List<UserLocation> toLocations(IEnumerable<Address> addresses)
        //{
        //    List<UserLocation> ulList = new List<UserLocation>();
        //    foreach (var a in addresses)
        //    {
        //        ulList.Add(new UserLocation()
        //        {
        //            Name = "",
        //            Address = a
        //        });
        //    }
        //    return ulList;
        //}

        //private void Dodaj_Click(object sender, RoutedEventArgs e)
        //{
        //    Address address = NewAddressCB.SelectedItem as Address;
        //    if (address == null) return;
        //    string name = NewAddressNameTB.Text;
        //    Task.Run(async () =>
        //    {
        //        if(await Client.AddAddress(address, name))
        //        {
        //            List<UserLocation> uL = await Client.GetMyAddresses();
        //            locations = uL.ToArray();
        //            Dispatcher.Invoke(() =>
        //            {
        //                NewAddressCB.Text = "";
        //                NewAddressNameTB.Text = "";
        //                MyAddressesDG.ItemsSource = uL;
        //                FromAddressCB.ItemsSource = uL;
        //                ToAddressCB.ItemsSource = uL;
        //                NewAddressCB.ItemsSource = null;
        //            });
        //        }

        //    });
        //}

        //private void Usun_Click(object sender, RoutedEventArgs e)
        //{
        //    string[] names = MyAddressesDG.SelectedCells.Select(p => p.Item).Select(p => ((UserLocation)p).Name).ToArray();
        //    Task.Run(async () =>
        //    {
        //        foreach (var name in names)
        //        {
        //            await Client.DeleteAddress(name);
        //        }
        //        List<UserLocation> uL = await Client.GetMyAddresses();
        //        locations = uL.ToArray();
        //        Dispatcher.Invoke(() =>
        //        {
        //            NewAddressCB.Text = "";
        //            NewAddressNameTB.Text = "";
        //            MyAddressesDG.ItemsSource = uL;
        //            FromAddressCB.ItemsSource = uL;
        //            ToAddressCB.ItemsSource = uL;
        //        });
        //    });
        //}



        //private void NewAddressCB_DropDownOpened(object sender, EventArgs e)
        //{
        //    string text = NewAddressCB.Text;
        //    if (text.Length < 3) return;
        //    Task.Run(async () =>
        //    {
        //        Address[] addresses = await Client.GetAddress(text);
        //        Dispatcher.Invoke(() =>
        //        {
        //            NewAddressCB.ItemsSource = addresses;
        //            //foreach (var address in addresses)
        //            //    NewAddressesSource.Add(address);
        //        });

        //    });
        //}

        //private void FromAddressCB_DropDownOpened(object sender, EventArgs e)
        //{
        //    string from = FromAddressCB.Text;
        //    Task.Run(async () =>
        //    {
        //        Address[] address = await Client.GetAddress(from);
        //        if (address.Length == 1)
        //        {
        //            UserLocation resLocation = new UserLocation()
        //            {
        //                Name = "",
        //                Address = address[0]
        //            };
        //            Dispatcher.Invoke(() =>
        //            {
        //                FromAddressCB.ItemsSource = locations.Union(new UserLocation[] { resLocation });

        //                FromAddressCB.SelectedItem = resLocation;
        //            });
        //        }
        //        if (address.Length > 1)
        //        {
        //            Dispatcher.Invoke(() =>
        //            {
        //                FromAddressCB.ItemsSource = toLocations(address);
        //            });
        //        }
        //    });
        //}

        //private void ToAddressCB_DropDownOpened(object sender, EventArgs e)
        //{
        //    string to = ToAddressCB.Text;
        //    Task.Run(async () =>
        //    {
        //        Address[] address = await Client.GetAddress(to);
        //        if (address.Length == 1)
        //        {
        //            UserLocation resLocation = new UserLocation()
        //            {
        //                Name = "",
        //                Address = address[0]
        //            };
        //            Dispatcher.Invoke(() =>
        //            {
        //                ToAddressCB.ItemsSource = locations.Union(new UserLocation[] { resLocation });

        //                ToAddressCB.SelectedItem = resLocation;
        //            });
        //        }
        //        if (address.Length > 1)
        //        {
        //            Dispatcher.Invoke(() =>
        //            {
        //                ToAddressCB.ItemsSource = toLocations(address);
        //            });
        //        }
        //    });
        //}

        //private void ChangePassword_Click(object sender, EventArgs e)
        //{
        //    navService.Navigate(new ChangePassword(navService));
        //}

        //private static MemoryLayer CreatePointLayer()
        //{
        //    return new MemoryLayer
        //    {
        //        Name = "Pins",
        //        IsMapInfoLayer = true,
        //        DataSource = new MemoryProvider(),
        //        Style = CreateBitmapStyle()
        //    };
        //}

        //private static SymbolStyle CreateBitmapStyle()
        //{
        //    var path = "..\\..\\..\\Icons\\pin_small.png"; 
        //    var bitmapId = GetBitmapIdForEmbeddedResource(path);
        //    var bitmapHeight = 176;
        //    return new SymbolStyle { BitmapId = bitmapId, SymbolScale = 0.20, SymbolOffset = new Offset(0, bitmapHeight * 0.5) };
        //}

        //private static int GetBitmapIdForEmbeddedResource(string imagePath)
        //{
        //    var image = new FileStream(imagePath, FileMode.Open);
        //    return BitmapRegistry.Instance.Register(image);
        //}

        //private IEnumerable<IFeature> GetCitiesFromEmbeddedResource()
        //{

        //    return UserLocations.Select(c =>
        //    {
        //        var feature = new Feature();
        //        var point = SphericalMercator.FromLonLat(double.Parse(c.Address.Lon, new CultureInfo("en-US", false).NumberFormat), double.Parse(c.Address.Lat, new CultureInfo("en-US", false).NumberFormat));
        //        feature.Geometry = point;
        //        feature["name"] = c.Name;
        //        return feature;
        //    });
        //}

        //private class City
        //{
        //    public string Country { get; set; }
        //    public string Name { get; set; }
        //    public double Lat { get; set; }
        //    public double Lng { get; set; }
        //}

        //public static IEnumerable<T> DeserializeFromStream<T>(Stream stream)
        //{
        //    var serializer = new JsonSerializer();

        //    using (var sr = new StreamReader(stream))
        //    using (var jsonTextReader = new JsonTextReader(sr))
        //    {
        //        return serializer.Deserialize<List<T>>(jsonTextReader);
        //    }
        //}
    }
}
