using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class CrewScheduleListViewModel : PFRow
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

		public ObservableCollection<CrewScheduleListViewModel> lstCrewMembers { get; set; }

	}
}
