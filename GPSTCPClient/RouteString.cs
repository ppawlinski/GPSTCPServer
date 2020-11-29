using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient
{
    public class RouteString
    {
        public RouteString(string string_)
        {
            _value = string_;
        }
        public string Trasa { get { return _value; } set { _value = value; } }
        string _value;
    }
}
