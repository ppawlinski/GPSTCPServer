using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPClient.Models
{
    class Intersection
    {
        [JsonPropertyName("out")]
        public int Out { get; set; }
        [JsonPropertyName("entry")]
        public IList<bool> Entry { get; set; }
        [JsonPropertyName("location")]
        public IList<double> Location { get; set; }
        [JsonPropertyName("bearings")]
        public IList<int> Bearings { get; set; }
        [JsonPropertyName("in")]
        public int? In { get; set; }
    }
}
