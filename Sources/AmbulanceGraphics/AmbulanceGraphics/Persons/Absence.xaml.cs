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

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for Assignment.xaml
	/// </summary>
	public partial class Absence : Window
	{
		bool IsDataChanged = false;

		HR_Absence absence;
		public Absence(int id_contract, int id_absence = 0)
		{
			InitializeComponent();

			if (id_absence == 0)
			{
				//Init default data
				this.absence = new HR_Absence();
				this.absence.id_contract = id_contract;

				this.absence.StartDate = DateTime.Now;
				this.absence.EndDate = DateTime.Now;
				this.absence.OrderDate = DateTime.Now;
				this.absence.SicknessIssueDate = DateTime.Now;				
			}
			else
			{
				using (var logic = new PersonalLogic())
				{
					absence = logic.GetAbsenceData(id_absence);
				}
			}
			
			this.DataContext = absence;
			this.LoadCombos();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			
			this.IsDataChanged = false;
		}

		private void LoadCombos()
		{
			using (var logic = new ComboBoxLogic())
			{
				List<ComboBoxModel> lstCombo;

				logic.NM_AbsenceTypes.FillComboBoxModel(out lstCombo, this.absence.id_absenceType);
				this.cmbAbsenceType.ItemsSource = lstCombo;

				List<ComboBoxModel> lstSicknessTypes = new List<ComboBoxModel>();

				var c1 = new ComboBoxModel() { id = 1, Name = "Първичен", IsActive = true };
				var c2 = new ComboBoxModel() { id = 2, Name = "Продължение", IsActive = true };

				lstSicknessTypes.Add(c1);
				lstSicknessTypes.Add(c2);

				this.cmbSicknessType.ItemsSource = lstSicknessTypes;

				this.cmbYear.ItemsSource = logic.GetPersonalYearHolidays(this.absence.id_contract);
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (this.IsDataChanged)
			{
				using (var logic = new PersonalLogic())
				{
					try
					{
						logic.HandleAbsenceSave(absence);
						this.IsDataChanged = false;
						MessageBox.Show("Данните са записани успешно");
						this.Close();
						//handle add new contract
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

		private void cmbAbsenceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(this.absence.id_absenceType == (int)AbsenceTypes.Sickness)
			{
				this.dpSicknessIssueDate.IsEnabled = true;
				this.cmbSicknessType.IsEnabled = true;
				this.txtSicknessAdditionalDocs.IsEnabled = true;
				this.txtSicknessAttachment7.IsEnabled = true;
				this.txtSicknessDeclaration39.IsEnabled = true;
				this.txtSicknessMKB.IsEnabled = true;
				this.txtSicknessNapDocs.IsEnabled = true;
				this.txtSicknessNumber.IsEnabled = true;
				this.txtSicknessReason.IsEnabled = true;
			}
			else
			{
				this.dpSicknessIssueDate.IsEnabled = false;
				this.cmbSicknessType.IsEnabled = false;
				this.txtSicknessAdditionalDocs.IsEnabled = false;
				this.txtSicknessAttachment7.IsEnabled = false;
				this.txtSicknessDeclaration39.IsEnabled = false;
				this.txtSicknessMKB.IsEnabled = false;
				this.txtSicknessNapDocs.IsEnabled = false;
				this.txtSicknessNumber.IsEnabled = false;
				this.txtSicknessReason.IsEnabled = false;
			}
		}
	}
}
