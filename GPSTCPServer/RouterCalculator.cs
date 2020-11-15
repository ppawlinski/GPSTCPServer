using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPSTCPServer.Models;

namespace GPSTCPServer
{
    class RouterCalculator
    {
        private OSRMRoute router;
        public bool OK { get;  }
        public RouterCalculator(Address origin, Address destination) {
            string url = String.Format("http://router.project-osrm.org/route/v1/driving/{0},{1};{2},{3}?steps=true&annotations=false", origin.Lon, origin.Lat, destination.Lon, destination.Lat);
            Task.Run(async () => router = JsonSerializer.Deserialize<OSRMRoute>(await GetRequest.GetFromURLAsync(url))).Wait();
            if (router.Code == "NoRoute")
            {
                OK = false;
            }
            else OK = true;
        }

        public string[] getInstructions()
        {
            var steps = router.Routes[0].Legs[0].Steps;
            string[] instructions = new string[steps.Count];
            for (int i = 0; i < steps.Count; i++)
            {
                instructions[i] = formatName(steps[i].Maneuver, steps[i].Name, steps[i].Distance.ToString());
            }
            return instructions;
        }

        private string formatName(Maneuver maneuver, string ulica, string dystans)
        {
            string type = maneuver.Type;
            string modifier = maneuver.Modifier;
            string instruction = "";
            switch (type)
            {
                case "depart":
                    instruction = "Zacznij na " + ulica;
                    return instruction;
                case "turn":
                    instruction = "Skręć ";
                    break;
                case "new name":
                    if (ulica.Trim() != "") instruction = "Kontynuuj na " + ulica + "\t" + dystans + "m";
                    else instruction = "Kontynuuj " + "\t" + dystans + "m";
                    return instruction;
                case "continue":
                    if (ulica.Trim() != "") instruction = "Kontynuuj na " + ulica + "\t" + dystans + "m";
                    else instruction = "Kontynuuj " + "\t" + dystans + "m";
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
                    if (ulica.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + ulica + "\t" + dystans + "m";
                    instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem " + "\t" + dystans + "m";
                    return instruction;
                case "notification":
                    break;
                case "exit roundabout":
                    if (ulica.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + ulica + "\t" + dystans + "m";
                    instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem " + "\t" + dystans + "m";
                    return instruction;
                case "exit rotary":
                    if (ulica.Trim() != "") instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem na " + ulica + "\t" + dystans + "m";
                    else instruction = "Na rondzie zjedź " + numberToPL(maneuver.Exit) + " zjazdem  " + "\t" + dystans + "m";
                    break;
            }

            switch (modifier)
            {
                case "right":
                    if (ulica.Trim() != "") instruction += "w prawo w " + ulica + "\t" + dystans + "m";
                    else instruction += "w prawo " + ulica + "\t" + dystans + "m";
                    break;
                case "left":
                    if (ulica.Trim() != "") instruction += "w lewo w " + ulica + "\t" + dystans + "m";
                    else instruction += "w lewo " + "\t" + dystans + "m";
                    break;
                case "slight right":
                    if (ulica.Trim() != "") instruction += (type == "continue" ? "Lekko w prawo w " : "lekko w prawo w ") + ulica + "\t" + dystans + "m";
                    else instruction += (type == "continue" ? "Lekko w prawo " : "lekko w prawo ") + "\t" + dystans + "m";
                    break;
                case "slight left":
                    if (ulica.Trim() != "") instruction += (type == "continue" ? "Lekko w lewo w " : "lekko w lewo w ") + ulica + "\t" + dystans + "m";
                    else instruction += (type == "continue" ? "Lekko w lewo " : "lekko w lewo ") + "\t" + dystans + "m";
                    break;
                case "uturn":

                    break;
                case "sharp right":

                    break;
                case "sharp left":

                    break;
                case "straight":
                    if (type != "turn") instruction += " prosto w " + ulica + "\t" + dystans + "m";
                    else instruction = "Jedź prosto w " + ulica + "\t" + dystans + "m";
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
}
