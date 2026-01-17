using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using EE.Doklad.Models;

namespace EE.Doklad.ViewModels
{
    public class RoofSectionViewModel : INotifyPropertyChanged
    {
        private readonly RoofSectionData _data;

        public event PropertyChangedEventHandler? PropertyChanged;

        public System.Collections.ObjectModel.ObservableCollection<RoofType> RoofTypes => _data.RoofTypes;
        
        public System.Collections.ObjectModel.ObservableCollection<RoofType> WarmRoofs => _data.WarmRoofs;
        
        public System.Collections.ObjectModel.ObservableCollection<RoofType> ColdRoofs => _data.ColdRoofs;

        public string Description
        {
            get => _data.Description;
            set
            {
                if (_data.Description != value)
                {
                    _data.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public ICommand RemoveRoofTypeCommand { get; }

        public int WarmRoofLimit => 6;
        public int ColdRoofLimit => 3;

        private RoofType? _selectedRoofType;
        public RoofType? SelectedRoofType
        {
            get => _selectedRoofType;
            set
            {
                if (_selectedRoofType != value)
                {
                    _selectedRoofType = value;
                    OnPropertyChanged(nameof(SelectedRoofType));
                }
            }
        }

        public RoofSectionViewModel()
            : this(new RoofSectionData())
        {
        }

        public RoofSectionViewModel(RoofSectionData data)
        {
            _data = data;
            
            RemoveRoofTypeCommand = new RelayCommand(param => RemoveRoofType(param as RoofType));
            
            // Първо синхронизираме колекциите преди да закачим събитието
            SyncTypeCollections();
            
            // След това закачаме събитието за бъдещи промени
            _data.RoofTypes.CollectionChanged += RoofTypes_CollectionChanged;
        }

        private void RoofTypes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SyncTypeCollections();
        }

        private void SyncTypeCollections()
        {
            _data.WarmRoofs.Clear();
            _data.ColdRoofs.Clear();

            foreach (var roofType in _data.RoofTypes)
            {
                if (roofType.Mode == RoofMode.Warm)
                {
                    _data.WarmRoofs.Add(roofType);
                }
                else if (roofType.Mode == RoofMode.Cold)
                {
                    _data.ColdRoofs.Add(roofType);
                }
            }
            
            OnPropertyChanged(nameof(WarmRoofs));
            OnPropertyChanged(nameof(ColdRoofs));
        }

        public bool TryAddRoof(RoofMode mode, out string? error)
        {
            error = null;

            if (mode == RoofMode.Warm && RoofTypes.Count(x => x.Mode == RoofMode.Warm) >= WarmRoofLimit)
            {
                error = $"Максимум {WarmRoofLimit} топли покрива.";
                return false;
            }

            if (mode == RoofMode.Cold && RoofTypes.Count(x => x.Mode == RoofMode.Cold) >= ColdRoofLimit)
            {
                error = $"Максимум {ColdRoofLimit} студени покрива.";
                return false;
            }

            var roofType = new RoofType
            {
                Number = RoofTypes.Count + 1,
                Name = $"Покрив тип {RoofTypes.Count + 1}",
                Mode = mode,
                IsSeed = false,
                Area = 0
            };

            if (mode == RoofMode.Warm)
            {
                roofType.WarmDetail = new WarmRoofDetail();
                if (!roofType.WarmDetail.Layers.Any())
                {
                    roofType.WarmDetail.Layers.Add(new RoofLayer());
                }
            }
            else if (mode == RoofMode.Cold)
            {
                roofType.ColdDetail = new ColdRoofDetail();
                if (!roofType.ColdDetail.U1.Layers.Any())
                    roofType.ColdDetail.U1.Layers.Add(new RoofLayer());
                if (!roofType.ColdDetail.U2.Layers.Any())
                    roofType.ColdDetail.U2.Layers.Add(new RoofLayer());
                if (!roofType.ColdDetail.Uw.Layers.Any())
                    roofType.ColdDetail.Uw.Layers.Add(new RoofLayer());
            }

            SelectedRoofType = roofType;
            RoofTypes.Add(roofType);
            OnPropertyChanged(nameof(RoofTypes));
            return true;
        }

        private void RemoveRoofType(RoofType? roofType)
        {
            if (roofType == null)
            {
                return;
            }

            var removedIndex = RoofTypes.IndexOf(roofType);
            RoofTypes.Remove(roofType);
            UpdateIndexes();
            SelectedRoofType = GetNextSelection(removedIndex);
            OnPropertyChanged(nameof(RoofTypes));
        }

        private RoofType? GetNextSelection(int removedIndex)
        {
            if (!RoofTypes.Any())
            {
                return null;
            }

            if (removedIndex < RoofTypes.Count && removedIndex >= 0)
            {
                return RoofTypes[removedIndex];
            }

            return RoofTypes.LastOrDefault();
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < RoofTypes.Count; i++)
            {
                RoofTypes[i].Number = i + 1;
                if (string.IsNullOrWhiteSpace(RoofTypes[i].Name))
                {
                    RoofTypes[i].Name = $"Покрив тип {i + 1}";
                }
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
