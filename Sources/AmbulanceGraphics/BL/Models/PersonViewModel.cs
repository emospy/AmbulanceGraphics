using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class PersonViewModel
	{
		public int id_person { get; set; }
		public string Name { get; set; }
		public string EGN { get; set; }
		public int? id_gender { get; set; }
		public string Address { get; set; }
		public string GSM { get; set; }
		public bool IsModified { get; set; }
	}
}
