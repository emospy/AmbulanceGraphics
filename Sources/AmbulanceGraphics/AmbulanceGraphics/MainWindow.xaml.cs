using AmbulanceGraphics.Nomenclatures;
using AmbulanceGraphics.Organisation;
using AmbulanceGraphics.Persons;
using BL.Logic;
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

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		UserInfo currentUser;
		public MainWindow(UserInfo currentUser)
		{
			InitializeComponent();
			this.currentUser = currentUser;
		}

		private void mnuAmbulances_Click(object sender, RoutedEventArgs e)
		{
			var win = new Ambulances();
			win.ShowDialog();
		}

		

		private void mnuPositionTypes_Click(object sender, RoutedEventArgs e)
		{
			var win = new PositionTypes();
			win.ShowDialog();
		}

		private void mnuGlobalPositions_Click(object sender, RoutedEventArgs e)
		{
			var win = new GlobalPositions();
			win.ShowDialog();
		}

		private void mnuOrganisationStructure_Click(object sender, RoutedEventArgs e)
		{
			var win = new OrganisationStructure();
			win.ShowDialog();
		}

		private void mnuPersonnel_Click(object sender, RoutedEventArgs e)
		{
			var win = new Personnel();
			win.ShowDialog();
		}

		private void mnuImport_Click(object sender, RoutedEventArgs e)
		{
			var win = new ImportData();
			win.ShowDialog();
		}
	}
}
