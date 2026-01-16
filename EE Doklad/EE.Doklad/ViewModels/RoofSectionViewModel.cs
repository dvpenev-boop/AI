using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using EE.Doklad.Models;

namespace EE.Doklad.ViewModels
{
    public class RoofSectionViewModel : INotifyPropertyChanged
    {
    public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<RoofType> RoofTypes { get; set; } = new();
        public string Description { get; set; } = string.Empty;

    public ICommand AddWarmRoofCommand { get; }
    public ICommand AddColdRoofCommand { get; }
    public ICommand RemoveRoofTypeCommand { get; }

        public int WarmRoofLimit => 6;
        public int ColdRoofLimit => 3;

        public RoofSectionViewModel()
        {
            AddWarmRoofCommand = new RelayCommand(_ => AddWarmRoof(), _ => CanAddWarmRoof());
            AddColdRoofCommand = new RelayCommand(_ => AddColdRoof(), _ => CanAddColdRoof());
            RemoveRoofTypeCommand = new RelayCommand(param => RemoveRoofType(param as RoofType));
        }

        private void AddWarmRoof()
        {
            int count = RoofTypes.Count(x => x.Mode == RoofMode.Warm) + 1;
            var roofType = new RoofType
            {
                Number = RoofTypes.Count + 1,
                Name = $"Топъл покрив тип {count}",
                Mode = RoofMode.Warm,
                WarmDetail = new WarmRoofDetail()
            };
            // Null protection
            if (roofType.WarmDetail == null)
                roofType.WarmDetail = new WarmRoofDetail();
            RoofTypes.Add(roofType);
            OnPropertyChanged(nameof(RoofTypes));
        }

        private void AddColdRoof()
        {
            int count = RoofTypes.Count(x => x.Mode == RoofMode.Cold) + 1;
            var roofType = new RoofType
            {
                Number = RoofTypes.Count + 1,
                Name = $"Студен покрив тип {count}",
                Mode = RoofMode.Cold,
                ColdDetail = new ColdRoofDetail()
            };
            // Null protection
            if (roofType.ColdDetail == null)
                roofType.ColdDetail = new ColdRoofDetail();
            RoofTypes.Add(roofType);
            OnPropertyChanged(nameof(RoofTypes));
        }

        private bool CanAddWarmRoof() => RoofTypes.Count(x => x.Mode == RoofMode.Warm) < WarmRoofLimit;
        private bool CanAddColdRoof() => RoofTypes.Count(x => x.Mode == RoofMode.Cold) < ColdRoofLimit;

        private void RemoveRoofType(RoofType? roofType)
        {
            if (roofType != null)
            {
                RoofTypes.Remove(roofType);
                // Re-number
                int i = 1;
                foreach (var r in RoofTypes)
                    r.Number = i++;
                OnPropertyChanged(nameof(RoofTypes));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
