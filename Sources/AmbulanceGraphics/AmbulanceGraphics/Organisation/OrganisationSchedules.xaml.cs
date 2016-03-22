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
	public partial class OrganisationSchedules : Window
	{
		private int id_selectedDepartment;
		private CrewSchedulesLogic2 logic;
		public OrganisationSchedules()
		{
			InitializeComponent();
			this.logic = new CrewSchedulesLogic2();
			this.RefreshTree();
			this.dpMonthSchedule.SelectedDate = DateTime.Now;
			this.dpMonthSchedule.SelectedDate = DateTime.Now;

			List<ComboBoxModel> lstModels;
			this.logic.NM_ScheduleTypes.FillComboBoxModel(out lstModels);
			this.cmbScheduleType.ItemsSource = lstModels;
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
				this.LoadCrewSchedules();
			}
		}

		private void LoadCrewSchedules()
		{
			var s1 = new Stopwatch();
			s1.Start();
			DateTime date;
			int id_scheduleType = 1;
			if (this.cmbScheduleType.SelectedItem != null)
			{
				var item = this.cmbScheduleType.SelectedItem as ComboBoxModel;

				if (item.id != 0)
				{
					id_scheduleType = item.id;
				}
			}
			if (this.dpMonthSchedule.SelectedDate != null)
			{
				date = this.dpMonthSchedule.SelectedDate.Value;
			}
			else
			{
				date = DateTime.Now;
			}
			this.radTreeListViewSchedule.ItemsSource = this.logic.GetDepartmentCrewsAndSchedules(this.id_selectedDepartment, date, id_scheduleType);
			s1.Stop();
		}

		private void RefreshTree()
		{
			this.RadViewSource.Items.Clear();
			this.PopulateTreeRoot(this.RadViewSource);
			this.RadViewSource.ExpandAll();
		}
		
		private void dpMonthSchedule_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			DateTime date;
			if (this.dpMonthSchedule.SelectedDate != null)
			{
				date = this.dpMonthSchedule.SelectedDate.Value;
				int id_scheduleType = 1;
				if (this.cmbScheduleType.SelectedItem != null)
				{
					var item = this.cmbScheduleType.SelectedItem as ComboBoxModel;

					if (item.id != 0)
					{
						id_scheduleType = item.id;
					}
				}

				this.radTreeListViewSchedule.ItemsSource = this.logic.GetDepartmentCrewsAndSchedules(this.id_selectedDepartment, date, id_scheduleType);

				int dm = DateTime.DaysInMonth(date.Year, date.Month);
				switch (dm)
				{
					case 28:
						sDay28.Visibility = Visibility.Visible;
						sDay29.Visibility = Visibility.Hidden;
						sDay30.Visibility = Visibility.Hidden;
						sDay31.Visibility = Visibility.Hidden;
						break;
					case 29:
						sDay28.Visibility = Visibility.Visible;
						sDay29.Visibility = Visibility.Visible;
						sDay30.Visibility = Visibility.Hidden;
						sDay31.Visibility = Visibility.Hidden;
						break;
					case 30:
						sDay28.Visibility = Visibility.Visible;
						sDay29.Visibility = Visibility.Visible;
						sDay30.Visibility = Visibility.Visible;
						sDay31.Visibility = Visibility.Hidden;
						break;
					case 31:
						sDay28.Visibility = Visibility.Visible;
						sDay29.Visibility = Visibility.Visible;
						sDay30.Visibility = Visibility.Visible;
						sDay31.Visibility = Visibility.Visible;
						break;
				}
			}
		}

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.RadViewSource_ItemClick(sender, null);
		}
	}
}
