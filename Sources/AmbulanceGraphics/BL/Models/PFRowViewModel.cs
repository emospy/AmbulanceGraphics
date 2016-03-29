using BL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;


namespace BL.Models
{
    public class PFRow : INotifyPropertyChanged
    {
		private double shifts;
		private double norm;
		private double totalCount;
		const double NightWorkHourCoefficient = 0.143;

		public GR_PresenceForms PF { get; set; }

		public CalendarRow cRow { get; set; }

		public int id_contract { get; set; }
	    public int id_person { get; set; }
		public List<GR_ShiftTypes> lstShiftTypes { get; set; }
        public DateTime RealDate { get; set; }
		public bool IsDataChanged { get; set; }
		public string Name { get; set; }        
        public double Shifts
        {
            get
            {
                return shifts;
            }
            set
            {
                shifts = value;
                //OnPropertyChanged("Shifts");
            }
        }
		public double Norm
		{
			get
			{
				return this.norm;//(int)Math.Round((this.norm * this.worktimeCoef));
			}
			set
			{
				this.norm = value;
			}
		}
		public double Difference
		{
			get
			{
				return Math.Round(this.Shifts - this.Norm, 0);
			}
		}
		public double WorkHours { get; set; }

		public int CountDayShifts { get; set; }
		public int CountNightShifts { get; set; }
		
