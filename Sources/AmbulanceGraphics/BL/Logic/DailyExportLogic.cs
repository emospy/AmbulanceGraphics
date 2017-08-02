using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DB;
using BL.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BL.Logic
{
	public class DailyExportLogic : CrewSchedulesLogic
	{
		internal ExcelPackage package;
		private List<GR_Crews2> lstTemporaryCrews;
		internal int currentCrewOrder;

		public void ExportDailyDepartmentSchedule(string fileName, DateTime date)
		{
			this.lstTemporaryCrews = this._databaseContext.GR_Crews2.Where(c => c.DateStart <= date && c.DateEnd >= date && c.IsTemporary).ToList();

			var sdfn = fileName + "NСофия Дневна.xlsm";
			ExportSofiaShifts(sdfn, date, true);
			var snfn = fileName + "NСофия Нощна.xlsm";
			ExportSofiaShifts(snfn, date, false);

			//var odfn = fileName + "Област Дневна.xlsm";
			//ExportRegionShifts(odfn, date, true);
			//var onfn = fileName + "Област Нощна.xlsm";
			//ExportRegionShifts(onfn, date, false);
		}

		private void ExportSofiaShifts(string fileName, DateTime date, bool IsDayShift)
		{
			FileInfo templateFile = new FileInfo("DailyTemplate.xlsm");
			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete(); // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}
			this.package = new ExcelPackage(newFile, templateFile);

			this.ExportSofiaOtherDepartments(date, IsDayShift, package);
			this.ExportSofiaCentralDepartments(date, IsDayShift, package);
			this.ExportSofiaDailyCrews(date, IsDayShift);
			this.package.Save();
		}

		private void ExportSofiaCentralDepartments(DateTime date, bool IsDayShift, ExcelPackage package)
		{
			var lstCentralDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 23
																					  || d.id_departmentParent == 23).ToList();

			int currentRow = 3;

			ExcelWorksheet worksheet = package.Workbook.Worksheets[5];

			this.PrintStationaryDepartments(date, IsDayShift, lstCentralDepartments, worksheet, currentRow);
		}

		private void ExportSofiaDailyCrews(DateTime date, bool IsDayShift)
		{
			var lstCentralDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 24).ToList();

            var lstCentralServiceDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 192
                                                                                        || d.id_department == 204).ToList();

            var lstCentralExternalDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 25
																						|| d.id_department == 26
																						|| d.id_department == 27).ToList();

			int currentRow = 1;
			int currentRowDrivers = 4;
			int currentRowSisters = 6;
			int currentRowDoctors = 4;
			int currentRowSanitars = 6;

			foreach (var dep in lstCentralDepartments)
			{
				this.PrintDailyCrewsAndSchedules(date, IsDayShift, dep, ref currentRow, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors, ref currentRowSanitars);
			}

            foreach (var dep in lstCentralServiceDepartments)
            {
                this.PrintDailyCrewsAndSchedulesOB(date, IsDayShift, dep, ref currentRow, 1);
            }

            foreach (var dep in lstCentralExternalDepartments)
			{
				this.PrintDailyCrewsAndSchedulesOB(date, IsDayShift, dep, ref currentRow, 1);
			}

            var worksheet = package.Workbook.Worksheets[1];
            //copy this all to the print page
            worksheet = package.Workbook.Worksheets[1];
            ExcelWorksheet w2 = package.Workbook.Worksheets[10];

            int i;
            for (i = 2; i <= currentRow; i++)
            {
                for (int j = 1; j <= 7; j++)  //G cloumn
                {
                    w2.Cells[i, j].Value = worksheet.Cells[i, j].Value;
                    w2.Cells[i, j].Style.Font.Size = worksheet.Cells[i, j].Style.Font.Size;
                    w2.Cells[i, j].Style.Font.Bold = worksheet.Cells[i, j].Style.Font.Bold;
                    w2.Cells[i, j].Style.HorizontalAlignment = worksheet.Cells[i, j].Style.HorizontalAlignment;
                }
            }
            i++;
            //continue to copy all other needed sheets
            worksheet = package.Workbook.Worksheets[3];
            for (int k = 3; k <= currentRowSisters; i++, k++)
            {
                for (int j = 1; j <= 7; j++) //G cloumn
                {
                    w2.Cells[i, j + 2].Value = worksheet.Cells[k, j].Value;
                    w2.Cells[i, j + 2].Style.Font.Size = worksheet.Cells[k, j].Style.Font.Size;
                    w2.Cells[i, j + 2].Style.Font.Bold = worksheet.Cells[k, j].Style.Font.Bold;
                    w2.Cells[i, j + 2].Style.HorizontalAlignment = worksheet.Cells[k, j].Style.HorizontalAlignment;
                }
            }
            i++;
            worksheet = package.Workbook.Worksheets[4];
            for (int k = 3; k <= currentRowSanitars; i++, k++)
            {
                for (int j = 1; j <= 7; j++) //G cloumn
                {
                    w2.Cells[i, j + 2].Value = worksheet.Cells[k, j].Value;
                    w2.Cells[i, j + 2].Style.Font.Size = worksheet.Cells[k, j].Style.Font.Size;
                    w2.Cells[i, j + 2].Style.Font.Bold = worksheet.Cells[k, j].Style.Font.Bold;
                    w2.Cells[i, j + 2].Style.HorizontalAlignment = worksheet.Cells[k, j].Style.HorizontalAlignment;
                }
            }

            worksheet = package.Workbook.Worksheets[1];
            worksheet.DeleteRow(currentRow + 1, 2000);
			worksheet = package.Workbook.Worksheets[4];
			worksheet.DeleteRow(currentRowSanitars + 1, 2000);
			worksheet = package.Workbook.Worksheets[3];
			worksheet.DeleteRow(currentRowSisters + 1, 2000);

			var lstRegionDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 28
																						|| d.id_department == 29
																						|| d.id_department == 30
																						|| d.id_department == 31
																						|| d.id_department == 32
																						|| d.id_department == 33
																						|| d.id_department == 34
																						|| d.id_department == 35
																						|| d.id_department == 36
																						|| d.id_department == 37
																						|| d.id_department == 38
																						|| d.id_department == 39
																						|| d.id_department == 40
																						|| d.id_department == 171).ToList();

			currentRow = 2;
			foreach (var dep in lstRegionDepartments)
			{
				this.PrintDailyCrewsAndSchedulesOB(date, IsDayShift, dep, ref currentRow, 2);
			}
			worksheet = package.Workbook.Worksheets[2];
			worksheet.DeleteRow(currentRow + 1, 2000);
        }

		private void PrintStationaryDepartments(DateTime date, bool IsDayShift, List<UN_Departments> lstCentralDepartments,
			ExcelWorksheet worksheet, int currentRow)
		{

			List<HR_Assignments> lstAsses = new List<HR_Assignments>();
			foreach (var department in lstCentralDepartments)
			{
				var res = this._databaseContext.HR_Assignments.Where(a => a.HR_StructurePositions.id_department == department.id_department
																	&& a.AssignmentDate <= date && a.ValidTo > date
																	&& a.HR_Contracts.IsFired == false)
						.ToList();

				lstAsses.AddRange(res);
			}

			lstAsses = lstAsses.OrderBy(p => p.HR_StructurePositions.HR_GlobalPositions.NM_PositionTypes.PositionOrder).ToList();

			var day = date.Day;
			foreach (var per in lstAsses)
			{
				var pf = this.GetPersonalSchedule(per.HR_Contracts.id_person, date, ScheduleTypes.DailySchedule).FirstOrDefault();
				if (pf == null)
				{
					continue;
				}
				if (IsDayShift)
				{
					if (pf[day] == (int)PresenceTypes.DayShift || pf[day] == (int)PresenceTypes.RegularShift)
					{
						this.PrintOtherRow(worksheet, currentRow, pf, per, true);
						currentRow++;
					}
				}
				else
				{
					if (pf[day] == (int)PresenceTypes.NightShift)
					{
						this.PrintOtherRow(worksheet, currentRow, pf, per, false);
						currentRow++;
					}
				}

			}
			//worksheet.Cells[1, 1].Value = date.ToShortDateString();
			//worksheet.Cells.AutoFitColumns(0);
			worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
			worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
			worksheet.DeleteRow(currentRow + 1, 1000);
			//worksheet.PrinterSettings.RepeatRows = new ExcelAddress(2, 1, 2, 50);
		}

		private void ExportSofiaOtherDepartments(DateTime date, bool IsDayShift, ExcelPackage package)
		{
			var lstOtherDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 19
																					  || d.id_department == 20
																					  || d.id_department == 21
																					  || d.id_department == 22
																					  || d.id_department == 165
																					  || d.id_departmentParent == 19
																					  || d.id_departmentParent == 20
																					  || d.id_departmentParent == 21
																					  || d.id_departmentParent == 22
																					  || d.id_departmentParent == 165
																					  ).ToList();

			int currentRow = 3;

			ExcelWorksheet worksheet = package.Workbook.Worksheets[6];

			this.PrintStationaryDepartments(date, IsDayShift, lstOtherDepartments, worksheet, currentRow);
		}

		private void PrintOtherRow(ExcelWorksheet worksheet, int currentRow, PFRow pf, HR_Assignments per, bool IsDayShift)
		{
			//Име Специалност Код Раб. време Забележка   Сл.Номер
			worksheet.Cells[currentRow, 1].Value = per.HR_Contracts.UN_Persons.Name;
			worksheet.Cells[currentRow, 2].Value = per.HR_StructurePositions.HR_GlobalPositions.Name;
			//worksheet.Cells[currentRow, 3].Value = per.SchedulesCode; //какъв е този код
			if (IsDayShift)
			{
				worksheet.Cells[currentRow, 3].Value = (per.GR_WorkHours == null) ? "" : per.GR_WorkHours.DayHours;
			}
			else
			{
				worksheet.Cells[currentRow, 3].Value = (per.GR_WorkHours == null) ? "" : per.GR_WorkHours.NightHours;
			}

			// 5 is for notes and will remain empty
			//worksheet.Cells[currentRow, 6].Value = per.HR_Contracts.TRZCode;
		}

		private void PrintDailyCrewsAndSchedules(DateTime date, bool IsDayShift, UN_Departments baseDepartment, ref int currentRow, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors, ref int currentRowSanitars)
		{
			var lstSubDeps = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == baseDepartment.id_department
																			&& a.id_department != a.id_departmentParent).OrderBy(a => a.TreeOrder).ToList();

			ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
			
			currentRow++;
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = baseDepartment.Name;
			worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			worksheet.Cells[currentRow, 3].Style.Font.Bold = true;

			int idx = this.CalculateLeadDepartmentIndex(date, baseDepartment.NumberShifts, IsDayShift);
			int id_selectedDepartment = lstSubDeps[idx].id_department;

			string ShitLabel = "";
			if (IsDayShift == false)
			{
				ShitLabel = string.Format("{0}/{1}.{2}.{3} {4} {5}", date.Day, date.AddDays(1).Day, date.AddDays(1).Month, date.AddDays(1).Year, lstSubDeps[idx].Name, "НОЩНА");
			}
			else
			{
				ShitLabel = string.Format("{0}.{1}.{2} {3} {4}", date.Day, date.Month, date.Year, lstSubDeps[idx].Name, "Дневна");
			}
			worksheet.Cells[1, 1].Value = ShitLabel;

			var lstDailyCrewSchedules = this.GetDepartmentCrewsAndSchedulesForDate(id_selectedDepartment, date, (int)ScheduleTypes.DailySchedule);
			//var lstDailyCrewSchedules = this.GetDepartmentCrewsAndSchedules(id_selectedDepartment, date, date, date, (int)ScheduleTypes.DailySchedule);

			this.PrintDailyCrews(date, lstDailyCrewSchedules, IsDayShift, ref currentRow, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors, ref currentRowSanitars);
		}

		private void PrintDailyCrewsAndSchedulesOB(DateTime date, bool IsDayShift, UN_Departments baseDepartment, ref int currentRow, int worksheetNum = 2)
		{
			var lstSubDeps = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == baseDepartment.id_department
																			&& a.id_department != a.id_departmentParent).OrderBy(a => a.TreeOrder).ToList();

			ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetNum];
			//worksheet.Cells[1, 1].Value = date.ToShortDateString();
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = baseDepartment.Name;
			worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			worksheet.Cells[currentRow, 3].Style.Font.Bold = true;


			int idx = this.CalculateLeadDepartmentIndex(date, baseDepartment.NumberShifts, IsDayShift);
			int id_selectedDepartment = lstSubDeps[idx].id_department;

			var lstDailyCrewSchedules = this.GetDepartmentCrewsAndSchedulesForDate(id_selectedDepartment, date, (int)ScheduleTypes.DailySchedule);

			this.PrintDailyCrewsOB(date, lstDailyCrewSchedules, IsDayShift, ref currentRow, worksheetNum);
		}

		private bool IsCrewFullNoDepart(DateTime date, CrewScheduleListViewModel cmv, bool IsDayShift)
		{
			var ct = (CrewTypes)cmv.id_crewType;

			switch (ct)
			{
				case CrewTypes.Reanimation:
					if (cmv.LstCrewMembers[0].id_person == 0)
					{
						return false;
					}
					//if the doctor is included in another crew for this date - cancel the crew
					var lstTmpDoctor = this.lstTemporaryCrews.Where(a => (a.id_assignment2 == cmv.LstCrewMembers[0].id_assignment
																			 || a.id_assignment4 == cmv.LstCrewMembers[0].id_assignment)
																			 && a.DateStart <= date && a.DateEnd >= date)
							.ToList();
					if (lstTmpDoctor.Count > 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv.id_person != 0
							&& cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.id_person = 0;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1].id_person != 0
							&& cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.LstCrewMembers[1].id_person = 0;
						}
					}
					else
					{
						if (cmv.id_person != 0
							&& cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.id_person = 0;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1].id_person != 0
							&& cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.LstCrewMembers[1].id_person = 0;
						}
					}
					break;
				case CrewTypes.Doctor:
					if (cmv.LstCrewMembers[0].id_person == 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv.id_person != 0
							&& cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.id_person = 0;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
					}

					else
					{
						if (cmv.id_person != 0
							&& cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.id_person = 0;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
					}
					break;
				case CrewTypes.Paramedic:
					if (cmv.LstCrewMembers[1].id_person == 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv.id_person != 0
							&& cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
					}
					else
					{
						if (cmv.id_person != 0
							&& cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
					}
					break;
				case CrewTypes.Corpse:
					if (cmv.id_person == 0)
					{
						return false;
					}

					var lstTmpDriver = this.lstTemporaryCrews.Where(a => a.id_assignment1 == cmv.id_assignment
																		|| a.id_assignment1 == cmv.id_assignment).ToList();
					if (lstTmpDriver.Count > 0)
					{
						return false;
					}

					if (IsDayShift)
					{
						if (cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[2].id_person != 0
							&& cmv.LstCrewMembers[2][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[2][date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.LstCrewMembers[2].id_person = 0;
						}
					}
					else
					{
						if (cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[2].id_person != 0
							&& cmv.LstCrewMembers[2][date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.LstCrewMembers[2].id_person = 0;
						}
					}
					break;
				default:
					return false;
			}
			return true;
		}

		private bool IsCrewFullNoDepart2(DateTime date, CrewScheduleListViewModel cmv, bool IsDayShift)
		{
			var ct = (CrewTypes)cmv.id_crewType;

			switch (ct)
			{
				case CrewTypes.Reanimation:
					if (cmv.LstCrewMembers[0].id_person == 0)
					{
						return false;
					}
					//if the doctor is included in another crew for this date - cancel the crew
					var lstTmpDoctor = this.lstTemporaryCrews.Where(a => a.id_assignment2 == cmv.LstCrewMembers[0].id_assignment
																			 || a.id_assignment4 == cmv.LstCrewMembers[0].id_assignment)
							.ToList();
					if (lstTmpDoctor.Count > 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.id_person = 0;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.LstCrewMembers[1].id_person = 0;
						}
					}
					else
					{
						if (cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.id_person = 0;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.LstCrewMembers[1].id_person = 0;
						}
					}
					break;
				case CrewTypes.Doctor:
					if (cmv.LstCrewMembers[0].id_person == 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
					}

					else
					{
						if (cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
					}
					break;
				case CrewTypes.Paramedic:
					if (cmv.LstCrewMembers[1].id_person == 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
					}
					else
					{
						if (cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
					}
					break;
				case CrewTypes.Corpse:
					if (cmv.id_person == 0)
					{
						return false;
					}

					var lstTmpDriver = this.lstTemporaryCrews.Where(a => a.id_assignment1 == cmv.id_assignment
																			 || a.id_assignment1 == cmv.id_assignment)
							.ToList();
					if (lstTmpDriver.Count > 0)
					{
						return false;
					}

					if (IsDayShift)
					{
						if (cmv[date.Day] != (int)PresenceTypes.DayShift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[2][date.Day] != (int)PresenceTypes.DayShift
							&& cmv.LstCrewMembers[2][date.Day] != (int)PresenceTypes.RegularShift)
						{
							cmv.LstCrewMembers[2].id_person = 0;
						}
					}
					else
					{
						if (cmv[date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[2][date.Day] != (int)PresenceTypes.NightShift)
						{
							cmv.LstCrewMembers[2].id_person = 0;
						}
					}
					break;
				default:
					return false;
			}
			return true;
		}

		private void PrintDailyCrews(DateTime date, List<CrewScheduleListViewModel> lstDepartmentCrews, bool IsDayShift, ref int currentRow, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors, ref int currentRowSanitars)
		{
			this.currentCrewOrder = 1;

			for (int crewCounter = 0; crewCounter < lstDepartmentCrews.Count; crewCounter++)
			{
				var cr = lstDepartmentCrews[crewCounter];
				if (cr.LstCrewMembers != null && cr.LstCrewMembers.Count > 0 && this.IsCrewFullNoDepart(date, cr, IsDayShift))
				{
					//PrintCrew where crews are printed;
					var cn = cr.CrewName;
					cr.CrewName = this.currentCrewOrder.ToString();

					this.PrintCrew(cr, ref currentRow, date, IsDayShift);

					//remove possibility for double printing
					for (int i = crewCounter + 1; i < lstDepartmentCrews.Count; i++)
					{
						var cr1 = lstDepartmentCrews[i];

						if (cr1.id_person == cr.id_person
							|| cr1.id_person == cr.LstCrewMembers[0].id_person
							|| cr1.id_person == cr.LstCrewMembers[1].id_person
							|| cr1.id_person == cr.LstCrewMembers[2].id_person)
						{
							cr1.id_person = 0;
						}
					}
					//go to next crew
					while (crewCounter + 1 < lstDepartmentCrews.Count && cn == lstDepartmentCrews[crewCounter + 1].CrewName && lstDepartmentCrews[crewCounter + 1].CrewName != "")
					{
						crewCounter++;
					}

					this.currentCrewOrder++;
				}
				else
				{
					if (cr.id_person != 0)
					{
						this.PrintStandAlone(cr, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors, ref currentRowSanitars, date, IsDayShift);
						for (int i = crewCounter + 1; i < lstDepartmentCrews.Count; i++)
						{
							var cr1 = lstDepartmentCrews[i];

							if (cr1.id_person == cr.id_person)
							{
								cr1.id_person = 0;
							}
						}
					}
				}
			}
		}

		private void PrintDailyCrewsOB(DateTime date, List<CrewScheduleListViewModel> lstDepartmentCrews, bool IsDayShift, ref int currentRow, int worksheetNum)
		{
			this.currentCrewOrder = 1;

			for (int crewCounter = 0; crewCounter < lstDepartmentCrews.Count; crewCounter++)
			{
				var cr = lstDepartmentCrews[crewCounter];
				//PrintCrew where crews are printed;
				//var cn = cr.CrewName;
				cr.CrewName = this.currentCrewOrder.ToString();

				for (int i = crewCounter + 1; i < lstDepartmentCrews.Count; i++)
				{
					var cr1 = lstDepartmentCrews[i];

					if (cr1.id_person == cr.id_person)
					{
						cr1.id_person = 0;
					}
				}

				this.PrintCrewOB(cr, ref currentRow, date, IsDayShift, worksheetNum);
				//go to next crew

			}
		}

		private void PrintCrew(CrewScheduleListViewModel cmv, ref int currentRow, DateTime date, bool IsDayShift)
		{
			ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер

			if (cmv.id_person == 0
				|| cmv[date.Day] == (int)PresenceTypes.DayShift && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.RegularShift && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.NightShift && IsDayShift == false)
			{
				worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
				//worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
				if (cmv.id_person != 0)
				{
					worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
					worksheet.Cells[currentRow, 3].Value = cmv.Name;
					worksheet.Cells[currentRow, 4].Value = cmv.ShortPosition;

					if (cmv.WorkTime != null)
					{

						var wt = cmv.WorkTime.Split(new char[] { ';' });
						if (IsDayShift)
						{
							worksheet.Cells[currentRow, 5].Value = wt[0];
						}
						else
						{
							worksheet.Cells[currentRow, 5].Value = wt[1];
						}
					}
					//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
					//worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
				}
				//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
				//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
			}

			currentRow++;

			if (cmv.LstCrewMembers[0].id_person == 0
				|| cmv.LstCrewMembers[0][date.Day] == (int)PresenceTypes.RegularShift && IsDayShift == true
				|| cmv.LstCrewMembers[0][date.Day] == (int)PresenceTypes.DayShift && IsDayShift == true
				|| cmv.LstCrewMembers[0][date.Day] == (int)PresenceTypes.NightShift && IsDayShift == false)
			{
				//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
				worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
				//worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
				//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
				if (cmv.LstCrewMembers[0].id_person != 0)
				{
					worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[0].Name;
					worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[0].ShortPosition;

					if (cmv.WorkTime != null)
					{
						var wt = cmv.WorkTime.Split(new char[] { ';' });

						if (IsDayShift)
						{
							worksheet.Cells[currentRow, 5].Value = wt[0];
						}
						else
						{
							worksheet.Cells[currentRow, 5].Value = wt[1];
						}
					}
					//worksheet.Cells[currentRow, 5].Value = cmv.;
					//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;

					//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
					//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
				}
			}


			currentRow++;
			if (cmv.LstCrewMembers[1].id_person == 0
				|| cmv.LstCrewMembers[1][date.Day] == (int)PresenceTypes.RegularShift && IsDayShift == true
				|| cmv.LstCrewMembers[1][date.Day] == (int)PresenceTypes.DayShift && IsDayShift == true
				|| cmv.LstCrewMembers[1][date.Day] == (int)PresenceTypes.NightShift && IsDayShift == false)
			{
				//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
				worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
				//worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
				if (cmv.LstCrewMembers[1].id_person != 0)
				{
					//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
					worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[1].Name;
					worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[1].ShortPosition;
					//worksheet.Cells[currentRow, 5].Value = cmv.;
					//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
					if (cmv.WorkTime != null)
					{
						var wt = cmv.WorkTime.Split(new char[] { ';' });

						if (IsDayShift)
						{
							worksheet.Cells[currentRow, 5].Value = wt[0];
						}
						else
						{
							worksheet.Cells[currentRow, 5].Value = wt[1];
						}
					}
					//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
					//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
				}
			}

			currentRow++;
			if (cmv.LstCrewMembers[2].id_person == 0
				|| cmv.LstCrewMembers[2][date.Day] == (int)PresenceTypes.RegularShift && IsDayShift == true
				|| cmv.LstCrewMembers[2][date.Day] == (int)PresenceTypes.DayShift && IsDayShift == true
				|| cmv.LstCrewMembers[2][date.Day] == (int)PresenceTypes.NightShift && IsDayShift == false)
			{
				//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
				worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
				//worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
				if (cmv.LstCrewMembers[2].id_person != 0)
				{
					//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
					worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[2].Name;
					worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[2].ShortPosition;
					//worksheet.Cells[currentRow, 5].Value = cmv.;
					//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
					if (cmv.WorkTime != null)
					{
						var wt = cmv.WorkTime.Split(new char[] { ';' });

						if (IsDayShift)
						{
							worksheet.Cells[currentRow, 5].Value = wt[0];
						}
						else
						{
							worksheet.Cells[currentRow, 5].Value = wt[1];
						}
					}
					//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
					//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
				}
			}
		}

		private void PrintCrewOB(CrewScheduleListViewModel cmv, ref int currentRow, DateTime date, bool IsDayShift, int worksheetNum)
		{
			ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetNum];

			if (cmv.id_person == 0
				|| cmv[date.Day] == (int)PresenceTypes.RegularShift && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.DayShift && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.NightShift && IsDayShift == false
				|| cmv[date.Day] == (int)PresenceTypes.BusinessTripDay && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.BusinessTripNight && IsDayShift == false)
			{
				currentRow++;
				//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер


				worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
				worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
				if (cmv.id_person != 0)
				{
					worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
					worksheet.Cells[currentRow, 3].Value = cmv.Name;
					worksheet.Cells[currentRow, 4].Value = cmv.ShortPosition;
					if (cmv.WorkTime != null)
					{
						var wt = cmv.WorkTime.Split(new char[] { ';' });

						if (IsDayShift)
						{
							worksheet.Cells[currentRow, 5].Value = wt[0];
						}
						else
						{
							worksheet.Cells[currentRow, 5].Value = wt[1];
						}
					}
				}
			}
		}

		private void PrintStandAlone(CrewScheduleListViewModel cmv, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors, ref int currentRowSanitars, DateTime date, bool IsDayShift)
		{
			ExcelWorksheet worksheet;

			if (cmv[date.Day] == (int)PresenceTypes.DayShift && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.RegularShift && IsDayShift == true
				|| cmv[date.Day] == (int)PresenceTypes.NightShift && IsDayShift == false)
			{
				int currentRow = 0;
				switch (cmv.ShortPosition)
				{
					case "Л":
						currentRowDoctors++;
						currentRow = currentRowDoctors;
						worksheet = package.Workbook.Worksheets[3];
						worksheet.InsertRow(currentRowDoctors, 1);
						//worksheet.Cells[currentRow, 3].Value = cmv.WorkTime;

						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 3].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 3].Value = wt[1];
							}
						}
						
						currentRowSisters++;
						break;
					case "Ф":
						currentRowDoctors++;
						currentRow = currentRowDoctors;
						worksheet = package.Workbook.Worksheets[3];
						worksheet.InsertRow(currentRowDoctors, 1);
						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 3].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 3].Value = wt[1];
							}
						}
						
						currentRowSisters++;
						break;
					case "Мс":
						currentRowSisters++;
						currentRow = currentRowSisters;
						worksheet = package.Workbook.Worksheets[3];
						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 3].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 3].Value = wt[1];
							}
						}
						break;
					case "Ак":
						currentRowSisters++;
						currentRow = currentRowSisters;
						worksheet = package.Workbook.Worksheets[3];
						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 3].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 3].Value = wt[1];
							}
						}
						break;
					case "С":
						currentRowSanitars++;
						currentRow = currentRowSanitars;
						worksheet = package.Workbook.Worksheets[4];
						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 4].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 4].Value = wt[1];
							}
						}
						break;
					case "Ш":
						currentRowDrivers++;
						currentRow = currentRowDrivers;
						worksheet = package.Workbook.Worksheets[4];
						worksheet.InsertRow(currentRowDrivers, 1);
						currentRowSanitars++;

						//Име	Специалност	Код	Номер кола	Раб. време	Забележка	Сл. Номер
						worksheet.Cells[currentRow, 1].Value = cmv.Name;
						worksheet.Cells[currentRow, 2].Value = cmv.ShortPosition;
						//worksheet.Cells[currentRow, 3].Value = cmv.;
						worksheet.Cells[currentRow, 3].Value = cmv.RegNumber;
						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 4].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 4].Value = wt[1];
							}
						}
						return;
						break;
					default:
						currentRowSanitars++;
						currentRow = currentRowSanitars;
						worksheet = package.Workbook.Worksheets[4];
						worksheet.InsertRow(currentRowSanitars, 1);
						if (cmv.WorkTime != null)
						{
							var wt = cmv.WorkTime.Split(new char[] { ';' });
							if (IsDayShift)
							{
								worksheet.Cells[currentRow, 4].Value = wt[0];
							}
							else
							{
								worksheet.Cells[currentRow, 4].Value = wt[1];
							}
						}
						break;
				}
				worksheet.Cells[currentRow, 1].Value = cmv.Name;
				worksheet.Cells[currentRow, 2].Value = cmv.ShortPosition;
				worksheet.Cells[currentRow, 5].Value = cmv.BaseDepartment;
			}
		}
	}
}
