using System;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под към неотопляем сутерен с изчисление на контакт със земята
    /// Изчислява еквивалентния коефициент Uub според спецификацията
    /// </summary>
    public class UnheatedBasementFloorStrategy : IFloorStrategy<FloorUnheatedBasementInput>
    {
        // Константи
        private const double RHO = 1.2;      // kg/m³
        private const double CP = 0.28;       // Wh/(kg·K)

        public FloorCalculationResult Calculate(FloorUnheatedBasementInput input)
        {
            var result = new FloorCalculationResult
            {
                FloorType = FloorType.UnheatedBasement,
                Area = input.Area
            };

            try
            {
                // === Валидация на входните данни ===
                if (input.Area <= 0 || input.Perimeter <= 0)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Площта и периметърът трябва да са положителни.";
                    return result;
                }

                // === 1. Изчисляване на U по ISO 6946 ===

                // 1.1 Под между отопляемото и сутерена (Uf_sus)
                double rLayersFloorToBasement = input.FloorToBasementLayers.Sum(layer => layer.Thickness / layer.Lambda);
                double rTotalFloorToBasement = input.RsiFloorToBasement + rLayersFloorToBasement + input.RseFloorToBasement;
                double ufSus = rTotalFloorToBasement > 0 ? 1.0 / rTotalFloorToBasement : 0;

                // 1.2 Подова плоча на сутерена (R_fb)
                double rLayersBasementFloor = input.BasementFloorLayers.Sum(layer => layer.Thickness / layer.Lambda);
                double rFb = input.RsiBasementFloor + rLayersBasementFloor + input.RseBasementFloor;

                // 1.3 Сутеренни стени към земя (R_wb)
                double rLayersBasementWall = input.BasementWallLayers.Sum(layer => layer.Thickness / layer.Lambda);
                double rWb = input.RsiBasementWall + rLayersBasementWall + input.RseBasementWall;

                // 1.4 Сутеренни стени над терена (Uw)
                double rLayersWallAboveGrade = input.WallAboveGradeLayers.Sum(layer => layer.Thickness / layer.Lambda);
                double rTotalWallAboveGrade = input.RsiWallAboveGrade + rLayersWallAboveGrade + input.RseWallAboveGrade;
                double uw = rTotalWallAboveGrade > 0 ? 1.0 / rTotalWallAboveGrade : 0;

                    // === Прозорци и врати в надтеренната част ===
                    double a_ag = input.HeightAboveGround * input.Perimeter;
                    double a_win = input.WindowArea;
                    double a_door = input.DoorArea;
                    double a_opaque = Math.Max(0, a_ag - a_win - a_door);
                    double u_win = input.WindowUValue;
                    double u_door = input.DoorUValue;
                    // Сборен топлопренос през надтеренната част
                    double aboveGradeTransfer = (uw * a_opaque) + (u_win * a_win) + (u_door * a_door);
                // === 2. Контакт със земята ===

                // 2.1 Еквивалентна дебелина на подовата плоча на сутерена
                double df = input.WallThicknessAtGrade + input.LambdaGround * rFb;

                // 2.2 Характеристичен размер
                double b = input.Area / (0.5 * input.Perimeter);

                // 2.3 Коефициент на пода на сутерена към земя (Uf_gb)
                double ufGb;
                double z = input.DepthBelowGround;

                if (df + 0.5 * z < b)
                {
                    // Коригирана формула по БДС EN ISO 13370: знаменателят е (πB + df + 0.5z)
                    double denominator = (Math.PI * b) + df + 0.5 * z;
                    double lnArg = (Math.PI * b) / (df + 0.5 * z) + 1.0;
                    ufGb = (2.0 * input.LambdaGround * Math.Log(lnArg)) / denominator;
                }
                else
                {
                    double denominator = 0.457 * b + df + 0.5 * z;
                    ufGb = input.LambdaGround / denominator;
                }

                // 2.4 Еквивалентна дебелина на стените на сутерена
                double dwb = input.LambdaGround * rWb;

                // 2.5 Коефициент на стените на сутерена към земя (Uw_gb)
                double uwGb = 0;
                if (z > 0)
                {
                    // Проверка: ако dwb < df, замени df = dwb (според спецификацията, раздел 8)
                    double dfForWall = df;
                    if (dwb < df)
                    {
                        dfForWall = dwb;
                    }

                    double factor = 1.0 + (0.5 * dfForWall) / (dfForWall + z);
                    double lnArg = z / dwb + 1.0;
                    uwGb = (2.0 * input.LambdaGround / (Math.PI * z)) * factor * Math.Log(lnArg);
                }

                // === 3. Вентилационна проводимост на сутерена ===
                // По БДС EN ISO 13370: използва се константа 0.33 Wh/(m³·K) за cp * ρ
                double hveB = 0.33 * input.AirChangeRate * input.Volume;

                // === 4. Еквивалентен коефициент Uub ===

                // 4.1 Сборен "изход" от сутерена
                double s = (input.Area * ufGb) +
                           (z * input.Perimeter * uwGb) +
                               aboveGradeTransfer +
                           hveB;

                // 4.2 Краен резултат
                double uub = 0;
                if (s > 0)
                {
                    double denominator = (1.0 / ufSus) + (input.Area / s);
                    uub = 1.0 / denominator;
                }

                // === Попълване на резултата ===
                result.U = uub;
                result.IsValid = true;

                // Диагностични компоненти
                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uf_sus",
                    Description = "Под между отопляемото и сутерена",
                    Value = ufSus,
                    Unit = "W/m²K"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uf_gb",
                    Description = "Под на сутерена към земя",
                    Value = ufGb,
                    Unit = "W/m²K"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uw_gb",
                    Description = "Стени на сутерена към земя",
                    Value = uwGb,
                    Unit = "W/m²K"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uw",
                    Description = "Стени на сутерена над терена",
                    Value = uw,
                    Unit = "W/m²K"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Hve_b",
                    Description = "Вентилационна проводимост на сутерена",
                    Value = hveB,
                    Unit = "W/K"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "B",
                    Description = "Характеристичен размер",
                    Value = b,
                    Unit = "m"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "df",
                    Description = "Еквивалентна дебелина на подовата плоча",
                    Value = df,
                    Unit = "m"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "dwb",
                    Description = "Еквивалентна дебелина на стените на сутерена",
                    Value = dwb,
                    Unit = "m"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "S",
                    Description = "Сборен изход от сутерена",
                    Value = s,
                    Unit = "W/K"
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uub",
                    Description = "Еквивалентен коефициент на топлопреминаване",
                    Value = uub,
                    Unit = "W/m²K"
                });

                // Предположения
                result.Assumptions.Add($"Площ: {input.Area:F2} m²");
                result.Assumptions.Add($"Периметър: {input.Perimeter:F2} m");
                result.Assumptions.Add($"Дълбочина под терена: {z:F2} m");
                result.Assumptions.Add($"Височина над терена: {input.HeightAboveGround:F2} m");
                result.Assumptions.Add($"Обем на сутерена: {input.Volume:F2} m³");
                result.Assumptions.Add($"Топлопроводност на земята: {input.LambdaGround:F2} W/m·K");
                result.Assumptions.Add($"Кратност на въздухообмена: {input.AirChangeRate:F2} 1/h");
                result.Assumptions.Add($"Характеристичен размер B: {b:F3} m");
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Грешка при изчисление: {ex.Message}";
            }

            return result;
        }
    }
}
