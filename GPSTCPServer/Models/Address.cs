using System.Text.Json.Serialization;

namespace GPSTCPServer.Models
{
    class Address
    {
        [JsonPropertyName("place_id")]
        public long PlaceId { get; set; }

        [JsonPropertyName("licence")]
        public string Licence { get; set; }

        [JsonPropertyName("osm_type")]
        public string OsmType { get; set; }

        [JsonPropertyName("osm_id")]
        public long OsmId { get; set; }

        [JsonPropertyName("boundingbox")]
        public string[] Boundingbox { get; set; }

        [JsonPropertyName("lat"), JsonConverter(typeof(GPSTCPServer.Converters.StringToDoubleConverter))]
        public double Lat { get; set; }

        [JsonPropertyName("lon"), JsonConverter(typeof(GPSTCPServer.Converters.StringToDoubleConverter))]
        public double Lon { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("class")]
        public string Class { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("importance")]
        public double Importance { get; set; }
    }
}
