using BL.DB;
using BL.Logic;
using BL.Models;
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
using Telerik.Windows.Controls;
using Zora.Core.Exceptions;
using Excel = Microsoft.Office.Interop.Excel;

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for GlobalPositions.xaml
	/// </summary>
	public partial class GlobalPositions : Window
	{
		//NomenclaturesLogic logic;
		public GlobalPositions()
		{
			InitializeComponent();
		}

		private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
		{
			this.RefreshGrid();
		}
		private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
		{
			this.RefreshGrid();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{			
			this.LoadCombos();
			this.RefreshGrid();
		}

		private void LoadCombos()
		{
			using (var clogic = new ComboBoxLogic())
			{
				List<ComboBoxModel> cmbModel;
				clogic.NM_PositionTypes.FillComboBoxModel(out cmbModel, 0);
				var lstPostionTypes = cmbModel;
				this.cmbPositionTypes.ItemsSource = lstPostionTypes;
			}
		}

		private void RefreshGrid()
		{
			using (var logic = new NomenclaturesLogic())
			{
				var lstGlobalPositions = logic.GetGlobalPositions(this.chkShowInactive.IsChecked == false);
				this.grGridView.ItemsSource = lstGlobalPositions;
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var win = new GlobalPosition(0);
			win.ShowDialog();
			this.RefreshGrid();
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var globalPostion = this.grGridView.SelectedItem as HR_GlobalPositions;
				var win = new GlobalPosition(globalPostion.id_globalPosition);
				win.ShowDialog();
			}
			this.RefreshGrid();
		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			btnEdit_Click(sender, e);
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			//Excel.Worksheet xlsheet;
			//Excel.Workbook xlwkbook;
			//var excelApp = new Excel.Application(); // Initialize a new Excel reader. Must be integrated with an Excel interface object.

			//xlwkbook = excelApp.Workbooks.Add(1);
			//xlsheet = (Excel.Worksheet)xlwkbook.Sheets[1];

			//var list = new List<string>();
			//list.Add("Alpha");

			//list.Add("Bravo");

			//list.Add("Charlie");

			//list.Add("Delta");

			//list.Add("Echo");
			//var flatList = string.Join(",", list.ToArray());
			//var cell = (Excel.Range)xlsheet.Cells[2, 2];
			//cell.Validation.Delete();
			//cell.Validation.Add(
			//Excel.XlDVType.xlValidateList,
			//Excel.XlDVAlertStyle.xlValidAlertStop,
			//Excel.XlFormatConditionOperator.xlBetween,
			//flatList,
			//Type.Missing);
			//cell.Validation.IgnoreBlank = true;
			//cell.Validation.InCellDropdown = true;

			//excelApp.Visible = true;
			using (var logic = new NomenclaturesLogic())
			{
				if (this.grGridView.SelectedItem != null)
				{
					var item = this.grGridView.SelectedItem as HR_GlobalPositions;

					try
					{
						logic.HR_GlobalPositions.Delete(item);
						logic.Save();
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
