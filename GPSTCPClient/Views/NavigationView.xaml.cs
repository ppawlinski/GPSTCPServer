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
        }
    }
}
