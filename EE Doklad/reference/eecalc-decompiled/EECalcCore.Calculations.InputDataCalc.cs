// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.InputDataCalc
using System;
using System.Collections.Generic;
using System.Linq;
using EECalcCore;
using EECalcCore.Calculations;

public static class InputDataCalc
{
	public static List<MonthlyDays> CalcPeriod(this Section section, int firstMonth, int lastMonth, int firstDay, int lastDay)
	{
		List<Month> list = new List<Month>();
		List<Enum> list2 = Enum.GetValues(typeof(Month)).Cast<Enum>().ToList();
		if (firstMonth == lastMonth)
		{
			list.Add((Month)(object)list2[firstMonth]);
		}
		else
		{
			if (firstMonth < lastMonth && lastMonth <= list2.Count)
			{
				for (int i = firstMonth; i <= lastMonth; i++)
				{
					list.Add((Month)(object)list2[i]);
				}
				return section.CalculateMonthlyDays(list, firstDay, lastDay);
			}
			for (int j = firstMonth; j < list2.Count; j++)
			{
				list.Add((Month)(object)list2[j]);
			}
			for (int k = 0; k <= lastMonth; k++)
			{
				list.Add((Month)(object)list2[k]);
			}
		}
		return section.CalculateMonthlyDays(list, firstDay, lastDay);
	}

	public static double CalcHours(this Section section, int startHour, int endHour)
	{
		return (endHour >= startHour) ? (endHour - startHour) : (24 - startHour + endHour);
	}

