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

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for Ambulance.xaml
	/// </summary>
	public partial class Ambulance : Window
	{
		GR_Ambulances ambulance;
		int id_ambulance;
		public Ambulance(int id_ambulance)
		{
			InitializeComponent();
			this.id_ambulance = id_ambulance;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (ambulance.id_ambulance == 0)
				{
					logic.GR_Ambulances.Add(ambulance);
				}
				else
				{
					logic.GR_Ambulances.Update(ambulance);
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
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (this.id_ambulance == 0)
				{
					this.ambulance = new GR_Ambulances();
					this.ambulance.IsActive = true;
				}
				else
				{
					this.ambulance = logic.GR_Ambulances.GetById(this.id_ambulance);
				}
			}

			using (var logic = new ComboBoxLogic())
			{
				List<ComboBoxModel> lstModel;
				logic.NM_AmbulanceTypes.FillComboBoxModel(out lstModel, this.ambulance.id_ambulanceType);
				this.cmbAmbulanceType.ItemsSource = lstModel;
			}
			this.DataContext = ambulance;
		}
	}
}
