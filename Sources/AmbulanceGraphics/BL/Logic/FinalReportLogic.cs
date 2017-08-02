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
	public class FinalReportLogic : ExportLogic
	{
		public void ExportMonthlyForecastReport(string fileName, DateTime date)
		{
			FileInfo templateFile = new FileInfo("MonthlyReport.xlsx");
			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete(); // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}
			this.package = new ExcelPackage(newFile, templateFile);

			// add a new worksheet to the empty workbook
			using (var logic = new SchedulesLogic())
			{
				int currentRow = 3;
				ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

				this.PrintDepartmentReport(logic, worksheet, ref currentRow, 0, date);


				//worksheet.Cells.AutoFitColumns(0);
				worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
				worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
			}
			package.Save();

		}

		private void PrintDepartmentReport(SchedulesLogic logic, ExcelWorksheet worksheet, ref int currentRow, int id_department, DateTime date)
		{
			List<UN_Departments> lstDepartments;

			if (id_department == 0)
			{
				lstDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive && d.id_department == d.id_departmentParent).ToList();
			}
			else
			{
				lstDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive && d.id_departmentParent == id_department && d.id_departmentParent != d.id_department).ToList();
			}


			foreach (var department in lstDepartments)
			{
				worksheet.Cells[currentRow, 1].Value = department.Name;
			    if (id_department == 0 || department.id_department == department.id_departmentParent)
			    {
			        worksheet.Cells[currentRow, 1].Style.Font.Size = 20;
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                }
			    else
			    {
                    worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                }
				currentRow++;

				this.PritntDepartmentAssignments(department.id_department, date, worksheet, ref currentRow);

				this.PrintDepartmentReport(logic, worksheet, ref currentRow, department.id_department, date);
			}
		}

		private void PritntDepartmentAssignments(int id_department, DateTime date, ExcelWorksheet worksheet, ref int currentRow)
		{
			DateTime startMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			var lstAssignments = this._databaseContext.HR_Assignments.Where(a => a.HR_StructurePositions.id_department == id_department
																				&& startMonth <= a.ValidTo && a.AssignmentDate < endMonth).ToList();

			CalendarRow cRow;
			CalendarRow cRowNH;
			CalendarRow cRowNH1;
			using (NomenclaturesLogic logic = new NomenclaturesLogic())
			{
				cRow = logic.FillCalendarRow(date);
				cRowNH = logic.FillCalendarRowNH(date);
				cRowNH1 = logic.FillCalendarRowNH(date.AddMonths(1));
			}

			foreach (var ass in lstAssignments)
			{
				//var PF = this._databaseContext.GR_PresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract
				//																	&& a.Date.Month == date.Month
				//																	&& a.Date.Year == date.Year
				//																	&& a.id_scheduleType == (int)ScheduleTypes.PresenceForm);
				//if (PF == null)
				//{
				//	continue;
				//}

				var DS = this._databaseContext.GR_PresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract
																					&& a.Date.Month == date.Month
																					&& a.Date.Year == date.Year
																					&& a.id_scheduleType == (int)ScheduleTypes.DailySchedule);

				if (DS == null)
				{
					continue;
				}

				int id_person = ass.HR_Contracts.id_person;

				//PFRow PresenceFrom = new PFRow();
				PFRow Result = new PFRow();
				//PFRow Result = new PFRow();
				//PresenceFrom.PF = PF;
				Result.PF = DS;
				//Result.PF = new GR_PresenceForms();


				//for (i = 1; i <= date.Day; i++)
				//{
				//	Result[i] = PresenceFrom[i];
				//}
				//int dm = DateTime.DaysInMonth(date.Year, date.Month);
				//for (; i <= dm; i++)
				//{
				//	Result[i] = DailySchedule[i];
				//}

				Result.LstWorktimeAbsences = this._databaseContext.GR_WorkTimeAbsence.Where(a => a.id_contract == ass.id_contract
																								 && a.Date.Month == date.Month
																								 && a.Date.Year == date.Year)
					.ToList();

				Result.RealDate = date;
				if (ass.HR_WorkTime == null)
				{
					continue;
				}
				Result.WorkHours = ass.HR_WorkTime.WorkHours;
				Result.lstShiftTypes = this._databaseContext.GR_ShiftTypes.ToList();
				Result.cRow = cRow;

				var workDaysNorm = cRow.WorkDays;
				var fad = ass.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false).AssignmentDate;
                if (fad.Year == date.Year && fad.Month == date.Month && ass.HR_Contracts.IsFired == false)
				{// assigned in the current month
					workDaysNorm = this.CalculateWorkDays(fad, new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)), this.lstCalendarRows);
				}
				else if ((fad.Year != date.Year || fad.Month != date.Month) && ass.HR_Contracts.IsFired && ass.HR_Contracts.DateFired.Value.Year == date.Year && ass.HR_Contracts.DateFired.Value.Month == date.Month)
				{//fired in the current month
					workDaysNorm = this.CalculateWorkDays(new DateTime(date.Year, date.Month, 1), ass.HR_Contracts.DateFired.Value, this.lstCalendarRows);
				}
				else if (fad.Year == date.Year && fad.Month == date.Month && ass.HR_Contracts.IsFired && ass.HR_Contracts.DateFired.Value.Year == date.Year && ass.HR_Contracts.DateFired.Value.Month == date.Month)
				{//assigned and fired in the same month
					workDaysNorm = this.CalculateWorkDays(fad, ass.HR_Contracts.DateFired.Value, this.lstCalendarRows);
				}

				Result.Norm = workDaysNorm * ass.HR_WorkTime.WorkHours;
				if (ass.GR_WorkHours != null)
				{
					Result.IsSumWorkTime = ass.GR_WorkHours.IsSumWorkTime;
				}

				Result.CalculateHours();

				int CountPresences = 0;
				double CountNightHours = 0;
				double NationalHolidayHours = 0;
				int countHolidays = 0;
				int contSickness = 0;
				int i;
				int dm = DateTime.DaysInMonth(date.Year, date.Month);
				for (i = 1; i <= dm; i++)
				{
					switch ((PresenceTypes)Result[i])
					{
						case PresenceTypes.Nothing:
							break;
						case PresenceTypes.DayShift: case PresenceTypes.BusinessTripDay:
							CountPresences++;
							if (cRowNH[i] == true)
							{
								NationalHolidayHours += 12;
							}
							break;
						case PresenceTypes.NightShift: case PresenceTypes.BusinessTripNight:
							CountNightHours += 8;
							CountPresences++;

							double ns = 0;
							double ne = 0;
							if (ass.GR_WorkHours?.NightStart.HasValue == true && ass.GR_WorkHours?.NightEnd.HasValue == true)
							{
								ns = 24 - (double)ass.GR_WorkHours.NightStart.Value.Hours;
								ne = (double)ass.GR_WorkHours.NightEnd.Value.Hours;
								if (ass.GR_WorkHours.NightStart.Value.Minutes == 30)
								{
									ns -= 0.5;
								}
								if (ass.GR_WorkHours.NightEnd.Value.Minutes == 30)
								{
									ne += 0.5;
								}
							}
							else
							{
								ns = 4;
								ne = 8;
							}

							if ((i + 1) <= dm && cRowNH[i] == true && cRowNH[i + 1] == true)
							{
								NationalHolidayHours += 12;
							}
							else if (cRowNH[i] == true)
							{
								NationalHolidayHours += ns;
								if ((i + 1) <= dm && cRowNH[i + 1] == true)
								{
									NationalHolidayHours += ne;
								}
								else if ((i + 1) > dm && cRowNH1[1] == true)
								{
									NationalHolidayHours += ne;
								}
							}
							else if (cRowNH[i] == false)
							{
								if ((i + 1) <= dm && cRowNH[i + 1] == true)
								{
									NationalHolidayHours += ne;
								}
								else if ((i + 1) > dm && cRowNH1[1] == true)
								{
									NationalHolidayHours += ne;
								}
							}
							break;
						case PresenceTypes.RegularShift:
							CountPresences++;
							if (cRowNH[i] == true)
							{
								NationalHolidayHours += Result.WorkHours;
							}
							break;
						case PresenceTypes.YearPaidHoliday:
							if (cRow[i])
							{
								countHolidays++;
							}
							break;
						case PresenceTypes.Sickness:
							contSickness ++;
							break;
						case PresenceTypes.Education:
							if (cRow[i])
							{
								countHolidays++;
							}
							break;
						case PresenceTypes.BusinessTrip:
							if (cRow[i])
							{
								countHolidays++;
							}
							break;
						case PresenceTypes.Motherhood:
							break;
						case PresenceTypes.OtherPaidHoliday:
							if (cRow[i])
							{
								countHolidays++;
							}
							break;
						case PresenceTypes.UnpaidHoliday:
							if (cRow[i])
							{
								countHolidays++;
							}
							break;
						case PresenceTypes.Absence:
							break;
						case PresenceTypes.InactiveSickness:
							break;
						default:
							break;
					}
				}

				double totalOverTime = 0;
				double NightOvertime = 0;
				double NightAbsence = 0;
				double NationalHolidayOvertime = 0;
				double NationalHolidayAbsence = 0;
				foreach (var wta in Result.LstWorktimeAbsences)
				{
					if (wta.StartTime == wta.EndTime)
					{
						continue;
					}
					if (wta.IsPresence == true)
					{
						totalOverTime += wta.WorkHours;
						#region  start Time over 22
						//if (wta.StartTime > new TimeSpan(22, 0, 0))
						//{
						//	if (wta.EndTime > wta.StartTime)
						//	{
						//		var r = wta.EndTime.Subtract(wta.StartTime);
						//		NightOvertime += r.Hours;
						//		if (r.Minutes > 45)
						//		{
						//			NightOvertime += 0.75;
						//		}
						//		else if (r.Minutes > 30)
						//		{
						//			NightOvertime += 0.5;
						//		}
						//		else if (r.Minutes > 15)
						//		{
						//			NightOvertime += 0.25;
						//		}
						//	}
						//	else if (wta.EndTime > new TimeSpan(6, 0, 0))
						//	{
						//		NightOvertime += 8;
						//	}
						//	else
						//	{
						//		var r = wta.EndTime.Add(new TimeSpan(24, 0, 0));
						//		r = r.Subtract(wta.StartTime);
						//		NightOvertime += r.Hours;
						//		if (r.Minutes > 45)
						//		{
						//			NightOvertime += 0.75;
						//		}
						//		else if (r.Minutes > 30)
						//		{
						//			NightOvertime += 0.5;
						//		}
						//		else if (r.Minutes > 15)
						//		{
						//			NightOvertime += 0.25;
						//		}
						//	}
						//}
						#endregion
						//else if(wta.StartTime < )
					}
					else
					{
						totalOverTime -= wta.WorkHours;
					}
				}

				worksheet.Cells[currentRow, 1].Value = ass.HR_Contracts.UN_Persons.Name;
				worksheet.Cells[currentRow, 2].Value = ass.HR_StructurePositions.HR_GlobalPositions.Name;
				worksheet.Cells[currentRow, 3].Value = CountPresences;
				worksheet.Cells[currentRow, 4].Value = Result.Norm;
				worksheet.Cells[currentRow, 5].Value = Result.CountRegularShifts;
				worksheet.Cells[currentRow, 6].Value = Result.CountDayShifts;
				worksheet.Cells[currentRow, 7].Value = Result.CountNightShifts;
				worksheet.Cells[currentRow, 8].Value = Result.TotalWorkedOut;
				worksheet.Cells[currentRow, 9].Value = Result.Difference;
				worksheet.Cells[currentRow, 10].Value = CountNightHours;
				worksheet.Cells[currentRow, 11].Value = NationalHolidayHours;
				worksheet.Cells[currentRow, 12].Value = totalOverTime;
				//worksheet.Cells[1, 13].Value = "Извънреден труд";
				//worksheet.Cells[1, 14].Value = "Нощен труд"; //извънреден
				//worksheet.Cells[1, 15].Value = "Национален празник"; //извънреден
				worksheet.Cells[currentRow, 16].Value = (Result.TotalWorkedOut - Result.Norm);
				worksheet.Cells[currentRow, 17].Value = countHolidays;
				worksheet.Cells[currentRow, 18].Value = contSickness;
				currentRow++;
			}
		}

	}
}
