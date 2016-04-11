using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class AmbulanceListViewModel
	{
		public int id_ambulance { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string WorkTime { get; set; }
		public bool IsActive { get; set; }
	}
}
