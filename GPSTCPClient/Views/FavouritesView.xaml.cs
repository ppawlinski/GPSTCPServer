using GPSTCPClient.ViewModel;
using GPSTCPClient.ViewModel.Components;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows.Input;

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
                var customPins = ((FavouritesVM)DataContext).FavMap.Pins;
                foreach (var loc in locs) FavMap.Children.Add(loc.Pin);
                foreach (var pin in customPins) FavMap.Children.Add(new Pushpin() { Location = pin.Location, ToolTip = pin.ToolTip });
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
