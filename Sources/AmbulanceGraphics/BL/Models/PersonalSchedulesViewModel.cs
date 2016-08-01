using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class PersonalSchedulesViewModel
	{
		public DateTime CurrentDate { get; set; }
		public PersonalSchedulesViewModel()
		{
			this.CurrentDate = DateTime.Now;
		}
		public int id_person { get; set; }
		public int id_contract { get; set; }
		public ScheduleTypes id_scheduleType { get; set; }
	}
}
