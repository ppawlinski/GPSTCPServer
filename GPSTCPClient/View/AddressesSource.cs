using GPSTCPClient.Helpers;
using GPSTCPClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.View
{
    public class AddressesSource : INotifyPropertyChanged
    {
        private ObservableCollection<Address> addresses;
        public ObservableCollection<Address> Addresses { 
            get
            {
                return addresses;
            }
            set
            {
                addresses = value;
                OnPropertyChanged(nameof(Addresses));
            }
        }

        public async void FillAddressess(Task<Address[]> loader)
        {
            var loaded = await loader;
            if (loaded != null && loaded.Length > 0) Addresses = new ObservableCollection<Address>(loaded);
        }

        public AddressesSource()
        {
            Addresses = new ObservableCollection<Address>();
         }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        private bool isDropDownOpen;
        public bool IsDropDownOpen
        {
            get
            {
                return isDropDownOpen;
            }
            set
            {
                isDropDownOpen = value;
                OnPropertyChanged(nameof(IsDropDownOpen));
            }
        }
    }
}
