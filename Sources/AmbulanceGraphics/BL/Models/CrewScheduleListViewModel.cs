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
		public string ShortPosition { get; set; }
		public bool IsActive { get; set; }
		public bool IsTemporary{ get; set; }
		public string CrewDate{ get; set; }
		public string CrewType { get; set; }
		public string RegNumber { get; set; }
		public string WorkTime { get; set; }
		public int RowPosition { get; set; }
		public int id_positionType { get; set; }
		public int id_crewType { get; set; }
		public int State { get; set; }
		public string BaseDepartment { get; set; }
		public List<CrewScheduleListViewModel> LstCrewMembers;
	}
}
