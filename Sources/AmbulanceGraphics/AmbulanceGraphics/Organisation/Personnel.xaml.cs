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

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for Personnel.xaml
	/// </summary>
	public partial class Personnel : Window
	{
		public Personnel()
		{
			InitializeComponent();
		}

		private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
		{

		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

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
				var lstDepartments = clogic.ReadAllDepartments();
				this.cmbLevel1.ItemsSource = lstDepartments;
				this.cmbLevel2.ItemsSource = lstDepartments;
				this.cmbLevel3.ItemsSource = lstDepartments;
				this.cmbLevel4.ItemsSource = lstDepartments;
			}
		}

		private void RefreshGrid()
		{
			List<PersonnelViewModel> lstPersonnel = new List<PersonnelViewModel>();
			using (var plogic = new PersonalLogic())
			{
				lstPersonnel = plogic.GetPersonnel(this.chkShowInactive.IsChecked == true);
				this.grGridView.ItemsSource = lstPersonnel;
			}
		}

		private void chkShowInactive_Click(object sender, RoutedEventArgs e)
		{
			this.RefreshGrid();
		}
	}
}
