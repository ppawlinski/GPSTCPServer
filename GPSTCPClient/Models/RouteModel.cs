using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.Models
{
    public class RouteModel
    {
        public double Duration { get; set; }
        public double Distance { get; set; }
        public StepModel[] Steps { get; set; }
    }
}
