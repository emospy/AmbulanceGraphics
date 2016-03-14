using BL.Logic;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for PersonAndAssignment.xaml
	/// </summary>
	public partial class AssignmentsTabItem : UserControl
	{
		int id_person;
		public AssignmentsTabItem()
		{
			InitializeComponent();
		}		

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.radTreeListView.ItemsSource = this.DataContext;
			var parent = ((((this.Parent as TabItem).Parent as TabControl).Parent as Grid).Parent as PersonFolder);
			this.id_person = parent.gPVM.PersonViewModel.id_person;
		}

		private void btnAddContract_Click(object sender, RoutedEventArgs e)
		{
			
			Assignment win = new Assignment(0, 0, this.id_person);
			win.ShowDialog();

			this.RefreshGrid(this.id_person);
		}

		private void RefreshGrid(int id_person)
		{
			using (var logic = new PersonalLogic())
			{
				var gpvm = logic.InitGPVM(id_person);

				this.DataContext = gpvm.lstContracts;
				this.radTreeListView.ItemsSource = gpvm.lstContracts;
				this.radTreeListView.Items.Refresh();
			}
		}

		private void btnAddAssignment_Click(object sender, RoutedEventArgs e)
		{
			if (this.radTreeListView.SelectedItem != null)
			{
				var contract = radTreeListView.SelectedItem as ContractsViewModel;
				Assignment win = new Assignment(contract.id_contract, 0, contract.id_person);
				win.ShowDialog();
				this.RefreshGrid(this.id_person);
			}
		}

		private void btnEditAssignment_Click(object sender, RoutedEventArgs e)
		{
			if (this.radTreeListView.SelectedItem != null)
			{
				var contract = radTreeListView.SelectedItem as ContractsViewModel;
				Assignment win = new Assignment(contract.id_contract, contract.id_assignment, contract.id_person);
				win.ShowDialog();
				this.RefreshGrid(this.id_person);
			}
		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditAssignment_Click(sender, e);
			this.RefreshGrid(this.id_person);
		}

		//private void Page_Loaded(object sender, RoutedEventArgs e)
		//{

		//}
	}
}
