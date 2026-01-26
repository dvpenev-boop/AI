using System.Collections.Generic;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    public interface IMaterialsRepository
    {
        IReadOnlyList<BuildingMaterialSeed> LoadSeed();
        IReadOnlyList<BuildingMaterialSeed> LoadTypical();
        IReadOnlyList<BuildingMaterialUser> LoadUser();
        void SaveUser(IReadOnlyList<BuildingMaterialUser> materials);
    }
}
