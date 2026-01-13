namespace EE.Doklad.Models
{
    /// <summary>
    /// Клетка в таблица с типизирана стойност
    /// </summary>
    public class Cell
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public CellType Type { get; set; } = CellType.Text;
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Валидира стойността според типа на клетката
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Value)) return true; // празно е валидно

            return Type switch
            {
                CellType.Number => double.TryParse(Value, out _),
                CellType.Date => System.DateTime.TryParse(Value, out _),
                CellType.Text => true,
                _ => true
            };
        }
    }

    public enum CellType
    {
        Text,
        Number,
        Date
    }
}
