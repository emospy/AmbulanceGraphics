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

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for Ambulance.xaml
	/// </summary>
	public partial class AmbulanceType : Window
	{
		NM_AmbulanceTypes ambulanceType;
		int id_ambulanceType;		
		public AmbulanceType(int id_ambulanceType)
		{
			InitializeComponent();
			this.id_ambulanceType = id_ambulanceType;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (ambulanceType.id_ambulanceType == 0)
				{
					logic.NM_AmbulanceTypes.Add(ambulanceType);
				}
				else
				{
					logic.NM_AmbulanceTypes.Update(ambulanceType);
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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (this.id_ambulanceType == 0)
				{
					this.ambulanceType = new NM_AmbulanceTypes();
					this.ambulanceType.IsActive = true;
				}
				else
				{
					this.ambulanceType = logic.NM_AmbulanceTypes.GetById(this.id_ambulanceType);
				}
			}
			this.DataContext = ambulanceType;
		}
	}
}
