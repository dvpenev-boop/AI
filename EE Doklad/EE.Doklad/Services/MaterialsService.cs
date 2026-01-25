using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    public sealed class MaterialsService
    {
        private readonly IMaterialsRepository _repo;

        public MaterialsService(IMaterialsRepository repo)
        {
            _repo = repo;
        }

        public IReadOnlyList<BuildingMaterialSeed> GetSeed() => _repo.LoadSeed();

        public IReadOnlyList<BuildingMaterialUser> GetUser() => _repo.LoadUser();

        public IReadOnlyList<BuildingMaterialRow> GetCombinedRows()
        {
            var seed = _repo.LoadSeed();
            var user = _repo.LoadUser();

            var seedRows = seed.Select(ToRow).ToList();
            var userRows = user.Select(ToRow).ToList();

            // Seed first, then user.
            return seedRows.Concat(userRows)
                .OrderBy(r => r.IsSeed ? 0 : 1)
                .ThenBy(r => r.Code ?? string.Empty)
                .ThenBy(r => r.NameBg)
                .ToList();
        }

        public void AddUserMaterial(BuildingMaterialUser mat)
        {
            if (string.IsNullOrWhiteSpace(mat.Id))
                mat.Id = "user-" + Guid.NewGuid().ToString("N");

            foreach (var v in mat.Variants)
                if (string.IsNullOrWhiteSpace(v.Id))
                    v.Id = mat.Id + "-v" + Guid.NewGuid().ToString("N");

            var list = _repo.LoadUser().ToList();
            list.Add(mat);
            _repo.SaveUser(list);
        }

        public void UpdateUserMaterial(BuildingMaterialUser mat)
        {
            if (string.IsNullOrWhiteSpace(mat.Id))
                throw new InvalidOperationException("Липсва Id на материал.");

            var list = _repo.LoadUser().ToList();
            var idx = list.FindIndex(x => x.Id == mat.Id);
            if (idx < 0)
                throw new InvalidOperationException("Материалът не е намерен.");

            foreach (var v in mat.Variants)
                if (string.IsNullOrWhiteSpace(v.Id))
                    v.Id = mat.Id + "-v" + Guid.NewGuid().ToString("N");

            list[idx] = mat;
            _repo.SaveUser(list);
        }

        public void DeleteUserMaterial(string id)
        {
            var list = _repo.LoadUser().ToList();
            list.RemoveAll(x => x.Id == id);
            _repo.SaveUser(list);
        }

        private static BuildingMaterialRow ToRow(BuildingMaterialSeed s)
        {
            var v0 = s.Variants?.FirstOrDefault();
            return new BuildingMaterialRow
            {
                Id = s.Id,
                Code = s.Code,
                NameBg = s.NameBg,
                IsSeed = true,
                RhoKgM3 = v0?.RhoKgM3,
                CJKgK = v0?.CJKgK,
                LambdaWMK = v0?.LambdaWMK,
                Mu = v0?.Mu,
                VariantCount = s.Variants?.Count ?? 0
            };
        }

        private static BuildingMaterialRow ToRow(BuildingMaterialUser u)
        {
            var v0 = u.Variants?.FirstOrDefault();
            return new BuildingMaterialRow
            {
                Id = u.Id,
                Code = null,
                NameBg = u.NameBg,
                IsSeed = false,
                RhoKgM3 = v0?.RhoKgM3,
                CJKgK = v0?.CJKgK,
                LambdaWMK = v0?.LambdaWMK,
                Mu = v0?.Mu,
                VariantCount = u.Variants?.Count ?? 0
            };
        }
    }
}
