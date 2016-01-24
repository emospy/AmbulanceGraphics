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
		private NomenclaturesLogic logic;
		public Department(int id_department, int id_departmentParent = 0)
		{
			InitializeComponent();
			this.id_department = id_department;
			this.id_departmentParent = id_departmentParent;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.logic = new NomenclaturesLogic();
			if (this.id_department == 0)
			{
				this.department = new UN_Departments();
				this.department.IsActive = true;
			}
			else
			{
				this.department = logic.UN_Departments.GetById(this.id_department);
			}

			this.DataContext = this.department;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			UN_DepartmentTree node;
			if(this.id_departmentParent == 0)
			{
				node = new UN_DepartmentTree();
				node.UN_Departments = this.department;
				//node.UN_DepartmentTree2 = node;
				node.IsActive = this.department.IsActive;
				logic.UN_DepartmentTree.Add(node);
			}
			else
			{
				node = logic.GetTreeNode(this.id_department);
				if(node == null)
				{
					node = new UN_DepartmentTree();
					node.UN_Departments = this.department;
					node.IsActive = this.department.IsActive;
					node.id_departmentParent = this.id_departmentParent;
					logic.UN_DepartmentTree.Add(node);
				}
			}
			if (this.department.id_department == 0)
			{
				logic.UN_Departments.Add(this.department);

			}
			else
			{
				logic.UN_Departments.Update(this.department);
				node.IsActive = department.IsActive;
				logic.UN_DepartmentTree.Update(node);
			}

			try
			{
				logic.Save();
				if(this.id_departmentParent == 0)
				{
					node.id_departmentParent = node.id_departmentTree;
					logic.Save();
				}

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
