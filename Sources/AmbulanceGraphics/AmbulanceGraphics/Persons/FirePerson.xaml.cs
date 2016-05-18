using System;
using System.Collections.Generic;
using System.IO;
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
using BL;
using BL.DB;
using BL.Logic;
using BL.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Persons
{
	/// <summary>
	/// Interaction logic for ExportSchedules.xaml
	/// </summary>
	public partial class FirePerson : Window
	{
		private int id_contract;
		public FirePerson(int id_contract)
		{
			InitializeComponent();
			this.id_contract = id_contract;
		}

		private void ExportSchedules_OnLoaded(object sender, RoutedEventArgs e)
		{
			this.dpMonth.SelectedDate = DateTime.Now;
		}

		private void btnFire_Click(object sender, RoutedEventArgs e)
		{
			if (this.dpMonth.SelectedDate.HasValue == false)
			{
				MessageBox.Show("Моля, изберете дата!");
				return;
			}

			try
			{
				using (PersonalLogic logic = new PersonalLogic())
				{
					logic.FirePerson(this.id_contract, this.dpMonth.SelectedDate.Value);
					MessageBox.Show("Договорът е прекратен");
				}
			}
			catch (Zora.Core.Exceptions.ZoraException ex)
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
