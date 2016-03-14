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
		public int? id_workTime { get; set; }
		public int? NumHolidays { get; set; }
		public int? AdditionalHolidays { get; set; }
		public string ContractNumber { get; set; }
		public DateTime? ContractDate { get; set; }
		public DateTime? TestContractDate { get; set; }
		public DateTime? EndContractDate { get; set; }
		public DateTime? AssignmentDate { get; set; }
		public bool IsAdditionalAssignment { get; set; }
		public double? BaseSalary { get; set; }
		public int? id_contractType { get; set; }
		public int? id_lawType { get; set; }
		public int id_contract { get; set; }
		public bool IsActive { get; set; }
		public int id_person { get; set; }

		public int? id_level1 { get; set; }
		public int? id_level2 { get; set; }
		public int? id_level3 { get; set; }
		public int? id_level4 { get; set; }
	}
}
