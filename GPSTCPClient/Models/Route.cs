using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPClient.Models
{
    class Route
    {
        [JsonPropertyName("legs")]
        public IList<Leg> Legs { get; set; }
        [JsonPropertyName("weight_name")]
        public string Weight_name { get; set; }
        [JsonPropertyName("geometry")]
        public string Geometry { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("distance")]
        public double Distance { get; set; }
        [JsonPropertyName("duration")]
        public double Duration { get; set; }
    }
}
