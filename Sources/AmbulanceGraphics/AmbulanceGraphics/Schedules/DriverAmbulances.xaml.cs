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

namespace AmbulanceGraphics.Schedules
{
	/// <summary>
	/// Interaction logic for Personnel.xaml
	/// </summary>
	public partial class DriverAmbulances : Window
	{
		public DriverAmbulances()
		{
			InitializeComponent();
		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditEmployee_Click(sender, e);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{			
			this.RefreshGrid();
		}

		private void RefreshGrid()
		{
			List<DriverAmbulancesViewModel> lstPersonnel;
			using (var plogic = new SchedulesLogic())
			{
				lstPersonnel = plogic.GetDriverAmbulances(false);
				this.grGridView.ItemsSource = lstPersonnel;
			}
		}

		private void btnEditEmployee_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as DriverAmbulancesViewModel;
				var win = new DriverAmbulance(item.id_driverAssignment, item.Name);
				win.ShowDialog();
			}
			this.RefreshGrid();
		}
	}
}
