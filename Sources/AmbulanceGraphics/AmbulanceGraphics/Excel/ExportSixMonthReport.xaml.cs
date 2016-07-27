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
	public partial class ExportSixMonthReport : Window
	{
		private int id_department;
		public ExportSixMonthReport(int id_department = 0)
		{
			InitializeComponent();
			this.dpStartMonth.SelectedDate = DateTime.Now;
			this.dpMiddleMonth.SelectedDate = DateTime.Now;
			this.dpEndMonth.SelectedDate = DateTime.Now;
			this.id_department = id_department;
		}

		private void btnGenerateSchedule_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.FileName = "Сумарен отчет.xlsx";
			if (sfd.ShowDialog() == true)
			{
				using (var logic = new SixMonthExportLogic())
				{
					try
					{
						logic.PrintDepartmentSixMonthExport(this.dpStartMonth.SelectedDate.Value, this.dpEndMonth.SelectedDate.Value, this.dpMiddleMonth.SelectedDate.Value, this.id_department, sfd.FileName );
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

		private void ExportSchedules_OnLoaded(object sender, RoutedEventArgs e)
		{
		}
	}
}
