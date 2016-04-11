using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class DriverAmbulancesViewModel
	{
		public int id_driverAssignment { get; set; }
		public string Name { get; set; }
		public string Level1 { get; set; }
		public string Level2 { get; set; }
		public string Level3 { get; set; }
		public string Level4 { get; set; }
		public string MainAmbulance { get; set; }
		public string SecondaryAmbulance { get; set; }
		public string WorkTime { get; set; }
		public string DayHours { get; set; }
		public string NightHours { get; set; }
		public int? id_mainAmbulance { get; set; }
	}
}
