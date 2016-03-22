using System;
using System.Collections.Generic;
using System.IO;
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
using BL.DB;
using BL.Logic;
using BL.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AmbulanceGraphics.Excel
{
	/// <summary>
	/// Interaction logic for ExportSchedules.xaml
	/// </summary>
	public partial class ExportSchedules : Window
	{
		public ExportSchedules()
		{
			InitializeComponent();
		}

		private void btnGenerateSchedule_Click(object sender, RoutedEventArgs e)
		{
			if (this.dpMonth.SelectedDate.HasValue == false)
			{
				MessageBox.Show("Моля, изберете дата!");
				return;
			}
			DateTime date = this.dpMonth.SelectedDate.Value;
            if (this.cmbScheduleType.SelectedIndex != 0)
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.FileName = date.Year + date.Month + date.Day + this.cmbScheduleType.Text + ".xlsx";
				if (sfd.ShowDialog() == true)
				{
					using ( var logic = new CrewSchedulesLogic2())
					{
						logic.ExportMonthlySchedule(sfd.FileName, "Месечен график", date, ScheduleTypes.ForecastMonthSchedule);
						MessageBox.Show("Експортирането завърши успешно");
						System.Diagnostics.Process.Start(sfd.FileName);
					}
				}
			}
		}

		
			

			

		private void ExportSchedules_OnLoaded(object sender, RoutedEventArgs e)
		{
			using (ComboBoxLogic logic = new ComboBoxLogic())
			{
				List<ComboBoxModel> lstModels;
				logic.NM_ScheduleTypes.FillComboBoxModel(out lstModels);
				this.cmbScheduleType.ItemsSource = lstModels;
			}
			this.dpMonth.SelectedDate = DateTime.Now;
		}
	}
}
