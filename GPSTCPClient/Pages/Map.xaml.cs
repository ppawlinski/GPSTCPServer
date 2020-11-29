using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using GPSTCPClient.Models;

namespace GPSTCPClient.Pages
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Page
    {
        private NavigationService navService;
        private Page prevPage;
        private UserLocation[] locations;
        private List<UserLocation> history;
        public UserLocation[] UserLocations { 
            get { 
                return locations; }  }

        public Map(NavigationService nav, Page prevP)
        {
            InitializeComponent();
            navService = nav;
            prevPage = prevP;
            List<UserLocation> userLocations = new List<UserLocation>();
            Task.Run(async () =>
            {
                userLocations = await Client.GetMyAddresses();
            }).Wait();
            FromAddressCB.ItemsSource = userLocations;
            ToAddressCB.ItemsSource = userLocations;
            locations = userLocations.ToArray();
            history = new List<UserLocation>();
            MyAddressesDG.ItemsSource = UserLocations;
        }


        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Client.Logout();
                Client.Disconnect();
            }).ContinueWith((t) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (prevPage is Login l)
                    {
                        l.LoginErrors.Text = String.Empty;
                    }
                    navService.GoBack();
                });
            });
        }

        private void Szukaj_Click(object sender, RoutedEventArgs e)
        {
            
            
            if (FromAddressCB.SelectedItem == null || ToAddressCB.SelectedItem == null)
            {
                
            }
            else
            {
                Address fromAddres = ((UserLocation)FromAddressCB.SelectedItem).Address;
                Address toAddres = ((UserLocation)ToAddressCB.SelectedItem).Address;
                string[] instructions = null;
                Task.Run(async () =>
                {
                    instructions = await Client.GetRoute(fromAddres, toAddres);
                    List<RouteString> routeStrings = new List<RouteString>();
                    foreach (string instr in instructions)
                    {
                        routeStrings.Add(new RouteString(instr));
                    }
                    Dispatcher.Invoke(() => Routes.ItemsSource = routeStrings);
                });
            }
        }

        private void FromAddressCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not null && sender is ComboBox cb)
            {
                if (cb.SelectedItem is UserLocation ul)
                {
                    if(!locations.Contains(ul)) history.Add(ul);
                }

            }
            FromAddressCB.ItemsSource = locations.Union(history);
        }

        private void ToAddressCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not null && sender is ComboBox cb)
            {
                if (cb.SelectedItem is UserLocation ul)
                {
                    history.Add(ul);
                }

            }
            ToAddressCB.ItemsSource = locations.Union(history).Distinct();
        }

        private List<UserLocation> toLocations(IEnumerable<Address> addresses)
        {
            List<UserLocation> ulList = new List<UserLocation>();
            foreach (var a in addresses)
            {
                ulList.Add(new UserLocation()
                {
                    Name = "",
                    Address = a
                });
            }
            return ulList;
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            Address address = NewAddressCB.SelectedItem as Address;
            if (address == null) return;
            string name = NewAddressNameTB.Text;
            Task.Run(async () =>
            {
                if(await Client.AddAddress(address, name))
                {
                    List<UserLocation> uL = await Client.GetMyAddresses();
                    locations = uL.ToArray();
                    Dispatcher.Invoke(() =>
                    {
                        NewAddressCB.Text = "";
                        NewAddressNameTB.Text = "";
                        MyAddressesDG.ItemsSource = uL;
                        FromAddressCB.ItemsSource = uL;
                        ToAddressCB.ItemsSource = uL;
                        NewAddressCB.ItemsSource = null;
                    });
                }

            });
        }

        private void Usun_Click(object sender, RoutedEventArgs e)
        {
            string[] names = MyAddressesDG.SelectedCells.Select(p => p.Item).Select(p => ((UserLocation)p).Name).ToArray();
            Task.Run(async () =>
            {
                foreach (var name in names)
                {
                    await Client.DeleteAddress(name);
                }
                List<UserLocation> uL = await Client.GetMyAddresses();
                locations = uL.ToArray();
                Dispatcher.Invoke(() =>
                {
                    NewAddressCB.Text = "";
                    NewAddressNameTB.Text = "";
                    MyAddressesDG.ItemsSource = uL;
                    FromAddressCB.ItemsSource = uL;
                    ToAddressCB.ItemsSource = uL;
                });
            });
        }



        private void NewAddressCB_DropDownOpened(object sender, EventArgs e)
        {
            string text = NewAddressCB.Text;
            if (text.Length < 3) return;
            Task.Run(async () =>
            {
                Address[] addresses = await Client.GetAddress(text);
                Dispatcher.Invoke(() =>
                {
                    NewAddressCB.ItemsSource = addresses;
                    //foreach (var address in addresses)
                    //    NewAddressesSource.Add(address);
                });

            });
        }

        private void FromAddressCB_DropDownOpened(object sender, EventArgs e)
        {
            string from = FromAddressCB.Text;
            Task.Run(async () =>
            {
                Address[] address = await Client.GetAddress(from);
                if (address.Length == 1)
                {
                    UserLocation resLocation = new UserLocation()
                    {
                        Name = "",
                        Address = address[0]
                    };
                    Dispatcher.Invoke(() =>
                    {
                        FromAddressCB.ItemsSource = locations.Union(new UserLocation[] { resLocation });

                        FromAddressCB.SelectedItem = resLocation;
                    });
                }
                if (address.Length > 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        FromAddressCB.ItemsSource = toLocations(address);
                    });
                }
            });
        }

        private void ToAddressCB_DropDownOpened(object sender, EventArgs e)
        {
            string to = ToAddressCB.Text;
            Task.Run(async () =>
            {
                Address[] address = await Client.GetAddress(to);
                if (address.Length == 1)
                {
                    UserLocation resLocation = new UserLocation()
                    {
                        Name = "",
                        Address = address[0]
                    };
                    Dispatcher.Invoke(() =>
                    {
                        ToAddressCB.ItemsSource = locations.Union(new UserLocation[] { resLocation });

                        ToAddressCB.SelectedItem = resLocation;
                    });
                }
                if (address.Length > 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ToAddressCB.ItemsSource = toLocations(address);
                    });
                }
            });
        }
    }
}
