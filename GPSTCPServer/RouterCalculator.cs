using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPSTCPServer.Models;

namespace GPSTCPServer
{
    public class RouterCalculator
    {
        private OSRMRoute router;
        public bool OK { get;  }
        public RouterCalculator(string originLon, string originLat, string destinationLon, string destinationLat) {
            string url = String.Format("http://router.project-osrm.org/route/v1/driving/{0},{1};{2},{3}?steps=true&annotations=false", originLon, originLat, destinationLon, destinationLat);
            Task.Run(async () => router = JsonSerializer.Deserialize<OSRMRoute>(await GetRequest.GetFromURLAsync(url))).Wait();
            if (router.Code == "NoRoute")
            {
                OK = false;
            }
            else OK = true;
        }

        public RouteModel[] GetInstructions()
        {
            var steps = router.Routes[0].Legs[0].Steps;
            RouteModel[] instructions = new RouteModel[steps.Count];
            for (int i = 0; i < steps.Count; i++)
            {
                instructions[i] = new RouteModel()
                {
                    Description = formatName(steps[i].Maneuver, steps[i].Name, steps[i].Distance.ToString()),
                    Intersections = steps[i].Intersections.Select(p => new Tuple<double, double>(p.Location[1], p.Location[0])).ToArray(),
                    Type = steps[i].Maneuver.Type
                };
            }
            return instructions;
        }

        private string formatName(Maneuver maneuver, string street, string distance)
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
                    if (street.Trim() != "") instruction = "Kontynuuj na " + street + "\t" + distance + "m";
                    else instruction = "Kontynuuj " + "\t" + distance + "m";
                    return instruction;
                case "continue":
                    if (street.Trim() != "") instruction = "Kontynuuj na " + street + "\t" + distance + "m";
                    else instruction = "Kontynuuj " + "\t" + distance + "m";
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
                    if (street.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + street + "\t" + distance + "m";
                    instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem " + "\t" + distance + "m";
                    return instruction;
                case "notification":
                    break;
                case "exit roundabout":
                    if (street.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + street + "\t" + distance + "m";
                    instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem " + "\t" + distance + "m";
                    return instruction;
                case "exit rotary":
                    if (street.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + street + "\t" + distance + "m";
                    else instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem  " + "\t" + distance + "m";
                    break;
            }

            switch (modifier)
            {
                case "right":
                    if (street.Trim() != "") instruction += "w prawo w " + street + "\t" + distance + "m";
                    else instruction += "w prawo " + street + "\t" + distance + "m";
                    break;
                case "left":
                    if (street.Trim() != "") instruction += "w lewo w " + street + "\t" + distance + "m";
                    else instruction += "w lewo " + "\t" + distance + "m";
                    break;
                case "slight right":
                    if (street.Trim() != "") instruction += (type == "continue" ? "Lekko w prawo w " : "lekko w prawo w ") + street + "\t" + distance + "m";
                    else instruction += (type == "continue" ? "Lekko w prawo " : "lekko w prawo ") + "\t" + distance + "m";
                    break;
                case "slight left":
                    if (street.Trim() != "") instruction += (type == "continue" ? "Lekko w lewo w " : "lekko w lewo w ") + street + "\t" + distance + "m";
                    else instruction += (type == "continue" ? "Lekko w lewo " : "lekko w lewo ") + "\t" + distance + "m";
                    break;
                case "uturn":

                    break;
                case "sharp right":

                    break;
                case "sharp left":

                    break;
                case "straight":
                    if (type != "turn") instruction += " prosto w " + street + "\t" + distance + "m";
                    else instruction = "Jedź prosto w " + street + "\t" + distance + "m";
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
        public Tuple<double,double>[] Intersections { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } //można enum z tego zrobić
    }
}
