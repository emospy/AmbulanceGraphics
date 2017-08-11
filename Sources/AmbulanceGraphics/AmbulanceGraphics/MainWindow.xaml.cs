using AmbulanceGraphics.Nomenclatures;
using AmbulanceGraphics.Organisation;
using AmbulanceGraphics.Persons;
using AmbulanceGraphics.Schedules;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AmbulanceGraphics.Excel;

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		UserInfo currentUser;
		public MainWindow(UserInfo currentUser)
		{
			InitializeComponent();
			this.currentUser = currentUser;
		}

		private void mnuAmbulances_Click(object sender, RoutedEventArgs e)
		{
			var win = new Ambulances();
			win.ShowDialog();
		}

		private void mnuPositionTypes_Click(object sender, RoutedEventArgs e)
		{
			var win = new PositionTypes();
			win.ShowDialog();
		}

		private void mnuGlobalPositions_Click(object sender, RoutedEventArgs e)
		{
			var win = new GlobalPositions();
			win.ShowDialog();
		}

		private void mnuOrganisationStructure_Click(object sender, RoutedEventArgs e)
		{
			var win = new OrganisationStructure();
			win.ShowDialog();
		}

		private void mnuPersonnel_Click(object sender, RoutedEventArgs e)
		{
			var win = new Personnel();
			win.ShowDialog();
		}

		private void mnuImport_Click(object sender, RoutedEventArgs e)
		{
			var win = new ImportData();
			win.ShowDialog();
		}

		private void mnuYearWorkDays_Click(object sender, RoutedEventArgs e)
		{
			YearWorkdays win = new YearWorkdays();
			win.ShowDialog();
		}

		private void mnuAmbulanceTypes_Click(object sender, RoutedEventArgs e)
		{
			AmbulanceTypes win = new AmbulanceTypes();
			win.ShowDialog();
		}

		private void mnuDriverAmbulances_Click(object sender, RoutedEventArgs e)
		{
			DriverAmbulances win = new DriverAmbulances();
			win.ShowDialog();
		}

		private void mnuLawTypes_Click(object sender, RoutedEventArgs e)
			
		{
			LawTypes win = new LawTypes();
			win.ShowDialog();
		}
		private void mnuContractTypes_Click(object sender, RoutedEventArgs e)
		{
			ContractTypes win = new ContractTypes();
			win.ShowDialog();
		}

		private void mnuScheduleTypes_Click(object sender, RoutedEventArgs e)
		{
			ScheduleTypes win = new ScheduleTypes();
			win.ShowDialog();
		}

		private void mnuCrewTypes_Click(object sender, RoutedEventArgs e)
		{
			CrewTypes win = new CrewTypes();
			win.ShowDialog();
		}

		private void mnuAbsenceTypes_Click(object sender, RoutedEventArgs e)
		{
			AbsenceTypes win = new AbsenceTypes();
			win.ShowDialog();
		}

		private void mnuGenerateSchedule_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new GenerateSchedule();
			win.ShowDialog();
		}

		private void mnuCrews_Click(object sender, RoutedEventArgs e)
		{
			var win = new OrganisationCrews();
			win.ShowDialog();
		}

		private void mnuSchedules_Click(object sender, RoutedEventArgs e)
		{
			var win = new OrganisationSchedules();
			win.ShowDialog();
		}

		private void MnuExportMonthlySchedule_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new ExportSchedules();
			
			win.ShowDialog();
		}

		private void MnuWorkHours_OnClick(object sender, RoutedEventArgs e)
		{
			var win = new WorkHours();
			win.ShowDialog();
		}

        private void mnuSickness_Click(object sender, RoutedEventArgs e)
        {
            var win = new ExportAbsenceReport();
            win.ShowDialog();
        }

        private void mnuHolidaysExport_Click(object sender, RoutedEventArgs e)
        {
            var win = new ExportHolidayReport();
            win.ShowDialog();
        }

        private void mnuDepatmentSchedules_Click(object sender, RoutedEventArgs e)
        {
            var win = new DepartmentSchedules();
            win.ShowDialog();
        }
    }
}
