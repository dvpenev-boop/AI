using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Read-only facade over methodology climate data (seeded in code/repo; not user-editable).
    /// </summary>
    public sealed class ClimateService
    {
        private readonly ClimateSeed _seed;
        private readonly Dictionary<int, ClimateZoneData> _byId;

        public ClimateService(IClimateRepository repository)
        {
            _seed = repository.LoadSeed();
            _byId = _seed.Zones.ToDictionary(z => z.Id);
        }

        public string ImportedBy => _seed.ImportedBy;
        public string Source => _seed.Source;
        public string Revision => _seed.Revision;
        public string Date => _seed.Date;

        public IReadOnlyList<ClimateZoneData> GetAllZones() => _seed.Zones;

        public ClimateZoneData GetZone(int id)
        {
            if (_byId.TryGetValue(id, out var z))
                return z;

            throw new ArgumentOutOfRangeException(nameof(id), $"Няма дефинирана климатична зона с Id={id}.");
        }

        public bool TryGetZone(int id, out ClimateZoneData? zone)
        {
            if (_byId.TryGetValue(id, out var z))
            {
                zone = z;
                return true;
            }

            zone = null;
            return false;
        }
    }
}
