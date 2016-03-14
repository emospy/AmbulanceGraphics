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
	public partial class AmbulanceTypes : Window
	{
		List<NM_AmbulanceTypes> lstAmbulanceTypes = new List<NM_AmbulanceTypes>();
		public AmbulanceTypes()
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
					var item = this.grGridView.SelectedItem as NM_AmbulanceTypes;

					try
					{
						logic.NM_AmbulanceTypes.Delete(item);
						logic.Save();
						this.RefreshDataSource();
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

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var AmbulanceType = this.grGridView.SelectedItem as NM_AmbulanceTypes;
				var win = new AmbulanceType(AmbulanceType.id_ambulanceType);
				win.ShowDialog();
				this.RefreshDataSource();
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var win = new AmbulanceType(0);
			win.ShowDialog();
			this.RefreshDataSource();
		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			btnEdit_Click(sender, e);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void RefreshDataSource()
		{
			using (var logic = new NomenclaturesLogic())
			{
				this.lstAmbulanceTypes = logic.NM_AmbulanceTypes.GetActive(this.chkShowInactive.IsChecked == true);
				this.grGridView.ItemsSource = lstAmbulanceTypes;
			}
		}
	}
}
