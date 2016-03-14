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
	/// Interaction logic for Department.xaml
	/// </summary>
	public partial class Department : Window
	{
		private int id_department;
		private int id_departmentParent;
		private UN_Departments department;
		
		public Department(int id_department, int id_departmentParent = 0)
		{
			InitializeComponent();
			this.id_department = id_department;
			this.id_departmentParent = id_departmentParent;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (this.id_department == 0)
				{
					this.department = new UN_Departments();
					this.department.IsActive = true;
					this.department.ActiveFrom = DateTime.Now;
				}
				else
				{
					this.department = logic.UN_Departments.GetById(this.id_department);
				}
			}
			this.DataContext = this.department;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{			
			using (var logic = new NomenclaturesLogic())
			{
				if (this.id_departmentParent == 0)
				{					
					this.department.Level = 1;
				}
				else if(department.Level == 0)
				{
					int level;					
					level = logic.GetTreeLevel(this.id_departmentParent);
					this.department.Level = level + 1;
				}
				if (this.department.id_department == 0)
				{
					logic.UN_Departments.Add(this.department);
				}
				else
				{
					logic.UN_Departments.Update(this.department);					
				}

				try
				{					
					logic.Save();
					if (this.id_departmentParent == 0)
					{
						this.department.id_departmentParent = this.department.id_department;
						this.department.TreeOrder = this.department.id_department;
						logic.Save();
					}
					else if (this.department.TreeOrder == 0)
					{
						this.department.id_departmentParent = this.id_departmentParent;
						this.department.TreeOrder = this.department.id_department;
						logic.Save();
					}
					this.Close();
				}
				catch (ZoraException ex)
				{
					MessageBox.Show(ex.Result.ErrorCodeMessage);
				}
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void chkIsActive_Checked(object sender, RoutedEventArgs e)
		{
			this.dpActiveTo.IsEnabled = false;
		}

		private void chkIsActive_Unchecked(object sender, RoutedEventArgs e)
		{
			this.dpActiveTo.IsEnabled = true;
		}
	}
}
