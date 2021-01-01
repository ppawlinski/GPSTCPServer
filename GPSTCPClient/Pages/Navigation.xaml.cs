using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;


namespace GPSTCPClient.Pages
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Navigation : Page
    {
        public Navigation()
        {
            InitializeComponent();
            MainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(GPSTCPClient.Client.ApiKey);
        }
    }
        
}
