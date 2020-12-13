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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Page
    {
        private NavigationService navService;
        public ChangePassword(NavigationService nav)
        {
            InitializeComponent();
            navService = nav;
        }

        private void Change_Click(object sender, EventArgs e)
        {
            PasswordChangeStatus.Text = String.Empty;
            if (String.IsNullOrWhiteSpace(OldPasswordBox.Password) || String.IsNullOrWhiteSpace(NewPasswordBox.Password) || String.IsNullOrWhiteSpace(PasswordConfirmBox.Password))
                PasswordChangeStatus.Text += "Uzupełnij wszystkie pola. ";
            if (NewPasswordBox.Password != PasswordConfirmBox.Password) PasswordChangeStatus.Text += "Hasła nie są takie same. ";
            if (!String.IsNullOrWhiteSpace((String)PasswordChangeStatus.Text)) return;
            try
            {
                string oldPassword = OldPasswordBox.Password;
                string newPassword = NewPasswordBox.Password;

                Task.Run(async () =>
                {
                    if (await Client.ChangePassword(oldPassword, newPassword))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            PasswordChangeStatus.Text = "Pomyślnie zmieniono hasło";
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            PasswordChangeStatus.Text = "Nie udało się zmienić hasła";
                        });
                    }
                    Client.Disconnect();
                });
            }
            catch (Exception)
            {
                PasswordChangeStatus.Text = "Nie udało się zmienić hasła";
            }

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            navService.GoBack();
        }
    }
}
