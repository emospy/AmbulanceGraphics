using BL;
using BL.DB;
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
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for Assignment.xaml
	/// </summary>
	public partial class WorkTimeAbsence : Window
	{
		bool IsDataChanged = false;

		GR_WorkTimeAbsence absence;
		public WorkTimeAbsence(int id_contract, int id_absence = 0)
		{
			InitializeComponent();

			if (id_absence == 0)
			{
				//Init default data
				this.absence = new GR_WorkTimeAbsence();
				this.absence.id_contract = id_contract;

				this.absence.Date = DateTime.Now;
				this.absence.EndTime = DateTime.Now.TimeOfDay;
				this.absence.IsActive = true;
				this.absence.IsPresence = true;
				this.absence.PrevMonthHours = 0;
				this.absence.Reasons = "";
				this.absence.StartTime = DateTime.Now.TimeOfDay;
				this.absence.IsGPSMatch = false;
			}
			else
			{
				using (var logic = new PersonalLogic())
				{
					absence = logic.GetWorkTimeAbsenceData(id_absence);
				}
			}

			using (var logic = new ComboBoxLogic())
			{
				List<ComboBoxModel> lstAmbulances;
				logic.GR_Ambulances.FillComboBoxModel(out lstAmbulances, this.absence.id_ambulance);
				this.cmbRegNumber.ItemsSource = lstAmbulances.OrderBy(a => a.Name);
			}

			this.DataContext = absence;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			
			this.IsDataChanged = false;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (this.IsDataChanged)
			{
				using (var logic = new PersonalLogic())
				{
					if (absence.id_worktimeAbsence == 0)
					{
						logic.GR_WorkTimeAbsence.Add(absence);
					}
					else
					{
						logic.GR_WorkTimeAbsence.Update(absence);
					}

					try
					{
						logic.Save();
						this.Close();
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

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			//Check for Data change
			if(this.IsDataChanged == true)
			{
				if(MessageBox.Show("Ивършени са промени по данните за назначението. Желаете ли да се откажете от тях?", "Въпрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					this.Close();
				}
			}
			else
			{
				this.Close();
			}
		}
		
		private void DataChanged(object sender, TextChangedEventArgs e)
		{
			this.IsDataChanged = true;
		}

		private void DataChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsDataChanged = true;
		}

		private void ChkIsPresence_OnChecked(object sender, RoutedEventArgs e)
		{
			this.IsDataChanged = true;
			//this.Content = "Присъствие";
		}

		private void ChkIsPresence_OnUnchecked(object sender, RoutedEventArgs e)
		{
			this.IsDataChanged = true;
			//this.Content = "Отсъствие";
		}

		private void CDataChanged(object sender, RoutedEventArgs e)
		{
			this.IsDataChanged = true;
		}

		private void tpStartTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsDataChanged = true;
			if (this.tpStartTime.SelectedTime.HasValue == false || this.tpEndTime.SelectedTime.HasValue == false)
			{
				this.niWorkHours.IsEnabled = true;
				return;
			}
			if (this.tpStartTime.SelectedTime.Value == this.tpEndTime.SelectedTime.Value)
			{
				this.niWorkHours.IsEnabled = true;
			}
			else
			{
				this.niWorkHours.IsEnabled = false;
				TimeSpan span;
				if (this.tpStartTime.SelectedTime.Value > this.tpEndTime.SelectedTime.Value)
				{
					span = tpEndTime.SelectedTime.Value.Add(new TimeSpan(24, 0, 0)).Subtract(this.tpStartTime.SelectedTime.Value);
				}
				else
				{
					span = this.tpEndTime.SelectedTime.Value.Subtract(tpStartTime.SelectedTime.Value);
				}

				double time = span.Hours;

				if (span.Minutes >= 30)
				{
					time += 0.5;
				}

				this.niWorkHours.Value = time;
			}
		}
	}
}
