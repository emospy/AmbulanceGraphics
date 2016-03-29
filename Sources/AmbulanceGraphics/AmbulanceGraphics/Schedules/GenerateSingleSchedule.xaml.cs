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
using System.Windows.Shapes;
using BL.Logic;

namespace AmbulanceGraphics.Schedules
{
	/// <summary>
	/// Interaction logic for GenerateSchedule.xaml
	/// </summary>
	public partial class GenerateSingleSchedule : Window
	{
		private int id_department;
		public GenerateSingleSchedule(int id_department)
		{
			InitializeComponent();
			this.dpMonth.SelectedDate = DateTime.Now.AddMonths(1);
			this.id_department = id_department;
		}

		private void btnGenerateSchedule_Click(object sender, RoutedEventArgs e)
		{
			if (this.dpMonth.SelectedDate.HasValue)
			{
				int startShift = 0;
				int.TryParse(this.txtStartShift.Text, out startShift);
				if (startShift > 0)
				{
					startShift -= 1;
				}
				using (var logic = new SchedulesLogic())
				{
					if (logic.IsMonthlyScheduleAlreadyGenerated(this.dpMonth.SelectedDate.Value))
					{
						if (
							MessageBox.Show("За избравия месец вече има генериран график. Желаете ли да го генерирате отново?", "Въпрос",
								MessageBoxButton.YesNo) == MessageBoxResult.No)
						{
							return;
						}
					}
					logic.GenerateSingleDepartmentSchedule(this.dpMonth.SelectedDate.Value, this.id_department, startShift);
					MessageBox.Show("Генерирането приключи успешно.");
				}
			}
		}
	}
}
