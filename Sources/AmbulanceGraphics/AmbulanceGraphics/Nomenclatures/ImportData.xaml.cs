using BL.DB;
using BL.Logic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace AmbulanceGraphics.Nomenclatures
{
	/// <summary>
	/// Interaction logic for ImportData.xaml
	/// </summary>
	public partial class ImportData : Window
	{
		public ImportData()
		{
			InitializeComponent();
		}

		private void btnImportGlobalPositions_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new SchedulesLogic())
			{
				logic.FinishMonth(new DateTime(2016, 2, 1));
				logic.FinishMonth(new DateTime(2016, 3, 1));
			}
			//using (var logic = new NomenclaturesLogic())
			//{
			//	logic.CleanParasiteSchedules();
			//}
		}

		private void btnImportPersonsAndPositions_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new SchedulesLogic())
			{
				logic.CopyCrews2(new DateTime(2016,6, 1));
			}
		}

		private void btnImportAmbulances_Click(object sender, RoutedEventArgs e)
		{
			//using (var logic = new PersonalLogic())
			//{
			//	logic.CleanCrews();
			//}
	
		}

		private void btnImportPersonalData_Click(object sender, RoutedEventArgs e)
		{
			//using (var logic = new NomenclaturesLogic())
			//{
			//	logic.RecalculateLevels();
			//	logic.Save();
			//}
		}
	}
}
