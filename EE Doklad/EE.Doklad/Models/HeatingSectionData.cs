using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    public enum HeatingCalculationMethod
    {
        AuerSoftware = 1,
        Rd0220_3 = 2,
        Ashrae8760 = 3
    }

    /// <summary>
    /// ����� �� ������ �10 - "���������"
    /// </summary>
    public partial class HeatingSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "\u041E\u0442\u043E\u043F\u043B\u0435\u043D\u0438\u0435";

        [ObservableProperty]
        private string? _description;

        public HeatingCalculationMethod CalculationMethod { get; set; }
            = HeatingCalculationMethod.AuerSoftware;

        // ========== ����� ������� ==========

        /// <summary>
        /// ����������� [1/�] - ��������� ��������, >= 0
        /// </summary>
        [ObservableProperty]
        private double _infiltration = 0.5;

        /// <summary>
        /// �������� ����������� [�C] - ���� �� ��� ������ �������� (����. 21.00)
        /// </summary>
        [ObservableProperty]
        private double _designTemperature = 20.0;

        /// <summary>
        /// Temperatura �� ��������� [�C]
        /// </summary>
        [ObservableProperty]
        private double _reductionTemperature = 16.0;

        /// <summary>
        /// ����������� �� �������� [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _emissionEfficiency = 100.0;

        /// <summary>
        /// ����������� �� ��������������� ����� [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _distributionEfficiency = 100.0;

        /// <summary>
        /// ����������� ���������� [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _automaticControl = 96.0;

    /// <summary>
    /// �������� ���������� [%] (0-100)
    /// </summary>
    [ObservableProperty]
    private double _energyManagement = 96.0;

    /// <summary>
    /// ��� �� ��������������� [%] (>=0, ���� �� ���� ��� 100)
    /// </summary>
    [ObservableProperty]
    private double _heatingEfficiency = 100.0;

    // ========== �������� �������� �� ��������� (�� ������ � ����������) ==========

    /// <summary>
    /// �������� �������� 1 (��1)
    /// </summary>
    [ObservableProperty]
    private VentilationEnergySource _energySource1 = new();

    /// <summary>
    /// �������� �������� 2 (��2) - ����������
    /// </summary>
    [ObservableProperty]
    private VentilationEnergySource? _energySource2 = null;

    /// <summary>
    /// �������� �� �� ����� �������� ��������
    /// </summary>
    [ObservableProperty]
    private bool _useSecondEnergySource = false;

        // ========== ��������� ==========

        /// <summary>
        /// ������� ������ �� ���������
        /// </summary>
        [ObservableProperty]
        private ActivityLevel _selectedActivityLevel = ActivityLevel.Cinema;
    }
}
