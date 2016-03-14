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
using Zora.Core.Exceptions;

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for Ambulances.xaml
	/// </summary>
	public partial class Ambulances : Window
	{
		List<GR_Ambulances> lstAmbulances = new List<GR_Ambulances>();
		public Ambulances()
		{
			InitializeComponent();
		}

		private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (this.grGridView.SelectedItem != null)
				{
					var item = this.grGridView.SelectedItem as GR_Ambulances;

					try
					{
						logic.GR_Ambulances.Delete(item);
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
			this.RefreshDataSource();
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var Ambulance = this.grGridView.SelectedItem as GR_Ambulances;
				var win = new Ambulance(Ambulance.id_ambulance);
				win.ShowDialog();
				this.RefreshDataSource();
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var win = new Ambulance(0);
			win.ShowDialog();
			this.RefreshDataSource();
		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			btnEdit_Click(sender, e);
			this.RefreshDataSource();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void RefreshDataSource()
		{
			using (var logic = new NomenclaturesLogic())
			{
				this.lstAmbulances = logic.GR_Ambulances.GetActive(this.chkShowInactive.IsChecked == false);
				this.grGridView.ItemsSource = lstAmbulances;
			}
		}
	}
}
