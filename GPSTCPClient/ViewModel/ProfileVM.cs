using GPSTCPClient.ViewModel.MVVM;
using GPSTCPClient.Views;
using MaterialDesignThemes.Wpf;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GPSTCPClient.ViewModel
{
    class ProfileVM : ViewModelBase
    {
        private string changePasswordError;
        private bool changePasswordEnabled;
        private string errorColor;
        private string dialogContent;

        public ProfileVM(MainVM mainVM)
        {
            MainVM = mainVM;
            ChangePasswordCommand = new Command(arg => ChangePassword(arg));
            ChangePasswordEnabled = true;
            ErrorColor = "Red";
        }

        private async void ChangePassword(object arg)
        {
            MainVM.Loading = true;
            ChangePasswordEnabled = false;
            ErrorColor = "Red";
            DialogContent = "Czy na pewno chcesz zmienić hasło?";
            string result = (string)await DialogHost.Show(new OkCancelDialog(), "ChangePassDialog");
            if (result == "Accept")
            {
                if (((IEnumerable)arg).Cast<object>()
                            .Select(x => (PasswordBox)x)
                            .ToArray() is PasswordBox[] arr)
                {
                    if (arr[0].Password != string.Empty && arr[1].Password != string.Empty && arr[2].Password != string.Empty)
                    {
                        if (arr[1].Password == arr[2].Password)
                        {
#if !DEBUG
                            var valid = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
                            if (valid.IsMatch(arr[1].Password))
                            {
#endif
                                if (await Client.ChangePassword(arr[0].Password, arr[1].Password))
                                {
                                    ChangePasswordError = "Hasło zmienione pomyślnie, za chwilę nastąpi wylogowanie...";
                                    ErrorColor = "Green";
                                    await Task.Delay(4000);
                                    MainVM.NavigateTo("Logout");
                                }
                                else
                                    ChangePasswordError = "Nie udało się zmienić hasła!";
#if !DEBUG
                            }
                            else
                            {
                                ChangePasswordError = "Hasło powinno składać się z 8-15 znaków, zawierać duże i małe litery, cyfry oraz znaki specjalne.";
                            }
#endif
                        }
                        else
                            ChangePasswordError = "Hasła muszą być takie same!";
                    }
                    else
                        ChangePasswordError = "Wszystkie pola muszą być wypełnione!";
                }
            }


            ChangePasswordEnabled = true;
            MainVM.Loading = false;
        }

        public MainVM MainVM { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public string ChangePasswordError
        {
            get { return changePasswordError; }
            set
            {
                changePasswordError = value;
                OnPropertyChanged(nameof(ChangePasswordError));
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
                OnPropertyChanged(nameof(ChangePasswordEnabled));
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
                OnPropertyChanged(nameof(ErrorColor));
            }
        }

        public string DialogContent
        {
            get
            {
                return dialogContent;
            }
            set
            {
                dialogContent = value;
                OnPropertyChanged(nameof(DialogContent));
            }
        }
    }
}
