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
    public class LoginVM : ViewModelBase
    {
        private MainVM mainVM;
        public LoginVM(MainVM mainVM_)
        {
            mainVM = mainVM_;
            LoginClickCommand = new Command(sender => LoginAction(sender));
            ServerAddress = "127.0.0.1";
            ServerPort = "2048";
            Login = "test";
            
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

        public async void LoginAction(object sender)
        {
            if(sender is PasswordBox passwordBox)
            {
                await Client.Connect(ServerAddress, int.Parse(ServerPort));
                await Client.Login(Login, passwordBox.Password);
                mainVM.SelectedVM = new NavigationVM();
            }
            
        }
    }
}
