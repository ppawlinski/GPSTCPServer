using GPSTCPClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.View
{
    public class UserLocation : INotifyPropertyChanged
    {
        private Address address;
        private string name;
        public Address Address { 
            get
            {
                return address;
            }
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public override string ToString()
        {
            if (Name != "") return $"[{Name}] {Address.DisplayName}";
            else return Address.DisplayName;

        }
    }
}
