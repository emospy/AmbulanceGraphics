using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class ContractsViewModel
	{
		public int id_person { get; set; }
		public int id_assignment { get; set; }
		public int id_contract { get; set; }
		public bool IsActive { get; set; }
		public bool IsFired { get; set; }		

		public string Status { get; set; }

		public DateTime? ActiveFrom { get; set; }

		public string ContractNumber { get; set; }
		public DateTime? ContractDate { get; set; }

		public string Level1 { get; set; }
		public string Level2 { get; set; }
		public string Level3 { get; set; }
		public string Level4 { get; set; }

		public int? id_level1 { get; set; }
		public int? id_level2 { get; set; }
		public int? id_level3 { get; set; }
		public int? id_level4 { get; set; }

		public string StructurePosition { get; set; }

		public string WorkTime { get; set; }

		public ObservableCollection<ContractsViewModel> lstAdditionalAssignments {get; set; }
	}
}

