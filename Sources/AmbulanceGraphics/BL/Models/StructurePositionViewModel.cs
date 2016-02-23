using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class StructurePositionViewModel
	{
		public int id_structurePosition { get; set; }
		public int id_department { get; set; }
		public string GlobalPositionName { get; set; }
		public string PositionType { get; set; }
		public string StaffCount { get; set; }
		public string Code { get; set; }
		public int Order { get; set; }
	}
}
