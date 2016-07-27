using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DB;
using BL.Models;
using OfficeOpenXml;

namespace BL.Logic
{
	public class SixMonthExportLogic : ExportLogic
	{
		public void PrintDepartmentSixMonthExport(DateTime startDate, DateTime endDate, DateTime middleDate, int id_department, string fileName)
		{
			startDate = new DateTime(startDate.Year, startDate.Month, 1);
			endDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
			FileInfo templateFile = new FileInfo("test.xlsx");
			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete(); // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}
			this.package = new ExcelPackage(newFile, templateFile);

			// add a new worksheet to the empty workbook
			ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

			int currentRow = 3;

			this.PritntDepartmentAssignments(id_department, startDate, endDate, middleDate, worksheet, ref currentRow);

			worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
			worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

			package.Save();

		}

		//private void PrintDepartmentReport(SchedulesLogic logic, ExcelWorksheet worksheet, ref int currentRow, int id_department, DateTime date)
		//{
		//	List<UN_Departments> lstDepartments;

		//	if (id_department == 0)
		//	{
		//		lstDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive && d.id_department == d.id_departmentParent).ToList();
		//	}
		//	else
		//	{
		//		lstDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive && d.id_departmentParent == id_department && d.id_departmentParent != d.id_department).ToList();
		//	}


		//	foreach (var department in lstDepartments)
		//	{
		//		worksheet.Cells[currentRow, 1].Value = department.Name;
		//		currentRow++;

		//		this.PritntDepartmentAssignments(department.id_department, date, worksheet, ref currentRow);

		//		this.PrintDepartmentReport(logic, worksheet, ref currentRow, department.id_department, date);
		//	}
		//}

		private void PritntDepartmentAssignments(int id_department, DateTime startDate, DateTime endDate, DateTime middleDate, ExcelWorksheet worksheet, ref int currentRow)
		{
			var lstAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true
																				 && a.HR_Contracts.IsFired == false
																				 && a.HR_StructurePositions.id_department == id_department).ToList();

			CalendarRow cRow;

			var lstCurrentAssignments =
				this._databaseContext.HR_Assignments.Where(
					a => a.HR_StructurePositions.id_department == id_department && a.IsActive == true).ToList();

			foreach (var ass in lstCurrentAssignments)
			{
				int currentColumn = 2;
				double diff = 0;
				for (DateTime currentDate = startDate; currentDate < endDate; currentDate = currentDate.AddMonths(1))
				{
					using (NomenclaturesLogic logic = new NomenclaturesLogic())
					{
						cRow = logic.FillCalendarRow(currentDate);
					}

					worksheet.Cells[currentRow, 1].Value = ass.HR_Contracts.UN_Persons.Name;

					PFRow pr = new PFRow();

					if (currentDate < middleDate && currentDate.Month != middleDate.Month)
					{
						pr.PF = this.lstPresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract && a.id_scheduleType == (int)ScheduleTypes.PresenceForm && a.Date.Month == currentDate.Month && a.Date.Year == currentDate.Year);
					}
					else if (currentDate.Month == middleDate.Month)
					{
						pr.PF = this.lstPresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract && a.id_scheduleType == (int)ScheduleTypes.DailySchedule && a.Date.Month == currentDate.Month && a.Date.Year == currentDate.Year);
					}
					else
					{
						pr.PF = this.lstPresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract && a.id_scheduleType == (int)ScheduleTypes.ForecastMonthSchedule && a.Date.Month == currentDate.Month && a.Date.Year == currentDate.Year);
					}

					if (pr.PF == null)
					{
						pr.PF = new GR_PresenceForms();
						
					}

					pr.WorkHours = ass.HR_WorkTime.WorkHours;
					pr.lstShiftTypes = this.lstShiftTypes;
					pr.cRow = cRow;
					pr.LstWorktimeAbsences =
						this.lstWorktimeAbsenceces.Where(
							a =>
								a.id_contract == ass.id_contract && a.Date.Year == currentDate.Year && a.Date.Month == currentDate.Month &&
								a.IsPrevMonthTransfer == false).ToList();
					var workDaysNorm = cRow.WorkDays;
					var endmonth = new DateTime(currentDate.Year, currentDate.Month,
						DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                    if (ass.AssignmentDate.Year == currentDate.Year && ass.AssignmentDate.Month == currentDate.Month)
					{
						workDaysNorm = this.CalculateWorkDays((DateTime)ass.AssignmentDate, endmonth, this.lstCalendarRows);
					}

					pr.Norm = workDaysNorm * (double)ass.HR_WorkTime.WorkHours;

					if (ass.GR_WorkHours != null)
					{
						pr.IsSumWorkTime = ass.GR_WorkHours.IsSumWorkTime;
					}
					if (ass.AssignmentDate > endmonth)
					{
						pr.Norm = 0;
					}
					pr.CalculateHours();

					worksheet.Cells[currentRow, 0 + currentColumn].Value = pr.Norm;
					worksheet.Cells[currentRow, 1 + currentColumn].Value = pr.Shifts;
					worksheet.Cells[currentRow, 2 + currentColumn].Value = pr.Difference;

					currentColumn += 3;
					diff += pr.Difference;
				}
				worksheet.Cells[currentRow, 3 + currentColumn].Value = diff;
				currentRow++;
			}
			
		}
	}
}
