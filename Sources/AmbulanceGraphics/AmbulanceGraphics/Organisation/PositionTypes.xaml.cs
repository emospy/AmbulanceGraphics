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
using System.Windows.Shapes;

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for PositionTypes.xaml
	/// </summary>
	public partial class PositionTypes : Window
	{
		public PositionTypes()
		{
			InitializeComponent();
		}

		private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
		{

		}

		private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
		{

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.RefreshGrid();
		}

		private void RefreshGrid()
		{
			using (var logic = new NomenclaturesLogic())
			{
				this.grGridView.ItemsSource = logic.NM_PositionTypes.GetAll();
			}
		}
	}
}
