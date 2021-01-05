using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            ServerAddress = "127.0.0.1";
            ServerPort = "2048";
            Login = "test";
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


        public ICommand LoginClickCommand { get; set; }

        public async void LoginAction(object arg)
        {
            mainVM.Loading = true;
            Errors = "";
            if(arg is PasswordBox passwordBox)
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
            mainVM.Loading = false;

        }

    }
}
