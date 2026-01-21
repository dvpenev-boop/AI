namespace EE.Doklad.Services
{
    /// <summary>
    /// Интерфейс за стратегия за изчисление на конкретен тип под
    /// </summary>
    public interface IFloorStrategy<TInput> where TInput : class
    {
        Models.FloorCalculationResult Calculate(TInput input);
    }
}
