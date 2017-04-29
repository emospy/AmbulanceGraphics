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
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Excel
{
	/// <summary>
	/// Interaction logic for ExportSchedules.xaml
	/// </summary>
	public partial class ExportHolidayReport : Window
	{
		public ExportHolidayReport()
		{
			InitializeComponent();
			this.dpStartMonth.SelectedDate = DateTime.Now;
			this.dpEndMonth.SelectedDate = DateTime.Now;
		    this.txtSickTreshold.Text = "0";
		}

		private void btnGenerateSchedule_Click(object sender, RoutedEventArgs e)
		{
		    int tres = 0;
		    if (int.TryParse(this.txtSickTreshold.Text, out tres) == false)
		    {
		        MessageBox.Show("Въведете праг за брой дни отпуск");
		        return;
		    }
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.FileName = "Сумарен отчет отпуски.xlsx";
			if (sfd.ShowDialog() == true)
			{
				using (var logic = new AbsenceExportLogic())
				{
					try
					{
						logic.GenerateAbsenceReport(sfd.FileName, this.dpStartMonth.SelectedDate.Value, this.dpEndMonth.SelectedDate.Value, tres, AbsenceExportTypes.Holidays);
						MessageBox.Show("Експортирането завърши успешно");
						System.Diagnostics.Process.Start(sfd.FileName);
					}
					catch (ZoraException ex)
					{
						MessageBox.Show(ex.Result.ErrorCodeMessage);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
			}
		}
	}
}
