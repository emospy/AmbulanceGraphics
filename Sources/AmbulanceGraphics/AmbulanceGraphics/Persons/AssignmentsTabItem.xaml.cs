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
	public partial class AssignmentsTabItem : UserControl
	{
		PersonnelViewModel pModel;

		public AssignmentsTabItem()
		{
			InitializeComponent();
		}		

		private void ContentChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.radTreeListView.ItemsSource = this.DataContext;
		}

		//public void ContentChanged(object sender, RoutedEventArgs e)
		//{
		//}

		//private void Page_Loaded(object sender, RoutedEventArgs e)
		//{

		//}
	}
}
