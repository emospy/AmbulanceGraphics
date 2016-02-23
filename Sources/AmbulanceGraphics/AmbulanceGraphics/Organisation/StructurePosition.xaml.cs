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
	public partial class StructurePosition : Window
	{
		private int id_structurePosition;
		private int id_selectedDepartment;
		private HR_StructurePositions structurePosition;
		private UN_Departments department;		

		public StructurePosition(int id_structurePosition, int id_department)
		{
			InitializeComponent();
			this.id_structurePosition = id_structurePosition;
			this.id_selectedDepartment = id_department;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				this.department = logic.UN_Departments.GetById(this.id_selectedDepartment);

				if (this.id_structurePosition == 0)
				{
					this.structurePosition = new HR_StructurePositions();
					this.structurePosition.IsActive = true;
					this.structurePosition.Code = department.Code;
					this.structurePosition.id_department = this.id_selectedDepartment;
					this.structurePosition.ActiveFrom = DateTime.Now;
				}
				else
				{
					this.structurePosition = logic.HR_StructurePositions.GetById(this.id_structurePosition);
				}

				var comboBoxLogic = new ComboBoxLogic();
				this.cmbPosition.ItemsSource = comboBoxLogic.ReadGlobalPositions(this.structurePosition.id_globalPosition);
				if (this.structurePosition.id_globalPosition != 0)
				{
					this.cmbPositionTypes.ItemsSource = comboBoxLogic.ReadPositionTypes(this.structurePosition.HR_GlobalPositions.id_positionType);
				}
				else
				{
					this.cmbPositionTypes.ItemsSource = comboBoxLogic.ReadPositionTypes();
				}
			}
			this.DataContext = this.structurePosition;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (this.structurePosition.id_structurePosition == 0)
				{
					logic.HR_StructurePositions.Add(this.structurePosition);
				}
				else
				{
					logic.HR_StructurePositions.Update(this.structurePosition);
				}

				try
				{
					logic.Save();
					if (structurePosition.Order == 0)
					{
						this.structurePosition.Order = this.structurePosition.id_structurePosition;
						logic.Save();
					}
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

		private void cmbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var gpos = this.cmbPosition.SelectedItem as HR_GlobalPositions;

			if(gpos != null && gpos.id_globalPosition != 0)
			{
				var lstPosTypes = this.cmbPositionTypes.Items.Cast<ComboBoxModel>().ToList();

				var posType = lstPosTypes.FirstOrDefault(a => a.id == gpos.id_positionType);
				this.cmbPositionTypes.SelectedItem = posType;
			}
		}

		private void chkIsActive_Checked(object sender, RoutedEventArgs e)
		{
			this.dpActiveTo.IsEnabled = false;
		}

		private void chkIsActive_Unchecked(object sender, RoutedEventArgs e)
		{
			this.dpActiveTo.IsEnabled = false;
		}
	}
}
