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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for PersonAndAssignment.xaml
	/// </summary>
	public partial class PersonTabItem : UserControl
	{		
		public PersonTabItem()
		{
			InitializeComponent();			
		}

		private void ContentChanged(object sender, TextChangedEventArgs e)
		{
			PersonViewModel vm = this.DataContext as PersonViewModel;			
            vm.IsModified = true;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			PersonViewModel vm = this.DataContext as PersonViewModel;
			vm.IsModified = false;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			var PVM = this.DataContext as PersonViewModel;
			if (PVM.IsModified == true)
			{
				using (var logic = new PersonalLogic())
				{
					try
					{
						if (PVM.id_person != 0)
						{
							logic.UpdatePerson(PVM);
						}
						else
						{
							logic.AddPerson(PVM);
						}
						PVM.IsModified = false;
						MessageBox.Show("Данните са запазени");
					}
					catch (Zora.Core.Exceptions.ZoraException ex)
					{
						MessageBox.Show(ex.Result.Message);
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
			//var PVM = this.DataContext as PersonViewModel;
			//if (PVM.IsModified)
			//{
			//	if (MessageBox.Show("Извършени са промени по личните данни на лицето. Желаете ли да се откажете от промените?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			//	{
			//		while (true)
			//		{
			//			var par = this.Parent;
			//			if (par is PersonFolder)
			//			{
			//				((PersonFolder)par).Close();
			//				break;
			//			}
			//		}
			//	}
			//}
			//else
			//{
			//	FrameworkElement par = this.Parent;
			//	while (true)
			//	{
			//		if (par is PersonFolder)
			//		{
			//			((PersonFolder)par).Close();
			//			break;
			//		}
			//		//par = par.ParentOfType<>();
			//	}
			//}
		}
	}
}
