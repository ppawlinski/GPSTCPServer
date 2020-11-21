using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace GPSTCPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        TcpClient client;
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                    client = new TcpClient();
                    client.Connect(ServerAddressBox.Text, int.Parse(ServerPortBox.Text));
                    client.GetStream().Write(Encoding.UTF8.GetBytes("HELLO"));
               
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            //Map mapPage = new Map();
            //this.Content = mapPage;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
