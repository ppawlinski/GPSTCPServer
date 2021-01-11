using GPSTCPClient.ViewModel.MVVM;
using GPSTCPClient.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    public class MainVM : ViewModelBase
    {
        public MainVM()
        {
            NavigateToCommand = new Command(arg => NavigateTo(arg));
            LogoutDialogCommand = new Command(arg => LogoutDialog());
            LoggedIn = false;
        }

        private async void LogoutDialog()
        {
            DialogContent = "Czy na pewno chcesz się wylogować?";
            string result = (string)await DialogHost.Show(new OkCancelDialog(), "LogoutDialog");
            if (result == "Accept") NavigateTo("Logout");
        }

        public void NavigateTo(object arg)
        {
            if (arg is string dest)
            {
                switch(dest)
                {
                    case "Login":
                        SelectedVM = new LoginVM(this);
                        break;
                    case "Logout":
                        Logout();
                        SelectedVM = new LoginVM(this);
                        navigation = null;
                        favourites = null;
                        MenuToggle = false;
                        LoggedIn = false;
                        break;
                    case "Navigation":
                        if (favourites == null || navigation == null)
                        {
                            favourites = new FavouritesVM(this);
                            navigation = new NavigationVM(favourites);
                            LoggedIn = true;
                        }
                        SelectedVM = navigation;
                        MenuToggle = false;
                        break;
                    case "Favourites":
                        SelectedVM = favourites;
                        MenuToggle = false;
                        break;
                    case "Register":
                        SelectedVM = new RegisterVM(this);
                        break;
                    case "Profile":
                        SelectedVM = new ProfileVM(this);
                        MenuToggle = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private ViewModelBase navigation;
        private ViewModelBase favourites;

        private async void Logout()
        {
            await Client.Logout();
        }

        public ICommand NavigateToCommand { get; set; }
        public ICommand LogoutDialogCommand { get; set; }

        public bool LoggedIn
        {
            set
            {
                if (value == true)
                {
                    LoggedInCv = Visibility.Visible;
                }
                else LoggedInCv = Visibility.Hidden;
            }
        }
        private Visibility loggedInCv;
        public Visibility LoggedInCv
        {
            get
            {
                return loggedInCv;
            }
            set
            {
                loggedInCv = value;
                OnPropertyChanged(nameof(LoggedInCv));
            }
        }
        private bool menuToggle;
        public bool MenuToggle
        {
            get
            {
                return menuToggle;
            }
            set
            {
                menuToggle = value;
                OnPropertyChanged(nameof(MenuToggle));
            }
        }
        private ViewModelBase selectedVM;
        public ViewModelBase SelectedVM
        {
            get
            {
                return selectedVM;
            }
            set
            {
                selectedVM = value;
                OnPropertyChanged(nameof(SelectedVM));
            }
        }
        private Visibility loadingCv;


        public Visibility LoadingCv
        {
            get
            {
                return loadingCv;
            }
            set
            {
                loadingCv = value;
                OnPropertyChanged(nameof(LoadingCv));
            }
        }

        public bool Loading
        {
            set
            {
                if (value == true) LoadingCv = Visibility.Visible;
                else LoadingCv = Visibility.Hidden;
            }
        }

        public string DialogContent { get; set; }

        public void SetLoading()
        {
            LoadingCv = Visibility.Visible;
        }
    }
}
