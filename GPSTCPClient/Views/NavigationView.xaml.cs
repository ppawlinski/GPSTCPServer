using GPSTCPClient.ViewModel;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;

namespace GPSTCPClient.Views
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            InitializeComponent();
            MainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(GPSTCPClient.Client.ApiKey);
            MainMap.MouseDoubleClick += (o, e) =>
            {
                e.Handled = true;
                ((NavigationVM)DataContext).MainMap.MapDoubleClickCommand.Execute(MainMap.ViewportPointToLocation(e.GetPosition(MainMap)));
            };
            this.Loaded += (s, e) =>
            {
                var locs = ((NavigationVM)DataContext).FavVM.Locations;
                var customPins = ((NavigationVM)DataContext).MainMap.Pins;
                foreach (var loc in locs) MainMap.Children.Add(loc.Pin);
                foreach (var pin in customPins) MainMap.Children.Add(new Pushpin() { Location = pin.Location, ToolTip = pin.ToolTip });
            };
        }
    }
}
