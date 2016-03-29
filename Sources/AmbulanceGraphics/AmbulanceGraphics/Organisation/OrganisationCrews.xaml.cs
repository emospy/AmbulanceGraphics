using AmbulanceGraphics.Persons;
using AmbulanceGraphics.Schedules;
using BL.DB;
using BL.Logic;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for OrganisationStructure.xaml
	/// </summary>
	public partial class OrganisationCrews : Window
	{
		private int id_selectedDepartment;
		private CrewSchedulesLogic logic;
		public OrganisationCrews()
		{
			InitializeComponent();
			this.logic = new CrewSchedulesLogic();
			this.RefreshTree();

			List<ComboBoxModel> lstModels;
			this.logic.NM_ScheduleTypes.FillComboBoxModel(out lstModels);
		}

		public List<StructureTreeViewModel> GetTreeNodes(bool IsRoot, int id_departmentParent, List<UN_Departments> lstAllDepartments)
		{
			if (IsRoot)
			{
				var result = lstAllDepartments.Where(a => a.id_department == a.id_departmentParent)
					.OrderBy(a => a.TreeOrder)
					.Select(a => new StructureTreeViewModel
					{
						DepartmentName = a.Name,
						id_departmentParent = (int)a.id_departmentParent,
						id_department = a.id_department,
						TreeOrder = a.TreeOrder
					}).ToList();
				return result;
			}
			else
			{
				var result = lstAllDepartments.Where(a => a.id_departmentParent == id_departmentParent && a.id_department != a.id_departmentParent)
					.OrderBy(a => a.TreeOrder)
					.Select(a => new StructureTreeViewModel
					{
						DepartmentName = a.Name,
						id_departmentParent = (int)a.id_departmentParent,
						id_department = a.id_department,
						TreeOrder = a.TreeOrder
					}).ToList();
				return result;
			}
		}

		private void PopulateTreeRoot(RadTreeView Tree)
		{
			var lstAllDepartments = logic.UN_Departments.GetActive(true);
			var rootItems = this.GetTreeNodes(true, 0, lstAllDepartments);
			foreach (var item in rootItems)
			{
				RadTreeViewItem it = new RadTreeViewItem();
				it.Tag = item;
				it.Header = item.DepartmentName;
				Tree.Items.Add(it);
				this.PopulateTreeNodes(item.id_department, it, lstAllDepartments);
			}
		}

		private void PopulateTreeNodes(int p, RadTreeViewItem parent, List<UN_Departments> lstAllDepartments)
		{
			var lstItems = this.GetTreeNodes(false, p, lstAllDepartments);
			foreach (var item in lstItems)
			{
				RadTreeViewItem it = new RadTreeViewItem();
				it.Tag = item;
				it.Header = item.DepartmentName;
				parent.Items.Add(it);
				this.PopulateTreeNodes(item.id_department, it, lstAllDepartments);
			}
		}

		private void RadViewSource_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				this.id_selectedDepartment = tag.id_department;

				this.LoadCrews();
			}
		}

		private void LoadCrews()
		{
			DateTime? date = null; ;
			if (this.dpMonth.SelectedDate != null)
			{
				date = this.dpMonth.SelectedDate.Value;
			}
			logic.RefreshCrews();
			this.radTreeListView.ItemsSource = logic.GetDepartmentCrews(this.id_selectedDepartment, date);
		}

		private void RefreshTree()
		{
			this.RadViewSource.Items.Clear();
			this.PopulateTreeRoot(this.RadViewSource);
			this.RadViewSource.ExpandAll();
		}

		private void btnEditCrew_Click(object sender, RoutedEventArgs e)
		{
			if (this.radTreeListView.SelectedItem != null && this.RadViewSource.SelectedItem != null)
			{
				var item = this.radTreeListView.SelectedItem as CrewListViewModel;
				var win = new Crew(item.id_crew, this.id_selectedDepartment);
				win.ShowDialog();
				this.LoadCrews();
			}
		}

		private void btnAddCrew_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var win = new Crew(0, this.id_selectedDepartment);
				win.ShowDialog();
				this.LoadCrews();
			}
		}

		private void radTreeListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditCrew_Click(sender, e);
		}

		private void dpMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			this.LoadCrews();
		}

		private void BtnDeleteCrew_OnClick(object sender, RoutedEventArgs e)
		{
			using (var logic = new SchedulesLogic())
			{
				if (this.RadViewSource.SelectedItem != null)
				{
					var item = this.radTreeListView.SelectedItem as CrewListViewModel;
					if (item == null)
					{
						return;
					}

					try
					{
						logic.DeleteCrew(item);
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
			this.LoadCrews();
		}
	}
}
