// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.DataRow
using System.ComponentModel;
using EECalcCore;

public class DataRow : INotifyPropertyChanged
{
	private double valuee;

	private string tag;

	private Fuel fuel;

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

	public Fuel Fuel
	{
		get
		{
			return fuel;
		}
		set
		{
			fuel = value;
			OnPropertyChanged("Fuel");
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
