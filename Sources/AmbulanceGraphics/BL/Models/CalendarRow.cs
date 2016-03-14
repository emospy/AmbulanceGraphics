using BL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class CalendarRow
	{
		private DateTime date;

		public bool Day1 { get; set; }
		public bool Day2 { get; set; }
		public bool Day3 { get; set; }
		public bool Day4 { get; set; }
		public bool Day5 { get; set; }
		public bool Day6 { get; set; }
		public bool Day7 { get; set; }
		public bool Day8 { get; set; }
		public bool Day9 { get; set; }
		public bool Day10 { get; set; }
		public bool Day11 { get; set; }
		public bool Day12 { get; set; }
		public bool Day13 { get; set; }
		public bool Day14 { get; set; }
		public bool Day15 { get; set; }
		public bool Day16 { get; set; }
		public bool Day17 { get; set; }
		public bool Day18 { get; set; }
		public bool Day19 { get; set; }
		public bool Day20 { get; set; }
		public bool Day21 { get; set; }
		public bool Day22 { get; set; }
		public bool Day23 { get; set; }
		public bool Day24 { get; set; }
		public bool Day25 { get; set; }
		public bool Day26 { get; set; }
		public bool Day27 { get; set; }
		public bool Day28 { get; set; }
		public bool Day29 { get; set; }
		public bool Day30 { get; set; }
		public bool Day31 { get; set; }

		public bool this[int index]
		{
			set
			{
				switch (index)
				{
					case 1:
						Day1 = value;
						break;
					case 2:
						Day2 = value;
						break;
					case 3:
						Day3 = value;
						break;
					case 4:
						Day4 = value;
						break;
					case 5:
						Day5 = value;
						break;
					case 6:
						Day6 = value;
						break;
					case 7:
						Day7 = value;
						break;
					case 8:
						Day8 = value;
						break;
					case 9:
						Day9 = value;
						break;
					case 10:
						Day10 = value;
						break;
					case 11:
						Day11 = value;
						break;
					case 12:
						Day12 = value;
						break;
					case 13:
						Day13 = value;
						break;
					case 14:
						Day14 = value;
						break;
					case 15:
						Day15 = value;
						break;
					case 16:
						Day16 = value;
						break;
					case 17:
						Day17 = value;
						break;
					case 18:
						Day18 = value;
						break;
					case 19:
						Day19 = value;
						break;
					case 20:
						Day20 = value;
						break;
					case 21:
						Day21 = value;
						break;
					case 22:
						Day22 = value;
						break;
					case 23:
						Day23 = value;
						break;
					case 24:
						Day24 = value;
						break;
					case 25:
						Day25 = value;
						break;
					case 26:
						Day26 = value;
						break;
					case 27:
						Day27 = value;
						break;
					case 28:
						Day28 = value;
						break;
					case 29:
						Day29 = value;
						break;
					case 30:
						Day30 = value;
						break;
					case 31:
						Day31 = value;
						break;
				}
			}
			get
			{
				switch (index)
				{
					case 1:
						return Day1;
					case 2:
						return Day2;
					case 3:
						return Day3;
					case 4:
						return Day4;
					case 5:
						return Day5;
					case 6:
						return Day6;
					case 7:
						return Day7;
					case 8:
						return Day8;
					case 9:
						return Day9;
					case 10:
						return Day10;
					case 11:
						return Day11;
					case 12:
						return Day12;
					case 13:
						return Day13;
					case 14:
						return Day14;
					case 15:
						return Day15;
					case 16:
						return Day16;
					case 17:
						return Day17;
					case 18:
						return Day18;
					case 19:
						return Day19;
					case 20:
						return Day20;
					case 21:
						return Day21;
					case 22:
						return Day22;
					case 23:
						return Day23;
					case 24:
						return Day24;
					case 25:
						return Day25;
					case 26:
						return Day26;
					case 27:
						return Day27;
					case 28:
						return Day28;
					case 29:
						return Day29;
					case 30:
						return Day30;
					case 31:
						return Day31;
				}
				return false;
			}
		}

		public void SetDayFromSP(HR_YearWorkDays day)
		{
			this[day.Date.Day] = (bool)day.IsWorkDay;
		}

		public CalendarRow(DateTime dateS, bool IsNH = false)
		{
			this.date = dateS;
			int dim = DateTime.DaysInMonth(date.Year, date.Month);
			if (IsNH == false)
			{
				for (int i = 1; i <= dim; i++)
				{
					DateTime CD = new DateTime(date.Year, date.Month, i);
					if (CD.DayOfWeek == DayOfWeek.Sunday || CD.DayOfWeek == DayOfWeek.Saturday)
					{
						this[i] = false;
					}
					else
					{
						this[i] = true;
					}
				}

				if (dim < 31)
				{
					for (int i = dim + 1; i <= 31; i++)
					{
						this[i] = false;
					}
				}
			}
			else
			{
				for (int i = 1; i <= 31; i++)
				{
					this[i] = false;
				}
			}	
		}		
	}
}
