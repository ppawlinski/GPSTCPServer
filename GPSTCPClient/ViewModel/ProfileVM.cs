using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    class ProfileVM : ViewModelBase
    {
        private PasswordBox oldPassword;
        private PasswordBox newPassword;
        private PasswordBox confirmNewPassword;
        private string changePasswordError;
        private bool changePasswordEnabled;
        private string errorColor;

        public ProfileVM(MainVM mainVM)
        {
            MainVM = mainVM;
            ChangePasswordCommand = new Command(ArgIterator => ChangePassword());
            ChangePasswordEnabled = true;
        }

        private async void ChangePassword()
        {
            MainVM.Loading = true;
            ChangePasswordEnabled = false;
            ErrorColor = "Red";
            if (OldPassword != null && NewPassword != null && ConfirmNewPassword != null)
            {
                if (NewPassword.Password == ConfirmNewPassword.Password)
                {
                    if (await Client.ChangePassword(oldPassword.Password, NewPassword.Password))
                    {
                        ChangePasswordError = "Hasło zmienione pomyślnie, za chwilę nastąpi wylogowanie...";
                        ErrorColor = "Green";
                        await Task.Delay(4000);
                        MainVM.NavigateTo("Logout");
                    }
                    else
                    {
                        ChangePasswordError = "Nie udało się zmienić hasła!";
                    }
                }
                else
                {
                    ChangePasswordError = "Hasła muszą być takie same!";
                }
            }
            else
            {
                ChangePasswordError = "Wszystkie pola muszą być wypełnione!";
            }
            ChangePasswordEnabled = true;
            MainVM.Loading = false;
        }

        public MainVM MainVM { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public PasswordBox OldPassword
        {
            get { return oldPassword; }
            set
            {
                oldPassword = value;
                OnPropertyChanged(nameof(oldPassword));
            }
        }
        public PasswordBox NewPassword
        {
            get { return newPassword; }
            set
            {
                newPassword = value;
                OnPropertyChanged(nameof(newPassword));
            }
        }
        public PasswordBox ConfirmNewPassword
        {
            get { return confirmNewPassword; }
            set
            {
                confirmNewPassword = value;
                OnPropertyChanged(nameof(confirmNewPassword));
            }
        }

        public string ChangePasswordError
        {
            get { return changePasswordError; }
            set
            {
                changePasswordError = value;
                OnPropertyChanged(nameof(changePasswordError));
            }
        }

        public bool ChangePasswordEnabled
        {
            get
            {
                return changePasswordEnabled;
            }
            set
            {
                changePasswordEnabled = value;
                OnPropertyChanged(nameof(changePasswordEnabled));
            }
        }

        public string ErrorColor
        {
            get
            {
                return errorColor;
            }
            set
            {
                errorColor = value;
                OnPropertyChanged(nameof(errorColor));
            }
        }
    }
}
