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
	public partial class OrganisationStructure : Window
	{
		private int id_selectedDepartment;
		public OrganisationStructure()
		{
			InitializeComponent();
			this.RefreshTree();
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
			using (var logic = new NomenclaturesLogic())
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
				var tabitem = this.tabControl.SelectedItem as TabItem;
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				this.id_selectedDepartment = tag.id_department;
				Stopwatch s1 = new Stopwatch();
				Stopwatch s2 = new Stopwatch();
				s1.Start();
				switch (tabitem.Header.ToString())
				{
					case "Длъжности":
						this.LoadPositions();
						break;
					case "Екипи":
						s2.Start();
						this.LoadCrews();
						s2.Stop();
						break;
					case "Персонал":
						this.LoadEmployees();
						break;
					case "Месечен график":
						this.LoadCrewSchedules();
						break;
				}
				s1.Stop();
			}
		}

		private void LoadCrews()
		{
			using (SchedulesLogic logic = new SchedulesLogic())
			{
				Stopwatch s1 = new Stopwatch();
				s1.Start();
				DateTime? date = null; ;
				if (this.dpMonth.SelectedDate != null)
				{
					date = this.dpMonth.SelectedDate.Value;
				}
				this.radTreeListView.ItemsSource = logic.GetDepartmentCrews(this.id_selectedDepartment, date);
				s1.Stop();
			}
		}

		private void LoadCrewSchedules()
		{
			using (SchedulesLogic logic = new SchedulesLogic())
			{
				Stopwatch s1 = new Stopwatch();
				s1.Start();
				DateTime? date = null; ;
				if (this.dpMonthSchedule.SelectedDate != null)
				{
					date = this.dpMonthSchedule.SelectedDate.Value;
				}
				this.radTreeListViewSchedule.ItemsSource = logic.GetDepartmentCrewsAndSchedules(this.id_selectedDepartment, date);
				s1.Stop();
			}
		}

		private void LoadEmployees()
		{
			using (PersonalLogic logic = new PersonalLogic())
			{
				this.grGridViewEmployees.ItemsSource = logic.GetPersonnel(false, this.id_selectedDepartment);
			}
		}

		private void MenuItemUp_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;

				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveTreeNodeUp(tag.id_department))
						{
							this.RefreshTree();
						}
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

		private void MenuItemDown_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveTreeNodeDown(tag.id_department))
						{
							this.RefreshTree();
						}
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

		private void btnAddRoot_Click(object sender, RoutedEventArgs e)
		{
			var win = new Department(0);
			win.ShowDialog();
			this.RefreshTree();
		}

		private void RefreshTree()
		{
			this.RadViewSource.Items.Clear();
			this.PopulateTreeRoot(this.RadViewSource);
			this.RadViewSource.ExpandAll();
		}

		private void btnAddChild_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				var win = new Department(0, tag.id_department);
				win.ShowDialog();
				this.RefreshTree();
			}
			else
			{
				MessageBox.Show("Трябва да изберете звено, за да добавите подзвено.");
			}
		}

		private void btnEditNode_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				var win = new Department(tag.id_department, tag.id_departmentParent);
				win.ShowDialog();
				this.RefreshTree();
			}
		}

		private void btnDeleteNode_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						logic.DeleteDepartment(tag.id_department);
						this.RefreshTree();
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

		private void btnAddPosition_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var win = new StructurePosition(0, this.id_selectedDepartment);
				win.ShowDialog();
				this.LoadPositions();
			}
			else
			{
				MessageBox.Show("Моля, изберете звено от дървовидната структура на организацията.");
			}
		}

		private void LoadPositions()
		{
			using (var logic = new NomenclaturesLogic())
			{
				this.grGridView.ItemsSource = logic.GetStructurePositions(this.id_selectedDepartment, this.chkShowInactive.IsChecked != true);
			}
		}

		private void btnEditPosition_Click(object sender, RoutedEventArgs e)
		{
			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;

			if (this.grGridView.SelectedItem != null)
			{
				var struc = this.grGridView.SelectedItem as StructurePositionViewModel;
				var win = new StructurePosition(struc.id_structurePosition, this.id_selectedDepartment);
				win.ShowDialog();
				this.LoadPositions();
			}
			else
			{
				MessageBox.Show("Моля, изберете длъжност, която да редактирате.");
			}
		}

		private void btnDeletePosition_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as StructurePositionViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						var it = logic.HR_StructurePositions.GetById(item.id_structurePosition);
						logic.HR_StructurePositions.Delete(it);
						logic.Save();
						this.LoadPositions();
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

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditPosition_Click(sender, e);
		}

		private void GridMenuItemUp_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as StructurePositionViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveStructurePositionUp(item.id_structurePosition) == true)
						{
							this.LoadPositions();
						}
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

		private void GridMenuItemDown_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as StructurePositionViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveStructurePositionDown(item.id_structurePosition) == true)
						{
							this.LoadPositions();
						}
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

		private void btnAddPerson_Click(object sender, RoutedEventArgs e)
		{
			var win = new PersonFolder(0);
			win.ShowDialog();
		}

		private void btnEditPerson_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridViewEmployees.SelectedItem != null)
			{
				var item = this.grGridViewEmployees.SelectedItem as PersonnelViewModel;
				var win = new PersonFolder(item.id_person);
				win.ShowDialog();
			}
		}

		private void btnEditCrew_Click(object sender, RoutedEventArgs e)
		{
			if (this.radTreeListView.SelectedItem != null && this.RadViewSource.SelectedItem != null)
			{
				var item = this.radTreeListView.SelectedItem as CrewListViewModel;
				var win = new Crew(item.id_crew, this.id_selectedDepartment);
				win.ShowDialog();
			}
		}

		private void btnAddCrew_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var win = new Crew(0, this.id_selectedDepartment);
				win.ShowDialog();
			}
		}

		private void radTreeListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditCrew_Click(sender, e);
		}

		private void grGridViewEmployees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditPerson_Click(sender, e);
		}

		private void dpMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void radTreeListView_SelectionChanged(object sender, SelectionChangeEventArgs e)
		{

		}

		private void dpMonthSchedule_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.RadViewSource_ItemClick(sender, null);
		}
	}
}
