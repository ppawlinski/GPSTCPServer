using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPServer.Models
{
    class Waypoint
    {
        [JsonPropertyName("hint")]
        public string Hint { get; set; }
        [JsonPropertyName("location")]
        public IList<double> Location { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
