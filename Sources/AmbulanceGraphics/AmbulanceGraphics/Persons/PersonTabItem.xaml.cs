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
		PersonViewModel vm;

		public PersonTabItem()
		{
			InitializeComponent();			
			
			this.vm = new PersonViewModel();
		}

		//public PersonTabItem(int id_person)
		//{
		//	InitializeComponent();
		//	this.id_person = id_person;			
		//}

		private void ContentChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			//if(id_person != 0)
			//{
			//	using (var logic = new PersonalLogic())
			//	{
			//		this.vm = logic.GetPersonModel(id_person);
			//		if (vm == null)
			//		{
			//			MessageBox.Show("Не е намерено лицето в базата данни");
			//		}
			//	}
			//}
			//this.DataContext = this.vm;
		}
	}
}
