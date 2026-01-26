using System.Collections.Generic;
using Newtonsoft.Json;

namespace EE.Doklad.Models
{
    public sealed class BuildingMaterialSeed
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("name_bg")]
        public string NameBg { get; set; } = string.Empty;

        [JsonProperty("variants")]
        public List<BuildingMaterialVariantSeed> Variants { get; set; } = new();
    }

    public sealed class BuildingMaterialVariantSeed
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("rho_kg_m3")]
        public double? RhoKgM3 { get; set; }

        [JsonProperty("c_j_kgk")]
        public double? CJKgK { get; set; }

        [JsonProperty("lambda_w_mk")]
        public double? LambdaWMK { get; set; }

        [JsonProperty("mu")]
        public double? Mu { get; set; }
    }

    public sealed class BuildingMaterialUser
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name_bg")]
        public string NameBg { get; set; } = string.Empty;

        [JsonProperty("variants")]
        public List<BuildingMaterialVariantUser> Variants { get; set; } = new();
    }

    public sealed class BuildingMaterialVariantUser
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("rho_kg_m3")]
        public double? RhoKgM3 { get; set; }

        [JsonProperty("c_j_kgk")]
        public double? CJKgK { get; set; }

        [JsonProperty("lambda_w_mk")]
        public double? LambdaWMK { get; set; }

        [JsonProperty("mu")]
        public double? Mu { get; set; }
    }

    public sealed class BuildingMaterialRow
    {
        public string Id { get; set; } = string.Empty;
        public string NameBg { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsSeed { get; set; }

        // Flatten one selected variant for grid preview.
        public double? RhoKgM3 { get; set; }
        public double? CJKgK { get; set; }
        public double? LambdaWMK { get; set; }
        public double? Mu { get; set; }

        public int VariantCount { get; set; }
    }

    /// <summary>
    /// Flattened material option for layer dropdowns.
    /// Each variant becomes a separate option.
    /// </summary>
    public sealed class MaterialOption
    {
        /// <summary>
        /// Composite ID: materialId|variantId
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Material name in Bulgarian (without code prefix)
        /// </summary>
        public string NameBg { get; set; } = string.Empty;

        /// <summary>
        /// Lambda value (W/mK) for this variant
        /// </summary>
        public double LambdaWmk { get; set; }

        /// <summary>
        /// Display string for dropdown
        /// </summary>
        public string Display => $"{NameBg} (λ={LambdaWmk:0.###} W/mK)";
    }
}
