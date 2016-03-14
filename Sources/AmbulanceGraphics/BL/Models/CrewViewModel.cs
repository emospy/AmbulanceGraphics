using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class CrewViewModel
	{
		public int id_crew { get; set; }
		public int id_department { get; set; }
		public string CrewName { get; set; }

		public int? id_assignment1 { get; set; }
		public int? id_assignment2 { get; set; }
		public int? id_assignment3 { get; set; }
		public int? id_assignment4 { get; set; }

		public int id_crewType { get; set; }

		public bool IsActive { get; set; }

		public bool IsTemporary { get; set; }

		public DateTime Date { get; set; }
	}
}
