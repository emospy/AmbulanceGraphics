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
	public partial class GenerateSchedule : Window
	{
		public GenerateSchedule()
		{
			InitializeComponent();
			this.dpMonth.SelectedDate = DateTime.Now.AddMonths(1);
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
					logic.GenerateSchedules(this.dpMonth.SelectedDate.Value, startShift);
					MessageBox.Show("Генерирането приключи успешно.");
				}
			}
		}
	}
}
