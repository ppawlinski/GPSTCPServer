using GPSTCPClient.ViewModel;
using GPSTCPClient.ViewModel.Components;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GPSTCPClient.Views
{
    /// <summary>
    /// Interaction logic for FavouritesView.xaml
    /// </summary>
    public partial class FavouritesView : UserControl
    {

        public FavouritesView()
        {
            InitializeComponent();
            FavMap.CredentialsProvider = new ApplicationIdCredentialsProvider(GPSTCPClient.Client.ApiKey);
            FavMap.MouseDoubleClick += (o, e) =>
            {
                e.Handled = true;
                ((FavouritesVM)DataContext).MapDoubleClickCommand.Execute(FavMap.ViewportPointToLocation(e.GetPosition(FavMap)));
            };
            this.Loaded += (s, e) =>
            {
                var locs = ((FavouritesVM)DataContext).Locations;
                var customPins = ((FavouritesVM)DataContext).Pins;
                foreach (var loc in locs) {
                    Pushpin np = new Pushpin() { Location = loc.Pin.Location, Content = loc.Pin.Content };
                    ToolTip tt = new ToolTip();
                    tt.Background = new SolidColorBrush(Color.FromRgb(0x42, 0x42, 0x42));
                    tt.Foreground = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xf3));
                    tt.Content = loc.Name;
                    np.ToolTip = tt;
                    FavMap.Children.Add(np);
                }
                foreach (var pin in customPins) {
                    Pushpin np = new Pushpin() { Location = pin.Location, Content = pin.Content };
                    if (pin.ToolTip != null)
                    {
                        ToolTip tt = new ToolTip();
                        tt.Background = new SolidColorBrush(Color.FromRgb(0x42, 0x42, 0x42));
                        tt.Foreground = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xf3));
                        tt.Content = pin.ToolTip.ToString();
                        np.ToolTip = tt;
                    }
                    FavMap.Children.Add(np);
                } 
            };
        }

        private void MyAddressesDG_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            UserLocation ul = e.Row.DataContext as UserLocation;
            e.Cancel = (ul.Name != "");
        }

        private void MyAddressesDG_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            UserLocation ul = e.Row.DataContext as UserLocation;
            e.Cancel = (ul.Cords != "0: 0");
        }


    }
}
