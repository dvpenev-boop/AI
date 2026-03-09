using System;
using EE.Doklad.Models;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Изчислява периодични коефициенти на топлопренасяне за подови конструкции
    /// в контакт със земята по ISO 13370, използвайки вече изчислени стационарни резултати.
    /// </summary>
    public static class GroundFloorPeriodicCalculator
    {
        /// <summary>
        /// Изчислява периодични коефициенти (Hpi, Hpe) и месечни еквивалентни HTC.
        /// </summary>
        /// <param name="input">Входни данни от съществуващите стратегии и климата.</param>
        /// <returns>Резултат с Hg, Hel, Hpi, Hpe, Delta, Beta и Hmonthly[12].</returns>
        /// <exception cref="ArgumentNullException">При null вход.</exception>
        /// <exception cref="ArgumentException">При невалидни входни данни.</exception>
        public static GroundFloorPeriodicResult Calculate(GroundFloorPeriodicInput input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.MonthlyExteriorTemperature is null || input.MonthlyExteriorTemperature.Length != 12)
            {
                throw new ArgumentException("MonthlyExteriorTemperature must contain exactly 12 values.", nameof(input));
            }

            if (input.LambdaGround <= 0.0)
            {
                throw new ArgumentException("LambdaGround must be greater than 0.", nameof(input));
            }

            if (input.RhoC <= 0.0)
            {
                throw new ArgumentException("RhoC must be greater than 0.", nameof(input));
            }

            // ISO 13370 Eq.(43): delta = sqrt(3.15e7 * lambda_g / (pi * rho_c))
            double delta = Math.Sqrt(3.15e7 * input.LambdaGround / (Math.PI * input.RhoC));

            double hg;
            double hel;
            double hpi;
            double hpe;
            int beta;

            switch (input.FloorType)
            {
                case FloorType.Ground:
                {
                    EnsurePositive(input.df, nameof(input.df));

                    // ISO 13370 Eq.(44): internal periodic HTC for slab-on-ground.
                    double ratio = delta / input.df;
                    double denominatorHpi = Math.Sqrt(Math.Pow(1.0 + ratio, 2.0) + 1.0) + 1.0;
                    hpi = input.Area * (input.LambdaGround / input.df) * Math.Sqrt(2.0 / denominatorHpi);

                    // ISO 13370 Eq.(45): external periodic HTC for slab-on-ground.
                    hpe = 0.37 * input.ExposedPerimeter * input.LambdaGround * Math.Log(delta / input.df + 1.0);

                    // ISO 13370 Table 4: time lag Beta.
                    beta = input.InsulationType == GroundInsulationType.UnderSlab ? 2 : 1;

                    hg = input.Hg_steady;
                    hel = input.Area * input.Ufg;
                    break;
                }

                case FloorType.HeatedBasement:
                {
                    EnsurePositive(input.df, nameof(input.df));
                    EnsurePositive(input.dw_b, nameof(input.dw_b));

                    // ISO 13370 Eq.(51): internal periodic HTC (floor + wall terms).
                    double ratioF = delta / input.df;
                    double denomF = Math.Sqrt(Math.Pow(1.0 + ratioF, 2.0) + 1.0) + 1.0;
                    double termFloor = input.Area * (input.LambdaGround / input.df) * Math.Sqrt(2.0 / denomF);

                    double ratioW = delta / input.dw_b;
                    double denomW = Math.Sqrt(Math.Pow(1.0 + ratioW, 2.0) + 1.0) + 1.0;
                    double termWall = input.BasementDepth * input.ExposedPerimeter * (input.LambdaGround / input.dw_b) * Math.Sqrt(2.0 / denomW);
                    hpi = termFloor + termWall;

                    // ISO 13370 Eq.(52): external periodic HTC for heated basement.
                    double expZ = Math.Exp(-input.BasementDepth / delta);
                    hpe = 0.37 * input.ExposedPerimeter * input.LambdaGround * (
                        expZ * Math.Log(delta / input.df + 1.0) +
                        2.0 * (1.0 - expZ) * Math.Log(delta / input.dw_b + 1.0));

                    beta = 1; // ISO 13370 Table 4
                    hg = input.Hg_steady;
                    hel = input.Area * input.Ufg;
                    break;
                }

                case FloorType.UnheatedBasement:
                {
                    EnsurePositive(input.df, nameof(input.df));
                    EnsurePositive(input.Ufg, nameof(input.Ufg));

                    // ISO 13370 Eq.(53): simplified internal periodic HTC.
                    hpi = 1.0 / (1.0 / (input.Area * input.Ufg) + delta / (input.Area * input.LambdaGround));

                    // ISO 13370 Eq.(54): external periodic HTC.
                    double expZ2 = Math.Exp(-input.BasementDepth / delta);
                    hpe = input.Area * input.Ufg * (
                        0.37 * input.ExposedPerimeter * input.LambdaGround *
                        (2.0 - expZ2) * Math.Log(delta / input.df + 1.0))
                        / (input.Area * input.LambdaGround / delta + input.Area * input.Ufg);

                    beta = 1; // ISO 13370 Table 4
                    hg = input.Hg_steady;
                    hel = input.Area * input.Ufg;
                    break;
                }

                case FloorType.ExternalAir:
                {
                    // No ground contact: periodic ground coefficients are not applicable.
                    hpi = 0.0;
                    hpe = 0.0;
                    beta = 0;
                    hg = input.Area * input.Ufg;
                    hel = hg;
                    break;
                }

                default:
                {
                    // Non-ground-contact / unsupported periodic case.
                    hpi = 0.0;
                    hpe = 0.0;
                    beta = 0;
                    hg = input.Hg_steady > 0.0 ? input.Hg_steady : input.Area * input.Ufg;
                    hel = input.Area * input.Ufg;
                    break;
                }
            }

            GroundFloorPeriodicResult result = new GroundFloorPeriodicResult
            {
                Hg = hg,
                Hel = hel,
                Hpi = hpi,
                Hpe = hpe,
                Delta = delta,
                Beta = beta,
                B = input.ExposedPerimeter > 0.0 ? input.Area / (0.5 * input.ExposedPerimeter) : 0.0,
                df = input.df,
                Ufg = input.Ufg
            };

            // Monthly equivalent HTC derived from ISO 13370 Eq.(24) monthly heat flux.
            result.Hmonthly = CalculateMonthlyHTC(
                result,
                input.MonthlyExteriorTemperature,
                input.AnnualMeanExteriorTemperature,
                theta_int: 20.0);

            return result;
        }

        /// <summary>
        /// Изчислява месечни еквивалентни коефициенти на топлопренасяне Hmonthly[12].
        /// </summary>
        /// <param name="result">Периодичен резултат с Hg, Hpe и Beta.</param>
        /// <param name="monthlyExteriorTemp">12 месечни външни температури, индекс 0=януари.</param>
        /// <param name="annualMeanExteriorTemp">Годишна средна външна температура theta_e_bar (°C).</param>
        /// <param name="theta_int">Вътрешна температура theta_int (°C).</param>
        /// <returns>Масив от 12 стойности Hmonthly (W/K).</returns>
        /// <exception cref="ArgumentNullException">При null аргументи.</exception>
        /// <exception cref="ArgumentException">При невалидни размери на масив.</exception>
        public static double[] CalculateMonthlyHTC(
            GroundFloorPeriodicResult result,
            double[] monthlyExteriorTemp,
            double annualMeanExteriorTemp,
            double theta_int)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (monthlyExteriorTemp is null)
            {
                throw new ArgumentNullException(nameof(monthlyExteriorTemp));
            }

            if (monthlyExteriorTemp.Length != 12)
            {
                throw new ArgumentException("monthlyExteriorTemp must contain exactly 12 values.", nameof(monthlyExteriorTemp));
            }

            double[] hMonthly = new double[12];

            for (int m = 0; m < 12; m++)
            {
                // Lagged month index according to ISO 13370 Table 4 time lag Beta.
                int lagged = (m - result.Beta + 12) % 12;
                double thetaEM = monthlyExteriorTemp[m];
                double thetaELagged = monthlyExteriorTemp[lagged];

                // ISO 13370 Eq.(24): monthly heat flux (Hpi term omitted for constant internal temperature).
                double phiM = result.Hg * (theta_int - annualMeanExteriorTemp)
                    + result.Hpe * (annualMeanExteriorTemp - thetaELagged);

                // Monthly equivalent HTC: Hmonthly[m] = Phi_m / (theta_int - theta_e_m).
                double deltaT = theta_int - thetaEM;
                hMonthly[m] = Math.Abs(deltaT) > 0.01 ? phiM / deltaT : result.Hg;
            }

            return hMonthly;
        }

        private static void EnsurePositive(double value, string name)
        {
            if (value <= 0.0)
            {
                throw new ArgumentException($"{name} must be greater than 0.", name);
            }
        }
    }
}
