using GPSTCPServer.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPSTCPServer
{
    public class RouterCalculator
    {
        private OSRMRoute router;
        public bool OK { get;  }
        public RouterCalculator(string originLon, string originLat, string destinationLon, string destinationLat) {
            string url = String.Format("http://router.project-osrm.org/route/v1/driving/{0},{1};{2},{3}?steps=true&annotations=false&geometries=polyline", originLon, originLat, destinationLon, destinationLat);
            Task.Run(async () => router = JsonSerializer.Deserialize<OSRMRoute>(await GetRequest.GetFromURLAsync(url))).Wait();
            if (router.Code == "NoRoute")
            {
                OK = false;
            }
            else OK = true;
        }

        public RouteModel GetInstructions()
        {
            var steps = router.Routes[0].Legs[0].Steps;
            StepModel[] instructions = new StepModel[steps.Count];
            for (int i = 0; i < steps.Count; i++)
            {
                instructions[i] = new StepModel()
                {
                    Description = formatName(steps[i].Maneuver, steps[i].Name),
                    Polyline = steps[i].Geometry,
                    Type = steps[i].Maneuver.Type,
                    Maneuver = new Tuple<double, double>(steps[i].Maneuver.Location[1], steps[i].Maneuver.Location[0]),
                    Distance = steps[i].Distance
                };
            }
            return new RouteModel()
            {
                Steps = instructions,
                Distance = router.Routes[0].Distance,
                Duration = router.Routes[0].Duration
            };
        }

        private string formatName(Maneuver maneuver, string street)
        {
            string type = maneuver.Type;
            string modifier = maneuver.Modifier;
            string instruction = "";
            switch (type)
            {
                case "depart":
                    instruction = "Zacznij na " + street;
                    return instruction;
                case "turn":
                    instruction = "Skręć ";
                    break;
                case "new name":
                    if (street.Trim() != "") instruction = "Kontynuuj na " + street;
                    else instruction = "Kontynuuj ";
                    return instruction;
                case "continue":
                    if (street.Trim() != "") instruction = "Kontynuuj na " + street;
                    else instruction = "Kontynuuj ";
                    break;
                case "arrive":
                    instruction = "Osiągnięto cel";
                    return instruction;
                case "merge":
                    instruction = "Skręć ";
                    break;
                case "ramp":
                    break;
                case "on ramp":
                    instruction = "Wjedź zjazdem ";
                    break;
                case "off ramp":
                    instruction = "Zjedź zjazdem ";
                    break;
                case "fork":
                    instruction = "Na rozwidleniu ";
                    break;
                case "end of road":
                    instruction = "Na końcu tej ulic skręć ";
                    break;
                case "use lane":
                    break;
                case "roundabout":
                    instruction = "Skręć ";
                    break;
                case "rotary":
                    instruction = "Skręć ";
                    break;
                case "roundabout turn":
                    if (street.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + street;
                    instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem ";
                    return instruction;
                case "notification":
                    break;
                case "exit roundabout":
                    if (street.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + street;
                    instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem ";
                    return instruction;
                case "exit rotary":
                    if (street.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + street;
                    else instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem  ";
                    break;
            }

            switch (modifier)
            {
                case "right":
                    if (street.Trim() != "") instruction += "w prawo w " + street;
                    else instruction += "w prawo " + street;
                    break;
                case "left":
                    if (street.Trim() != "") instruction += "w lewo w " + street;
                    else instruction += "w lewo ";
                    break;
                case "slight right":
                    if (street.Trim() != "") instruction += (type == "continue" ? "Lekko w prawo w " : "lekko w prawo w ") + street;
                    else instruction += (type == "continue" ? "Lekko w prawo " : "lekko w prawo ");
                    break;
                case "slight left":
                    if (street.Trim() != "") instruction += (type == "continue" ? "Lekko w lewo w " : "lekko w lewo w ") + street;
                    else instruction += (type == "continue" ? "Lekko w lewo " : "lekko w lewo ");
                    break;
                case "uturn":

                    break;
                case "sharp right":

                    break;
                case "sharp left":

                    break;
                case "straight":
                    if (type != "turn") instruction += " prosto w " + street;
                    else instruction = "Jedź prosto w " + street;
                    break;
            }
            return instruction;
        }

        private string numberToPL(int exit)
        {
            switch (exit)
            {
                case 1:
                    return "pierwszym";
                case 2:
                    return "drugim";
                case 3:
                    return "trzecim";
                case 4:
                    return "czwartym";
                case 5:
                    return "piątym";
                case 6:
                    return "szóstm";
                case 7:
                    return "siódmym";
            }
            return "";
        }
    }
    public class RouteModel
    {
        public double Duration { get; set; }
        public double Distance { get; set; }
        public StepModel[] Steps { get; set; }
    }
    public class StepModel
    {
        public string Polyline { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } //można enum z tego zrobić
        public Tuple<double,double> Maneuver { get; set; }
        public double Distance { get; set; }
    }
}
