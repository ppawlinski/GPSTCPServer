using GPSTCPClient.ViewModel.MVVM;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    public class LoginVM : ViewModelBase
    {

        public LoginVM(MainVM mainVM_)
        {
            mainVM = mainVM_;
            LoginClickCommand = new Command(arg => LoginAction(arg));
            RegisterClickCommand = new Command(arg => Register());
            ServerAddress = "127.0.0.1";
            ServerPort = "2048";
            Login = "test";
            loginEnabled = true;
            registerEnabled = true;
        }
        private MainVM mainVM;
        private string errors;
        public string Errors
        {
            get
            {
                return errors;
            }
            set
            {
                errors = value;
                OnPropertyChanged(nameof(Errors));
            }
        }

        private string serverAddress;

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
        private string serverPort;
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
        private string login;

        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

        private bool loginEnabled;
        public bool LoginEnabled
        {
            get
            {
                return loginEnabled;
            }
            set
            {
                loginEnabled = value;
                OnPropertyChanged(nameof(LoginEnabled));
            }
        }

        private bool registerEnabled;
        public bool RegisterEnabled
        {
            get
            {
                return registerEnabled;
            }
            set
            {
                registerEnabled = value;
                OnPropertyChanged(nameof(RegisterEnabled));
            }
        }
        public ICommand LoginClickCommand { get; set; }

        public async void LoginAction(object arg)
        {
            LoginEnabled = false;
            RegisterEnabled = false;
            mainVM.Loading = true;
            Errors = "";
            if (arg is PasswordBox passwordBox)
            {
                if (!await Client.Connect(serverAddress, int.Parse(serverPort)))
                {
                    Errors = "Błąd połączenia";
                }
                else if (await Client.Login(login, passwordBox.Password))
                {
                    //mainVM.SelectedVM = new NavigationVM(mainVM);
                    mainVM.NavigateTo("Navigation");
                }
                else
                {
                    Errors = "Błędne dane logowania";
                }
            }
            LoginEnabled = true;
            RegisterEnabled = true;
            mainVM.Loading = false;

        }
        public ICommand RegisterClickCommand { get; set; }
        private void Register()
        {
            mainVM.NavigateTo("Register");
        }

    }
}
