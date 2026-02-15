using System;

namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// DTO за сериализация на ClimatePoint (struct не се сериализира оптимално).
    /// </summary>
    public class ClimatePointDto
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public double DryBulbC { get; set; }
        public double RH { get; set; }

        public ClimatePointDto() { }

        public ClimatePointDto(ClimatePoint point)
        {
            Month = point.Month;
            Day = point.LocalTime.Day;
            Hour = point.Hour;
            DryBulbC = point.DryBulbC;
            RH = point.RH;
        }

        public ClimatePoint ToClimatePoint(int fixedYear)
        {
            var dateTime = new DateTime(fixedYear, Month, Day, Hour, 0, 0, DateTimeKind.Unspecified);
            return new ClimatePoint(dateTime, DryBulbC, RH);
        }
    }
}
