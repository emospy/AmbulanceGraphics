using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class StructureTreeViewModel
	{
		public int id_department { get; set; }
		public int id_departmentParent { get; set; }
		public string DepartmentName { get; set; }
		public int TreeOrder { get; set; }
	}
}
