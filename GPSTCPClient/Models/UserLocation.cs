using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.Models
{
    public class UserLocation
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            if (Name != "") return $"[{Name}] {Address.DisplayName}";
            else return Address.DisplayName;

        }
    }
}
