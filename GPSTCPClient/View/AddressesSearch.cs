using GPSTCPClient.Helpers;
using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
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
    public class AddressesSearch : ViewModelBase
    {
        public AddressesSearch()
        {
            Addresses = new ObservableCollection<Address>();
            SelectedAddress = new Address();
            SelectedAddressText = "";
            IsDropDownOpen = false;
        }
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

        private string selectedAddressText;

        public string SelectedAddressText
        {
            get
            {
                return selectedAddressText;
            }
            set
            {
                selectedAddressText = value;

                if (selectedAddressText.Length >= 3 && !Addresses.Any(p => p.DisplayName == value))
                {
                    FindAddressForFav(value);
                }
                else
                {
                    IsDropDownOpen = false;
                }
                OnPropertyChanged(nameof(SelectedAddressText));
            }
        }
        private Address selectedAddress;
        public Address SelectedAddress
        {
            get
            {
                return selectedAddress;
            }
            set
            {
                selectedAddress = value;
                OnPropertyChanged(nameof(SelectedAddress));
            }
        }

        private void FindAddressForFav(string val)
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                if (SelectedAddressText == val)
                {
                    Addresses = new ObservableCollection<Address>(await Client.GetAddress(val));
                    IsDropDownOpen = true;
                }
            }
            );
        }


    }
}
