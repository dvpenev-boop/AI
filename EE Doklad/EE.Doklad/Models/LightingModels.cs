using System.Collections.Generic;
using Newtonsoft.Json;

namespace EE.Doklad.Models
{
    // Seed data from embedded JSON
    public sealed class LightingSeed
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("powerW")]
        public int PowerW { get; set; }
    }

    // User-defined lighting components
    public sealed class LightingUser
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("powerW")]
        public int PowerW { get; set; }
    }

    // Row for DataGrid display
    public sealed class LightingRow
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int PowerW { get; set; }
        public bool IsSeed { get; set; }
    }
}
