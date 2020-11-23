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
using System.Security.Cryptography;

namespace GPSTCPClient.Pages
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        private NavigationService navService;
        private Page prevPage;
        public Register(NavigationService nav,Page prevP)
        {
            InitializeComponent();
            navService = nav;
            if(prevP is Login login)
            {
                ServerAddressBox.Text = login.ServerAddressBox.Text;
                ServerPortBox.Text = login.ServerPortBox.Text;
                LoginBox.Text = login.LoginBox.Text;
            }
            
            prevPage = prevP;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterFullfilmentError.Text = String.Empty;
            if (String.IsNullOrWhiteSpace(LoginBox.Text) || String.IsNullOrWhiteSpace(ServerAddressBox.Text) || String.IsNullOrWhiteSpace(ServerPortBox.Text)) 
                    RegisterFullfilmentError.Text += "Uzupełnij wszystkie pola. "; 
            if (PasswordBox.Password != PasswordConfirmBox.Password) RegisterFullfilmentError.Text += "Hasła nie są takie same. ";
            if (!int.TryParse(ServerPortBox.Text, out int port)) RegisterFullfilmentError.Text += "Wprowadź poprawny numer portu. ";
            if (!IPAddress.TryParse(ServerAddressBox.Text, out IPAddress address)) RegisterFullfilmentError.Text += "Wprowadź poprawny adres IP. ";
            if (String.IsNullOrWhiteSpace((String)RegisterFullfilmentError.Text)) return;
            try
            {
                Task.Run(async () => await Client.Connect(ServerAddressBox.Text, int.Parse(ServerPortBox.Text))).Wait();
            }
            catch (FormatException)
            {
                MessageBox.Show("Błędny format portu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
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
                    //TODO loguj w zależności od odpwiedzi z bool Client.Register()
                    Task.Run(async () =>
                    {
                        await Client.Register(LoginBox.Text, Convert.ToBase64String(data));
                    }).ContinueWith((t) =>
                    {
                        md5.Dispose();
                        Client.Disconnect();
                        Dispatcher.Invoke(() =>
                        {
                            navService.GoBack();
                        });
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    md5.Dispose();
                    return;
                }
            }
            

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(prevPage is Login login)
            {
                login.ServerAddressBox.Text = ServerAddressBox.Text;
                login.ServerPortBox.Text = ServerPortBox.Text;
                login.LoginBox.Text = LoginBox.Text;
            }
            navService.GoBack();
            
        }
    }
}
