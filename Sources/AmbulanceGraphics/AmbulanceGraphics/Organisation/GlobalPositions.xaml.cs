using BL.DB;
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
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for GlobalPositions.xaml
	/// </summary>
	public partial class GlobalPositions : Window
	{
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
			var logic = new ComboBoxLogic();
			var lstPostionTypes = logic.ReadPositionTypes();
			this.cmbPositionTypes.ItemsSource = lstPostionTypes;
		}

		private void RefreshGrid()
		{
			var logic = new NomenclaturesLogic();
			var lstGlobalPositions = logic.GetGlobalPositions(this.chkShowInactive.IsChecked == false);
			this.grGridView.ItemsSource = lstGlobalPositions;
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

		}
	}
}
