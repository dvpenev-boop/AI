// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.SavingsData
using System.ComponentModel;

public class SavingsData : INotifyPropertyChanged
{
	private string technology;

	private double oldValue;

	private double valuee;

	private string row;

	private string tag;

	private double netEnergy;

	private double netEnergyNMinusOne;

	private double saving;

	private double savingNMinusOne;

	private double part;

	private double actualSaving;

	public string Technology
	{
		get
		{
			return technology;
		}
		set
		{
			technology = value;
			OnPropertyChanged("Technology");
		}
	}

	public double OldValue
	{
		get
		{
			return oldValue;
		}
		set
		{
			oldValue = value;
			OnPropertyChanged("OldValue");
		}
	}

	public double Value
	{
		get
		{
			return valuee;
		}
		set
		{
			valuee = value;
			OnPropertyChanged("Value");
		}
	}

	public string Row
	{
		get
		{
			return row;
		}
		set
		{
			row = value;
			OnPropertyChanged("Row");
		}
	}

	public string Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
			OnPropertyChanged("Tag");
		}
	}

	public double NetEnergy
	{
		get
		{
			return netEnergy;
		}
		set
		{
			netEnergy = value;
			OnPropertyChanged("NetEnergy");
		}
	}

	public double NetEnergyNMinusOne
	{
		get
		{
			return netEnergyNMinusOne;
		}
		set
		{
			netEnergyNMinusOne = value;
			OnPropertyChanged("NetEnergy2");
		}
	}

	public double Saving
	{
		get
		{
			return saving;
		}
		set
		{
			saving = value;
			OnPropertyChanged("Saving");
		}
	}

	public double SavingNMinusOne
	{
		get
		{
			return savingNMinusOne;
		}
		set
		{
			savingNMinusOne = value;
			OnPropertyChanged("Saving2");
		}
	}

	public double Part
	{
		get
		{
			return part;
		}
		set
		{
			part = value;
			OnPropertyChanged("Part");
		}
	}

	public double ActualSaving
	{
		get
		{
			return actualSaving;
		}
		set
		{
			actualSaving = value;
			OnPropertyChanged("ActualSaving");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
