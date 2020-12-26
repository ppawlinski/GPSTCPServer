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
            LoginBox.Text = "test";
            PasswordBox.Password = "123";
            navService = nav;
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(async () =>
                {
                    string ip = null;
                    int port = 0;
                    Dispatcher.Invoke(() =>
                    {
                        ip = ServerAddressBox.Text;
                        port = int.Parse(ServerPortBox.Text);
                    });
                    await Client.Connect(ip, port);
                    string data = null;
                    string login = null;
                    Dispatcher.Invoke(() =>
                    {
                        data = PasswordBox.Password;
                        login = LoginBox.Text;
                    });
                    if (await Client.Login(login, data))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            navService.Navigate(new Navigation());
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            LoginErrors.Foreground = Brushes.Red;
                            LoginErrors.Text = "Błędne dane logowania";
                        });
                    }
                });
            } catch(FormatException)
            {
                MessageBox.Show("Błędny format portu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            
            //GPSTCPClient.Pages.Register registerPage = new Pages.Register(this);
            navService.Navigate(new Register(navService,this));
        }
    }
}
