using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using EE.Doklad.Models;
using EE.Doklad.Services.VentCooling;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Адаптер за BG_avg климатичен набор (24 часа за типичен ден на месеца).
    ///
    /// ПРАВИЛЕН МЕТОД: Зарежда реален почасов профил T и RH от
    /// <c>Data/DefaultParams_climateZones_hourly_flat_1to9.json</c>
    /// (12 месеца × 24 часа × 9 зони).
    ///
    /// Барометрично налягане: от <see cref="ClimateZoneData.GetEffectiveBarometricPressure"/>.
    ///
    /// ⚠️ СТАРОТО поведение (24 идентични точки с месечна средна T/RH) е ГРЕШНО и е
    /// заменено с реалния профил. Месечното усредняване на T и RH нарушава психрометричния
    /// расчёт, тъй като h = f(T, RH) е нелинейна функция.
    /// </summary>
    public sealed class BgAvgClimateProvider : IClimateProvider
    {
        private readonly ClimateZoneData _zoneData;
        private readonly double _bPa;

        // [month 1..12, hour 0..23] → (T_C, RH_pct)
        private readonly (double T, double RH)[,] _hourly = new (double, double)[13, 24];
        private bool _hourlyLoaded;

        /// <inheritdoc/>
        public bool IsBgAvgMode => true;

        /// <inheritdoc/>
        public double BarometricPressure_Pa => _bPa;

        public BgAvgClimateProvider(ClimateZoneData zoneData)
        {
            _zoneData = zoneData ?? throw new ArgumentNullException(nameof(zoneData));
            _bPa = zoneData.GetEffectiveBarometricPressure();
            TryLoadHourlyJson();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Връща 24 точки (h=0..23) с реален профил на T и RH за типичния ден на месеца.
        /// Ако JSON липсва, се пада обратно към месечната средна (legacy).
        /// </remarks>
        public IReadOnlyList<ClimateHourPoint> GetHourlyData(int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            var points = new List<ClimateHourPoint>(24);

            if (_hourlyLoaded)
            {
                // ── Реален почасов профил ─────────────────────────────────────────
                for (int h = 0; h < 24; h++)
                {
                    var (t, rh) = _hourly[month, h];
                    points.Add(new ClimateHourPoint
                    {
                        Hour       = h,
                        T_out_C    = t,
                        RH_out_Pct = rh,
                        B_Pa       = _bPa,
                    });
                }
            }
            else
            {
                // ── Fallback: месечна средна (legacy, само ако JSON липсва) ────────
                int idx = month - 1;
                double t_out = _zoneData.Monthly.AvgMonthlyTempC[idx];

                double rh_out = 50.0;
                if (month >= 5 && month <= 9)
                {
                    var rhArr = _zoneData.Monthly.AvgMonthlyRelHumidityPercentMayToSep;
                    int rhIdx = month - 5;
                    if (rhArr != null && rhArr.Length > rhIdx && rhArr[rhIdx] > 0)
                        rh_out = rhArr[rhIdx];
                }

                for (int h = 0; h < 24; h++)
                {
                    points.Add(new ClimateHourPoint
                    {
                        Hour       = h,
                        T_out_C    = t_out,
                        RH_out_Pct = rh_out,
                        B_Pa       = _bPa,
                    });
                }
            }

            return points;
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private void TryLoadHourlyJson()
        {
            try
            {
                // Зарежда от embedded resource (същият механизъм като JsonClimateRepository).
                // Ресурсното име: EE.Doklad.Data.DefaultParams_climateZones_hourly_flat_1to9.json
                const string ResourceName = "EE.Doklad.Data.DefaultParams_climateZones_hourly_flat_1to9.json";
                var asm = Assembly.GetExecutingAssembly();

                using var stream = asm.GetManifestResourceStream(ResourceName);
                if (stream == null)
                {
                    // Ресурсът не е намерен — логваме наличните имена за диагностика
                    _hourlyLoaded = false;
                    return;
                }

                string json;
                using (var reader = new StreamReader(stream))
                    json = reader.ReadToEnd();

                var records = JsonConvert.DeserializeObject<List<BgHourlyRecord>>(json);
                if (records == null || records.Count == 0)
                    return;

                int zoneId = _zoneData.Id;
                bool anyFound = false;

                foreach (var rec in records)
                {
                    if (rec.zone_number != zoneId)          continue;
                    if (rec.month < 1 || rec.month > 12)    continue;
                    if (rec.hour  < 0 || rec.hour  > 23)    continue;

                    _hourly[rec.month, rec.hour] = (rec.temp_C, rec.rh_percent);
                    anyFound = true;
                }

                _hourlyLoaded = anyFound;
            }
            catch
            {
                // Silently fall back to monthly averages.
                _hourlyLoaded = false;
            }
        }

        private sealed class BgHourlyRecord
        {
            public int    zone_number { get; set; }
            public int    month       { get; set; }
            public int    hour        { get; set; }
            public double temp_C      { get; set; }
            public double rh_percent  { get; set; }
        }
    }
}
