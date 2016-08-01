using BL.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using BL;
using BL.Models;
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for PersonalSchedulesTabItem.xaml
	/// </summary>
	public partial class PersonalSchedulesTabItem : UserControl
	{
		private int id_person;
		private CalendarRow cRow;
		private CalendarRow cRowF;
		public PersonalSchedulesTabItem()
		{
			InitializeComponent();

			this.LoadCombos();
		}

		private void pdMonth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			var vm = this.DataContext as PersonalSchedulesViewModel;
			DateTime date;
			if (this.dpMonth.SelectedDate.HasValue == false)
			{
				return;
			}
			date = this.dpMonth.SelectedDate.Value;
			date = new DateTime(date.Year, date.Month, 1);

			using (var logic = new NomenclaturesLogic())
			{
				this.cRow = logic.FillCalendarRow(date);
				this.cRowF = logic.FillCalendarRow(date.AddMonths(1));
			}

			this.id_person = vm.id_person;
			using (var logic = new SchedulesLogic())
			{
				var forecast = logic.GetPersonalSchedule(this.id_person, date.AddMonths(1), ScheduleTypes.ForecastMonthSchedule);
				this.grGridViewForecastSchedule.ItemsSource = forecast;
				var dailySchedule = logic.GetPersonalSchedule(this.id_person, date, ScheduleTypes.DailySchedule);
				this.grGridViewSchedule.ItemsSource = dailySchedule;
				var presenceForm = logic.GetPersonalSchedule(this.id_person, date, ScheduleTypes.PresenceForm);
				this.grGridViewPresenceForm.ItemsSource = presenceForm;
			}
			this.ColorGridHeadres();
		}

		private void ColorGridHeadres()
		{
			int x = 0;
			var date = this.dpMonth.SelectedDate.Value;
			var dym = DateTime.DaysInMonth(date.Year, date.Month);
			switch (dym)
			{
				case 28:
					this.cmbDay29.IsVisible = false;
					this.cmbDay30.IsVisible = false;
					this.cmbDay31.IsVisible = false;

					this.cmbpDay29.IsVisible = false;
					this.cmbpDay30.IsVisible = false;
					this.cmbpDay31.IsVisible = false;
					break;
				case 29:
					this.cmbDay29.IsVisible = true;
					this.cmbDay30.IsVisible = false;
					this.cmbDay31.IsVisible = false;

					this.cmbpDay29.IsVisible = true;
					this.cmbpDay30.IsVisible = false;
					this.cmbpDay31.IsVisible = false;
					break;
				case 30:
					this.cmbDay29.IsVisible = true;
					this.cmbDay30.IsVisible = true;
					this.cmbDay31.IsVisible = false;

					this.cmbpDay29.IsVisible = true;
					this.cmbpDay30.IsVisible = true;
					this.cmbpDay31.IsVisible = false;
					break;
				case 31:
					this.cmbDay29.IsVisible = true;
					this.cmbDay30.IsVisible = true;
					this.cmbDay31.IsVisible = true;

					this.cmbpDay29.IsVisible = true;
					this.cmbpDay30.IsVisible = true;
					this.cmbpDay31.IsVisible = true;
					break;
			}
			for (int i = 1; i <= dym; i++)
			{
				if (this.cRow[i] == false)
				{
					this.grGridViewSchedule.Columns[i + x].HeaderCellStyle = (Style)this.FindResource("ColumnHeaderStyleWeekend");
					this.grGridViewPresenceForm.Columns[i + x].HeaderCellStyle = (Style)this.FindResource("ColumnHeaderStyleWeekend");
				}
				else
				{
					this.grGridViewSchedule.Columns[i + x].HeaderCellStyle = (Style)this.FindResource("ColumnHeaderStyleWeek");
					this.grGridViewPresenceForm.Columns[i + x].HeaderCellStyle = (Style)this.FindResource("ColumnHeaderStyleWeek");
				}
			}

			date = date.AddMonths(1);
			dym = DateTime.DaysInMonth(date.Year, date.Month);
			for (int i = 1; i <= dym; i++)
			{
				if (this.cRowF[i] == false)
				{
					this.grGridViewForecastSchedule.Columns[i + x].HeaderCellStyle = (Style)this.FindResource("ColumnHeaderStyleWeekend");
				}
				else
				{
					this.grGridViewForecastSchedule.Columns[i + x].HeaderCellStyle = (Style)this.FindResource("ColumnHeaderStyleWeek");
				}
			}
			switch (dym)
			{
				case 28:
					this.cmbfDay29.IsVisible = false;
					this.cmbfDay30.IsVisible = false;
					this.cmbfDay31.IsVisible = false;
					break;
				case 29:
					this.cmbfDay29.IsVisible = true;
					this.cmbfDay30.IsVisible = false;
					this.cmbfDay31.IsVisible = false;
					break;
				case 30:
					this.cmbfDay29.IsVisible = true;
					this.cmbfDay30.IsVisible = true;
					this.cmbfDay31.IsVisible = false;
					break;
				case 31:
					this.cmbfDay29.IsVisible = true;
					this.cmbfDay30.IsVisible = true;
					this.cmbfDay31.IsVisible = true;
					break;
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var vm = this.DataContext as PersonalSchedulesViewModel;

			if (vm.id_scheduleType == ScheduleTypes.DailySchedule)
			{
				this.lblDaily.Background = new System.Windows.Media.SolidColorBrush(Colors.Yellow);
			}
			else if (vm.id_scheduleType == ScheduleTypes.ForecastMonthSchedule)
			{
				this.lblForecast.Background = new System.Windows.Media.SolidColorBrush(Colors.Yellow);
				vm.CurrentDate = vm.CurrentDate.AddMonths(-1);
			}
			else if (vm.id_scheduleType == ScheduleTypes.PresenceForm)
			{
				this.lblPresenceForm.Background = new System.Windows.Media.SolidColorBrush(Colors.Yellow);
			}

			this.dpMonth.SelectedDate = vm.CurrentDate;
		}

		private void LoadCombos()
		{
			using (var logic = new NomenclaturesLogic())
			{
				List<ComboBoxModel> lstShiftTypes;

				logic.GR_ShiftTypes.FillComboBoxModel(out lstShiftTypes);

				this.cmbfDay1.ItemsSource = lstShiftTypes;
				this.cmbfDay2.ItemsSource = lstShiftTypes;
				this.cmbfDay3.ItemsSource = lstShiftTypes;
				this.cmbfDay4.ItemsSource = lstShiftTypes;
				this.cmbfDay5.ItemsSource = lstShiftTypes;
				this.cmbfDay6.ItemsSource = lstShiftTypes;
				this.cmbfDay7.ItemsSource = lstShiftTypes;
				this.cmbfDay8.ItemsSource = lstShiftTypes;
				this.cmbfDay9.ItemsSource = lstShiftTypes;
				this.cmbfDay10.ItemsSource = lstShiftTypes;
				this.cmbfDay11.ItemsSource = lstShiftTypes;
				this.cmbfDay12.ItemsSource = lstShiftTypes;
				this.cmbfDay13.ItemsSource = lstShiftTypes;
				this.cmbfDay14.ItemsSource = lstShiftTypes;
				this.cmbfDay15.ItemsSource = lstShiftTypes;
				this.cmbfDay16.ItemsSource = lstShiftTypes;
				this.cmbfDay17.ItemsSource = lstShiftTypes;
				this.cmbfDay18.ItemsSource = lstShiftTypes;
				this.cmbfDay19.ItemsSource = lstShiftTypes;
				this.cmbfDay20.ItemsSource = lstShiftTypes;
				this.cmbfDay21.ItemsSource = lstShiftTypes;
				this.cmbfDay22.ItemsSource = lstShiftTypes;
				this.cmbfDay23.ItemsSource = lstShiftTypes;
				this.cmbfDay24.ItemsSource = lstShiftTypes;
				this.cmbfDay25.ItemsSource = lstShiftTypes;
				this.cmbfDay26.ItemsSource = lstShiftTypes;
				this.cmbfDay27.ItemsSource = lstShiftTypes;
				this.cmbfDay28.ItemsSource = lstShiftTypes;
				this.cmbfDay29.ItemsSource = lstShiftTypes;
				this.cmbfDay30.ItemsSource = lstShiftTypes;
				this.cmbfDay31.ItemsSource = lstShiftTypes;

				this.cmbDay1.ItemsSource = lstShiftTypes;
				this.cmbDay2.ItemsSource = lstShiftTypes;
				this.cmbDay3.ItemsSource = lstShiftTypes;
				this.cmbDay4.ItemsSource = lstShiftTypes;
				this.cmbDay5.ItemsSource = lstShiftTypes;
				this.cmbDay6.ItemsSource = lstShiftTypes;
				this.cmbDay7.ItemsSource = lstShiftTypes;
				this.cmbDay8.ItemsSource = lstShiftTypes;
				this.cmbDay9.ItemsSource = lstShiftTypes;
				this.cmbDay10.ItemsSource = lstShiftTypes;
				this.cmbDay11.ItemsSource = lstShiftTypes;
				this.cmbDay12.ItemsSource = lstShiftTypes;
				this.cmbDay13.ItemsSource = lstShiftTypes;
				this.cmbDay14.ItemsSource = lstShiftTypes;
				this.cmbDay15.ItemsSource = lstShiftTypes;
				this.cmbDay16.ItemsSource = lstShiftTypes;
				this.cmbDay17.ItemsSource = lstShiftTypes;
				this.cmbDay18.ItemsSource = lstShiftTypes;
				this.cmbDay19.ItemsSource = lstShiftTypes;
				this.cmbDay20.ItemsSource = lstShiftTypes;
				this.cmbDay21.ItemsSource = lstShiftTypes;
				this.cmbDay22.ItemsSource = lstShiftTypes;
				this.cmbDay23.ItemsSource = lstShiftTypes;
				this.cmbDay24.ItemsSource = lstShiftTypes;
				this.cmbDay25.ItemsSource = lstShiftTypes;
				this.cmbDay26.ItemsSource = lstShiftTypes;
				this.cmbDay27.ItemsSource = lstShiftTypes;
				this.cmbDay28.ItemsSource = lstShiftTypes;
				this.cmbDay29.ItemsSource = lstShiftTypes;
				this.cmbDay30.ItemsSource = lstShiftTypes;
				this.cmbDay31.ItemsSource = lstShiftTypes;

				this.cmbpDay1.ItemsSource = lstShiftTypes;
				this.cmbpDay2.ItemsSource = lstShiftTypes;
				this.cmbpDay3.ItemsSource = lstShiftTypes;
				this.cmbpDay4.ItemsSource = lstShiftTypes;
				this.cmbpDay5.ItemsSource = lstShiftTypes;
				this.cmbpDay6.ItemsSource = lstShiftTypes;
				this.cmbpDay7.ItemsSource = lstShiftTypes;
				this.cmbpDay8.ItemsSource = lstShiftTypes;
				this.cmbpDay9.ItemsSource = lstShiftTypes;
				this.cmbpDay10.ItemsSource = lstShiftTypes;
				this.cmbpDay11.ItemsSource = lstShiftTypes;
				this.cmbpDay12.ItemsSource = lstShiftTypes;
				this.cmbpDay13.ItemsSource = lstShiftTypes;
				this.cmbpDay14.ItemsSource = lstShiftTypes;
				this.cmbpDay15.ItemsSource = lstShiftTypes;
				this.cmbpDay16.ItemsSource = lstShiftTypes;
				this.cmbpDay17.ItemsSource = lstShiftTypes;
				this.cmbpDay18.ItemsSource = lstShiftTypes;
				this.cmbpDay19.ItemsSource = lstShiftTypes;
				this.cmbpDay20.ItemsSource = lstShiftTypes;
				this.cmbpDay21.ItemsSource = lstShiftTypes;
				this.cmbpDay22.ItemsSource = lstShiftTypes;
				this.cmbpDay23.ItemsSource = lstShiftTypes;
				this.cmbpDay24.ItemsSource = lstShiftTypes;
				this.cmbpDay25.ItemsSource = lstShiftTypes;
				this.cmbpDay26.ItemsSource = lstShiftTypes;
				this.cmbpDay27.ItemsSource = lstShiftTypes;
				this.cmbpDay28.ItemsSource = lstShiftTypes;
				this.cmbpDay29.ItemsSource = lstShiftTypes;
				this.cmbpDay30.ItemsSource = lstShiftTypes;
				this.cmbpDay31.ItemsSource = lstShiftTypes;
			}
		}

		private void BtnSave_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var logic = new SchedulesLogic())
				{
					var lstForecast = this.grGridViewForecastSchedule.ItemsSource as List<PFRow>;
					if (lstForecast != null && lstForecast.Count > 0)
					{
						logic.SavePersonalSchedule(lstForecast.First());
					}
					var lstday = this.grGridViewSchedule.ItemsSource as List<PFRow>;
					if (lstday != null && lstday.Count > 0)
					{
						logic.SavePersonalSchedule(lstday.First());
					}
					var lstpf = this.grGridViewPresenceForm.ItemsSource as List<PFRow>;
					if (lstpf != null && lstpf.Count > 0)
					{
						logic.SavePersonalSchedule(lstpf.First());
					}
				}
				this.pdMonth_SelectedDateChanged(this, null);
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

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.dpMonth.SelectedDate.HasValue == false)
			{
				return;
			}
			try
			{
				using (var logic = new SchedulesLogic())
				{
					logic.CreatePersonalScheduleForMonth(this.id_person, this.dpMonth.SelectedDate.Value.AddMonths(1), (int)ScheduleTypes.ForecastMonthSchedule);
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (this.dpMonth.SelectedDate.HasValue == false)
			{
				return;
			}
			try
			{
				using (var logic = new SchedulesLogic())
				{
					logic.CreatePersonalScheduleForMonth(this.id_person, this.dpMonth.SelectedDate.Value, (int)ScheduleTypes.PresenceForm);
					logic.CreatePersonalScheduleForMonth(this.id_person, this.dpMonth.SelectedDate.Value, (int)ScheduleTypes.DailySchedule);
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

		private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
		{
			this.pdMonth_SelectedDateChanged(sender, null);
		}
	}
}
