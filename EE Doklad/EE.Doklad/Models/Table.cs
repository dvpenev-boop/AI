using System.Collections.Generic;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Базов клас за таблица
    /// </summary>
    public abstract class Table
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public List<string> ColumnHeaders { get; set; } = new();
        public List<Row> Rows { get; set; } = new();
        public abstract bool IsDynamic { get; }
    }

    /// <summary>
    /// Таблица с фиксиран брой редове (потребителят не може да добавя/маха редове)
    /// </summary>
    public class FixedTable : Table
    {
        public override bool IsDynamic => false;
    }

    /// <summary>
    /// Таблица с променлив брой редове (потребителят може да добавя/маха редове)
    /// </summary>
    public class DynamicTable : Table
    {
        public override bool IsDynamic => true;
    }
}
