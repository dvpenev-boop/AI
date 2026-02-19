using System;

namespace EE.Doklad.Services.Psychrometrics
{
    /// <summary>
    /// Набор от психрометрични стойности за едно въздушно състояние.
    /// Формули по Наредба 7257_1, §3.14 (3.98 – 3.106).
    /// </summary>
    public readonly struct AirState
    {
        /// <summary>Температура [°C]</summary>
        public double T_C { get; init; }

        /// <summary>Относителна влажност [%] (0-100)</summary>
        public double RH_Pct { get; init; }

        /// <summary>Барометрично налягане [Pa]</summary>
        public double B_Pa { get; init; }

        /// <summary>Налягане на насищане [Pa] – (3.98)</summary>
        public double p_ws_Pa { get; init; }

        /// <summary>Парциално налягане на водните пари [Pa] – (3.99)</summary>
        public double p_w_Pa { get; init; }

        /// <summary>Влагосъдържание [kg_w / kg_da] – (3.100)</summary>
        public double x_kgkg { get; init; }

        /// <summary>Специфична енталпия на влажния въздух [kJ/kg_da] – (3.104)</summary>
        public double h_kJkg { get; init; }

        /// <summary>Плътност на сухия въздух [kg_da/m³] – (3.105)</summary>
        public double rho_da_kgm3 { get; init; }

        /// <summary>Плътност на влажния въздух [kg/m³] – (3.106)</summary>
        public double rho_kgm3 { get; init; }

        /// <summary>Енталпия на водните пари [kJ/kg_w] – (3.103)</summary>
        public double h_w_kJkg { get; init; }
    }

    /// <summary>
    /// Интерфейс за психрометрични изчисления.
    /// </summary>
    public interface IPsychrometrics
    {
        /// <summary>
        /// Изчислява пълното въздушно състояние от температура, RH и барометрично налягане.
        /// </summary>
        AirState Compute(double tempC, double rhPercent, double bPa);

        /// <summary>
        /// Налягане на насищане по формула (3.98):
        /// p_ws = exp(77.3450 + 0.0057·T – 7235/T) / T^8.2   [T = t + 273.15 K]
        /// </summary>
        double SaturationPressure_Pa(double tempC);
    }
}
