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
	/// Interaction logic for YearWorkdays.xaml
	/// </summary>
	public partial class YearWorkdays : Window
	{			
		DateTime CurrentDate;

		public YearWorkdays()
		{		
			InitializeComponent();
		}

		void InitDataGrid()
		{
			var lstCalRow = new List<CalendarRow>();
			if (this.dpCurrentDate.SelectedDate != null)
			{
				using (var logic = new NomenclaturesLogic())
				{
					var row = logic.FillCalendarRow(this.dpCurrentDate.SelectedDate.Value);
					row.Description = "Работни дни";
					var row2 = logic.FillCalendarRowNH(this.dpCurrentDate.SelectedDate.Value);
					lstCalRow.Add(row);
					row2.Description = "Национални празници";
					lstCalRow.Add(row2);

					int dmon = DateTime.DaysInMonth(this.dpCurrentDate.SelectedDate.Value.Year, this.dpCurrentDate.SelectedDate.Value.Month);
					switch (dmon)
					{
						case 28:
							dgcmb29.Visibility = Visibility.Hidden;
							dgcmb30.Visibility = Visibility.Hidden;
							dgcmb31.Visibility = Visibility.Hidden;
							break;
						case 29:
							dgcmb29.Visibility = Visibility.Visible;
							dgcmb30.Visibility = Visibility.Hidden;
							dgcmb31.Visibility = Visibility.Hidden;
							break;
						case 30:
							dgcmb29.Visibility = Visibility.Visible;
							dgcmb30.Visibility = Visibility.Visible;
							dgcmb31.Visibility = Visibility.Hidden;
							break;
						case 31:
							dgcmb29.Visibility = Visibility.Visible;
							dgcmb30.Visibility = Visibility.Visible;
							dgcmb31.Visibility = Visibility.Visible;
							break;
					}
					this.dgWorkDays.ItemsSource = lstCalRow;
					
				}
				this.ColorGridHeadres();
			}
			else
			{
				return;
			}
		}

		private void ColorGridHeadres()
		{
			int x = this.dgcmb1.DisplayIndex;
			for (int i = 0; i < DateTime.DaysInMonth(this.CurrentDate.Year, this.CurrentDate.Month); i++)
			{
				DateTime tmpDate = new DateTime(this.CurrentDate.Year, this.CurrentDate.Month, i + 1);
				if (tmpDate.DayOfWeek == DayOfWeek.Sunday || tmpDate.DayOfWeek == DayOfWeek.Saturday)
				{
					this.dgWorkDays.Columns[i + x].HeaderStyle = (Style)this.FindResource("ColumnHeaderStyleWeekend");
				}
				else
				{
					this.dgWorkDays.Columns[i + x].HeaderStyle = (Style)this.FindResource("ColumnHeaderStyleWeek");
				}
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var logic = new NomenclaturesLogic())
				{
					CalendarRow cal = ((List<CalendarRow>)this.dgWorkDays.ItemsSource).First();
					CalendarRow calNH = ((List<CalendarRow>)this.dgWorkDays.ItemsSource).Last();
					logic.SaveCalendarRow(cal, this.dpCurrentDate.SelectedDate.Value);
					logic.SaveCalendarRowNH(calNH, this.dpCurrentDate.SelectedDate.Value);
				}
			}
			catch (Zora.Core.Exceptions.ZoraException ex)
			{
				MessageBox.Show(ex.Result.ErrorCodeMessage);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void dtpCurrentDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((this.dpCurrentDate.SelectedDate.Value.Month != this.CurrentDate.Month) || (this.dpCurrentDate.SelectedDate.Value.Year != this.CurrentDate.Year))
			{
				this.CurrentDate = this.dpCurrentDate.SelectedDate.Value;
				this.InitDataGrid();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.CurrentDate = DateTime.Now;
			this.dpCurrentDate.SelectedDate = DateTime.Now;

			InitDataGrid();
		}

		private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
		{
			this.InitDataGrid();
		}
	}	
}