	public static List<MonthlyDays> CalculateMonthlyDays(this Section section, List<Month> period, int firstDay, int lastDay)
	{
		List<MonthlyDays> list = new List<MonthlyDays>();
		foreach (Month item in period)
		{
			int num = DateTime.DaysInMonth(2006, (int)(item + 1));
			int hollydays = section.GetHollydays(item);
			if (item == period.First())
			{
				if (period.Count == 1)
				{
					int num2 = (num - hollydays) / 7;
					MonthlyDays monthlyDays = new MonthlyDays
					{
						Month = item,
						TotalDays = num,
						Weeks = num2,
						Holydays = hollydays
					};
					for (int i = firstDay; i <= lastDay; i++)
					{
						DateTime dateTime = new DateTime(2006, (int)(item + 1), i);
						if (dateTime.DayOfWeek == DayOfWeek.Saturday)
						{
							monthlyDays.Saturdays++;
						}
						else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
						{
							monthlyDays.Sundays++;
						}
						else
						{
							monthlyDays.WorkDays++;
						}
					}
					monthlyDays.WorkDays = ((monthlyDays.WorkDays > hollydays) ? (monthlyDays.WorkDays - hollydays) : 0);
					list.Add(monthlyDays);
					break;
				}
				double weeksInMonth = GetWeeksInMonth(firstDay, lastDay, num, hollydays, isFirstMonth: true);
				if (firstDay == num)
				{
					weeksInMonth = GetWeeksInMonth(firstDay, lastDay, num + 1, hollydays, isFirstMonth: true);
				}
				if (firstDay > 21)
				{
					int num3 = num - firstDay + 1;
					list.Add(new MonthlyDays
					{
						Month = item,
						Saturdays = 0,
						Sundays = 0,
						WorkDays = ((num3 > hollydays) ? (num3 - hollydays) : 0),
						TotalDays = num,
						Holydays = hollydays,
						Weeks = weeksInMonth
					});
					continue;
				}
				if (firstDay > 14)
				{
					int num3 = num - firstDay + 1 - 2;
					list.Add(new MonthlyDays
					{
						Month = item,
						Saturdays = 1,
						Sundays = 1,
						WorkDays = ((num3 > hollydays) ? (num3 - hollydays) : 0),
						TotalDays = num,
						Holydays = hollydays,
						Weeks = weeksInMonth
					});
					continue;
				}
				if (firstDay > 7)
				{
					int num3 = num - firstDay + 1 - 4;
					list.Add(new MonthlyDays
					{
						Month = item,
						Saturdays = 2,
						Sundays = 2,
						WorkDays = ((num3 > hollydays) ? (num3 - hollydays) : 0),
						TotalDays = num,
						Holydays = hollydays,
						Weeks = weeksInMonth
					});
					continue;
				}
				MonthlyDays monthlyDays2 = new MonthlyDays
				{
					Month = item,
					TotalDays = num,
					Holydays = hollydays,
					Weeks = weeksInMonth
				};
				for (int j = firstDay; j <= num; j++)
				{
					DateTime dateTime2 = new DateTime(2006, (int)(item + 1), j);
					if (dateTime2.DayOfWeek == DayOfWeek.Saturday)
					{
						monthlyDays2.Saturdays++;
					}
					else if (dateTime2.DayOfWeek == DayOfWeek.Sunday)
					{
						monthlyDays2.Sundays++;
					}
					else
					{
						monthlyDays2.WorkDays++;
					}
				}
				monthlyDays2.WorkDays = ((monthlyDays2.WorkDays > hollydays) ? (monthlyDays2.WorkDays - hollydays) : 0);
				list.Add(monthlyDays2);
				continue;
			}
			if (item == period.Last())
			{
				if (lastDay > num)
				{
					lastDay = num;
				}
				double weeksInMonth2 = GetWeeksInMonth(firstDay, lastDay, num, hollydays, isFirstMonth: false);
				if (lastDay < 7)
				{
					list.Add(new MonthlyDays
					{
						Month = item,
						Saturdays = 0,
						Sundays = 0,
						WorkDays = ((lastDay > hollydays) ? (lastDay - hollydays) : 0),
						TotalDays = num,
						Holydays = hollydays,
						Weeks = weeksInMonth2
					});
					continue;
				}
				if (lastDay < 14)
				{
					int num3 = lastDay - 2;
					list.Add(new MonthlyDays
					{
						Month = item,
						Saturdays = 1,
						Sundays = 1,
						WorkDays = ((num3 > hollydays) ? (num3 - hollydays) : 0),
						TotalDays = num,
						Holydays = hollydays,
						Weeks = weeksInMonth2
					});
					continue;
				}
				if (lastDay < 21)
				{
					int num3 = lastDay - 4;
					list.Add(new MonthlyDays
					{
						Month = item,
						Saturdays = 2,
						Sundays = 2,
						WorkDays = ((num3 > hollydays) ? (num3 - hollydays) : 0),
						TotalDays = num,
						Holydays = hollydays,
						Weeks = weeksInMonth2
					});
					continue;
				}
				MonthlyDays monthlyDays3 = new MonthlyDays
				{
					Month = item,
					TotalDays = num,
					Holydays = hollydays,
					Weeks = weeksInMonth2
				};
				for (int k = 1; k <= lastDay; k++)
				{
					DateTime dateTime3 = new DateTime(2006, (int)(item + 1), k);
					if (dateTime3.DayOfWeek == DayOfWeek.Saturday)
					{
						monthlyDays3.Saturdays++;
					}
					else if (dateTime3.DayOfWeek == DayOfWeek.Sunday)
					{
						monthlyDays3.Sundays++;
					}
					else
					{
						monthlyDays3.WorkDays++;
					}
				}
				monthlyDays3.WorkDays = ((monthlyDays3.WorkDays > hollydays) ? (monthlyDays3.WorkDays - hollydays) : 0);
				list.Add(monthlyDays3);
				continue;
			}
			double weeks = ((double)num - (double)hollydays) / 7.0;
			MonthlyDays monthlyDays4 = new MonthlyDays
			{
				Month = item,
				TotalDays = num,
				Holydays = hollydays,
				Weeks = weeks
			};
			for (int l = 1; l <= num; l++)
			{
				DateTime dateTime4 = new DateTime(2006, (int)(item + 1), l);
				if (dateTime4.DayOfWeek == DayOfWeek.Saturday)
				{
					monthlyDays4.Saturdays++;
				}
				else if (dateTime4.DayOfWeek == DayOfWeek.Sunday)
				{
					monthlyDays4.Sundays++;
				}
				else
				{
					monthlyDays4.WorkDays++;
				}
			}
			monthlyDays4.WorkDays = ((monthlyDays4.WorkDays > hollydays) ? (monthlyDays4.WorkDays - hollydays) : 0);
			list.Add(monthlyDays4);
		}
		return list;
	}

	private static double GetWeeksInMonth(double startingday, double endDay, double daysinMonth, double holydays, bool isFirstMonth)
	{
		if (isFirstMonth)
		{
			return (daysinMonth - startingday + 1.0 > holydays) ? ((daysinMonth - startingday + 1.0 - holydays) / 7.0) : 0.0;
		}
		return (endDay > holydays) ? ((endDay - holydays) / 7.0) : 0.0;
	}

	private static int GetHollydays(this Section section, Month month)
	{
		return month switch
		{
			Month.January => section.Holidays.January, 
			Month.February => section.Holidays.February, 
			Month.March => section.Holidays.March, 
			Month.April => section.Holidays.April, 
			Month.May => section.Holidays.May, 
			Month.June => section.Holidays.June, 
			Month.July => section.Holidays.July, 
			Month.August => section.Holidays.August, 
			Month.September => section.Holidays.September, 
			Month.October => section.Holidays.October, 
			Month.November => section.Holidays.November, 
			Month.December => section.Holidays.December, 
			_ => 0, 
		};
	}
}
