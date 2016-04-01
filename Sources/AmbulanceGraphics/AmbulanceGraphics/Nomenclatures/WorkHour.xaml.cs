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
	public partial class WorkHour : Window
	{
		GR_WorkHours workHour;
		int id_workHour;
		private bool IsDataChanged = false;
		public WorkHour(int id_workHour)
		{
			InitializeComponent();
			this.id_workHour = id_workHour;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (workHour.id_workHours == 0)
				{
					logic.GR_WorkHours.Add(workHour);
				}
				else
				{
					logic.GR_WorkHours.Update(workHour);
				}

				try
				{
					logic.Save();
					this.IsDataChanged = false;
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
				if (this.id_workHour == 0)
				{
					this.workHour = new GR_WorkHours();
					this.workHour.IsActive = true;
				}
				else
				{
					this.workHour = logic.GR_WorkHours.GetById(this.id_workHour);
				}
			}

			this.DataContext = workHour;
			this.IsDataChanged = false;
		}

		private void DataChanged(object sender, TextChangedEventArgs e)
		{
			this.IsDataChanged = true;
		}
	}
}
