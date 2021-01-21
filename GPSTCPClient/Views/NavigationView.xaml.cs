using GPSTCPClient.ViewModel;
using Microsoft.Maps.MapControl.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                ((NavigationVM)DataContext).MapDoubleClickCommand.Execute(MainMap.ViewportPointToLocation(e.GetPosition(MainMap)));
            };
            this.Loaded += (s, e) =>
            {
                var nvm = (NavigationVM)DataContext;
                var locs = nvm.FavVM.Locations;
                var customPins = nvm.Pins;
                foreach (var loc in locs) MainMap.Children.Add(loc.Pin);
                foreach (var pin in customPins) MainMap.Children.Add(new Pushpin() {Content = pin.Content, Location = pin.Location, ToolTip = pin.ToolTip });
            };
        }

    }
}
