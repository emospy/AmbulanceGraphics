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
using BL;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for Assignment.xaml
	/// </summary>O
	public partial class Assignment : Window
	{
		bool IsDataChanged = false;
		int id_contract;
		int id_assignment;
		int id_person;

		AssignmentViewModel model;
		public Assignment(int id_contract, int id_assignment, int id_person)
		{
			InitializeComponent();

			this.id_contract = id_contract;
			this.id_assignment = id_assignment;
			this.id_person = id_person;

			if (this.id_contract == 0)
			{
				//Init default data
				this.model = new AssignmentViewModel();
				this.model.id_person = this.id_person;
				this.model.IsActive = true;
				this.model.IsAdditionalAssignment = false;
				this.model.AssignmentDate = DateTime.Now;
				this.model.ContractDate = DateTime.Now;
				this.model.ValidTo = new DateTime(2080, 1, 1);

				this.cmbLevel1.IsEnabled = true;
				this.cmbLevel2.IsEnabled = false;
				this.cmbLevel3.IsEnabled = false;
				this.cmbLevel4.IsEnabled = false;
				this.cmbStructurePosition.IsEnabled = false;
			}
			else if (this.id_assignment == 0)
			{
				//Create new assignment loading data from the previous one
				this.model = new AssignmentViewModel();
				using (var logic = new PersonalLogic())
				{		
								
					model = logic.InitNextAssignmentViewModel(this.id_contract);
					model.id_assignment = 0;
				}
			}
			else
			{
				using (var logic = new PersonalLogic())
				{
					//load assignment data
					model = logic.InitAssignmentViewModel(id_assignment);
				}
			}
			this.DataContext = model;
			this.LoadCombos();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.IsDataChanged = false;
			if (Settings.id_userLogin == 1)
			{
				this.dpValidTo.IsEnabled = true;
			}
		}

		private void LoadCombos()
		{
			using (var logic = new ComboBoxLogic())
			{
				List<ComboBoxModel> lstCombo;
				logic.NM_LawTypes.FillComboBoxModel(out lstCombo, this.model.id_lawType);
				this.cmbLawType.ItemsSource = lstCombo;
				logic.NM_ContractTypes.FillComboBoxModel(out lstCombo, this.model.id_contractType);
				this.cmbContractType.ItemsSource = lstCombo;
				logic.HR_WorkTime.FillComboBoxModel(out lstCombo, this.model.id_workTime);
				this.cmbWorkTime.ItemsSource = lstCombo;

				
				this.cmbWorkHours.ItemsSource = logic.ReadWorkHours(this.model.id_workHours);

				this.cmbLevel1.ItemsSource = logic.GetLevel(this.model.id_level1, 0);
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
						logic.HandleAssignmentSave(model);
						this.IsDataChanged = false;
						MessageBox.Show("Данните са записани успешно");
						//handle add new contract
						this.Close();
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

		private void cmbLevel1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(this.cmbLevel1.SelectedItem != null)
			{
				using (var logic = new ComboBoxLogic())
				{
					var item = cmbLevel1.SelectedItem as ComboBoxModel;
					if (item != null && item.id != 0)
					{
						this.cmbLevel2.IsEnabled = true;
						this.cmbLevel2.ItemsSource = logic.GetLevel(0, item.id);

						var empty = new List<ComboBoxModel>();
						this.cmbLevel3.ItemsSource = empty;
						this.cmbLevel4.ItemsSource = empty;						
						this.cmbLevel3.IsEnabled = false;
						this.cmbLevel4.IsEnabled = false;

						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, item.id);
						this.cmbStructurePosition.IsEnabled = true;
	                }
					else
					{
						var empty = new List<ComboBoxModel>();
                        this.cmbStructurePosition.ItemsSource = empty;
						this.cmbLevel2.ItemsSource = empty;
						this.cmbLevel3.ItemsSource = empty;
						this.cmbLevel4.ItemsSource = empty;

						this.cmbStructurePosition.IsEnabled = false;
						this.cmbLevel2.IsEnabled = false;
						this.cmbLevel3.IsEnabled = false;
						this.cmbLevel4.IsEnabled = false;
					}
				}
			}
			else
			{
				var empty = new List<ComboBoxModel>();
				this.cmbStructurePosition.ItemsSource = empty;
				this.cmbLevel2.ItemsSource = empty;
				this.cmbLevel3.ItemsSource = empty;
				this.cmbLevel4.ItemsSource = empty;

				this.cmbStructurePosition.IsEnabled = false;
				this.cmbLevel2.IsEnabled = false;
				this.cmbLevel3.IsEnabled = false;
				this.cmbLevel4.IsEnabled = false;
			}
			this.IsDataChanged = true;
		}

		private void cmbLevel2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			using (var logic = new ComboBoxLogic())
			{
				if (this.cmbLevel2.SelectedItem != null)
				{
					var item = cmbLevel2.SelectedItem as ComboBoxModel;
					if (item != null && item.id != 0)
					{
						this.cmbLevel3.IsEnabled = true;
						this.cmbLevel3.ItemsSource = logic.GetLevel(0, item.id);
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, item.id);

						var empty = new List<ComboBoxModel>();
						this.cmbLevel4.ItemsSource = empty;
						this.cmbLevel4.IsEnabled = false;
					}
					else
					{
						var empty = new List<ComboBoxModel>();
						var pitem = cmbLevel1.SelectedItem as ComboBoxModel;
						if (pitem != null)
						{
							this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, pitem.id);
						}
						this.cmbLevel3.ItemsSource = empty;
						this.cmbLevel4.ItemsSource = empty;

						this.cmbLevel3.IsEnabled = false;
						this.cmbLevel4.IsEnabled = false;
					}
				}
				else
				{
					var pitem = cmbLevel1.SelectedItem as ComboBoxModel;
					var empty = new List<ComboBoxModel>();
					if (pitem != null)
					{
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, pitem.id);
					}
					this.cmbLevel3.ItemsSource = empty;
					this.cmbLevel4.ItemsSource = empty;

					this.cmbLevel3.IsEnabled = false;
					this.cmbLevel4.IsEnabled = false;
				}
			}
			this.IsDataChanged = true;
		}

		private void cmbLevel3_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			using (var logic = new ComboBoxLogic())
			{
				if (this.cmbLevel3.SelectedItem != null)
				{
					var item = cmbLevel3.SelectedItem as ComboBoxModel;
					if (item != null && item.id != 0)
					{
						this.cmbLevel4.IsEnabled = true;
						this.cmbLevel4.ItemsSource = logic.GetLevel(0, item.id);
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, item.id);
					}
					else
					{
						var empty = new List<ComboBoxModel>();
						var pitem = cmbLevel2.SelectedItem as ComboBoxModel;
						if (pitem != null)
						{
							this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, pitem.id);
						}
						this.cmbLevel4.ItemsSource = empty;
						this.cmbLevel4.IsEnabled = false;
					}
				}
				else
				{
					var pitem = cmbLevel2.SelectedItem as ComboBoxModel;
					if (pitem != null)
					{
						var empty = new List<ComboBoxModel>();
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, pitem.id);
						
						this.cmbLevel4.ItemsSource = empty;
						this.cmbLevel4.IsEnabled = false;
					}
				}
			}
			this.IsDataChanged = true;
		}

		private void cmbLevel4_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			using (var logic = new ComboBoxLogic())
			{
				if (this.cmbLevel4.SelectedItem != null)
				{
					var item = cmbLevel4.SelectedItem as ComboBoxModel;
					if (item != null && item.id != 0)
					{						
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, item.id);
					}
					else
					{
						var pitem = cmbLevel3.SelectedItem as ComboBoxModel;
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, pitem.id); ;
					}
				}
				else
				{
					var pitem = cmbLevel3.SelectedItem as ComboBoxModel;
					if (pitem != null)
					{
						this.cmbStructurePosition.ItemsSource = logic.GetStructurePositions(0, pitem.id);
					}
				}
			}
			this.IsDataChanged = true;
		}
		
		private void DataChanged(object sender, TextChangedEventArgs e)
		{
			this.IsDataChanged = true;
		}

		private void DataChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsDataChanged = true;
		}
	}
}
