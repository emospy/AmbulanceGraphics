using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class AssignmentViewModel
	{
		public int id_assignment { get; set; }
		public int id_structurePosition { get; set; }
		public int id_worktime { get; set; }
		public int NumHolidays { get; set; }
		public int NumAdditionalHolidays { get; set; }
		public string ContractNumber { get; set; }
		public DateTime ContractDate { get; set; }
		public DateTime TestContractDate { get; set; }
		public DateTime EndContractDate { get; set; }
		public DateTime AssignmentDate { get; set; }
	}
}
