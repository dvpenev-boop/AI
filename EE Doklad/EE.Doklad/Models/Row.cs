using System.Collections.Generic;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Ред в таблица
    /// </summary>
    public class Row
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public List<Cell> Cells { get; set; } = new();
    }
}
