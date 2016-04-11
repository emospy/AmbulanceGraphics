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
	public partial class AbsenceTabItem : UserControl
	{
		int id_person;
		int id_contract;
		public AbsenceTabItem()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var parent = ((((this.Parent as TabItem).Parent as TabControl).Parent as Grid).Parent as PersonFolder);
			this.id_person = parent.gPVM.PersonViewModel.id_person;

			if(this.id_person == 0)
			{
				this.id_contract = 0;
				return;
			}

			var lstContracts = parent.gPVM.lstContracts.Where(b => b.IsFired == false).ToList();

			if (lstContracts!= null && lstContracts.Count > 0)
			{
				id_contract = parent.gPVM.lstContracts.FirstOrDefault(b => b.IsFired == false).id_contract;
			}
			else
			{
				this.id_contract = 0;
			}

			this.radGridView.ItemsSource = parent.gPVM.lstAbsences;
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

				//this.DataContext = gpvm.lstContracts;
				this.radGridView.ItemsSource = gpvm.lstAbsences;
				this.radGridView.Items.Refresh();
			}
		}

		
		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditAbsence_Click(sender, e);
			this.RefreshGrid(this.id_person);
		}

		private void btnAddAbsence_Click(object sender, RoutedEventArgs e)
		{
			if (this.id_contract != 0)
			{
				Absence win = new Absence(this.id_contract);
				win.ShowDialog();
				this.RefreshGrid(this.id_person);
			}
		}

		private void btnEditAbsence_Click(object sender, RoutedEventArgs e)
		{
			if (this.radGridView.SelectedItem != null)
			{
				var absence = radGridView.SelectedItem as AbsenceListViewModel;
				Absence win = new Absence(this.id_contract, absence.id_absence);
				win.ShowDialog();
				this.RefreshGrid(this.id_person);
			}
		}

		private void btnDeleteAbsence_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
