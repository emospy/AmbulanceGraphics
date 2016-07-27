using BL;
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

namespace AmbulanceGraphics.Schedules
{
	/// <summary>
	/// Interaction logic for Crew.xaml
	/// </summary>
	public partial class Crew : Window
	{
		bool IsChanged = false;
		int id_department;
		int id_crew;
		CrewViewModel crewModel;

		public Crew(int id_crew, int id_department)
		{
			InitializeComponent();
			this.id_department = id_department;
			this.id_crew = id_crew;

			this.LoadCrew();

			using (var logic = new NomenclaturesLogic())
			{
				List<ComboBoxModel> lstCrewTypes;
				logic.NM_CrewTypes.FillComboBoxModel(out lstCrewTypes);
				this.cmbCrewType.ItemsSource = lstCrewTypes;
			}

			using (var logic = new SchedulesLogic())
			{
				var lstDriverAmbulances = logic.GetDriverAmbulances(false);
				this.cmbAmbulance.ItemsSource = lstDriverAmbulances;
				this.cmbWorkTime.ItemsSource = lstDriverAmbulances;
			}
			
			if (crewModel.IsTemporary == false)
			{
				this.LoadCombosShift();
			}
			else
			{
				this.LoadCombosDepartment();
			}
			//if (this.crewModel.id_crewType == (int)CrewTypes.Corpse)
			//{
			//	cmbCrewType_SelectionChanged(this, null);
			//}
		}

		private void LoadCrew()
		{
			using (var logic = new SchedulesLogic())
			{
				this.crewModel = logic.GetCrewViewModel(id_crew);
				if(this.crewModel == null)
				{
					this.crewModel = new CrewViewModel();
					this.crewModel.id_department = this.id_department;
					this.crewModel.IsActive = true;
					this.crewModel.DateStart = DateTime.Now;
					this.crewModel.DateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
				}
			}
			this.DataContext = this.crewModel;
		}

		private void LoadCombosShift()
		{
			using (var logic = new PersonalLogic())
			{
				var drivers = logic.GetPersonnel(false, this.id_department, (int) PositionTypes.Driver).OrderBy(a => a.Name);
				var doctors = logic.GetPersonnel(false, this.id_department, (int) PositionTypes.Doctor).OrderBy(a => a.Name);
				var medicalStaff = logic.GetPersonnel(false, this.id_department, (int) PositionTypes.MedicalStaff).OrderBy(a => a.Name);
				var other = logic.GetPersonnel(false, this.id_department).OrderBy(a => a.Name);

				var lstDrivers = new List<PersonnelViewModel>();
				lstDrivers.Add(new PersonnelViewModel {Name = " "});
				lstDrivers.AddRange(drivers);
				this.cmbMember1Name.ItemsSource = lstDrivers;

				var lstDoctors = new List<PersonnelViewModel>();
				lstDoctors.Add(new PersonnelViewModel {Name = " "});
				lstDoctors.AddRange(doctors);
				this.cmbMember2Name.ItemsSource = lstDoctors;

				var lstMedicalStaff = new List<PersonnelViewModel>();
				lstMedicalStaff.Add(new PersonnelViewModel {Name = " "});
				lstMedicalStaff.AddRange(medicalStaff);
				this.cmbMember3Name.ItemsSource = lstMedicalStaff;

				var lstOthers = new List<PersonnelViewModel>();
				lstOthers.Add(new PersonnelViewModel {Name = " "});
				lstOthers.AddRange(other);
				this.cmbMember4Name.ItemsSource = lstOthers;
			}
		}

		private void LoadCombosDepartment()
		{
			using (var logic = new PersonalLogic())
			{
				var drivers = logic.GetPersonnelForParent(this.crewModel.id_departmentParent, (int)PositionTypes.Driver).OrderBy(a => a.Name);
				var doctors = logic.GetPersonnelForParent(this.crewModel.id_departmentParent, (int)PositionTypes.Doctor).OrderBy(a => a.Name);
				var medicalStaff = logic.GetPersonnelForParent(this.crewModel.id_departmentParent, (int)PositionTypes.MedicalStaff).OrderBy(a => a.Name);
				var other = logic.GetPersonnelForParent(this.crewModel.id_departmentParent).OrderBy(a => a.Name);

				var lstDrivers = new List<PersonnelViewModel>();
				lstDrivers.Add(new PersonnelViewModel { Name = " " });
				lstDrivers.AddRange(drivers);
				this.cmbMember1Name.ItemsSource = lstDrivers;

				var lstDoctors = new List<PersonnelViewModel>();
				lstDoctors.Add(new PersonnelViewModel { Name = " " });
				lstDoctors.AddRange(doctors);
				this.cmbMember2Name.ItemsSource = lstDoctors;

				var lstMedicalStaff = new List<PersonnelViewModel>();
				lstMedicalStaff.Add(new PersonnelViewModel { Name = " " });
				lstMedicalStaff.AddRange(medicalStaff);
				this.cmbMember3Name.ItemsSource = lstMedicalStaff;

				var lstOthers = new List<PersonnelViewModel>();
				lstOthers.Add(new PersonnelViewModel { Name = " " });
				lstOthers.AddRange(other);
				this.cmbMember4Name.ItemsSource = lstOthers;
			}
		}

