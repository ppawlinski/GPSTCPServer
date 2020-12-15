using GPSTCPClient.Models;
using GPSTCPClient.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GPSTCPClient.ViewModel
{
    public class NavigationVM
    {
        private UserLocationsSource locations;

        public UserLocationsSource Locations
        {
            get
            {
                return locations;
            }
            set
            {
                locations = value;
            }
        }

        public NavigationVM()
        {
            Locations = new UserLocationsSource(Client.GetMyAddresses());
        }
    }
}
