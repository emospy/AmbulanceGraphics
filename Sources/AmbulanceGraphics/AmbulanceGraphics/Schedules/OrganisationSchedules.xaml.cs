﻿using AmbulanceGraphics.Persons;
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
	public partial class OrganisationSchedules : Window
	{
		private int id_selectedDepartment;
		private CrewSchedulesLogic logic;
		private bool IsReady = false;
		public OrganisationSchedules()
		{
			InitializeComponent();
			this.logic = new CrewSchedulesLogic();
			this.RefreshTree();
			this.dpMonthТо.SelectedDate = DateTime.Now;
			this.dpMonthFrom.SelectedDate = DateTime.Now;

			List<ComboBoxModel> lstModels;
			this.logic.NM_ScheduleTypes.FillComboBoxModel(out lstModels);
			this.cmbScheduleType.ItemsSource = lstModels;
			
			this.dpMonthCurrent.SelectedDate = DateTime.Now;

			this.IsReady = true;
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
			int id_scheduleType = (int)ScheduleTypes.DailySchedule;
			
			if (this.cmbScheduleType.SelectedItem != null)
			{
				var item = this.cmbScheduleType.SelectedItem as ComboBoxModel;

				if (item.id != 0)
				{
					id_scheduleType = item.id;
				}
			}
			if (this.dpMonthFrom.SelectedDate != null)
			{
				date = this.dpMonthFrom.SelectedDate.Value;
			}
			else
			{
				date = DateTime.Now;
			}
			try
			{
				this.radTreeListViewSchedule.ItemsSource = this.logic.GetDepartmentCrewsAndSchedules(this.id_selectedDepartment, this.dpMonthFrom.SelectedDate.Value, this.dpMonthCurrent.SelectedDate.Value, this.dpMonthТо.SelectedDate.Value, id_scheduleType);
			}
			catch (ZoraException ex)
			{
				MessageBox.Show(ex.Result.ErrorCodeMessage);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			if (this.id_selectedDepartment != 0)
			{
				var ls = logic.GetLeadShiftNumber(this.dpMonthCurrent.SelectedDate.Value, this.id_selectedDepartment);
				lblLeadShift.Content = ls.ToString();
			}
			else
			{
				lblLeadShift.Content = "";
			}

			s1.Stop();
		}

		private void RefreshTree()
		{
			this.RadViewSource.Items.Clear();
			this.PopulateTreeRoot(this.RadViewSource);
			this.RadViewSource.ExpandAll();
		}

		private void RefreshSchedules(int id_scheduleType)
		{
			this.logic = new CrewSchedulesLogic();
			this.logic.RefreshPresenceFroms();
			this.radTreeListViewSchedule.ItemsSource = this.logic.GetDepartmentCrewsAndSchedules(this.id_selectedDepartment, this.dpMonthFrom.SelectedDate.Value, this.dpMonthCurrent.SelectedDate.Value, this.dpMonthТо.SelectedDate.Value, id_scheduleType);

			if (this.id_selectedDepartment != 0)
			{
				var ls = logic.GetLeadShiftNumber(this.dpMonthCurrent.SelectedDate.Value, this.id_selectedDepartment);
				lblLeadShift.Content = ls.ToString();
			}
			else
			{
				lblLeadShift.Content = "";
			}
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
				int id_scheduleType = (int)ScheduleTypes.DailySchedule;
				if (this.cmbScheduleType.SelectedItem != null)
				{
					var item = this.cmbScheduleType.SelectedItem as ComboBoxModel;

					if (item != null && item.id != 0)
					{
						id_scheduleType = item.id;
					}
				}

				this.RefreshSchedules(id_scheduleType);

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

		private void BtnGenerateSchedule_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem == null)
			{
				MessageBox.Show("Моля, изберете звено!");
				return;
			}

			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;
			this.id_selectedDepartment = tag.id_department;
			var win = new GenerateSingleSchedule(tag.id_department);
			win.ShowDialog();
			this.dpMonthSchedule_SelectedDateChanged(sender, null);
		}

		private void BtnApproveSchedule_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.dpMonthCurrent.SelectedDate.HasValue == false)
			{
				MessageBox.Show("Моля, изберете дата!");
				return;
			}
			if (this.RadViewSource.SelectedItem == null)
			{
				MessageBox.Show("Моля, изберете звено!");
				return;
			}

			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;
			this.id_selectedDepartment = tag.id_department;

			try
			{
				if (MessageBox.Show(
						"Утвърждаване на месечен график. След утвърждаването няма да може да се нанасят повече корекции по утвърдения график!",
						"Въпрос", MessageBoxButton.YesNo) == MessageBoxResult.No)
				{
					return;
				}
				using (var logic = new SchedulesLogic())
				{
					logic.ApproveForecastScheduleForDepartment(tag.id_department, this.dpMonthCurrent.SelectedDate.Value);
				}

				this.dpMonthSchedule_SelectedDateChanged(sender, null);
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

		private void BtnPrintSchedule_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.dpMonthCurrent.SelectedDate.HasValue == false)
			{
				MessageBox.Show("Моля, изберете дата!");
				return;
			}
			DateTime date = this.dpMonthCurrent.SelectedDate.Value;
			if (this.cmbScheduleType.SelectedIndex == 0)
			{
				MessageBox.Show("Моля, изберете вид график!");
				return;
			}

			if (this.RadViewSource.SelectedItem == null)
			{
				MessageBox.Show("Моля, изберете звено!");
				return;
			}

			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;
			this.id_selectedDepartment = tag.id_department;
			try
			{
				var st = (ScheduleTypes)this.cmbScheduleType.SelectedValue;
				if (st == ScheduleTypes.DailySchedule)
				{
					//Print department daily shcedule in template
					SaveFileDialog sfd = new SaveFileDialog();
					//sfd.FileName = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + " " + this.cmbScheduleType.Text;
					sfd.FileName = string.Format("{0}-{1:00}-{2:00} Сменен график", date.Year, date.Month, date.Day);
					if (sfd.ShowDialog() == true)
					{
						//using (var logic = new ExportLogic())
						//{
						//	logic.ExportDailyDepartmentSchedule(sfd.FileName, date);
						//	//System.Diagnostics.Process.Start(sfd.FileName);
						//	MessageBox.Show("Разпечатването завърши");
						//}

						using (var logic = new DailyExportLogic())
						{
							logic.ExportDailyDepartmentSchedule(sfd.FileName, date);
							MessageBox.Show("Разпечатването завърши");
						}
					}
				}
				else if (st == ScheduleTypes.SixMonthSchedule)
				{
					var win = new ExportSixMonthReport(this.id_selectedDepartment);
					win.ShowDialog();
				}
				else 
				{
					SaveFileDialog sfd = new SaveFileDialog();
					sfd.FileName = date.Year + date.Month + date.Day + " " + tag.DepartmentName + " " + this.cmbScheduleType.Text + ".xlsx";
					if (sfd.ShowDialog() == true)
					{

						using (var logic = new ForecastReportLogic())
						{
							logic.ExportSingleForecastMonthlyShedule(sfd.FileName, this.dpMonthFrom.SelectedDate.Value, this.dpMonthCurrent.SelectedDate.Value, this.dpMonthТо.SelectedDate.Value, st,
								tag.id_department);
							System.Diagnostics.Process.Start(sfd.FileName);
						}
					}
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

		private void RadTreeListViewSchedule_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (this.radTreeListViewSchedule.SelectedItem == null)
			{
				return;
			}

			var item = this.radTreeListViewSchedule.SelectedItem as CrewScheduleListViewModel;
			if (item != null && item.id_person != 0)
			{
				ScheduleTypes id_scheduleType = (this.cmbScheduleType.SelectedValue != null)? (ScheduleTypes)this.cmbScheduleType.SelectedValue: 0;
				var date = DateTime.Now;
				if (this.dpMonthCurrent.SelectedDate.HasValue)
				{
					date = this.dpMonthCurrent.SelectedDate.Value;
				}
                var win = new PersonFolder(item.id_person, date, id_scheduleType);
				win.ShowDialog();
				this.dpMonthSchedule_SelectedDateChanged(sender, null);
			}
		}

		private void BtnCopyToPresenceForm_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.dpMonthFrom.SelectedDate.HasValue == false)
			{
				MessageBox.Show("Моля, изберете дата!");
				return;
			}
			DateTime date = this.dpMonthFrom.SelectedDate.Value;
			DateTime dateTo = this.dpMonthТо.SelectedDate.Value;
			if (this.cmbScheduleType.SelectedIndex == 0)
			{
				MessageBox.Show("Моля, изберете вид график!");
				return;
			}

			if (this.RadViewSource.SelectedItem == null)
			{
				MessageBox.Show("Моля, изберете звено!");
				return;
			}

			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;
			this.id_selectedDepartment = tag.id_department;
			try
			{
				using (var logic = new SchedulesLogic())
				{
					logic.CopyScheduleToPF(this.id_selectedDepartment, date, dateTo);
				}
				this.dpMonthSchedule_SelectedDateChanged(sender, null);
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

		private void CmbScheduleType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.IsReady)
			{
				this.dpMonthSchedule_SelectedDateChanged(sender, e);
			}
		}

		private void BtnPrintMonthlySchedule_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.dpMonthCurrent.SelectedDate.HasValue == false)
			{
				MessageBox.Show("Моля, изберете дата!");
				return;
			}
			DateTime date = this.dpMonthCurrent.SelectedDate.Value;
			if (this.cmbScheduleType.SelectedIndex == 0)
			{
				MessageBox.Show("Моля, изберете вид график!");
				return;
			}

			if (this.RadViewSource.SelectedItem == null)
			{
				MessageBox.Show("Моля, изберете звено!");
				return;
			}

			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;
			this.id_selectedDepartment = tag.id_department;
			try
			{
				var st = (ScheduleTypes)this.cmbScheduleType.SelectedValue;
				if (st == ScheduleTypes.DailySchedule)
				{
					SaveFileDialog sfd = new SaveFileDialog();
					st = ScheduleTypes.DailySchedule;
					sfd.FileName = date.Year + date.Month + " " + tag.DepartmentName + " Отработен график.xlsx";
					if (sfd.ShowDialog() == true)
					{
						using (var logic = new ForecastReportLogic())
						{
							logic.ExportSingleForecastMonthlyShedule(sfd.FileName, this.dpMonthFrom.SelectedDate.Value, this.dpMonthCurrent.SelectedDate.Value, this.dpMonthТо.SelectedDate.Value, st,
								tag.id_department);
							System.Diagnostics.Process.Start(sfd.FileName);
						}
					}
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