		#region days
		public string Day1
		{
			get
			{
				if(this.PF == null)
				{
					return "";
				}				
				return this.lstShiftTypes[(this.PF.id_day1 == null)? 0 : (int)this.PF.id_day1].Name;
			}
		}
		public string Day2
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day2 == null)? 0 : (int)this.PF.id_day2].Name;
			}
		}
		public string Day3
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day3 == null)? 0 : (int)this.PF.id_day3].Name;
			}
		}
		public string Day4
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day4 == null)? 0 : (int)this.PF.id_day4].Name;
			}
		}
		public string Day5
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day5 == null)? 0 : (int)this.PF.id_day5].Name;
			}
		}
		public string Day6
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day6 == null)? 0 : (int)this.PF.id_day6].Name;
			}
		}
		public string Day7
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day7 == null)? 0 : (int)this.PF.id_day7].Name;
			}
		}
		public string Day8
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day8 == null)? 0 : (int)this.PF.id_day8].Name;
			}
		}
		public string Day9
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day9 == null)? 0 : (int)this.PF.id_day9].Name;
			}
		}
		public string Day10
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day10 == null)? 0 : (int)this.PF.id_day10].Name;
			}
		}
		public string Day11
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day11 == null)? 0 : (int)this.PF.id_day11].Name;
			}
		}
		public string Day12
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day12 == null)? 0 : (int)this.PF.id_day12].Name;
			}
		}
		public string Day13
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day13 == null)? 0 : (int)this.PF.id_day13].Name;
			}
		}
		public string Day14
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day14 == null)? 0 : (int)this.PF.id_day14].Name;
			}
		}
		public string Day15
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day15 == null)? 0 : (int)this.PF.id_day15].Name;
			}
		}
		public string Day16
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day16 == null)? 0 : (int)this.PF.id_day16].Name;
			}
		}
		public string Day17
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day17 == null)? 0 : (int)this.PF.id_day17].Name;
			}
		}
		public string Day18
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day18 == null)? 0 : (int)this.PF.id_day18].Name;
			}
		}
		public string Day19
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day19 == null)? 0 : (int)this.PF.id_day19].Name;
			}
		}
		public string Day20
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day20 == null)? 0 : (int)this.PF.id_day20].Name;
			}
		}
		public string Day21
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day21 == null)? 0 : (int)this.PF.id_day21].Name;
			}
		}
		public string Day22
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day22 == null)? 0 : (int)this.PF.id_day22].Name;
			}
		}
		public string Day23
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day23 == null)? 0 : (int)this.PF.id_day23].Name;
			}
		}
		public string Day24
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day24 == null)? 0 : (int)this.PF.id_day24].Name;
			}
		}
		public string Day25
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day25 == null)? 0 : (int)this.PF.id_day25].Name;
			}
		}
		public string Day26
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day26 == null)? 0 : (int)this.PF.id_day26].Name;
			}
		}
		public string Day27
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day27 == null)? 0 : (int)this.PF.id_day27].Name;
			}
		}
		public string Day28
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day28 == null)? 0 : (int)this.PF.id_day28].Name;
			}
		}
		public string Day29
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day29 == null)? 0 : (int)this.PF.id_day29].Name;
			}
		}
		public string Day30
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day30 == null)? 0 : (int)this.PF.id_day30].Name;
			}
		}
		public string Day31
		{
			get
			{
				if (this.PF == null)
				{
					return "";
				}
				return this.lstShiftTypes[(this.PF.id_day31 == null)? 0 : (int)this.PF.id_day31].Name;
			}
		}
		#endregion

		public int this[int index]
		{
			set
			{
				switch (index)
				{
					case 1:
						PF.id_day1 = value;
						break;
					case 2:
						PF.id_day2 = value;
						break;
					case 3:
						PF.id_day3 = value;
						break;
					case 4:
						PF.id_day4 = value;
						break;
					case 5:
						PF.id_day5 = value;
						break;
					case 6:
						PF.id_day6 = value;
						break;
					case 7:
						PF.id_day7 = value;
						break;
					case 8:
						PF.id_day8 = value;
						break;
					case 9:
						PF.id_day9 = value;
						break;
					case 10:
						PF.id_day10 = value;
						break;
					case 11:
						PF.id_day11 = value;
						break;
					case 12:
						PF.id_day12 = value;
						break;
					case 13:
						PF.id_day13 = value;
						break;
					case 14:
						PF.id_day14 = value;
						break;
					case 15:
						PF.id_day15 = value;
						break;
					case 16:
						PF.id_day16 = value;
						break;
					case 17:
						PF.id_day17 = value;
						break;
					case 18:
						PF.id_day18 = value;
						break;
					case 19:
						PF.id_day19 = value;
						break;
					case 20:
						PF.id_day20 = value;
						break;
					case 21:
						PF.id_day21 = value;
						break;
					case 22:
						PF.id_day22 = value;
						break;
					case 23:
						PF.id_day23 = value;
						break;
					case 24:
						PF.id_day24 = value;
						break;
					case 25:
						PF.id_day25 = value;
						break;
					case 26:
						PF.id_day26 = value;
						break;
					case 27:
						PF.id_day27 = value;
						break;
					case 28:
						PF.id_day28 = value;
						break;
					case 29:
						PF.id_day29 = value;
						break;
					case 30:
						PF.id_day30 = value;
						break;
					case 31:
						PF.id_day31 = value;
						break;
				}
			}
			get
			{
				switch (index)
				{
					case 1:
						return (PF.id_day1 == null)? 0 : (int)PF.id_day1;
					case 2:
						return (PF.id_day2 == null)? 0 : (int)PF.id_day2;
					case 3:
						return (PF.id_day3 == null)? 0 : (int)PF.id_day3;
					case 4:
						return (PF.id_day4 == null)? 0 : (int)PF.id_day4;
					case 5:
						return (PF.id_day5 == null)? 0 : (int)PF.id_day5;
					case 6:
						return (PF.id_day6 == null)? 0 : (int)PF.id_day6;
					case 7:
						return (PF.id_day7 == null)? 0 : (int)PF.id_day7;
					case 8:
						return (PF.id_day8 == null)? 0 : (int)PF.id_day8;
					case 9:
						return (PF.id_day9 == null)? 0 : (int)PF.id_day9;
					case 10:
						return (PF.id_day10 == null)? 0 : (int)PF.id_day10;
					case 11:
						return (PF.id_day11 == null)? 0 : (int)PF.id_day11;
					case 12:
						return (PF.id_day12 == null)? 0 : (int)PF.id_day12;
					case 13:
						return (PF.id_day13 == null)? 0 : (int)PF.id_day13;
					case 14:
						return (PF.id_day14 == null)? 0 : (int)PF.id_day14;
					case 15:
						return (PF.id_day15 == null)? 0 : (int)PF.id_day15;
					case 16:
						return (PF.id_day16 == null)? 0 : (int)PF.id_day16;
					case 17:
						return (PF.id_day17 == null)? 0 : (int)PF.id_day17;
					case 18:
						return (PF.id_day18 == null)? 0 : (int)PF.id_day18;
					case 19:
						return (PF.id_day19 == null)? 0 : (int)PF.id_day19;
					case 20:
						return (PF.id_day20 == null)? 0 : (int)PF.id_day20;
					case 21:
						return (PF.id_day21 == null)? 0 : (int)PF.id_day21;
					case 22:
						return (PF.id_day22 == null)? 0 : (int)PF.id_day22;
					case 23:
						return (PF.id_day23 == null)? 0 : (int)PF.id_day23;
					case 24:
						return (PF.id_day24 == null)? 0 : (int)PF.id_day24;
					case 25:
						return (PF.id_day25 == null)? 0 : (int)PF.id_day25;
					case 26:
						return (PF.id_day26 == null)? 0 : (int)PF.id_day26;
					case 27:
						return (PF.id_day27 == null)? 0 : (int)PF.id_day27;
					case 28:
						return (PF.id_day28 == null)? 0 : (int)PF.id_day28;
					case 29:
						return (PF.id_day29 == null)? 0 : (int)PF.id_day29;
					case 30:
						return (PF.id_day30 == null)? 0 : (int)PF.id_day30;
					case 31:
						return (PF.id_day31 == null)? 0 : (int)PF.id_day31;
				}
				return 0;
			}
		}

		public void CalculateHours()
        {
            double numHours = 0;
			int countDayShifts = 0, countNightShifts = 0;

			if (this.PF == null)
			{
				return;
			}

            for (int i = 1; i <= DateTime.DaysInMonth(RealDate.Year, RealDate.Month); i++)
            {
                GR_ShiftTypes pt = this.lstShiftTypes.Find(pts => pts.id_shiftType == this[i]);
	            if (this.cRow[i] == true) //За работни дни смята всичко
	            {
		            if (pt.Duration.Hours == 0 && pt.id_shiftType != 0)
		            {
			            numHours += this.WorkHours;
		            }
		            else
		            {
			            numHours += pt.Duration.Hours;
		            }
	            }
	            else if(pt.id_shiftType == (int)PresenceTypes.DayShift //За неработни дни само ако е присъствена смяна
						|| pt.id_shiftType == (int)PresenceTypes.NightShift
						|| pt.id_shiftType == (int)PresenceTypes.RegularShift)
	            {
					if (pt.Duration.Hours == 0 && pt.id_shiftType != 0)
					{
						numHours += this.WorkHours;
					}
					else
					{
						numHours += pt.Duration.Hours;
					}
				}

	            if (pt.id_shiftType == (int) PresenceTypes.DayShift)
	            {
		            countDayShifts ++;
	            }
				if (pt.id_shiftType == (int)PresenceTypes.NightShift)
				{
					countNightShifts++;
					if (this.WorkHours == 8 || this.WorkHours == 4)
					{
						numHours += 1.14; //Night shift correction
					}
				}
			}
			this.CountDayShifts = countDayShifts;
			this.CountNightShifts = countNightShifts;
            this.Shifts = numHours;
        }

    //    public void CalculateInitialHours(List<UN_PresenceType> lstPT, double? MonthlyNorm, List<UN_WorktimeAbsence> lstAbsences)
    //    {
    //        double NumHours = 0;
    //        double Miss = 0;
    //        double Gain = 0;
    //        this.NightHours = 0;
    //        for (int i = 1; i < 32; i++)
    //        {
    //            var pt = lstPT.Find(
    //                pts => pts.id_presenceType == this[i]);
    //            NumHours += pt.DefaultHours;
    //            if (pt.StartHour > pt.EndHour)
    //            { //definitely night shift
				//	this.NightHours += Constants.DefaultNightHours;
    //            }
    //            if (pt.id_presenceType == (int)PresenceTypes.Compensation)
    //            {
    //                this.UsedCompensation += Constants.DefaultShiftHours;
    //            }
    //            if (pt.id_presenceType == (int)PresenceTypes.Unpaid)
    //            {
    //                this.Unpaid += Constants.DefaultShiftHours;
    //            }
    //            if (pt.id_presenceType == (int)PresenceTypes.Holiday)
    //            {
    //                this.Holiday += Constants.DefaultShiftHours;
    //            }
    //            if (pt.id_presenceType == (int)PresenceTypes.Sickness)
    //            {
    //                this.Sickness += Constants.DefaultShiftHours;
    //            }
    //        }
    //        if (MonthlyNorm.HasValue)
    //        {
    //            this.Norm = (int)MonthlyNorm.Value;
    //        }
    //        else
    //        {
    //            this.Norm = 0;
    //        }
    //        this.Shifts = NumHours;
    //        foreach (UN_WorktimeAbsence ab in lstAbsences)
    //        {
    //            if (ab.NumberHours.HasValue)
    //            {
    //                if (ab.Absence == true)
    //                {
    //                    Miss += ab.NumberHours.Value;
    //                    if (ab.AdminCompensation != null)
    //                    {
    //                        Miss += ab.AdminCompensation.Value;
    //                    }
    //                }
    //                else
    //                {
    //                    Gain += ab.NumberHours.Value;
    //                    if (ab.AdminCompensation != null)
    //                    {
    //                        Gain += ab.AdminCompensation.Value;
    //                    }
    //                }
    //            }
    //        }
    //        this.Overtime = Gain;
    //        this.Absences = Miss;
    //    }

        public PFRow()
        {
            //this.RealDate = RealDate;
            //for (int i = 1; i <= 31; i++ )
            //{
            //    this[i] = 0;
            //}
            //this.lstMonthHolidays = lstHolidays;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
	            this.IsDataChanged = true;
				this.CalculateHours();
            }
        }
    }

    //class PFStorn
    //{
    //    public string Name { get; set; }
    //    public int id_sysco { get; set; }
    //    public double M1Shifts { get; set; }
    //    public double M1Absences { get; set; }
    //    public double M1Overtime { get; set; }
    //    public double M1Total { get; set; }
    //    public double M1Norm { get; set; }
    //    public double M1Difference { get; set; }
    //    public double M1Compensation { get; set; }
    //    public double M1Holidays { get; set; }
    //    public double M1Sickness { get; set; }
    //    public double M1Unpaid { get; set; }
    //    public double M1UsedCompensation { get; set; }
    //    public double M1Night { get; set; }        

    //    public double M2Shifts { get; set; }
    //    public double M2Absences { get; set; }
    //    public double M2Overtime { get; set; }
    //    public double M2Total { get; set; }
    //    public double M2Norm { get; set; }
    //    public double M2Difference { get; set; }
    //    public double M2Compensation { get; set; }
    //    public double M2Holidays { get; set; }
    //    public double M2Sickness { get; set; }
    //    public double M2Unpaid { get; set; }
    //    public double M2UsedCompensation { get; set; }
    //    public double M2Night { get; set; }
        
    //    public double M3Shifts { get; set; }
    //    public double M3Absences { get; set; }
    //    public double M3Overtime { get; set; }
    //    public double M3Total { get; set; }
    //    public double M3Norm { get; set; }
    //    public double M3Difference { get; set; }
    //    public double M3Compensation { get; set; }
    //    public double M3Holidays { get; set; }
    //    public double M3Sickness { get; set; }
    //    public double M3Unpaid { get; set; }
    //    public double M3UsedCompensation { get; set; }
    //    public double M3Night { get; set; }

    //    public double TShifts { get; set; }
    //    public double TAbsences { get; set; }
    //    public double TOvertime { get; set; }
    //    public double TTotal { get; set; }
    //    public double TNorm { get; set; }
    //    public double TDifference { get; set; }
    //    public double TCompensation { get; set; }
    //    public double THolidays { get; set; }
    //    public double TSickness { get; set; }
    //    public double TUnpaid { get; set; }
    //    public double TUsedCompensation { get; set; }
    //    public double TNight { get; set; }
    //}    

    //class PNames
    //{
    //    public string Name { get; set; }
    //    public int id_sysco { get; set; }
    //}

    //class PFComp
    //{
    //    public string Name { get; set; }
    //    public int id_sysco { get; set; }
    //    public double Shifts { get; set; }
    //    public double Absences { get; set; }
    //    public double Overtime { get; set; }
    //    public double Total { get; set; }
    //    public double Norm { get; set; }
    //    public double Difference { get; set; }
    //    public double Compensation { get; set; }
    //}
}
