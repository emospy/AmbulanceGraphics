using BL.DB;
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

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for GlobalPosition.xaml
	/// </summary>
	public partial class GlobalPosition : Window
	{
		private int id_globalPosition;
		private HR_GlobalPositions globalPosition;	

		public GlobalPosition(int id_globalPosition)
		{
			InitializeComponent();
			this.id_globalPosition = id_globalPosition;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{				
				if (this.id_globalPosition == 0)
				{
					this.globalPosition = new HR_GlobalPositions();
					this.globalPosition.IsActive = true;
					this.globalPosition.ActiveFrom = DateTime.Now;
				}
				else
				{
					this.globalPosition = logic.HR_GlobalPositions.GetById(this.id_globalPosition);
				}

				using (var comboBoxLogic = new ComboBoxLogic())
				{
					List<ComboBoxModel> cmbModel;
					comboBoxLogic.NM_PositionTypes.FillComboBoxModel(out cmbModel, this.globalPosition.id_globalPosition);
					this.cmbPositionTypes.ItemsSource = cmbModel;
				}
				this.DataContext = this.globalPosition;
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
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
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void chkIsActive_Checked(object sender, RoutedEventArgs e)
		{
			this.dpValidTo.IsEnabled = false;
		}

		private void chkIsActive_Unchecked(object sender, RoutedEventArgs e)
		{
			this.dpValidTo.IsEnabled = true;
		}
	}
}
