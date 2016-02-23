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
using Telerik.Windows.Controls;
using Zora.Core.Exceptions;

namespace AmbulanceGraphics.Organisation
{
	/// <summary>
	/// Interaction logic for OrganisationStructure.xaml
	/// </summary>
	public partial class OrganisationStructure : Window
	{
		private int id_selectedDepartment;
		public OrganisationStructure()
		{
			InitializeComponent();
			this.RefreshTree();
		}

		private void PopulateTreeRoot(RadTreeView Tree)
		{
			using (var logic = new NomenclaturesLogic())
			{
				var rootItems = logic.GetTreeNodes(true, 0);
				foreach (var item in rootItems)
				{
					RadTreeViewItem it = new RadTreeViewItem();
					it.Tag = item;
					it.Header = item.DepartmentName;
					Tree.Items.Add(it);
					this.PopulateTreeNodes(item.id_department, it, logic);
				}
			}
		}

		private void PopulateTreeNodes(int p, RadTreeViewItem parent, NomenclaturesLogic logic)
		{
			var lstItems = logic.GetTreeNodes(false, p);
			foreach (var item in lstItems)
			{
				RadTreeViewItem it = new RadTreeViewItem();
				it.Tag = item;
				it.Header = item.DepartmentName;
				parent.Items.Add(it);
				this.PopulateTreeNodes(item.id_department, it, logic);
			}
		}

		private void RadViewSource_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				this.id_selectedDepartment = tag.id_department;
				this.LoadPositions();
			}
		}

		private void MenuItemUp_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;

				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveTreeNodeUp(tag.id_department))
						{
							this.RefreshTree();
						}
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

		private void MenuItemDown_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveTreeNodeDown(tag.id_department))
						{
							this.RefreshTree();
						}
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
				var win = new Department(0, tag.id_department);
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
				var win = new Department(tag.id_department, tag.id_departmentParent);
				win.ShowDialog();
				this.RefreshTree();
			}
		}

		private void btnDeleteNode_Click(object sender, RoutedEventArgs e)
		{
			if (this.RadViewSource.SelectedItem != null)
			{
				var item = this.RadViewSource.SelectedItem as RadTreeViewItem;
				var tag = item.Tag as StructureTreeViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						logic.DeleteDepartment(tag.id_department);
						this.RefreshTree();
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
			using (var logic = new NomenclaturesLogic())
			{
				this.grGridView.ItemsSource = logic.GetStructurePositions(this.id_selectedDepartment, this.chkShowInactive.IsChecked != true);
			}
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
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as StructurePositionViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						var it = logic.HR_StructurePositions.GetById(item.id_structurePosition);
						logic.HR_StructurePositions.Delete(it);
						logic.Save();
						this.LoadPositions();
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

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.btnEditPosition_Click(sender, e);
		}

		private void GridMenuItemUp_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as StructurePositionViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveStructurePositionUp(item.id_structurePosition) == true)
						{
							this.LoadPositions();
						}
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

		private void GridMenuItemDown_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var item = this.grGridView.SelectedItem as StructurePositionViewModel;
				using (var logic = new NomenclaturesLogic())
				{
					try
					{
						if (logic.MoveStructurePositionDown(item.id_structurePosition) == true)
						{
							this.LoadPositions();
						}
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
	}
}
