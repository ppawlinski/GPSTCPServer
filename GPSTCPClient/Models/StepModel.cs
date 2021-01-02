using System;

namespace GPSTCPClient.Models
{
    public class StepModel
    {
        public string Polyline { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } //można enum z tego zrobić
        public Tuple<double, double> Maneuver { get; set; }
        public double Distance { get; set; }
    }
}