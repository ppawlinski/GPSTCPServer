using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPClient.Models
{
    public class RouteModel
    {
        public Tuple<double, double>[] Intersections { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } //można enum z tego zrobić
        public Tuple<double, double> Maneuver { get; set; }
        public double Distance { get; set; }
    }
}
