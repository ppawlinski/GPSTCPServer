using System;
using System.Collections.Generic;
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

namespace GPSTCPClient.Pages
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Page
    {
        private NavigationService navService;
        private Page prevPage;
        public Map(NavigationService nav, Page prevP)
        {
            InitializeComponent();
            navService = nav;
            prevPage = prevP;
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
                    navService.GoBack();
                });
            });

            
        }
    }
}
