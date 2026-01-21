using EE.Doklad.Models;
using System;
using System.Linq;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под към земя по ISO 13370
    /// </summary>
    public class GroundFloorStrategy : IFloorStrategy<FloorGroundInput>
    {
        public FloorCalculationResult Calculate(FloorGroundInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new FloorCalculationResult
            {
                FloorType = FloorType.Ground,
                Area = input.Area,
                Components = new System.Collections.Generic.List<FloorCalcComponent>()
            };

            // Валидации
            string error = string.Empty;
            if (input.Area <= 0) error += "A <= 0. ";
            if (input.Perimeter <= 0) error += "P <= 0. ";
            if (input.LambdaGround <= 0) error += "λg <= 0. ";
            if (input.Layers.Any(l => l.Thickness < 0)) error += "Има слой с δ < 0. ";
            if (input.Layers.Any(l => l.Lambda <= 0)) error += "Има слой с λ <= 0. ";
            if (input.WallThickness < 0) error += "dw,e < 0. ";
            if (input.Df <= 0 && !input.IsDfAuto) error += "df <= 0. ";
            if ((input.InsulationType == GroundInsulationType.Edge || input.InsulationType == GroundInsulationType.UnderSlab))
            {
                if (input.InsulationWidth <= 0) error += "D <= 0. ";
                if (input.InsulationThickness <= 0) error += "dn <= 0. ";
                if (input.InsulationResistance <= 0) error += "Rn <= 0. ";
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                result.IsValid = false;
                result.ErrorMessage = error.Trim();
                return result;
            }

            // (А) Характеристичен размер
            double B = input.Area / (0.5 * input.Perimeter);

            // (Б) Rf;sog (R на пода по ISO6946)
            double Rf = input.Layers.Sum(l => l.Thickness / l.Lambda);
            // Rsi се подава отделно

            // (В) df
            double df = input.Df;
            double dw_e = input.WallThickness;
            if (input.IsDfAuto)
            {
                df = dw_e + input.LambdaGround * (Rf + input.Rsi + 0.04); // 0.04 = Rse
            }

            // (добавка по методика)
            double z = input.BasementDepth; // използвай публичното property, генерирано от ObservableProperty
            double df_eff = df + 0.5 * z;

            // (Г) U0 = Ufgb (без периферна изолация) по методиката
            double U0 = 0;
            string branch = "";
            if (df_eff < B)
            {
                // формула (13) от методиката
                U0 = 2 * input.LambdaGround / (Math.PI * B + df_eff) * Math.Log((Math.PI * B) / df_eff + 1);
                branch = "df+0.5z < B (методика, формула 13)";
            }
            else
            {
                // формула (14) от методиката
                U0 = 1 / (0.457 * B / input.LambdaGround + df_eff);
                branch = "df+0.5z >= B (методика, формула 14)";
            }

            double U = U0;
            double Psi_g_ed = 0;
            string insulationBranch = "";

            if (input.InsulationType == GroundInsulationType.None)
            {
                // U = U0
                insulationBranch = "Без периферна изолация";
            }
            else if (input.InsulationType == GroundInsulationType.Edge || input.InsulationType == GroundInsulationType.UnderSlab)
            {
                // (Е) Хоризонтална/вертикална изолация
                double Rn = input.InsulationResistance;
                double dn = input.InsulationThickness;
                double D = input.InsulationWidth;
                double lambda_g = input.LambdaGround;
                double R_ = Rn - dn / lambda_g;
                double d_ = R_ * lambda_g;
                if (R_ < 0)
                {
                    insulationBranch = "R' < 0 (възможен случай при фундамент с ниска λ)";
                }
                // ln аргументи
                double arg1, arg2;
                if (input.InsulationType == GroundInsulationType.Edge)
                {
                    arg1 = D / df + 1;
                    arg2 = D / (df + d_) + 1;
                    if (arg1 > 0 && arg2 > 0)
                    {
                        Psi_g_ed = -(lambda_g / Math.PI) * (Math.Log(arg1) - Math.Log(arg2));
                        U = U0 + (2 * Psi_g_ed) / B;
                        insulationBranch += " (Edge/Horizontal)";
                    }
                    else
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "ln аргумент <= 0 (Edge)";
                        return result;
                    }
                }
                else // UnderSlab = Vertical
                {
                    arg1 = 2 * D / df + 1;
                    arg2 = 2 * D / (df + d_) + 1;
                    if (arg1 > 0 && arg2 > 0)
                    {
                        Psi_g_ed = -(lambda_g / Math.PI) * (Math.Log(arg1) - Math.Log(arg2));
                        U = U0 + (2 * Psi_g_ed) / B;
                        insulationBranch += " (Vertical)";
                    }
                    else
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "ln аргумент <= 0 (Vertical)";
                        return result;
                    }
                }
            }

            result.U = U;
            result.IsValid = true;
            result.Components.Add(new FloorCalcComponent
            {
                Name = "Под към земя",
                U = U,
                Area = input.Area,
                Description = $"B = {B:F2} m, Rf = {Rf:F3}, dw,e = {dw_e:F3}, df = {df:F3}, U0 = {U0:F3}, Psi_g_ed = {Psi_g_ed:F3}, U = {U:F3}, {branch} {insulationBranch}"
            });
            result.Assumptions.Add($"B = {B:F2} m");
            result.Assumptions.Add($"Rf = {Rf:F3}");
            result.Assumptions.Add($"dw,e = {dw_e:F3}");
            result.Assumptions.Add($"df = {df:F3}");
            result.Assumptions.Add($"U0 = {U0:F3}");
            result.Assumptions.Add($"Psi_g_ed = {Psi_g_ed:F3}");
            result.Assumptions.Add($"U = {U:F3}");
            result.Assumptions.Add($"branch: {branch} {insulationBranch}");

            return result;
        }
    }
}
