using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPClient.Models
{
    class Leg
    {
        [JsonPropertyName("steps")]
        public IList<Step> Steps { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("distance")]
        public double Distance { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; }
        [JsonPropertyName("duration")]
        public double Duration { get; set; }
    }
}
