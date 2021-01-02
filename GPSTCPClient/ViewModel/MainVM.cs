using GPSTCPClient.ViewModel.MVVM;
using System.Windows;

namespace GPSTCPClient.ViewModel
{
    public class MainVM : ViewModelBase
    {
        public MainVM()
        {
            MenuButtonVisible = Visibility.Visible;
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
        private Visibility menuButtonVisible;

        public Visibility MenuButtonVisible
        {
            get
            {
                return menuButtonVisible;
            }
            set
            {
                menuButtonVisible = value;
                OnPropertyChanged(nameof(MenuButtonVisible));
            }
        }
    }
}
