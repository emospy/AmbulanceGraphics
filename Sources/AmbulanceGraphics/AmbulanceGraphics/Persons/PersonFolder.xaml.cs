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
	/// Interaction logic for PersonFolder.xaml
	/// </summary>
	public partial class PersonFolder : Window
	{
		public GenericPersonViewModel gPVM;
		public PersonFolder(int id_person)
		{
			InitializeComponent(); 

			this.RefreshData(id_person);
		}

		private void RefreshData(int id_person)
		{
			if (id_person != 0)
			{
				using (var logic = new PersonalLogic())
				{
					this.gPVM = logic.InitGPVM(id_person);
					this.Title = gPVM.PersonViewModel.Name;
				}
			}
			else
			{
				this.gPVM = new GenericPersonViewModel();
				this.gPVM.PersonViewModel = new PersonViewModel();
				this.gPVM.SchedulesViewModel = new PersonalSchedulesViewModel();
				this.Title = "Нов служител";
			}
			this.DataContext = this.gPVM;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if(this.gPVM.PersonViewModel.IsModified == true)
			{
				using (var logic = new PersonalLogic())
				{
					try
					{
						if (this.gPVM.PersonViewModel.id_person != 0)
						{
							logic.UpdatePerson(this.gPVM.PersonViewModel);
						}
						else
						{
							logic.AddPerson(this.gPVM.PersonViewModel);
						}
						this.gPVM.PersonViewModel.IsModified = false;
						MessageBox.Show("Данните са запазени");
					}
					catch(Zora.Core.Exceptions.ZoraException ex)
					{
						MessageBox.Show(ex.Result.Message);
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			if(this.gPVM.PersonViewModel.IsModified)
			{
				if(MessageBox.Show("Извършени са промени по личните данни на лицето. Желаете ли да се откажете от промените?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					this.Close();
				}
				else
				{
					this.Close();
				}
			}
			else
			{
				this.Close();
			}
		}
	}
}
