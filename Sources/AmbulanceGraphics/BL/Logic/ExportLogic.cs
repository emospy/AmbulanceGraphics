using BL.DB;
using BL.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class ExportLogic : CrewSchedulesLogic
	{
		internal ExcelPackage package;
		private List<GR_Crews2> lstTemporaryCrews;
		private int currentCrewOrder;

		public void ExportDailyDepartmentSchedule(string fileName, DateTime date)
		{
			this.lstTemporaryCrews = this._databaseContext.GR_Crews2.Where(c => c.DateStart <= date && c.DateEnd >= date).ToList();

			var sdfn = fileName + "София Дневна.xlsm";
			ExportSofiaShifts(sdfn, date, true);
			var snfn = fileName + "София Нощна.xlsm";
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
			var lstCentralDepartments = this._databaseContext.UN_Departments.Where(d => d.id_department == 24
																						|| d.id_department == 25
																						|| d.id_department == 26
																						|| d.id_department == 27).ToList();

			int currentRow = 2;
			int currentRowDrivers = 3;
			int currentRowSisters = 3;
			int currentRowDoctors = 3;

			foreach (var dep in lstCentralDepartments)
			{
				this.PrintDailyCrewsAndSchedules(date, IsDayShift, dep, ref currentRow, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors);
			}
		}

		private void PrintStationaryDepartments(DateTime date, bool IsDayShift, List<UN_Departments> lstCentralDepartments,
			ExcelWorksheet worksheet, int currentRow)
		{
			foreach (var department in lstCentralDepartments)
			{
				var lstPeople =
					this._databaseContext.HR_Assignments.Where(a => a.HR_StructurePositions.id_department == department.id_department
																	&& a.IsActive == true
																	&& a.HR_Contracts.IsFired == false).ToList();
				var day = date.Day;
				foreach (var per in lstPeople)
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
			}
			worksheet.Cells[1, 1].Value = date.ToShortDateString();
			//worksheet.Cells.AutoFitColumns(0);
			worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
			worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
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
																					  || d.id_departmentParent == 165).ToList();

			int currentRow = 3;

			ExcelWorksheet worksheet = package.Workbook.Worksheets[6];

			this.PrintStationaryDepartments(date, IsDayShift, lstOtherDepartments, worksheet, currentRow);
		}

		private void PrintOtherRow(ExcelWorksheet worksheet, int currentRow, PFRow pf, HR_Assignments per, bool IsDayShift)
		{
			//Име Специалност Код Раб. време Забележка   Сл.Номер
			worksheet.Cells[currentRow, 1].Value = per.HR_Contracts.UN_Persons.Name;
			worksheet.Cells[currentRow, 2].Value = per.HR_StructurePositions.HR_GlobalPositions.Name;
			worksheet.Cells[currentRow, 3].Value = per.SchedulesCode; //какъв е този код
			if (IsDayShift)
			{
				worksheet.Cells[currentRow, 4].Value = (per.GR_WorkHours == null) ? "" : per.GR_WorkHours.DayHours;
			}
			else
			{
				worksheet.Cells[currentRow, 4].Value = (per.GR_WorkHours == null) ? "" : per.GR_WorkHours.NightHours;
			}

			// 5 is for notes and will remain empty
			worksheet.Cells[currentRow, 6].Value = per.HR_Contracts.TRZCode;
		}

		private void PrintDailyCrewsAndSchedules(DateTime date, bool IsDayShift, UN_Departments baseDepartment, ref int currentRow, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors)
		{
			this.cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == date.Year && a.date.Month == date.Month);
			DateTime pdate = date.AddMonths(-1);
			this.prevCRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == pdate.Year && a.date.Month == pdate.Month);
			if (cRow == null)
			{
				return;
			}

			List<int> lstDepIds = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == baseDepartment.id_department).Select(a => a.id_department).ToList();

			var lstSubDeps = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == baseDepartment.id_department
																			&& a.id_department != a.id_departmentParent).OrderBy(a => a.TreeOrder).ToList();

			ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
			worksheet.Cells[1, 1].Value = date.ToShortDateString();
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = baseDepartment.Name;


			int idx = this.CalculateLeadDepartment(date, baseDepartment.NumberShifts, IsDayShift);
			int id_selectedDepartment = lstSubDeps[idx].id_department;

			var lstPfs = this._databaseContext.GR_PresenceForms.Where(p =>
													p.Date.Year == date.Year
													&& p.Date.Month == date.Month
													&& p.id_scheduleType == (int)ScheduleTypes.DailySchedule
													&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => lstDepIds.Contains(a.HR_StructurePositions.id_department) && a.IsActive == true) != null)
													.ToList();

			List<GR_PresenceForms> lstPfsToRemove = new List<GR_PresenceForms>();

			foreach (var pf in lstPfs)
			{
				var pfRow = new PFRow();
				pfRow.PF = pf;
				if (pfRow[date.Day] != (int)PresenceTypes.DayShift
					&& pfRow[date.Day] != (int)PresenceTypes.NightShift
					&& pfRow[date.Day] != (int)PresenceTypes.RegularShift)
				{
					lstPfsToRemove.Add(pf);
				}
			}

			foreach (var pf in lstPfsToRemove)
			{
				lstPfs.Remove(pf);
			};

			List<PersonnelViewModel> lstAssignments = lstPfs.Select(pf => pf.HR_Contracts.HR_Assignments.Where(a => a.IsActive == true)
			.Select(a => new PersonnelViewModel
			{
				id_person = a.HR_Contracts.id_person,
				Name = a.HR_Contracts.UN_Persons.Name,
				Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
				id_assignment = a.id_assignment,
				id_contract = a.id_contract,
				id_department = a.HR_StructurePositions.id_department,
				WorkHours = a.HR_WorkTime.WorkHours,
				Order = a.HR_StructurePositions.Order,
				WorkZoneDay = (a.GR_WorkHours == null) ? "" : a.GR_WorkHours.DayHours,
				WorkZoneNight = (a.GR_WorkHours == null) ? "" : a.GR_WorkHours.NightHours,
				ShortPosition = a.HR_StructurePositions.HR_GlobalPositions.NameShort,
			}).FirstOrDefault()).ToList();

			var lstDepartmentCrews =
				this.lstCrews.Where(c => c.id_department == id_selectedDepartment).OrderBy(c => c.Name).ToList();

			this.PrintDailyCrews(date, lstDepartmentCrews, lstAssignments, cRow, IsDayShift, ref currentRow);

			this.PrintDailyAssignments(date, lstAssignments, id_selectedDepartment, cRow, IsDayShift, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors);
			//return lstCrewModel;
		}

		private void PrintDailyAssignments(DateTime date, List<PersonnelViewModel> lstAssignments, int id_selectedDepartment, CalendarRow cRow, bool IsDayshift, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors)
		{
			lstAssignments = lstAssignments.OrderBy(a => a.Order).ThenBy(a => a.Name).ToList();
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_selectedDepartment);
			List<CrewScheduleListViewModel> lstStandAlones = new List<CrewScheduleListViewModel>();
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();

				this.FillPersonalCrewScheduleModel(date, (int)ScheduleTypes.DailySchedule, lstAssignments, null, cmv,
					ass.id_assignment, IsDayshift);

				cmv.BaseDepartment = dep.UN_Departments2.Name;
				cmv.id_department = id_selectedDepartment;
				cmv.IsActive = true;
				cmv.CalculateHours();

				lstStandAlones.Add(cmv);
			}
			foreach (var s in lstStandAlones)
			{
				if (IsDayshift)
				{
					if (s[date.Day] == (int)PresenceTypes.DayShift
						|| s[date.Day] == (int)PresenceTypes.RegularShift)
					{
						this.PrintStandAlone(s, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors);
					}
				}
				else
				{
					if (s[date.Day] == (int)PresenceTypes.NightShift)
					{
						this.PrintStandAlone(s, ref currentRowDrivers, ref currentRowSisters, ref currentRowDoctors);
					}
				}
			}
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
					var lstTmpDoctor = this.lstTemporaryCrews.Where(a => a.id_assignment2 == cmv.LstCrewMembers[0].id_assignment
																			 || a.id_assignment4 == cmv.LstCrewMembers[0].id_assignment)
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

		private void PrintDailyCrews(DateTime date, List<GR_Crews2> lstDepartmentCrews, List<PersonnelViewModel> lstAssignments, CalendarRow cRow, bool IsDayShift, ref int currentRow)
		{
			this.currentCrewOrder = 1;
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();
				var lstAssignmentsToRemove = new List<PersonnelViewModel>();

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(date, (int)ScheduleTypes.DailySchedule, lstAssignments, crew, cmv,
						crew.id_assignment1);
					lstAssignmentsToRemove.Add(ass);
					var drAmb = lstDriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.MainAmbulance;
						if (IsDayShift)
						{
							cmv.WorkTime = drAmb.DayHours;
						}
						else
						{
							cmv.WorkTime = drAmb.NightHours;
						}
					}
				}
				else
				{
					continue;
				}

				if (crew.id_assignment2 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, (int)ScheduleTypes.DailySchedule, lstAssignments, crew, cp,
						crew.id_assignment2);
					lstAssignmentsToRemove.Add(ass);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, (int)ScheduleTypes.DailySchedule, lstAssignments, crew, cp,
						crew.id_assignment3);
					lstAssignmentsToRemove.Add(ass);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, (int)ScheduleTypes.DailySchedule, lstAssignments, crew, cp,
						crew.id_assignment4);
					lstAssignmentsToRemove.Add(ass);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}

				if (this.IsCrewFullNoDepart(date, cmv, IsDayShift) == true)
				{
					this.PrintCrew(cmv, ref currentRow);
					this.currentCrewOrder++;
					foreach (var l in lstAssignmentsToRemove)
					{
						lstAssignments.Remove(l);
					}
				}
			}
		}

		private void PrintCrew(CrewScheduleListViewModel cmv, ref int currentRow)
		{
			ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер

			worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
			worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
			if (cmv.id_person != 0)
			{
				worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
				worksheet.Cells[currentRow, 3].Value = cmv.Name;
				worksheet.Cells[currentRow, 4].Value = cmv.ShortPosition;
				//worksheet.Cells[currentRow, 5].Value = cmv.;
				//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
				worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
			}
			//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
			worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
			if (cmv.LstCrewMembers[0].id_person != 0)
			{
				worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[0].Name;
				worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[0].ShortPosition;
				//worksheet.Cells[currentRow, 5].Value = cmv.;
				//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
				worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
				//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
				//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
			}

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
			worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
			if (cmv.LstCrewMembers[1].id_person != 0)
			{
				//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
				worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[1].Name;
				worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[1].ShortPosition;
				//worksheet.Cells[currentRow, 5].Value = cmv.;
				//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
				worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
				//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
				//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
			}

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = this.currentCrewOrder;
			worksheet.Cells[currentRow, 11].Value = cmv.CrewName;
			if (cmv.LstCrewMembers[2].id_person != 0)
			{
				//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
				worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[2].Name;
				worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[2].ShortPosition;
				//worksheet.Cells[currentRow, 5].Value = cmv.;
				//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
				worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
				//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
				//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
			}
		}

		private void PrintStandAlone(CrewScheduleListViewModel cmv, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors)
		{
			ExcelWorksheet worksheet;
			int currentRow = 0;
			switch (cmv.ShortPosition)
			{
				case "Л":
					currentRowDoctors++;
					currentRow = currentRowDoctors;
					worksheet = package.Workbook.Worksheets[4];
					break;
				case "Ф":
					currentRowSisters++;
					currentRow = currentRowSisters;
					worksheet = package.Workbook.Worksheets[3];
					break;
				case "Мс":
					currentRowSisters++;
					currentRow = currentRowSisters;
					worksheet = package.Workbook.Worksheets[3];
					break;
				case "Ак":
					currentRowSisters++;
					currentRow = currentRowSisters;
					worksheet = package.Workbook.Worksheets[3];
					break;
				case "С":
					currentRowSisters++;
					currentRow = currentRowSisters;
					worksheet = package.Workbook.Worksheets[3];
					break;
				case "Ш":
					currentRowDrivers++;
					currentRow = currentRowDrivers;
					worksheet = package.Workbook.Worksheets[2];

					//Име	Специалност	Код	Номер кола	Раб. време	Забележка	Сл. Номер
					worksheet.Cells[currentRow, 1].Value = cmv.Name;
					worksheet.Cells[currentRow, 2].Value = cmv.ShortPosition;
					//worksheet.Cells[currentRow, 3].Value = cmv.;
					worksheet.Cells[currentRow, 4].Value = cmv.RegNumber;
					worksheet.Cells[currentRow, 5].Value = cmv.WorkTime;
					return;
					break;
				default:
					currentRowDrivers++;
					currentRow = currentRowDrivers;
					worksheet = package.Workbook.Worksheets[2];
					break;
			}
			worksheet.Cells[currentRow, 1].Value = cmv.Name;
			worksheet.Cells[currentRow, 2].Value = cmv.ShortPosition;
			worksheet.Cells[currentRow, 5].Value = cmv.WorkTime;
			worksheet.Cells[currentRow, 8].Value = cmv.BaseDepartment;
		}

	}
}
