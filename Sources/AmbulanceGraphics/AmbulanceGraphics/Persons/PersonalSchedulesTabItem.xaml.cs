using BL.Logic;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for PersonalSchedulesTabItem.xaml
	/// </summary>
	public partial class PersonalSchedulesTabItem : UserControl
	{
		private int id_person;
		public PersonalSchedulesTabItem()
		{
			InitializeComponent();
		}

		private void pdMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			var parent = ((((this.Parent as TabItem).Parent as TabControl).Parent as Grid).Parent as PersonFolder);
			this.id_person = parent.gPVM.PersonViewModel.id_person;
			using (var logic = new SchedulesLogic())
			{
				this.grGridViewSchedule.ItemsSource = logic.GetPersonalSchedule(this.id_person, this.dpMonth.SelectedDate.Value);
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.dpMonth.SelectedDate = DateTime.Now;

			this.LoadCombos();
		}

		private void LoadCombos()
		{
			using (var logic = new NomenclaturesLogic())
			{
				
			}
		}
	}
}
