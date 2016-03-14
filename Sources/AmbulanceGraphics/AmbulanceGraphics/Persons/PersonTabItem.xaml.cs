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
	}
}
