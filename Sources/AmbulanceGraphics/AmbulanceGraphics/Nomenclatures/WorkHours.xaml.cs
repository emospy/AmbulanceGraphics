﻿using BL.DB;
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
	/// Interaction logic for Ambulances.xaml
	/// </summary>
	public partial class WorkHours : Window
	{
		List<GR_WorkHours> lstWorkHours = new List<GR_WorkHours>();
		public WorkHours()
		{
			InitializeComponent();
		}

		private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			using (var logic = new NomenclaturesLogic())
			{
				if (this.grGridView.SelectedItem != null)
				{
					var item = this.grGridView.SelectedItem as GR_WorkHours;

					try
					{
						logic.GR_WorkHours.Delete(item);
						logic.Save();
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
			this.RefreshDataSource();
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (this.grGridView.SelectedItem != null)
			{
				var wh = this.grGridView.SelectedItem as GR_WorkHours;
				var win = new WorkHour(wh.id_workHours) ;
				win.ShowDialog();
				this.RefreshDataSource();
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var win = new WorkHour(0);
			win.ShowDialog();
			this.RefreshDataSource();
		}

		private void grGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			btnEdit_Click(sender, e);
			this.RefreshDataSource();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.RefreshDataSource();
		}

		private void RefreshDataSource()
		{
			using (var logic = new NomenclaturesLogic())
			{
				this.lstWorkHours = logic.GR_WorkHours.GetActive(this.chkShowInactive.IsChecked == false);
				this.grGridView.ItemsSource = lstWorkHours;
			}
		}
	}
}
