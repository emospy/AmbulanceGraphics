using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models
{
	public class GenericPersonViewModel
	{
		public PersonViewModel PersonViewModel { get; set; }
		public ObservableCollection<ContractsViewModel> lstContracts { get; set; }

		public List<AbsenceListViewModel> lstAbsences { get; set; }
		//public List<YearHolidaysViewModel> lstAbsences { get; set; }
	}
}
