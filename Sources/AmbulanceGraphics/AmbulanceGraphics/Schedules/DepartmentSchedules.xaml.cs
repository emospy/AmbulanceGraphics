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
using AmbulanceGraphics.Excel;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using BL;
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Schedules
{
	/// <summary>
	/// Interaction logic for OrganisationStructure.xaml
	/// </summary>
	public partial class DepartmentSchedules : Window
	{
		private int id_selectedDepartment;
		private SchedulesLogic logic;
		private bool IsReady = false;
		public DepartmentSchedules()
		{
			InitializeComponent();
			this.logic = new SchedulesLogic();
			this.RefreshTree();

			this.dpMonthCurrent.SelectedDate = DateTime.Now;

			this.IsReady = true;

            List<ComboBoxModel> lstShiftTypes;

            logic.GR_ShiftTypes.FillComboBoxModel(out lstShiftTypes);
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
			    if (tag.id_department != tag.id_departmentParent)
			    {
			        this.LoadSchedules();
			    }
			    else
			    {
			        this.radTreeListViewSchedule.ItemsSource = null;
			    }
			}
		}

		private void LoadSchedules()
		{
			var s1 = new Stopwatch();
			s1.Start();
			DateTime date;
			
			try
			{
                var model = this.logic.GetDepartmentSchedules(this.id_selectedDepartment, this.dpMonthCurrent.SelectedDate.Value);

			    this.radTreeListViewSchedule.ItemsSource = model;
			}
			catch (ZoraException ex)
			{
				MessageBox.Show(ex.Result.ErrorCodeMessage);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			s1.Stop();
		}

		private void RefreshTree()
		{
			this.RadViewSource.Items.Clear();
			this.PopulateTreeRoot(this.RadViewSource);
			this.RadViewSource.ExpandAll();
		}

		private void RefreshSchedules()
		{
			this.logic = new SchedulesLogic();
			
			this.radTreeListViewSchedule.ItemsSource = this.logic.GetDepartmentSchedules(this.id_selectedDepartment, this.dpMonthCurrent.SelectedDate.Value);
		}

		private void dpMonthSchedule_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.IsReady == false)
			{
				return;
			}
			DateTime date;
			if (this.dpMonthCurrent.SelectedDate != null)
			{
				date = this.dpMonthCurrent.SelectedDate.Value;

				this.RefreshSchedules();

				int dm = DateTime.DaysInMonth(date.Year, date.Month);
				switch (dm)
				{
					case 28:
						cmbDay28.Visibility = Visibility.Visible;
						cmbDay29.Visibility = Visibility.Hidden;
						cmbDay30.Visibility = Visibility.Hidden;
						cmbDay31.Visibility = Visibility.Hidden;
						break;
					case 29:
						cmbDay28.Visibility = Visibility.Visible;
						cmbDay29.Visibility = Visibility.Visible;
						cmbDay30.Visibility = Visibility.Hidden;
						cmbDay31.Visibility = Visibility.Hidden;
						break;
					case 30:
						cmbDay28.Visibility = Visibility.Visible;
						cmbDay29.Visibility = Visibility.Visible;
						cmbDay30.Visibility = Visibility.Visible;
						cmbDay31.Visibility = Visibility.Hidden;
						break;
					case 31:
						cmbDay28.Visibility = Visibility.Visible;
						cmbDay29.Visibility = Visibility.Visible;
						cmbDay30.Visibility = Visibility.Visible;
						cmbDay31.Visibility = Visibility.Visible;
						break;
				}
				this.ColorGridHeadres(date);
			}
		}

		private void ColorGridHeadres(DateTime dm)
		{
			CalendarRow cRow;
			using (var logic = new NomenclaturesLogic())
			{
				cRow = logic.FillCalendarRow(dm);

			}
			int x = 6;
			for (int i = 1; i <= DateTime.DaysInMonth(dm.Year, dm.Month); i++)
			{
				if (cRow[i] == false)
				{
					this.radTreeListViewSchedule.Columns[i + x].HeaderStyle = (Style)this.FindResource("ColumnHeaderStyleWeekend");
				}
				else
				{
					this.radTreeListViewSchedule.Columns[i + x].HeaderStyle = (Style)this.FindResource("ColumnHeaderStyleWeek");
				}
			}
		}

		private void BtnPrintMonthlySchedule_OnClick(object sender, RoutedEventArgs e)
		{
            SaveFileDialog sfd = new SaveFileDialog();
            
            sfd.FileName = "Годишна схема на дежурствата.xlsx";
		    if (sfd.ShowDialog() == true)
		    {
		        using (var lo = new SchemeExportLogic())
		        {
		            lo.ExportYearScheme(sfd.FileName, this.dpMonthCurrent.SelectedDate.Value);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
		    }
		}
	}
}