using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class PersonnelViewModel
	{
		public int id_person { get; set; }
		public string  EGN { get; set; }
		public int? id_assignment { get; set; }

		public int? id_department { get; set; }
		public int? id_contract { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public string Level1 { get; set; }
		public string Level2 { get; set; }
		public string Level3 { get; set; }
		public string Level4 { get; set; }

		public int? id_level1 { get; set; }
		public int? id_level2 { get; set; }
		public int? id_level3 { get; set; }
		public int? id_level4 { get; set; }

		public int id_positionType { get; set; }

		public double? WorkHours { get; set; }

		public string WorkZoneDay { get; set; }
		public string WorkZoneNight { get; set; }

		public int Order { get; set; }

		public string ShortPosition { get; set; }
	}
}
