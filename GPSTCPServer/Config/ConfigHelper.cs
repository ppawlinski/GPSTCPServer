using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPServer.Config
{
    public static class ConfigHelper
    {
        public static string CnnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
