using GPSTCPClient.ViewModel;
using GPSTCPClient.ViewModel.Components;
using Microsoft.Maps.MapControl.WPF;
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

        private void FavMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ((FavouritesVM)DataContext).MapDoubleClickCommand.Execute(FavMap.ViewportPointToLocation(e.GetPosition(FavMap)));
        }


    }
}
