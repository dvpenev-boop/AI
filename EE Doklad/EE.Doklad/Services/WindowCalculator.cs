using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Service за изчисления свързани с прозорци и врати (секция 9)
    /// </summary>
    public static class WindowCalculator
    {
        /// <summary>
        /// Групира партиди по фасада и тип прозорец
        /// </summary>
        public static List<WindowSummaryRow> GroupBatches(IEnumerable<WindowBatch> batches)
        {
            var groups = batches
                .GroupBy(b => new
                {
                    b.Orientation,
                    // Групираме по ключови характеристики
                    TypeSignature = GetTypeSignature(b)
                })
                .Select(g =>
                {
                    var batchesList = g.ToList();
                    var firstBatch = batchesList.First();

                    return new WindowSummaryRow
                    {
                        Orientation = g.Key.Orientation,
                        TypeSignature = g.Key.TypeSignature,
                        TypeName = string.IsNullOrEmpty(firstBatch.TypeName) 
                            ? $"{firstBatch.Width:F2}×{firstBatch.Height:F2} {GetKindLabel(firstBatch.Kind)}"
                            : firstBatch.TypeName,
                        TotalCount = batchesList.Sum(b => b.Count),
                        ATotalGross = batchesList.Sum(b => b.Count * b.AreaGross),
                        ATotalGlass = batchesList.Sum(b => b.Count * b.AreaGlass),
                        UAvg = CalculateUAvg(batchesList),
                        GAvg = CalculateGAvg(batchesList),
                        Batches = batchesList
                    };
                })
                .OrderBy(r => r.Orientation)
                .ThenBy(r => r.TypeName)
                .ToList();

            return groups;
        }

        /// <summary>
        /// Генерира сигнатура на типа за групиране
        /// </summary>
        private static string GetTypeSignature(WindowBatch batch)
        {
            if (!string.IsNullOrEmpty(batch.TypeName))
            {
                return batch.TypeName;
            }

            // Ако няма TypeName, групираме по размери и характеристики
            return $"{batch.Width:F2}x{batch.Height:F2}|{batch.Kind}|{batch.UValue:F2}|{batch.GN:F3}|{batch.OpticalType}|{batch.FrameFraction:F2}";
        }

        /// <summary>
        /// Изчислява средно претеглен U спрямо площта
        /// </summary>
        private static double CalculateUAvg(List<WindowBatch> batches)
        {
            double totalUA = batches.Sum(b => b.Count * b.AreaGross * b.UValue);
            double totalA = batches.Sum(b => b.Count * b.AreaGross);

            return totalA > 0 ? totalUA / totalA : 0;
        }

        /// <summary>
        /// Изчислява средно претеглен g спрямо площта на стъклото
        /// </summary>
        private static double CalculateGAvg(List<WindowBatch> batches)
        {
            double totalGAgl = batches.Sum(b => b.Count * b.AreaGlass * b.GEff);
            double totalAgl = batches.Sum(b => b.Count * b.AreaGlass);

            return totalAgl > 0 ? totalGAgl / totalAgl : 0;
        }

        /// <summary>
        /// Връща етикет за вида прозорец/врата
        /// </summary>
        private static string GetKindLabel(WindowKind kind)
        {
            return kind switch
            {
                WindowKind.Window => "Прозорец",
                WindowKind.Door => "Врата",
                _ => "Неизвестен"
            };
        }

        /// <summary>
        /// Връща етикет за ориентация
        /// </summary>
        public static string GetOrientationLabel(Orientation orientation)
        {
            return orientation switch
            {
                Orientation.East => "И",
                Orientation.NorthEast => "СИ",
                Orientation.North => "С",
                Orientation.NorthWest => "СЗ",
                Orientation.West => "З",
                Orientation.SouthWest => "ЮЗ",
                Orientation.South => "Ю",
                Orientation.SouthEast => "ЮИ",
                _ => "?"
            };
        }

        /// <summary>
        /// Връща g_alt (g_gl,alt;wi) за даден вид остъкляване (Таблица 3 - използвани стойности)
        /// </summary>
        public static double GetGlazingGAlt(GlazingType glazing)
        {
            return glazing switch
            {
                GlazingType.Single => 0.85,
                GlazingType.Double => 0.75,
                GlazingType.DoubleSelective => 0.67,
                GlazingType.Triple => 0.70,
                GlazingType.TripleSelective => 0.50,
                _ => 0.75
            };
        }

        /// <summary>
        /// Изчислява площта на стъклото: A_gl = A_gross * (1 - F_fr)
        /// </summary>
        public static double CalculateAreaGlass(double areaGross, double frameFraction)
        {
            return areaGross * (1 - frameFraction);
        }

        /// <summary>
        /// Изчислява g_eff_base по формули 3.41 и 3.42
        /// </summary>
        public static double CalculateGEffBase(double gN, OpticalType opticalType, bool hasShading)
        {
            if (opticalType == OpticalType.Clear && !hasShading)
            {
                // Формула 3.41
                return 0.90 * gN;
            }
            else
            {
                // Формула 3.42
                // TODO: В първа версия използваме placeholder g_alt = g_dif = g_n
                double gAlt = gN;
                double gDif = gN;
                return 0.75 * gAlt + 0.25 * gDif;
            }
        }

        /// <summary>
        /// Изчислява финалното g_eff след прилагане на shading коефициент
        /// </summary>
        public static double CalculateGEff(double gEffBase, double shadingReductionFactor)
        {
            return gEffBase * shadingReductionFactor;
        }

        /// <summary>
        /// Връща каталог с опции за слънцезащита (Таблица 4)
        /// </summary>
        public static List<ShadingOption> GetShadingOptions()
        {
            var options = new List<ShadingOption>();

            // A) Бели венециански щори (α = 0.10)
            options.Add(new ShadingOption
            {
                Id = "whiteBlind_005",
                CategoryName = "Бели венециански щори",
                AbsorptionAlpha = 0.10,
                TransmittanceTau = 0.05,
                FShadeInt = 0.25,
                FShadeExt = 0.10
            });
            options.Add(new ShadingOption
            {
                Id = "whiteBlind_010",
                CategoryName = "Бели венециански щори",
                AbsorptionAlpha = 0.10,
                TransmittanceTau = 0.10,
                FShadeInt = 0.30,
                FShadeExt = 0.15
            });
            options.Add(new ShadingOption
            {
                Id = "whiteBlind_030",
                CategoryName = "Бели венециански щори",
                AbsorptionAlpha = 0.10,
                TransmittanceTau = 0.30,
                FShadeInt = 0.45,
                FShadeExt = 0.35
            });

            // B) Бели завеси (α = 0.10)
            options.Add(new ShadingOption
            {
                Id = "whiteCurtain_050",
                CategoryName = "Бели завеси",
                AbsorptionAlpha = 0.10,
                TransmittanceTau = 0.50,
                FShadeInt = 0.65,
                FShadeExt = 0.55
            });
            options.Add(new ShadingOption
            {
                Id = "whiteCurtain_070",
                CategoryName = "Бели завеси",
                AbsorptionAlpha = 0.10,
                TransmittanceTau = 0.70,
                FShadeInt = 0.80,
                FShadeExt = 0.75
            });
            options.Add(new ShadingOption
            {
                Id = "whiteCurtain_090",
                CategoryName = "Бели завеси",
                AbsorptionAlpha = 0.10,
                TransmittanceTau = 0.90,
                FShadeInt = 0.95,
                FShadeExt = 0.95
            });

            // C) Цветен текстил (α = 0.30)
            options.Add(new ShadingOption
            {
                Id = "coloredTextile_010",
                CategoryName = "Цветен текстил",
                AbsorptionAlpha = 0.30,
                TransmittanceTau = 0.10,
                FShadeInt = 0.42,
                FShadeExt = 0.17
            });
            options.Add(new ShadingOption
            {
                Id = "coloredTextile_030",
                CategoryName = "Цветен текстил",
                AbsorptionAlpha = 0.30,
                TransmittanceTau = 0.30,
                FShadeInt = 0.57,
                FShadeExt = 0.37
            });
            options.Add(new ShadingOption
            {
                Id = "coloredTextile_050",
                CategoryName = "Цветен текстил",
                AbsorptionAlpha = 0.30,
                TransmittanceTau = 0.50,
                FShadeInt = 0.77,
                FShadeExt = 0.57
            });

            // D) Текстил с алуминиево покритие (α = 0.20)
            options.Add(new ShadingOption
            {
                Id = "aluminumTextile_005",
                CategoryName = "Текстил с алуминиево покритие",
                AbsorptionAlpha = 0.20,
                TransmittanceTau = 0.05,
                FShadeInt = 0.20,
                FShadeExt = 0.08
            });

            return options;
        }

        /// <summary>
        /// Групира опциите за слънцезащита по категория
        /// </summary>
        public static Dictionary<string, List<ShadingOption>> GetShadingOptionsByCategory()
        {
            return GetShadingOptions()
                .GroupBy(o => o.CategoryName)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Връща предефинирани профили на препятствия
        /// </summary>
        public static List<ObstacleProfile> GetObstacleProfiles()
        {
            var profiles = new List<ObstacleProfile>();

            // None - без препятствия
            profiles.Add(new ObstacleProfile
            {
                Id = "none",
                Name = "Без препятствия",
                MonthlyFactors = Enumerable.Repeat(1.0, 12).ToArray()
            });

            // Балкон (типичен пример - засенчване по-силно през лятото)
            profiles.Add(new ObstacleProfile
            {
                Id = "balcony",
                Name = "Балкон",
                MonthlyFactors = new[] { 0.7, 0.7, 0.6, 0.5, 0.5, 0.5, 0.5, 0.5, 0.6, 0.7, 0.7, 0.7 }
            });

            // Съседна сграда
            profiles.Add(new ObstacleProfile
            {
                Id = "adjacentBuilding",
                Name = "Съседна сграда",
                MonthlyFactors = new[] { 0.5, 0.5, 0.6, 0.6, 0.7, 0.7, 0.7, 0.6, 0.6, 0.5, 0.5, 0.5 }
            });

            // Дървета (по-силно засенчване когато има листа)
            profiles.Add(new ObstacleProfile
            {
                Id = "trees",
                Name = "Дървета",
                MonthlyFactors = new[] { 0.9, 0.9, 0.8, 0.6, 0.5, 0.4, 0.4, 0.4, 0.5, 0.7, 0.8, 0.9 }
            });

            // Custom - потребителят задава сам
            profiles.Add(new ObstacleProfile
            {
                Id = "custom",
                Name = "Custom",
                MonthlyFactors = Enumerable.Repeat(1.0, 12).ToArray()
            });

            return profiles;
        }
    }
}