		private void DataChanged(object sender, TextChangedEventArgs e)
		{
			this.IsChanged = true;
		}

		private void DataChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsChanged = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.IsChanged = false;
		}

		private void cmbMember1Name_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsChanged = true;
			if(this.cmbMember1Name.SelectedIndex == 0)
			{
				this.txtMember1Position.Text = "";
			}
			else
			{
				var item = this.cmbMember1Name.SelectedItem as PersonnelViewModel;
				if (item != null)
				{
					this.txtMember1Position.Text = item.Position;

					var ambItem = this.cmbAmbulance.SelectedItem as DriverAmbulancesViewModel;
				}
			}
		}

		private void cmbMember2Name_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsChanged = true;
			if (this.cmbMember2Name.SelectedIndex == 0)
			{
				this.txtMember2Position.Text = "";
			}
			else
			{
				var item = this.cmbMember2Name.SelectedItem as PersonnelViewModel;
				if (item == null)
				{
					return;
				}
				this.txtMember2Position.Text = item.Position;
			}
		}

		private void cmbMember3Name_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsChanged = true;
			if (this.cmbMember3Name.SelectedIndex == 0)
			{
				this.txtMember3Position.Text = "";
			}
			else
			{
				var item = this.cmbMember3Name.SelectedItem as PersonnelViewModel;
				if (item == null)
				{
					return;
				}
				this.txtMember3Position.Text = item.Position;
			}
		}

		private void cmbMember4Name_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsChanged = true;
			if (this.cmbMember4Name.SelectedIndex == 0)
			{
				this.txtMember4Position.Text = "";
			}
			else
			{
				var item = this.cmbMember4Name.SelectedItem as PersonnelViewModel;
				if (item == null)
				{
					return;
				}
				this.txtMember4Position.Text = item.Position;
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (this.IsChanged)
			{
				using (var logic = new SchedulesLogic())
				{
					try
					{
						if (this.crewModel.IsTemporary == false)
						{
							List<string> lstMessages = logic.CheckReassignPersonalCrew(this.crewModel);
							if (lstMessages.Count > 0)
							{
								string message = "Лицата ";
								foreach (var mes in lstMessages)
								{
									message += " " + mes + "\n";
								}
								message += "\n" + "ще бъдат преместени в този екип. Желаете ли да продължите?";

								//if (MessageBox.Show(message, "Въпрос", MessageBoxButton.YesNo) == MessageBoxResult.No)
								//{
								//	return;
								//}
							}
						}
						if (this.crewModel.id_crew == 0)
						{
							logic.AddCrew(this.crewModel);
						}
						else
						{
							logic.UpdateCrew(this.crewModel);
						}
						this.IsChanged = false;
						MessageBox.Show("Данните са запазени");
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
			if(this.IsChanged)
			{
				if(MessageBox.Show("Извършени са промени в данните. Искате ли да се откажете от промените?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					this.Close();
				}
			}
			else
			{
				this.Close();
			}
		}

		private void chkIsTemporary_Checked(object sender, RoutedEventArgs e)
		{
			this.IsChanged = true;
			this.LoadCombosDepartment();
		}

		private void chkIsTemporary_Unchecked(object sender, RoutedEventArgs e)
		{
			this.IsChanged = true;
			this.LoadCombosShift();
		}

		private void cmbCrewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsChanged = true;
			if (this.crewModel.id_crewType != 0)
			{
				using(var logic = new PersonalLogic())
				{
					var other = new List<PersonnelViewModel>();
     //               if ((int)this.crewModel.id_crewType == (int) CrewTypes.Corpse)
					//{
					//	other = logic.GetSanitars().OrderBy(a => a.Name).ToList();
					//}
					//else
					//{
						if (this.chkIsTemporary.IsChecked.Value)
						{
							other = logic.GetPersonnelForParent(this.crewModel.id_departmentParent).OrderBy(a => a.Name).ToList();
						}
						else
						{
							other = logic.GetPersonnel(false, this.id_department).OrderBy(a => a.Name).ToList();
						}
					//}

					var lstOthers = new List<PersonnelViewModel>();
					lstOthers.Add(new PersonnelViewModel {Name = " "});
					lstOthers.AddRange(other);
					this.cmbMember4Name.ItemsSource = lstOthers;
				}
			}
		}
	}
}
