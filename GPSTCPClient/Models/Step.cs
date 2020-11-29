using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPClient.Models
{
    class Step
    {
        [JsonPropertyName("intersections")]
        public IList<Intersection> Intersections { get; set; }
        [JsonPropertyName("driving_side")]
        public string Driving_side { get; set; }
        [JsonPropertyName("geometry")]
        public string Geometry { get; set; }
        [JsonPropertyName("duration")]
        public double Duration { get; set; }
        [JsonPropertyName("distance")]
        public double Distance { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("mode")]
        public string Mode { get; set; }
        [JsonPropertyName("maneuver")]
        public Maneuver Maneuver { get; set; }
    }
}
