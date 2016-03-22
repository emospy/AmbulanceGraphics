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

namespace AmbulanceGraphics.Schedules
{
	/// <summary>
	/// Interaction logic for Ambulance.xaml
	/// </summary>
	public partial class DriverAmbulance : Window
	{
		GR_DriverAmbulances driverAmbulance;
		int id_driverAssignment;
		private bool IsDataChanged = false;
		public DriverAmbulance(int id_driverAssignment, string DriverName)
		{
			InitializeComponent();
			this.id_driverAssignment = id_driverAssignment;
			this.Title = "Линейки на шофьор " + DriverName;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (IsDataChanged)
			{
				using (var logic = new SchedulesLogic())
				{
					if (this.driverAmbulance.id_driverAmbulance != 0)
					{
						logic.GR_DriverAmbulances.Update(driverAmbulance);
					}
					else
					{
						logic.GR_DriverAmbulances.Add(driverAmbulance);
					}

					try
					{
						logic.Save();
						this.IsDataChanged = false;
						MessageBox.Show("Данните са запазени");
					}
					catch (ZoraException ex)
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
			this.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			using (var logic = new SchedulesLogic())
			{
				this.driverAmbulance = logic.GetActiveDriverAmbulance(this.id_driverAssignment);
				if (this.driverAmbulance == null)
				{
					this.driverAmbulance = new GR_DriverAmbulances();
					this.driverAmbulance.id_driverAssignment = this.id_driverAssignment;
					this.driverAmbulance.IsActive = true;
				}
			}

			using (var logic = new ComboBoxLogic())
			{
				List<ComboBoxModel> lstAmbulances;
				logic.GR_Ambulances.FillComboBoxModel(out lstAmbulances, this.driverAmbulance.id_primaryAmbulance);
				this.cmbPrimaryAmbulance.ItemsSource = lstAmbulances.OrderBy(a => a.Name);

				List<ComboBoxModel> lstSecondaryAmbulances;
				logic.GR_Ambulances.FillComboBoxModel(out lstSecondaryAmbulances, this.driverAmbulance.id_secondaryAmbulance);
				this.cmbSecondaryAmbulance.ItemsSource = lstSecondaryAmbulances.OrderBy(a => a.Name);
			}
			this.DataContext = driverAmbulance;
			this.IsDataChanged = false;
		}

		private void DataChanged(object sender, SelectionChangedEventArgs e)
		{
			this.IsDataChanged = true;
		}
	}
}
