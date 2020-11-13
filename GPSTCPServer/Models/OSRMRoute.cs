using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPSTCPServer.Models
{
    class OSRMRoute
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("waypoints")]
        public IList<Waypoint> Waypoints { get; set; }
        [JsonPropertyName("routes")]
        public IList<Route> Routes { get; set; }
    }
}
