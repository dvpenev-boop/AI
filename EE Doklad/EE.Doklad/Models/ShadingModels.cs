using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Тип на обект за засенчване
    /// </summary>
    public enum ShadingType
    {
        Overhang,      // Навес (хоризонтален елемент над прозореца)
        LeftFin,       // Ляво странично ребро
        RightFin,      // Дясно странично ребро
        Obstacle,      // Препятствие (сграда, дърво и др.)
        Setback        // Отстъп/комбинация
    }

    /// <summary>
    /// Обект за засенчване (навес, ребро, препятствие)
    /// </summary>
    public partial class ShadingObject : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ObservableProperty]
        private ShadingType type = ShadingType.Overhang;

        /// <summary>
        /// Дълбочина D (m) - за навеси и ребра
        /// </summary>
        [ObservableProperty]
        private double depth;

        /// <summary>
        /// Разстояние L (m):
        /// - За навеси: вертикално разстояние от горния ръб на прозореца до долния ръб на навеса
        /// - За ребра: хоризонтално разстояние от страничния ръб на прозореца до ребрата
        /// </summary>
        [ObservableProperty]
        private double distance;

        [ObservableProperty]
        private string name = string.Empty;

        /// <summary>
        /// Допълнителни параметри за препятствия (за бъдещо разширение)
        /// </summary>
        public Dictionary<string, double>? AdditionalParams { get; set; }
    }

    /// <summary>
    /// Конфигурация на засенчването за прозорец
    /// </summary>
    public partial class ShadingConfig : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Списък с обекти за засенчване
        /// </summary>
        public ObservableCollection<ShadingObject> Shadings { get; } = new ObservableCollection<ShadingObject>();

        /// <summary>
        /// Месечни коефициенти на намаление на прякото засенчване F_sh,dir[m] (Jan..Dec)
        /// </summary>
        [ObservableProperty]
        private double[] fshDirMonthly = new double[12] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        /// <summary>
        /// Режим на редакция (Simple/Custom)
        /// </summary>
        [ObservableProperty]
        private ShadingEditMode editMode = ShadingEditMode.Simple;

        /// <summary>
        /// Географска ширина (за изчисления)
        /// </summary>
        [ObservableProperty]
        private double latitude = 42.7;

        /// <summary>
        /// Северно полукълбо
        /// </summary>
        [ObservableProperty]
        private bool northHemisphere = true;
    }

    /// <summary>
    /// Режим на редакция на засенчването
    /// </summary>
    public enum ShadingEditMode
    {
        Simple,  // Прост режим (checkbox за overhang, left fin, right fin)
        Custom   // Custom режим (DataGrid със списък от обекти)
    }

    /// <summary>
    /// Резултат от изчисление на засенчване за един месец
    /// </summary>
    public class MonthlyShadingResult
    {
        public int Month { get; set; }              // 1..12
        public string MonthName { get; set; } = string.Empty;
        public double HOverhang { get; set; }       // h_ovh[m] - височина засенчване от навеси
        public double WFinLeft { get; set; }        // w_finl[m] - ширина засенчване от ляво ребро
        public double WFinRight { get; set; }       // w_finr[m] - ширина засенчване от дясно ребро
        public double HObstacle { get; set; }       // h_obst[m] - височина засенчване от препятствия
        public double HSun { get; set; }            // h_sun[m] - осветена височина
        public double WSun { get; set; }            // w_sun[m] - осветена ширина
        public double FshDir { get; set; }          // F_sh,dir[m] - коефициент (0..1)
    }
}
