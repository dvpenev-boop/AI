using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models;

/// <summary>
/// Тип неклиматизирана зона
/// </summary>
public enum ZtuType
{
    /// <summary>
    /// Външна неклиматизирана зона (ztue)
    /// </summary>
    External,

    /// <summary>
    /// Вътрешна неклиматизирана зона (ztui)
    /// </summary>
    Internal
}

/// <summary>
/// Вид детайл на ограждащ елемент към ZTU (определя Rsi)
/// </summary>
public enum ElementKind
{
    /// <summary>
    /// Стена (вертикална) - Rsi = 0.13 m²K/W
    /// </summary>
    Wall,

    /// <summary>
    /// Покрив/Таван (топлина нагоре) - Rsi = 0.10 m²K/W
    /// </summary>
    Roof,

    /// <summary>
    /// Под (топлина надолу) - Rsi = 0.17 m²K/W
    /// </summary>
    Floor
}

/// <summary>
/// Слой в многослойна конструкция на ограждащ елемент към ZTU
/// </summary>
public partial class ZtuLayer : ObservableObject
{
    [ObservableProperty]
    private string _materialName = string.Empty;

    [ObservableProperty]
    private string? _selectedMaterialId;

    [ObservableProperty]
    private double _thickness = 0.0; // mm

    [ObservableProperty]
    private double _lambda = 0.0; // W/(m·K)

    /// <summary>
    /// Термично съпротивление на слоя R = d/λ (m²K/W)
    /// </summary>
    public double R => Lambda > 0 ? (Thickness / 1000.0) / Lambda : 0.0;

    partial void OnSelectedMaterialIdChanged(string? value)
    {
        if (string.IsNullOrEmpty(value) || MaterialOptions == null) 
            return;

        var selected = MaterialOptions.FirstOrDefault(m => m.Id == value);
        if (selected != null)
        {
            MaterialName = selected.Display;
            Lambda = selected.LambdaWmk;
        }
    }

    partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));
    partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));

    /// <summary>
    /// Material options for dropdown (injected by ViewModel)
    /// </summary>
    public System.Collections.Generic.IReadOnlyList<MaterialOption>? MaterialOptions { get; set; }
}

/// <summary>
/// Ограждащ елемент към или в неклиматизирана зона
/// </summary>
public partial class ZtuElement : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Вид детайл (стена/покрив/под) - определя Rsi
    /// </summary>
    [ObservableProperty]
    private ElementKind _kind = ElementKind.Wall;

    /// <summary>
    /// Площ на елемента (m²) - без разпределение по ориентация
    /// </summary>
    [ObservableProperty]
    private double _area = 0.0;

    /// <summary>
    /// Слоеве на конструкцията (отвътре навън)
    /// </summary>
    public ObservableCollection<ZtuLayer> Layers { get; } = new();

    /// <summary>
    /// Изчислено U (W/m²K) за елемента.
    /// За boundary към ZTU: Rsi от двете страни според Kind.
    /// За boundary към външен въздух: Rsi вътре + Rse вън.
    /// </summary>
    [ObservableProperty]
    private double _uValue = 0.0;

    /// <summary>
    /// Дали елементът е към външна среда (true) или разделящ между ztc и ztu (false)
    /// </summary>
    [ObservableProperty]
    private bool _isToExternalEnvironment = false;

    /// <summary>
    /// Схема/изображение на конструкцията
    /// </summary>
    [ObservableProperty]
    private AttachmentData? _schemeAttachment;
}

/// <summary>
/// Неклиматизирана зона (ztu)
/// </summary>
public partial class ZtuZone : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private ZtuType _type = ZtuType.External;

    [ObservableProperty]
    private string _notes = string.Empty;

    /// <summary>
    /// Елементи на ZTU към външна среда (участват в Hztu,e)
    /// </summary>
    public ObservableCollection<ZtuElement> ElementsToExternal { get; } = new();

    /// <summary>
    /// Разделящи елементи ZTC↔ZTU (участват в Hztc-ztu)
    /// </summary>
    public ObservableCollection<ZtuElement> ElementsToBoundary { get; } = new();

    [ObservableProperty]
    private double _manualUnconditionedTempWinterC = 10.0;

    [ObservableProperty]
    private double _manualUnconditionedTempSummerC = 25.0;
}

/// <summary>
/// Данни за секция "Неклиматизирани зони (ztu)"
/// </summary>
public partial class UnconditionedZoneSectionData : ObservableObject
{
    [ObservableProperty]
    private string _title = "Неклиматизирани зони (ztu)";

    [ObservableProperty]
    private string _description = "Попълнете данните за неклиматизирани зони.";

    /// <summary>
    /// Списък с неклиматизирани зони
    /// </summary>
    public ObservableCollection<ZtuZone> Zones { get; } = new();

    // ====== Нови полета за двурежимен вход (зима/лято) ======
    /// <summary>
    /// Вътрешна температура - ЛЯТО (°C). Фиксирано по спецификация (по подразбиране 25°C).
    /// </summary>
    [ObservableProperty]
    private double _thetaIntSummer = 25.0;

    /// <summary>
    /// Ако е true, използваме ръчно зададена зимна температура вместо автоматично изчислената.
    /// </summary>
    [ObservableProperty]
    private bool _isWinterTempOverride = false;

    /// <summary>
    /// Ръчно зададена зимна температура (nullable). Използва се само ако IsWinterTempOverride == true.
    /// </summary>
    [ObservableProperty]
    private double? _thetaIntWinterOverride = null;
    
    // ====== Temperatures of the unconditioned space (adjacent space) used for Qtr ======
    /// <summary>
    /// Температура на неклиматизираното помещение - ЗИМА (°C)
    /// </summary>
    [ObservableProperty]
    private double _thetaAdjWinter = 5.0;

    /// <summary>
    /// Температура на неклиматизираното помещение - ЛЯТО (°C)
    /// </summary>
    [ObservableProperty]
    private double _thetaAdjSummer = 25.0;
}
