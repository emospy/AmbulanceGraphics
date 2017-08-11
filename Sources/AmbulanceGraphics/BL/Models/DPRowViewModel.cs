using BL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;


namespace BL.Models
{
    public class DPRow : INotifyPropertyChanged
    {
        private double shifts;
        private double norm;

        
        public DateTime Date { get; set; }

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
        public double TotalWorkedOut
        {
            get
            {
                return Math.Round(this.Shifts, 0);
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
        public double Difference { get; set; }
        public double WorkHours { get; set; }

        public int CountDayShifts { get; set; }
        public int CountNightShifts { get; set; }

        #region days
        public int day1 { get; set; }
        public int day2 { get; set; }
        public int day3 { get; set; }
        public int day4 { get; set; }
        public int day5 { get; set; }
        public int day6 { get; set; }
        public int day7 { get; set; }
        public int day8 { get; set; }
        public int day9 { get; set; }
        public int day10 { get; set; }
        public int day11 { get; set; }
        public int day12 { get; set; }
        public int day13 { get; set; }
        public int day14 { get; set; }
        public int day15 { get; set; }
        public int day16 { get; set; }
        public int day17 { get; set; }
        public int day18 { get; set; }
        public int day19 { get; set; }
        public int day20 { get; set; }
        public int day21 { get; set; }
        public int day22 { get; set; }
        public int day23 { get; set; }
        public int day24 { get; set; }
        public int day25 { get; set; }
        public int day26 { get; set; }
        public int day27 { get; set; }
        public int day28 { get; set; }
        public int day29 { get; set; }
        public int day30 { get; set; }
        public int day31 { get; set; }
        #endregion

        public virtual int this[int index]
        {
            set
            {
                switch (index)
                {
                    case 1:
                        day1 = value;
                        break;
                    case 2:
                        day2 = value;
                        break;
                    case 3:
                        day3 = value;
                        break;
                    case 4:
                        day4 = value;
                        break;
                    case 5:
                        day5 = value;
                        break;
                    case 6:
                        day6 = value;
                        break;
                    case 7:
                        day7 = value;
                        break;
                    case 8:
                        day8 = value;
                        break;
                    case 9:
                        day9 = value;
                        break;
                    case 10:
                        day10 = value;
                        break;
                    case 11:
                        day11 = value;
                        break;
                    case 12:
                        day12 = value;
                        break;
                    case 13:
                        day13 = value;
                        break;
                    case 14:
                        day14 = value;
                        break;
                    case 15:
                        day15 = value;
                        break;
                    case 16:
                        day16 = value;
                        break;
                    case 17:
                        day17 = value;
                        break;
                    case 18:
                        day18 = value;
                        break;
                    case 19:
                        day19 = value;
                        break;
                    case 20:
                        day20 = value;
                        break;
                    case 21:
                        day21 = value;
                        break;
                    case 22:
                        day22 = value;
                        break;
                    case 23:
                        day23 = value;
                        break;
                    case 24:
                        day24 = value;
                        break;
                    case 25:
                        day25 = value;
                        break;
                    case 26:
                        day26 = value;
                        break;
                    case 27:
                        day27 = value;
                        break;
                    case 28:
                        day28 = value;
                        break;
                    case 29:
                        day29 = value;
                        break;
                    case 30:
                        day30 = value;
                        break;
                    case 31:
                        day31 = value;
                        break;
                }
            }
            get
            {
                switch (index)
                {
                    case 1:
                        return day1;
                    case 2:
                        return day2;
                    case 3:
                        return day3;
                    case 4:
                        return day4;
                    case 5:
                        return day5;
                    case 6:
                        return day6;
                    case 7:
                        return day7;
                    case 8:
                        return day8;
                    case 9:
                        return day9;
                    case 10:
                        return day10;
                    case 11:
                        return day11;
                    case 12:
                        return day12;
                    case 13:
                        return day13;
                    case 14:
                        return day14;
                    case 15:
                        return day15;
                    case 16:
                        return day16;
                    case 17:
                        return day17;
                    case 18:
                        return day18;
                    case 19:
                        return day19;
                    case 20:
                        return day20;
                    case 21:
                        return day21;
                    case 22:
                        return day22;
                    case 23:
                        return day23;
                    case 24:
                        return day24;
                    case 25:
                        return day25;
                    case 26:
                        return day26;
                    case 27:
                        return day27;
                    case 28:
                        return day28;
                    case 29:
                        return day29;
                    case 30:
                        return day30;
                    case 31:
                        return day31;
                }
                return 0;
            }
        }
        public void CalculateHours()
        {
            double numHours = 0;
            int countDayShifts = 0, countNightShifts = 0, countRegularShifts = 0;

            int helpHoliday = 0;
            int countWorkDays = 0;

            #region Sumdays
            for (int i = 1; i <= DateTime.DaysInMonth(Date.Year, Date.Month); i++)
            {
                if (this[i] != 0)
                {
                    numHours += 12;
                }

                if (this[i] == (int)PresenceTypes.DayShift)
                {
                    countDayShifts++;
                }
                if (this[i] == (int)PresenceTypes.NightShift)
                {
                    countNightShifts++;
                }
            }
            #endregion

            this.CountDayShifts = countDayShifts;
            this.CountNightShifts = countNightShifts;

            this.Shifts = numHours;
            this.Difference = this.Shifts - this.Norm;
            this.Difference = Math.Round(this.Difference);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
                this.CalculateHours();
            }
        }
    }

    public class DPRowViewModel : DPRow, INotifyPropertyChanged
    {
        #region days
        public new string Day1 { get; set; }
        public new string Day2 { get; set; }
        public new string Day3 { get; set; }
        public new string Day4 { get; set; }
        public new string Day5 { get; set; }
        public new string Day6 { get; set; }
        public new string Day7 { get; set; }
        public new string Day8 { get; set; }
        public new string Day9 { get; set; }
        public new string Day10 { get; set; }
        public new string Day11 { get; set; }
        public new string Day12 { get; set; }
        public new string Day13 { get; set; }
        public new string Day14 { get; set; }
        public new string Day15 { get; set; }
        public new string Day16 { get; set; }
        public new string Day17 { get; set; }
        public new string Day18 { get; set; }
        public new string Day19 { get; set; }
        public new string Day20 { get; set; }
        public new string Day21 { get; set; }
        public new string Day22 { get; set; }
        public new string Day23 { get; set; }
        public new string Day24 { get; set; }
        public new string Day25 { get; set; }
        public new string Day26 { get; set; }
        public new string Day27 { get; set; }
        public new string Day28 { get; set; }
        public new string Day29 { get; set; }
        public new string Day30 { get; set; }
        public new string Day31 { get; set; }
        #endregion

        public new string this[int index]
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
                return "";
            }
        }
    }
}
