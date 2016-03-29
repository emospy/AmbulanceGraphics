using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BL.Models
{
	public class CrewListViewModel
	{
		public int id_crew { get; set; }
		public int id_department { get; set; }
		public string CrewName { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public bool IsActive { get; set; }
		public string CrewType { get; set; }
		public string RegNumber { get; set; }
		public string WorkTime { get; set; }
		public bool IsTemporary { get; set; }
		public string CrewDate { get; set; }
		public string State { get; set; }

		public SolidColorBrush Background { get; set; }

		//public List<CrewListViewModel> lstCrewMembers { get; set; }
	}
}