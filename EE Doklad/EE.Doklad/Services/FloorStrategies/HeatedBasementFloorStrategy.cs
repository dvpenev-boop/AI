using System;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под над отопляем сутерен според ISO 13370
    /// Изчислява само стационарната част (U на под и стени към земя, Hg)
    /// </summary>
    public class HeatedBasementFloorStrategy : IFloorStrategy<FloorHeatedBasementInput>
    {
        private const double Rsi = 0.17; // Вътрешна повърхностна съпротивление (m²K/W)
        private const double Rse = 0.04; // Външна повърхностна съпротивление (m²K/W)

        public FloorCalculationResult Calculate(FloorHeatedBasementInput input)
        {
            var result = new FloorCalculationResult
            {
                FloorType = FloorType.HeatedBasement,
                Area = input.Area
            };

            try
            {
                // Валидация на входните данни
                if (input.Area <= 0 || input.Perimeter <= 0 || input.Depth <= 0)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Площ, периметър и дълбочина трябва да са > 0";
                    return result;
                }

                if (input.LambdaGround <= 0)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Топлопроводност на земята трябва да е > 0";
                    return result;
                }

                // Променливи
                double A = input.Area;
                double P = input.Perimeter;
                double z = input.Depth;
                double d_we = input.WallThicknessAtGrade;
                double lambda_g = input.LambdaGround;
                double psi_wf = input.PsiWallFloor;

                // 1. Характеристичен размер B
                double B = A / (0.5 * P);

                // 2. Термично съпротивление на подовата плоча R_f;b
                double R_f_b = input.FloorLayers.Sum(layer => layer.Thickness / layer.Lambda);

                // 3. Еквивалентна дебелина на пода d_f
                double d_f = d_we + lambda_g * (Rsi + R_f_b + Rse);

                // 4. U на подовата плоча към земя U_f;g;b
                double U_f_g_b;
                if (d_f + 0.5 * z < B)
                {
                    // Формула (45) от ISO 13370
                    double arg = (Math.PI * B) / (d_f + 0.5 * z) + 1;
                    U_f_g_b = (2 * lambda_g) / (Math.PI * (B + d_f + 0.5 * z)) * Math.Log(arg);
                }
                else
                {
                    // Формула (46) от ISO 13370
                    U_f_g_b = lambda_g / (0.457 * B + d_f + 0.5 * z);
                }

                // 5. Термично съпротивление на стените R_w;b
                double R_w_b = input.WallLayers.Sum(layer => layer.Thickness / layer.Lambda);

                // 6. Еквивалентна дебелина на стените d_w;b
                double d_w_b = lambda_g * (Rsi + R_w_b + Rse);

                // 7. U на сутеренните стени към земя U_w;g;b
                // Формула (47) от ISO 13370
                double d_for_formula = d_w_b < d_f ? d_w_b : d_f;
                double factor = 1.0 + (0.5 * d_for_formula) / (d_for_formula + z);
                double U_w_g_b = (2 * lambda_g) / (Math.PI * z) * factor * Math.Log(z / d_w_b + 1);

                // 8. Стационарен коефициент на топлопренос към земя H_g
                // Формула от спецификацията
                double H_g = A * U_f_g_b + z * P * U_w_g_b + P * psi_wf;

                // 9. Площ на стените към земя
                double A_walls = z * P;

                // Съхраняване на резултатите
                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uf;g;b",
                    Value = U_f_g_b,
                    U = U_f_g_b,
                    Area = A
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Uw;g;b",
                    Value = U_w_g_b,
                    U = U_w_g_b,
                    Area = A_walls
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Hg",
                    Value = H_g
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "B",
                    Value = B
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "df",
                    Value = d_f
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "dwb",
                    Value = d_w_b
                });

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Awalls",
                    Value = A_walls
                });

                // Assumptions за диагностика
                result.Assumptions.Add($"B (характеристичен размер) = {B:F3} m");
                result.Assumptions.Add($"R_f;b (под) = {R_f_b:F3} m²K/W");
                result.Assumptions.Add($"d_f (екв. дебелина под) = {d_f:F3} m");
                result.Assumptions.Add($"R_w;b (стена) = {R_w_b:F3} m²K/W");
                result.Assumptions.Add($"d_w;b (екв. дебелина стена) = {d_w_b:F3} m");
                result.Assumptions.Add($"U_f;g;b (под към земя) = {U_f_g_b:F3} W/m²K");
                result.Assumptions.Add($"U_w;g;b (стена към земя) = {U_w_g_b:F3} W/m²K");
                result.Assumptions.Add($"H_g (стационарен коеф.) = {H_g:F3} W/K");
                result.Assumptions.Add($"Площ стени към земя = {A_walls:F2} m²");

                // За таблицата използваме U на пода като основен резултат
                result.U = U_f_g_b;
                result.IsValid = true;
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
