using BL.DB;
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
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for GlobalPosition.xaml
	/// </summary>
	public partial class GlobalPosition : Window
	{
		private int id_globalPosition;
		private HR_GlobalPositions globalPosition;
		private NomenclaturesLogic logic;

		public GlobalPosition(int id_globalPosition)
		{
			InitializeComponent();
			this.id_globalPosition = id_globalPosition;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.logic = new NomenclaturesLogic();
			if (this.id_globalPosition == 0)
			{
				this.globalPosition = new HR_GlobalPositions();
				this.globalPosition.IsActive = true;
			}
			else
			{
				this.globalPosition = logic.HR_GlobalPositions.GetById(this.id_globalPosition);
			}

			var comboBoxLogic = new ComboBoxLogic();
			this.cmbPositionTypes.ItemsSource = comboBoxLogic.ReadPositionTypes(this.globalPosition.id_globalPosition);

			this.DataContext = this.globalPosition;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (this.globalPosition.id_globalPosition == 0)
			{
				logic.HR_GlobalPositions.Add(this.globalPosition);
			}
			else
			{
				logic.HR_GlobalPositions.Update(this.globalPosition);
			}

			try
			{
				logic.Save();
				this.Close();
			}
			catch (ZoraException ex)
			{
				MessageBox.Show(ex.Result.ErrorCodeMessage);
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
