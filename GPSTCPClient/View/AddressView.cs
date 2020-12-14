using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.View
{
    public class AddressView : UserLocationsSource, INotifyPropertyChanged
    {
        private string input;
        public string Input { 
            get {
                return input;
            } 
            set {
                input = value;
                OnPropertyChanged(nameof(Input));
            } 
        }
    }
}
