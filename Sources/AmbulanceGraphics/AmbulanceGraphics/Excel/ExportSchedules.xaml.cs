﻿using System;
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
			//var item = this.cmbScheduleType.SelectedItem as ComboBoxModel;
			//if (item.id == 0)
			//{
			//	return;
			//}
   //         if (item.id == (int)ScheduleTypes.ForecastMonthSchedule)
			//{
			//	SaveFileDialog sfd = new SaveFileDialog();
			//	sfd.FileName = date.Year + date.Month + date.Day + this.cmbScheduleType.Text + ".xlsx";
			//	if (sfd.ShowDialog() == true)
			//	{
			//		using ( var logic = new ForecastReportLogic())
			//		{
			//			try
			//			{
			//				//logic.ExportMonthlySchedule(sfd.FileName, "Месечен график", dateBegin , ScheduleTypes.ForecastMonthSchedule);
			//				//MessageBox.Show("Експортирането завърши успешно");
			//				//System.Diagnostics.Process.Start(sfd.FileName);
			//				MessageBox.Show("Тази функция е временн изключена.");
			//			}
			//			catch (ZoraException ex)
			//			{
			//				MessageBox.Show(ex.Result.ErrorCodeMessage);
			//			}
			//			catch (Exception ex)
			//			{
			//				MessageBox.Show(ex.Message);
			//			}
			//		}
			//	}
			//}
			//if (item.id == (int)ScheduleTypes.MonthReport)
			//{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.FileName = date.Year + date.Month + date.Day + this.cmbScheduleType.Text + ".xlsx";
				if (sfd.ShowDialog() == true)
				{
					using (var logic = new FinalReportLogic())
					{
						try
						{
							logic.ExportMonthlyForecastReport(sfd.FileName, date);
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
			//}
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
