using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class WorkTimeAbsenceListViewModel
	{
		public int id_worktimeAbsence { get; set; }
		public int id_contract { get; set; }
		public string Date { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public bool IsPresence { get; set; }
		public String Reasons { get; set; }
		public double? PrevMonthHours { get; set; }
		public double NumberHours { get; set; }
		public int CourseNumber { get; set; }
		public string RegNumber { get; set; }
		public bool IsGPSMatch { get; set; }
		public bool IsPrevMonthTransfer { get; set; }
		public DateTime GPSEndHour { get; set; }
	}
}
