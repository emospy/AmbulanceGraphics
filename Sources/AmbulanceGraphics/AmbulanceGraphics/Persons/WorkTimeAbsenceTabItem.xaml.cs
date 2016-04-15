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
	public partial class WorkTimeAbsenceTabItem : UserControl
	{
		private int? id_contract;
		public WorkTimeAbsenceTabItem()
		{
			InitializeComponent();
			
			
		}		

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var parent = ((((this.Parent as TabItem)?.Parent as TabControl)?.Parent as Grid)?.Parent as PersonFolder);
			this.id_contract = parent?.gPVM.lstContracts?.FirstOrDefault(b => b.IsFired == false)?.id_contract;
			this.dpCurrentDate.SelectedDate = DateTime.Now.Date;
		}

		private void RefreshGrid()
		{
			if (this.id_contract == null)
			{
				return;
			}
			using (var logic = new PersonalLogic())
			{
				this.dgWorktimeAbsence.ItemsSource = logic.GetWorkTimeAbsenceListViewModel((int)this.id_contract, this.dpCurrentDate.SelectedDate.Value);
				this.dgWorktimeAbsence.Items.Refresh();
			}
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (this.dgWorktimeAbsence.SelectedItem != null && this.id_contract != null)
			{
				var absence = this.dgWorktimeAbsence.SelectedItem as WorkTimeAbsenceListViewModel;
				if (absence == null)
				{
					return;
				}
				var win = new WorkTimeAbsence((int)this.id_contract, absence.id_worktimeAbsence);
				win.ShowDialog();
				this.RefreshGrid();
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (this.id_contract != null && this.id_contract != 0)
			{
				WorkTimeAbsence win = new WorkTimeAbsence((int)id_contract, 0);
				win.ShowDialog();

				this.RefreshGrid();
			}
			else
			{
				MessageBox.Show("Моля, първо запазете данни за назначение");
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{

		}

		private void dpCurrentDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.id_contract != null && this.id_contract != 0)
			{
				using (var logic = new PersonalLogic())
				{
					var lst = logic.GetWorkTimeAbsenceListViewModel((int) id_contract, this.dpCurrentDate.SelectedDate.Value).ToList();
					this.dgWorktimeAbsence.ItemsSource = lst;
				}
			}
		}

		private void DgWorktimeAbsence_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEdit_Click(sender, e);
			this.RefreshGrid();
		}
	}
}
