using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Enum за степените на активност
    /// </summary>
    public enum ActivityLevel
    {
        Cinema,              // кино, театър, училище
        Office,              // работа на компютър, хотел, рецепция, магазин
        HotelReceptionKasier, // офисна работа, столови, магазини, хотел, рецепция, каса
        StandingLightWork,    // стоящ, правомагазин, ходещ, баня, лаборатория
        WalkingSeated,        // ходещ, седнал
        ModerateWork,         // средна работа, слуга, фризьор
        LightWorkSeated,      // лека работа седнал, механична продукция
        Dancing,              // танцуване, лека партийна работа
        FastWalking,          // бързо ходене, планинско ходене
        HeavyWork             // тежка работа, атлети, спортуване
    }

    /// <summary>
    /// Данни за една температурна колона в таблицата
    /// </summary>
    public class TemperatureColumn
    {
        /// <summary>
        /// Температура в °C
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Явна топлина (W)
        /// </summary>
        public double SensibleHeat { get; set; }

        /// <summary>
        /// Скрита топлина (W)
        /// </summary>
        public double LatentHeat { get; set; }
    }

    /// <summary>
    /// Ред от таблицата за активности
    /// </summary>
    public class ActivityRow
    {
        /// <summary>
        /// Ключ на активността
        /// </summary>
        public ActivityLevel ActivityLevel { get; set; }

        /// <summary>
        /// Показвано име на български
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Описание на местоположението
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Средна стойност на метаболитна активност (W/m²)
        /// </summary>
        public int MetabolicRate { get; set; }

        /// <summary>
        /// Данни по температурни колони
        /// </summary>
        public List<TemperatureColumn> TemperatureColumns { get; set; } = new();
    }

    /// <summary>
    /// Служба за достъп до данните от таблицата с активности
    /// </summary>
    public static class ActivityDataService
    {
        private static readonly List<ActivityRow> _activityData = new()
        {
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.Cinema,
                DisplayName = "Кино, театър, училище",
                Location = "Кино, театър, училище",
                MetabolicRate = 100,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 50, LatentHeat = 50 },
                    new() { Temperature = 27, SensibleHeat = 55, LatentHeat = 45 },
                    new() { Temperature = 26, SensibleHeat = 60, LatentHeat = 40 },
                    new() { Temperature = 24, SensibleHeat = 67, LatentHeat = 33 },
                    new() { Temperature = 22, SensibleHeat = 72, LatentHeat = 28 },
                    new() { Temperature = 20, SensibleHeat = 79, LatentHeat = 21 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.Office,
                DisplayName = "Работа на компютър",
                Location = "Работа на компютър",
                MetabolicRate = 120,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 50, LatentHeat = 70 },
                    new() { Temperature = 27, SensibleHeat = 55, LatentHeat = 65 },
                    new() { Temperature = 26, SensibleHeat = 60, LatentHeat = 60 },
                    new() { Temperature = 24, SensibleHeat = 70, LatentHeat = 50 },
                    new() { Temperature = 22, SensibleHeat = 78, LatentHeat = 42 },
                    new() { Temperature = 20, SensibleHeat = 84, LatentHeat = 36 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.HotelReceptionKasier,
                DisplayName = "Офисна работа, столова, магазини",
                Location = "Хотел, рецепция, касиер",
                MetabolicRate = 130,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 50, LatentHeat = 80 },
                    new() { Temperature = 27, SensibleHeat = 56, LatentHeat = 74 },
                    new() { Temperature = 26, SensibleHeat = 60, LatentHeat = 70 },
                    new() { Temperature = 24, SensibleHeat = 70, LatentHeat = 60 },
                    new() { Temperature = 22, SensibleHeat = 78, LatentHeat = 52 },
                    new() { Temperature = 20, SensibleHeat = 86, LatentHeat = 44 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.StandingLightWork,
                DisplayName = "Стоящ, правомагазин, ходещ, баня",
                Location = "Лабораторна работа",
                MetabolicRate = 130,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 50, LatentHeat = 80 },
                    new() { Temperature = 27, SensibleHeat = 56, LatentHeat = 74 },
                    new() { Temperature = 26, SensibleHeat = 60, LatentHeat = 70 },
                    new() { Temperature = 24, SensibleHeat = 70, LatentHeat = 60 },
                    new() { Temperature = 22, SensibleHeat = 78, LatentHeat = 52 },
                    new() { Temperature = 20, SensibleHeat = 86, LatentHeat = 44 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.WalkingSeated,
                DisplayName = "Ходещ, седнал",
                Location = "Ходещ, седнал",
                MetabolicRate = 150,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 53, LatentHeat = 97 },
                    new() { Temperature = 27, SensibleHeat = 58, LatentHeat = 92 },
                    new() { Temperature = 26, SensibleHeat = 64, LatentHeat = 86 },
                    new() { Temperature = 24, SensibleHeat = 76, LatentHeat = 74 },
                    new() { Temperature = 22, SensibleHeat = 84, LatentHeat = 66 },
                    new() { Temperature = 20, SensibleHeat = 90, LatentHeat = 60 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.ModerateWork,
                DisplayName = "Средна работа",
                Location = "Слуга, фризьор",
                MetabolicRate = 160,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 55, LatentHeat = 105 },
                    new() { Temperature = 27, SensibleHeat = 60, LatentHeat = 100 },
                    new() { Temperature = 26, SensibleHeat = 68, LatentHeat = 92 },
                    new() { Temperature = 24, SensibleHeat = 80, LatentHeat = 80 },
                    new() { Temperature = 22, SensibleHeat = 90, LatentHeat = 70 },
                    new() { Temperature = 20, SensibleHeat = 98, LatentHeat = 62 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.LightWorkSeated,
                DisplayName = "Лека работа седнал",
                Location = "Механична продукция",
                MetabolicRate = 220,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 55, LatentHeat = 165 },
                    new() { Temperature = 27, SensibleHeat = 52, LatentHeat = 168 },
                    new() { Temperature = 26, SensibleHeat = 70, LatentHeat = 150 },
                    new() { Temperature = 24, SensibleHeat = 85, LatentHeat = 135 },
                    new() { Temperature = 22, SensibleHeat = 100, LatentHeat = 120 },
                    new() { Temperature = 20, SensibleHeat = 115, LatentHeat = 105 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.Dancing,
                DisplayName = "Танцуване, лека партийна работа",
                Location = "Парти",
                MetabolicRate = 250,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 62, LatentHeat = 188 },
                    new() { Temperature = 27, SensibleHeat = 70, LatentHeat = 180 },
                    new() { Temperature = 26, SensibleHeat = 78, LatentHeat = 172 },
                    new() { Temperature = 24, SensibleHeat = 94, LatentHeat = 156 },
                    new() { Temperature = 22, SensibleHeat = 110, LatentHeat = 140 },
                    new() { Temperature = 20, SensibleHeat = 125, LatentHeat = 125 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.FastWalking,
                DisplayName = "Бързо ходене",
                Location = "Планинско ходене",
                MetabolicRate = 300,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 80, LatentHeat = 220 },
                    new() { Temperature = 27, SensibleHeat = 88, LatentHeat = 212 },
                    new() { Temperature = 26, SensibleHeat = 96, LatentHeat = 204 },
                    new() { Temperature = 24, SensibleHeat = 110, LatentHeat = 190 },
                    new() { Temperature = 22, SensibleHeat = 130, LatentHeat = 170 },
                    new() { Temperature = 20, SensibleHeat = 145, LatentHeat = 155 }
                }
            },
            new ActivityRow
            {
                ActivityLevel = ActivityLevel.HeavyWork,
                DisplayName = "Тежка работа",
                Location = "Атлети, спортуване",
                MetabolicRate = 430,
                TemperatureColumns = new List<TemperatureColumn>
                {
                    new() { Temperature = 28, SensibleHeat = 132, LatentHeat = 298 },
                    new() { Temperature = 27, SensibleHeat = 138, LatentHeat = 292 },
                    new() { Temperature = 26, SensibleHeat = 144, LatentHeat = 286 },
                    new() { Temperature = 24, SensibleHeat = 154, LatentHeat = 276 },
                    new() { Temperature = 22, SensibleHeat = 170, LatentHeat = 260 },
                    new() { Temperature = 20, SensibleHeat = 188, LatentHeat = 242 }
                }
            }
        };

        /// <summary>
        /// Връща всички налични активности
        /// </summary>
        public static IReadOnlyList<ActivityRow> GetAllActivities() => _activityData.AsReadOnly();

        /// <summary>
        /// Връща активност по ключ
        /// </summary>
        public static ActivityRow? GetActivity(ActivityLevel level)
        {
            return _activityData.FirstOrDefault(a => a.ActivityLevel == level);
        }

        /// <summary>
        /// Изчислява sensible и latent heat за дадена температура чрез линейна интерполация
        /// </summary>
        /// <param name="level">Степен на активност</param>
        /// <param name="temperature">Температура в °C</param>
        /// <returns>Tuple (SensibleHeat, LatentHeat) в W/човек</returns>
        public static (double SensibleHeat, double LatentHeat) CalculateHeatForTemperature(
            ActivityLevel level,
            double temperature)
        {
            var activity = GetActivity(level);
            if (activity == null)
                return (0, 0);

            var columns = activity.TemperatureColumns.OrderBy(c => c.Temperature).ToList();
            
            if (columns.Count == 0)
                return (0, 0);

            // Ако температурата е под минималната - clamp към минималната
            if (temperature <= columns.First().Temperature)
            {
                var first = columns.First();
                return (first.SensibleHeat, first.LatentHeat);
            }

            // Ако температурата е над максималната - clamp към максималната
            if (temperature >= columns.Last().Temperature)
            {
                var last = columns.Last();
                return (last.SensibleHeat, last.LatentHeat);
            }

            // Намираме двете съседни колони за интерполация
            TemperatureColumn? lowerColumn = null;
            TemperatureColumn? upperColumn = null;

            for (int i = 0; i < columns.Count - 1; i++)
            {
                if (columns[i].Temperature <= temperature && temperature <= columns[i + 1].Temperature)
                {
                    lowerColumn = columns[i];
                    upperColumn = columns[i + 1];
                    break;
                }
            }

            // Ако намерим точно съвпадение
            var exactMatch = columns.FirstOrDefault(c => Math.Abs(c.Temperature - temperature) < 0.001);
            if (exactMatch != null)
            {
                return (exactMatch.SensibleHeat, exactMatch.LatentHeat);
            }

            // Линейна интерполация
            if (lowerColumn != null && upperColumn != null)
            {
                double alpha = (temperature - lowerColumn.Temperature) / 
                              (upperColumn.Temperature - lowerColumn.Temperature);

                double sensible = lowerColumn.SensibleHeat + 
                                 alpha * (upperColumn.SensibleHeat - lowerColumn.SensibleHeat);
                
                double latent = lowerColumn.LatentHeat + 
                               alpha * (upperColumn.LatentHeat - lowerColumn.LatentHeat);

                return (sensible, latent);
            }

            // Fallback (не трябва да се случва)
            return (0, 0);
        }
    }
}
