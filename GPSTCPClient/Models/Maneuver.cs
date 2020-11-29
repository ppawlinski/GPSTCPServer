using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPClient.Models
{
    class Maneuver
    {
        [JsonPropertyName("bearing_after")]
        public int Bearing_after { get; set; }
        [JsonPropertyName("location")]
        public IList<double> Location { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("bearing_before")]
        public int Bearing_before { get; set; }
        [JsonPropertyName("modifier")]
        public string Modifier { get; set; }
        [JsonPropertyName("exit")]
        public int Exit { get; set; }
    }
}
