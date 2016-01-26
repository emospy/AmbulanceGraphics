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

		private int id_selectedDepartment;
		public OrganisationStructure()
		{
			InitializeComponent();
			this.logic = new NomenclaturesLogic();
			this.RefreshTree();
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
			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;
			this.id_selectedDepartment = tag.id_departmnet;
			this.LoadPositions();
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
			this.RadViewSource.ExpandAll();

		}

		private void btnAddChild_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
                var win = new Department(0, tag.id_departmentTree);
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
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				var win = new Department(tag.id_departmnet, tag.id_departmentTree);
				win.ShowDialog();
				this.RefreshTree();
			}
		}

		private void btnDeleteNode_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnAddPosition_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var win = new StructurePosition(0, this.id_selectedDepartment);
				win.ShowDialog();
				this.LoadPositions();
			}
			else
			{
				MessageBox.Show("Моля, изберете звено от дървовидната структура на организацията.");
			}
		}

		private void LoadPositions()
		{
			this.grGridView.ItemsSource = this.logic.GetStructurePositions(this.id_selectedDepartment, this.chkShowInactive.IsChecked != true);
		}

		private void btnEditPosition_Click(object sender, RoutedEventArgs e)
		{
			var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
			var tag = item.Tag as StructureTreeViewModel;

			if (this.grGridView.SelectedItem != null)
			{
				var struc = this.grGridView.SelectedItem as StructurePositionViewModel;
				var win = new StructurePosition(struc.id_structurePosition, this.id_selectedDepartment);
				win.ShowDialog();
				this.LoadPositions();
			}
			else
			{
				MessageBox.Show("Моля, изберете длъжност, която да редактирате.");
			}
		}

		private void btnDeletePosition_Click(object sender, RoutedEventArgs e)
		{

		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditPosition_Click(sender, e);
		}

		private void GridMenuItemUp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void GridMenuItemDown_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
