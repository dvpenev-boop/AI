using System;
using System.Collections.Generic;
using EE.Doklad.Models;
using QuestPDF.Infrastructure;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Дескриптор на секция за PDF рендериране
    /// </summary>
    public class SectionDescriptor
    {
        public string Id { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public SectionType Type { get; set; }
        
        /// <summary>
        /// Функция за проверка дали секцията има данни
        /// </summary>
        public Func<Section, bool>? HasData { get; set; }
        
        /// <summary>
        /// Функция за рендериране на секцията
        /// </summary>
        public Action<IContainer, Section>? Render { get; set; }
    }

    /// <summary>
    /// Регистър на всички секции за PDF експорт
    /// </summary>
    public class SectionRenderRegistry
    {
        private readonly List<SectionDescriptor> _sections = new();

        public IReadOnlyList<SectionDescriptor> Sections => _sections;

        public void Register(SectionDescriptor descriptor)
        {
            _sections.Add(descriptor);
        }

        /// <summary>
        /// Намиране на дескриптор по тип на секция
        /// </summary>
        public SectionDescriptor? FindByType(SectionType type)
        {
            return _sections.Find(s => s.Type == type);
        }
    }
}
