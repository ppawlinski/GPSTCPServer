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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace GPSTCPClient.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private NavigationService navService;
        public Login(NavigationService nav)
        {
            InitializeComponent();
            ServerAddressBox.Text = "127.0.0.1";
            ServerPortBox.Text = "2048";
            LoginBox.Text = "text";
            PasswordBox.Password = "test";
            navService = nav;
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(async () => await Client.Connect(ServerAddressBox.Text, int.Parse(ServerPortBox.Text))).Wait();
            } catch(FormatException)
            {
                MessageBox.Show("Błędny format portu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(PasswordBox.Password));
                try
                {
                    //TODO loguj w zależności od odpwiedzi z bool Client.Login()
                    Task.Run(async () =>
                    {
                        await Client.Login(LoginBox.Text, Convert.ToBase64String(data));
                    }).ContinueWith((t) =>
                    {
                        md5.Dispose();
                        Dispatcher.Invoke(() =>
                        {
                            navService.Navigate(new Map(navService, this));
                        });
                    });        
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    md5.Dispose();
                    return;
                }
            }


            //Map mapPage = new Map();
            //this.Content = mapPage;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            
            //GPSTCPClient.Pages.Register registerPage = new Pages.Register(this);
            navService.Navigate(new Register(navService,this));
        }
    }
}
