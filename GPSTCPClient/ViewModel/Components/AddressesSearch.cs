using GPSTCPClient.Models;
using GPSTCPClient.ViewModel.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GPSTCPClient.ViewModel.Components
{
    public class AddressesSearch : ViewModelBase
    {
        public AddressesSearch()
        {
            Addresses = new ObservableCollection<Address>();
            SelectedAddress = new Address();
            SelectedAddressText = "";
        }
        private ObservableCollection<Address> addresses;
        public ObservableCollection<Address> Addresses
        {
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
            if (loaded != null && loaded.Length > 0)
            {
                Addresses.Clear();
                loaded.ToList().ForEach(p => Addresses.Add(p));
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

        private string selectedAddressText;

        public string SelectedAddressText
        {
            get
            {
                return selectedAddressText;
            }
            set
            {
                if (value != selectedAddress?.ToString())
                {
                    selectedAddress = null;
                    Addresses.Clear();
                    selectedAddressText = value;
                    IsDropDownOpen = false;
                }
                else selectedAddressText = value;

                if (value != null && selectedAddressText.Length >= 3 && !Addresses.Any(p => p?.DisplayName == value))
                {
                    FindAddressForFav(value);
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
                OnSelected(new PropertyChangedEventArgs(nameof(value)));
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
                      return await Client.GetAddress(val);
                  }
                  return null;
              }
            ).ContinueWith(task =>
            {
                if (task.Result != null)
                {
                    Addresses.Clear();
                    Addresses.AddRange(task.Result);
                    IsDropDownOpen = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public event EventHandler OnSelectedAction;
        protected virtual void OnSelected(EventArgs e)
        {
            EventHandler handler = OnSelectedAction;
            handler?.Invoke(this, e);
        }

    }
}
