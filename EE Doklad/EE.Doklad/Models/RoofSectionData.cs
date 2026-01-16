using System.Collections.Generic;

namespace EE.Doklad.Models
{
    public class RoofSectionData
    {
        public string Description { get; set; } = string.Empty;
        public List<RoofType> RoofTypes { get; set; } = new();
    }

    public class RoofType
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public RoofMode Mode { get; set; }
        public decimal? U { get; set; } // null or placeholder for cold roof
        public decimal A { get; set; }
        public WarmRoofDetail? WarmDetail { get; set; }
        public ColdRoofDetail? ColdDetail { get; set; }
    }

    public enum RoofMode
    {
        Warm,
        Cold
    }

    public class WarmRoofDetail
    {
        public List<RoofLayer> Layers { get; set; } = new();
        public decimal Rsi { get; set; } = 0.10m;
        public decimal Rse { get; set; } = 0.04m;
    }

    public class ColdRoofDetail
    {
        // 5.1 Geometry
        public decimal Vp { get; set; } // V′ (m³)
        public decimal Ap { get; set; } // A′ (m²)
        public decimal? Deltavc => (Ap > 0) ? Vp / Ap : null; // δвс (m)
        // 5.2 Areas
        public decimal A1 { get; set; } // m²
        public decimal A2 { get; set; } // m²
        public decimal Aw { get; set; } // m²
        // 5.3 Ventilation
        public ColdRoofSpaceType SpaceType { get; set; } = ColdRoofSpaceType.Sealed;
        public decimal n { get; set; } = 0.1m; // h⁻¹
        public decimal V { get; set; } // m³
        // 5.4 Temperatures
        public decimal Ti { get; set; } // θi (°C)
        public decimal Te { get; set; } // θe (°C)
        // 5.5 Constructions
        public RoofLayerTable U1 { get; set; } = new();
        public RoofLayerTable U2 { get; set; } = new();
        public RoofLayerTable Uw { get; set; } = new();
    }

    public enum ColdRoofSpaceType
    {
        Sealed, // уплътнено
        Unsealed // неуплътнено
    }

    public class RoofLayerTable
    {
        public List<RoofLayer> Layers { get; set; } = new();
        public decimal Rsi { get; set; } = 0.10m;
        public decimal Rse { get; set; } = 0.04m;
        public bool RsiEditable { get; set; } = true;
        public bool RseEditable { get; set; } = true;
    }

    public class RoofLayer
    {
        public string Material { get; set; } = string.Empty;
        public decimal Thickness { get; set; } // δ (m)
        public decimal Lambda { get; set; } // λ (W/mK)
        public decimal R => (Lambda > 0) ? Thickness / Lambda : 0;
    }
}
