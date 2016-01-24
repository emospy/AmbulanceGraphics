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
using Telerik.Windows.Controls;

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for OrganisationStructure.xaml
	/// </summary>
	public partial class OrganisationStructure : Window
	{
		NomenclaturesLogic logic;
		public OrganisationStructure()
		{
			InitializeComponent();
			this.logic = new NomenclaturesLogic();
			this.PopulateTreeRoot(this.RadViewSource);
		}

		private void PopulateTreeRoot(RadTreeView Tree)
		{
			var rootItems = this.logic.GetTreeNodes(true, 0);
			foreach (var item in rootItems)
			{
				RadTreeViewItem it = new RadTreeViewItem();
				it.Tag = item;
				it.Header = item.DepartmentName;
				Tree.Items.Add(it);
				this.PopulateTreeNodes(item.id_departmentTree, it);
			}
		}

		private void PopulateTreeNodes(int p, RadTreeViewItem parent)
		{
			var lstItems = this.logic.GetTreeNodes(false, p);
			foreach (var item in lstItems)
			{
				RadTreeViewItem it = new RadTreeViewItem();
				it.Tag = item;
				it.Header = item.DepartmentName;
				parent.Items.Add(it);
				this.PopulateTreeNodes(item.id_departmentTree, it);
			}
		}

		private void RadViewSource_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{

		}

		private void MenuItemUp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MenuItemDown_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnAddRoot_Click(object sender, RoutedEventArgs e)
		{
			var win = new Department(0);
			win.ShowDialog();
			this.RefreshTree();
		}

		private void RefreshTree()
		{
			this.RadViewSource.Items.Clear();
			this.PopulateTreeRoot(this.RadViewSource);
		}

		private void btnAddChild_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as StructureTreeViewModel;
				var win = new Department(0, item.id_departmentTree);
				win.ShowDialog();
				this.RefreshTree();
			}
			else
			{
				MessageBox.Show("Трябва да изберете звено, за да добавите подзвено.");
			}
		}

		private void btnEditNode_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as StructureTreeViewModel;
				var win = new Department(item.id_departmnet, item.id_departmentTree);
				win.ShowDialog();
				this.RefreshTree();
			}
		}

		private void btnDeleteNode_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
