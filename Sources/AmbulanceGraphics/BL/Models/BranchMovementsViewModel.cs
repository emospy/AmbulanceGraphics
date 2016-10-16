using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class BranchMovementsViewModel
	{
		public DateTime Date { get; set; }
		public int? id_branch { get; set; }
		public string ShiftType { get; set; }

		public int id_presenceForm { get; set; }

		public int id_branchMovement { get; set; }
	}
}
