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
            // default: seed + user
            return GetCombinedRows(includeSeed: true, includeTypical: false, includeUser: true);
        }

        public IReadOnlyList<BuildingMaterialRow> GetCombinedRows(bool includeSeed, bool includeTypical, bool includeUser)
        {
            var rows = new List<BuildingMaterialRow>();

            if (includeSeed)
            {
                var seed = _repo.LoadSeed();
                rows.AddRange(seed.Select(ToRow));
            }

            if (includeTypical)
            {
                var typical = _repo.LoadTypical();
                rows.AddRange(typical.Select(s =>
                {
                    var r = ToRow(s);
                    // treat typical as readonly (same as seed)
                    r.IsSeed = true;
                    return r;
                }));
            }

            if (includeUser)
            {
                var user = _repo.LoadUser();
                var userRows = user
                    .Where(u => !string.IsNullOrWhiteSpace(u.NameBg))
                    .Select(ToRow)
                    .ToList();
                rows.AddRange(userRows);
            }

            // Seed/typical first, then user.
            return rows
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

        /// <summary>
        /// Returns all materials (seed + user) flattened by variants.
        /// Each variant becomes a separate MaterialOption.
        /// </summary>
        public IReadOnlyList<MaterialOption> GetMaterialOptionsFlattened()
        {
            var options = new List<MaterialOption>();

            // Process seed materials
            var seed = _repo.LoadSeed();
            foreach (var mat in seed)
            {
                if (mat.Variants != null)
                {
                    foreach (var variant in mat.Variants)
                    {
                        if (variant.LambdaWMK.HasValue)
                        {
                            options.Add(new MaterialOption
                            {
                                Id = $"{mat.Id}|{variant.Id}",
                                NameBg = mat.NameBg,
                                LambdaWmk = variant.LambdaWMK.Value
                            });
                        }
                    }
                }
            }

            // Process user materials
            var user = _repo.LoadUser();
            foreach (var mat in user)
            {
                if (!string.IsNullOrWhiteSpace(mat.NameBg) && mat.Variants != null)
                {
                    foreach (var variant in mat.Variants)
                    {
                        if (variant.LambdaWMK.HasValue)
                        {
                            options.Add(new MaterialOption
                            {
                                Id = $"{mat.Id}|{variant.Id}",
                                NameBg = mat.NameBg,
                                LambdaWmk = variant.LambdaWMK.Value
                            });
                        }
                    }
                }
            }

            return options;
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
