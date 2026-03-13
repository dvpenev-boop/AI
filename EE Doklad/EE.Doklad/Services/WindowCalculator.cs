using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Режим на агрегиране за обобщените таблици
    /// </summary>
    public enum WindowSummarizationMode
    {
        /// <summary>Отопление – използва GEffHeat</summary>
        Heating,
        /// <summary>Охлаждане – използва GEffCool</summary>
        Cooling
    }

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
                        SystemLabel = firstBatch.SystemDisplayLabel,
                        TypeName = string.IsNullOrEmpty(firstBatch.TypeName) 
                            ? $"{firstBatch.Width:F2}×{firstBatch.Height:F2} {GetKindLabel(firstBatch.Kind)}"
                            : firstBatch.TypeName,
                        TotalCount = batchesList.Sum(b => b.Count),
                        ATotalGross = batchesList.Sum(b => b.Count * b.AreaGross),
                        ATotalGlass = batchesList.Sum(b => b.Count * b.AreaGlass),
                        UAvg = CalculateUAvg(batchesList),
                        GAvg = CalculateGAvgForMode(batchesList, WindowSummarizationMode.Heating),
                        GAvgHeat = CalculateGAvgForMode(batchesList, WindowSummarizationMode.Heating),
                        GAvgCool = CalculateGAvgForMode(batchesList, WindowSummarizationMode.Cooling),
                        Batches = batchesList
                    };
                })
                .OrderBy(r => r.Orientation)
                .ThenBy(r => r.TypeName)
                .ToList();

            return groups;
        }

        /// <summary>
        /// Групира партиди по фасада и тип прозорец за конкретен режим (Отопление/Охлаждане).
        /// Единна логика без copy-paste – разликата е само кои g_eff стойности влизат.
        /// </summary>
        public static List<WindowSummaryRow> GroupBatchesForMode(
            IEnumerable<WindowBatch> batches,
            WindowSummarizationMode mode,
            List<int>? coolingMonths = null)
        {
            var groups = batches
                .GroupBy(b => new
                {
                    b.Orientation,
                    TypeSignature = GetTypeSignature(b)
                })
                .Select(g =>
                {
                    var batchesList = g.ToList();
                    var firstBatch = batchesList.First();
                    double gAvg = CalculateGAvgForMode(batchesList, mode, coolingMonths);

                    return new WindowSummaryRow
                    {
                        Orientation = g.Key.Orientation,
                        TypeSignature = g.Key.TypeSignature,
                        SystemLabel = firstBatch.SystemDisplayLabel,
                        TypeName = string.IsNullOrEmpty(firstBatch.TypeName)
                            ? $"{firstBatch.Width:F2}×{firstBatch.Height:F2} {GetKindLabel(firstBatch.Kind)}"
                            : firstBatch.TypeName,
                        TotalCount = batchesList.Sum(b => b.Count),
                        ATotalGross = batchesList.Sum(b => b.Count * b.AreaGross),
                        ATotalGlass = batchesList.Sum(b => b.Count * b.AreaGlass),
                        UAvg = CalculateUAvg(batchesList),
                        GAvg = gAvg,
                        GAvgHeat = mode == WindowSummarizationMode.Heating ? gAvg : CalculateGAvgForMode(batchesList, WindowSummarizationMode.Heating),
                        GAvgCool = mode == WindowSummarizationMode.Cooling ? gAvg : CalculateGAvgForMode(batchesList, WindowSummarizationMode.Cooling, coolingMonths),
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
        /// Изчислява средно претеглен g спрямо площта на стъклото – за съответния режим.
        /// Режим Heating → GEffHeat; Режим Cooling → GEffCool.
        /// Ако g_eff_heat/g_eff_cool не са изрично зададени (0), се използва GEff (обединена стойност).
        /// </summary>
        private static double CalculateGAvgForMode(List<WindowBatch> batches, WindowSummarizationMode mode,
                                                    List<int>? coolingMonths = null)
        {
            double totalGAgl = batches.Sum(b =>
            {
                double gEff = GetGEffForMode(b, mode, coolingMonths);
                return b.Count * b.AreaGlass * gEff;
            });
            double totalAgl = batches.Sum(b => b.Count * b.AreaGlass);

            return totalAgl > 0 ? totalGAgl / totalAgl : 0;
        }

        /// <summary>
        /// Изчислява средно претеглен g спрямо площта на стъклото (обратна съвместимост)
        /// </summary>
        private static double CalculateGAvg(List<WindowBatch> batches)
        {
            return CalculateGAvgForMode(batches, WindowSummarizationMode.Heating);
        }

        /// <summary>
        /// Връща g_eff за партида и режим.
        /// При Door + F_fr=100% → 0 (независимо от режима).
        /// Приоритет: 1) GEffHeat/GEffCool (изчислени при Save), 2) per-mode shading factor × per-mode GEffBase.
        /// </summary>
        public static double GetGEffForMode(WindowBatch batch, WindowSummarizationMode mode)
        {
            // Плътна врата – няма остъкляване, g=0 за двата режима
            if (batch.Kind == WindowKind.Door && batch.FrameFraction >= 1.0)
                return 0.0;

            if (mode == WindowSummarizationMode.Heating)
            {
                // Ако е изрично зададено при Save, ползваме него
                if (batch.GEffHeat > 0) return batch.GEffHeat;
                // Иначе – изчисляваме от per-mode shading factor × per-mode GEffBase
                double srf = batch.ShadingModeHeat > 0 ? batch.ShadingReductionFactorHeat : 1.0;
                return batch.GEffBaseHeat * srf;
            }
            else
            {
                if (batch.GEffCool > 0) return batch.GEffCool;
                double srf = batch.ShadingModeCool > 0 ? batch.ShadingReductionFactorCool : 1.0;
                return batch.GEffBaseCool * srf;
            }
        }

        /// <summary>
        /// Връща g_eff за партида и режим, като за охлаждане ПРЕИЗЧИСЛЯВА стойността
        /// от FshDirMonthly и текущите месеци на охладителния сезон (live season months).
        /// Използва per-mode GEffBase (GEffBaseHeat / GEffBaseCool) — промяна на единия режим
        /// НЕ влияе на другия.
        /// </summary>
        public static double GetGEffForMode(
            WindowBatch batch,
            WindowSummarizationMode mode,
            List<int>? coolingMonths)
        {
            if (batch.Kind == WindowKind.Door && batch.FrameFraction >= 1.0)
                return 0.0;

            if (mode == WindowSummarizationMode.Heating || coolingMonths == null)
                return GetGEffForMode(batch, mode);

            // Охлаждане с live month list
            if (coolingMonths.Count == 0) return 0.0;

            double srf = batch.ShadingModeCool > 0 ? batch.ShadingReductionFactorCool : 1.0;
            var fsh = batch.FshDirMonthly;
            // If no monthly shading array -> fall back to flat GEffBaseCool * srf
            if (fsh == null || fsh.Length < 12)
                return batch.GEffBaseCool * srf;

            return coolingMonths.Average(m => batch.GEffBaseCool * srf * fsh[m]);
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

        public static string GetSystemLabel(WindowBatch batch)
        {
            return batch.SystemDisplayLabel;
        }

        public static List<WindowProfileSystemOption> GetProfileSystemOptions()
        {
            return new List<WindowProfileSystemOption>
            {
                new() { Id = "PVC_60", Material = "PVC", MountingDepthLabel = "60 mm", MountingDepthMm = 60, VisibleHeightMm = 53 },
                new() { Id = "PVC_70", Material = "PVC", MountingDepthLabel = "70 mm", MountingDepthMm = 70, VisibleHeightMm = 55 },
                new() { Id = "PVC_76", Material = "PVC", MountingDepthLabel = "76 mm", MountingDepthMm = 76, VisibleHeightMm = 58 },
                new() { Id = "PVC_80_82", Material = "PVC", MountingDepthLabel = "80-82 mm", MountingDepthMm = 81, VisibleHeightMm = 60 },
                new() { Id = "PVC_88_90", Material = "PVC", MountingDepthLabel = "88-90 mm", MountingDepthMm = 89, VisibleHeightMm = 63 },
                new() { Id = "AL_60", Material = "AL", MountingDepthLabel = "60 mm", MountingDepthMm = 60, VisibleHeightMm = 48 },
                new() { Id = "AL_65_70", Material = "AL", MountingDepthLabel = "65-70 mm", MountingDepthMm = 67.5, VisibleHeightMm = 53 },
                new() { Id = "AL_75_80", Material = "AL", MountingDepthLabel = "75-80 mm", MountingDepthMm = 77.5, VisibleHeightMm = 56 },
                new() { Id = "AL_90", Material = "AL", MountingDepthLabel = "90 mm", MountingDepthMm = 90, VisibleHeightMm = 60 },
                new() { Id = "OTHER", Material = "Друго", MountingDepthLabel = "Ръчно", RequiresManualInput = true }
            };
        }

        public static List<WindowThermalBridgeOption> GetThermalBridgeOptions()
        {
            return new List<WindowThermalBridgeOption>
            {
                new() { Id = "NO_INSULATION", InstallationType = "Фасада без изолация", Psi = 0.06 },
                new() { Id = "STANDARD_RETURN", InstallationType = "Стандартно обръщане", Psi = 0.04 },
                new() { Id = "OFFSET_INSTALL", InstallationType = "Изнесен монтаж", Psi = 0.01 }
            };
        }

        public static List<WindowSystemLossSummaryRow> BuildSystemLossSummary(IEnumerable<WindowBatch> batches)
        {
            var thermalBridgeOptions = GetThermalBridgeOptions();

            return batches
                .GroupBy(GetSystemLabel)
                .Select(g =>
                {
                    var items = g.ToList();
                    double totalArea = items.Sum(b => b.Count * b.AreaGross);
                    double hel = items.Sum(b => b.Count * b.AreaGross * b.UValue);
                    double htb = items.Sum(b => b.Count * CalculatePerimeter(b) * b.ThermalBridgePsiDisplay);

                    return new WindowSystemLossSummaryRow
                    {
                        SystemLabel = string.IsNullOrWhiteSpace(g.Key) ? "-" : g.Key,
                        TotalArea = totalArea,
                        AverageUw = totalArea > 0 ? hel / totalArea : 0.0,
                        Hel = hel,
                        Htb = htb,
                        Htotal = hel + htb,
                        ThermalBridgeModeLabel = items.Any(b => b.HasThermalBridge) ? "детайлно" : "няма",
                        Batches = items,
                        ThermalBridgeOptions = thermalBridgeOptions
                    };
                })
                .OrderBy(r => r.SystemLabel)
                .ToList();
        }

        public static double CalculatePerimeter(WindowBatch batch)
        {
            if (batch.Width <= 0 || batch.Height <= 0)
                return 0.0;

            return 2.0 * (batch.Width + batch.Height);
        }

        public static double CalculateFrameFractionFromProfile(double widthMeters, double heightMeters, double visibleHeightMm)
        {
            if (widthMeters <= 0 || heightMeters <= 0 || visibleHeightMm <= 0)
                return 0;

            double profileVisibleMeters = visibleHeightMm / 1000.0;
            double glassWidth = Math.Max(0, widthMeters - 2 * profileVisibleMeters);
            double glassHeight = Math.Max(0, heightMeters - 2 * profileVisibleMeters);
            double grossArea = widthMeters * heightMeters;
            if (grossArea <= 0)
                return 0;

            double glassArea = glassWidth * glassHeight;
            return Math.Clamp(1.0 - (glassArea / grossArea), 0.0, 1.0);
        }

        public static double CalculateUwFromDetailedInputs(double frameFraction, double uFrame, double uGlass)
        {
            frameFraction = Math.Clamp(frameFraction, 0.0, 1.0);
            return frameFraction * uFrame + (1.0 - frameFraction) * uGlass;
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

        // ── Seasonal month helpers ────────────────────────────────────────────────

        /// <summary>
        /// Връща списък с 0-based месечни индекси (0=Jan…11=Dec) за периода
        /// [startMonth1to12 .. endMonth1to12], третирайки всеки месец като ПЪЛЕН.
        /// Ако startMonth &gt; endMonth → периодът обхваща края на годината
        /// (например 10-12 и след това 1-4).
        /// Ако startMonth или endMonth е извън [1..12] → връща празен списък.
        /// </summary>
        public static List<int> GetMonthsInclusive(int startMonth1to12, int endMonth1to12)
        {
            var months = new List<int>();
            if (startMonth1to12 < 1 || startMonth1to12 > 12) return months;
            if (endMonth1to12   < 1 || endMonth1to12   > 12) return months;

            int sm = startMonth1to12 - 1; // convert to 0-based
            int em = endMonth1to12   - 1;

            if (sm <= em)
            {
                for (int m = sm; m <= em; m++)
                    months.Add(m);
            }
            else
            {
                // cross-year: e.g. Oct(9) → Apr(3)
                for (int m = sm; m < 12; m++)
                    months.Add(m);
                for (int m = 0; m <= em; m++)
                    months.Add(m);
            }
            return months;
        }

        /// <summary>
        /// Tries to build the heating month list from Section-5 data (climate-zone lookup).
        /// Returns true and a non-empty list if a valid heating season is defined and enabled.
        /// startMonth/endMonth are 1-based (October = 10, April = 4).
        /// </summary>
        public static bool TryGetHeatingMonths(
            int? climateZone, bool heatingEnabled,
            out List<int> months)
        {
            months = new List<int>();
            if (!heatingEnabled) return false;
            int zone = climateZone ?? 1;
            zone = Math.Clamp(zone, 1, 9);
            // heating season start/end per zone (same data as AddWindowFullDialog.HeatingSeason)
            (int sm, int em)[] heatZones =
            {
                (10, 4), // zone 1
                (10, 4), // zone 2
                (10, 4), // zone 3
                (10, 4), // zone 4
                (10, 4), // zone 5
                (10, 4), // zone 6
                (10, 4), // zone 7
                (10, 4), // zone 8
                (10, 4), // zone 9
            };
            var z = heatZones[zone - 1];
            months = GetMonthsInclusive(z.sm, z.em);
            return months.Count > 0;
        }

        /// <summary>
        /// Tries to build the cooling month list from Section-5 explicit cooling season months.
        /// Returns true and a non-empty list if both start and end months are valid and the season is enabled.
        /// startMonth and endMonth are 1-based nullable ints (as stored in ObjectDataSectionData).
        /// </summary>
        public static bool TryGetCoolingMonths(
            int? coolingStartMonth, int? coolingEndMonth, bool coolingEnabled,
            out List<int> months)
        {
            months = new List<int>();
            if (!coolingEnabled) return false;
            if (!coolingStartMonth.HasValue || !coolingEndMonth.HasValue) return false;
            months = GetMonthsInclusive(coolingStartMonth.Value, coolingEndMonth.Value);
            return months.Count > 0;
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
