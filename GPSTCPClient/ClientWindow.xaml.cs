using System.IO;
using System.Windows;

namespace GPSTCPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
            if (File.Exists("apikey.txt"))
            {
                 GPSTCPClient.Client.ApiKey = File.ReadAllText("apikey.txt");
            }
            Navigation.NavigationService.Navigate(new Pages.Login(Navigation.NavigationService));
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
