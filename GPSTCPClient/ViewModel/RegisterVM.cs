using GPSTCPClient.ViewModel.Components;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    class RegisterVM : ViewModelBase
    {
        private bool returnEnabled;
        private bool registerEnabled;
        private string registerError;
        private string login;
        private PasswordBox password;
        private PasswordBox confirmPassword;
        private string serverAddress;
        private string serverPort;
        private string errorColor;
        public RegisterVM(MainVM mainVM_)
        {
            mainVM = mainVM_;
            RegisterClickCommand = new Command(arg => RegisterAction());
            ReturnClickCommand = new Command(arg => ReturnAction());
            ServerAddress = "127.0.0.1";
            ServerPort = "2048";
            Login = string.Empty;
            registerEnabled = true;
            returnEnabled = true;
        }

        private void ReturnAction()
        {
            mainVM.NavigateTo("Login");
        }

        private async void RegisterAction()
        {
            ReturnEnabled = false;
            RegisterEnabled = false;
            mainVM.Loading = true;
            RegisterError = "";
            ErrorColor = "Red";
            if (Password != null && ConfirmPassword != null && Login != string.Empty && Password.Password != string.Empty && ConfirmPassword.Password != string.Empty)
            {
                if (Password?.Password == ConfirmPassword?.Password)
                {
                    if (!await Client.Connect(serverAddress, int.Parse(serverPort)))
                    {
                        RegisterError = "Błąd połączenia";
                    }
                    else if (await Client.Register(login, Password.Password))
                    {
                        RegisterError = "Pomyślnie utworzono konto.";
                        ErrorColor = "Green";
                        Login = string.Empty;
                    }
                    else
                        RegisterError = "Nie udało się utworzyć konta!";
                }
                else
                    RegisterError = "Hasła powinny być takie same!";

                Password.Password = string.Empty;
                ConfirmPassword.Password = string.Empty;
            }
            else
                RegisterError = "Wszystkie pola powinny być wypełnione!";
            ReturnEnabled = true;
            RegisterEnabled = true;
            mainVM.Loading = false;
        }

        public MainVM mainVM { get; set; }
        public ICommand RegisterClickCommand { get; set; }
        public ICommand ReturnClickCommand { get; }
        public string ServerAddress
        {
            get
            {
                return serverAddress;
            }
            set
            {
                serverAddress = value;
                OnPropertyChanged(nameof(ServerAddress));
            }
        }
        public string ServerPort
        {
            get
            {
                return serverPort;
            }
            set
            {
                serverPort = value;
                OnPropertyChanged(nameof(ServerPort));
            }
        }
        public bool ReturnEnabled
        {
            get { return returnEnabled; }
            set
            {
                returnEnabled = value;
                OnPropertyChanged(nameof(returnEnabled));
            }
        }
        public bool RegisterEnabled
        {
            get { return registerEnabled; }
            set
            {
                registerEnabled = value;
                OnPropertyChanged(nameof(registerEnabled));
            }
        }
        public string RegisterError
        {
            get { return registerError; }
            set
            {
                registerError = value;
                OnPropertyChanged(nameof(registerError));
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged(nameof(login));
            }
        }
        public PasswordBox Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(password));
            }
        }
        public PasswordBox ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                OnPropertyChanged(nameof(confirmPassword));
            }
        }
        public string ErrorColor
        {
            get { return errorColor; }
            set
            {
                errorColor = value;
                OnPropertyChanged(nameof(errorColor));
            }
        }
    }
}
