using BL.Logic;
using BL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BL;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for PersonFolder.xaml
	/// </summary>
	public partial class PersonFolder : Window
	{
		public GenericPersonViewModel gPVM;
		public PersonFolder(int id_person, DateTime currentDate, ScheduleTypes id_scheduleType)
		{
			InitializeComponent(); 

			this.RefreshData(id_person, currentDate, id_scheduleType);
		}

		private void RefreshData(int id_person, DateTime currentDate, ScheduleTypes id_scheduleType)
		{
			if (id_person != 0)
			{
				using (var logic = new PersonalLogic())
				{
					this.gPVM = logic.InitGPVM(id_person, currentDate, id_scheduleType);
					this.Title = gPVM.PersonViewModel.Name;
				}
			}
			else
			{
				this.gPVM = new GenericPersonViewModel();
				this.gPVM.PersonViewModel = new PersonViewModel();
				this.gPVM.SchedulesViewModel = new PersonalSchedulesViewModel();
				this.Title = "Нов служител";
			}
			this.DataContext = this.gPVM;
		}

		private void PersonFolder_OnClosing(object sender, CancelEventArgs e)
		{
			if (this.gPVM.PersonViewModel.IsModified)
			{
				if (MessageBox.Show("Извършени са промени по личните данни на лицето. Желаете ли да се откажете от промените?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
				{
					e.Cancel = true;
				}
			}
		}
	}
}
