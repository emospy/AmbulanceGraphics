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
	public partial class ExportTwelveReport : Window
	{
		public ExportTwelveReport()
		{
			InitializeComponent();
			this.dpStartMonth.SelectedDate = DateTime.Now;
			this.dpEndMonth.SelectedDate = DateTime.Now;
		}

		private void btnGenerateSchedule_Click(object sender, RoutedEventArgs e)
		{
		    
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.FileName = "Отчет 12 часови дежурства.xlsx";
			if (sfd.ShowDialog() == true)
			{
				using (var logic = new TwelveExportLogic())
				{
					try
					{
						logic.GenerateTwelveReport(sfd.FileName, this.dpStartMonth.SelectedDate.Value, this.dpEndMonth.SelectedDate.Value);
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
