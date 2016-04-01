using BL.DB;
using BL.Models;
using System;
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
		private ExcelPackage package;
        public void ExportSingleDepartmentMonthlySchedule(string fileName, DateTime date,
			ScheduleTypes scheduleType, int id_department)
		{
			if (scheduleType != ScheduleTypes.FinalMonthSchedule && scheduleType != ScheduleTypes.ForecastMonthSchedule)
			{
				return;
			}

			var department = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);
			if (department == null)
			{
				return;
			}

			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete(); // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}

			using (ExcelPackage package = new ExcelPackage(newFile))
			{
				// add a new worksheet to the empty workbook
				int currentRow = 2;
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.Name);
				//Add the headers
				this.PrintColumnHeaders(date, worksheet);

				this.ExportCurrentDepartment(date, scheduleType, currentRow, worksheet, department);

				worksheet.Cells.AutoFitColumns(0);
				worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
				worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
				worksheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");
				worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
				worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
				worksheet.Cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
				worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
				worksheet.Cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
				worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
				worksheet.Cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

				package.Save();
			}
		}

		private void PrintColumnHeaders(DateTime date, ExcelWorksheet worksheet)
		{
			worksheet.Cells[1, 1].Value = "№";
			worksheet.Cells[1, 2].Value = "Линейка";
			worksheet.Cells[1, 3].Value = "Data";
			worksheet.Cells[1, 4].Value = "Име";
			worksheet.Cells[1, 5].Value = "Рв";
			worksheet.Cells[1, 6].Value = "Длъжност";
			int col;
			for (col = 1; col <= DateTime.DaysInMonth(date.Year, date.Month); col++)
			{
				worksheet.Cells[1, col + 6].Value = col.ToString();
			}
			//Д	Н	изр.ч.	Ч.+	Ч.-
			worksheet.Cells[1, 6 + col + 1].Value = "Д";
			worksheet.Cells[1, 6 + col + 2].Value = "Н";
			worksheet.Cells[1, 6 + col + 3].Value = "изр.ч.";
			worksheet.Cells[1, 6 + col + 4].Value = "Ч+";
			worksheet.Cells[1, 6 + col + 5].Value = "Ч-";
		}

		public void ExportMonthlySchedule(string fileName, string header, DateTime date, ScheduleTypes scheduleType)
		{
			if (scheduleType != ScheduleTypes.FinalMonthSchedule && scheduleType != ScheduleTypes.ForecastMonthSchedule)
			{
				return;
			}

			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete();  // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}

			using (ExcelPackage package = new ExcelPackage(newFile))
			{
				// add a new worksheet to the empty workbook
				using (var logic = new SchedulesLogic())
				{
					var lstAllDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive).ToList();
					var lstDepartments = lstAllDepartments.Where(d => d.IsActive == true
																		&& d.NumberShifts > 1).ToList();

					foreach (var department in lstDepartments)
					{
						int currentRow = 2;
						ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.Name);
						//Add the headers
						this.PrintColumnHeaders(date, worksheet);
						List<UN_Departments> subDeps = lstAllDepartments.Where(d => d.id_departmentParent == department.id_department
																			&& d.NumberShifts < 2)
								.OrderBy(a => a.TreeOrder).ToList();
						foreach (var dep in subDeps)
						{
							currentRow = ExportCurrentDepartment(date, scheduleType, currentRow, worksheet, dep);
						}
						worksheet.Cells.AutoFitColumns(0);
						worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
						worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
						worksheet.PrinterSettings.RepeatRows = new ExcelAddress(1, 1, 1, 50);
					}
				}
				package.Save();
			}
		}

		private int ExportCurrentDepartment(DateTime date, ScheduleTypes scheduleType, int currentRow, ExcelWorksheet worksheet,
			UN_Departments dep)
		{
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = dep.Name;
			currentRow++;

			var recModel = new CrewRecapitulation();
			var lstCrewSchedules = this.GetDepartmentCrewsAndSchedules(dep.id_department, date, (int)scheduleType);
			//.OrderBy(c => c.CrewName).ThenBy(c => c.RowPosition);
			foreach (var crew in lstCrewSchedules)
			{
				if (crew.LstCrewMembers != null && crew.LstCrewMembers.Count > 0)
				{
					currentRow++;
				}
				
				this.PrintScheduleRow(worksheet, crew, currentRow, date);
				currentRow++;

				if (crew.LstCrewMembers != null && crew.LstCrewMembers.Count > 0)
				{
					//foreach (var cr in crew.LstCrewMembers)
					//{
					//	this.PrintScheduleRow(worksheet, cr, currentRow, date);
					//	currentRow++;
					//}
					if (crew.IsTemporary == true)
					{
						switch ((CrewTypes)crew.id_crewType)
						{
							case CrewTypes.Reanimation:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalDoctorCrewsW7);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalDoctorCrewsW8);
								}
								break;
							case CrewTypes.Doctor:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalDoctorCrewsW7);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalDoctorCrewsW8);
								}
								break;
							case CrewTypes.Paramedic:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalMedicalCrewsW7);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalMedicalCrewsW8);
								}
								break;
							default:
								break;
						}
					}
					else
					{
						switch ((CrewTypes)crew.id_crewType)
						{
							case CrewTypes.Reanimation:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.DoctorCrewsW7);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.DoctorCrewsW8);
								}
								break;
							case CrewTypes.Doctor:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.DoctorCrewsW7);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.DoctorCrewsW8);
								}
								break;
							case CrewTypes.Paramedic:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.MedicalCrewsW7);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.MedicalCrewsW8);
								}
								break;
							default:
								break;
						}
					}
				}
				else
				{
					if (crew.id_positionType == (int)PositionTypes.Driver)
					{
						if (crew.WorkTime == "7-19")
						{
							this.InsertInRecapitulation(crew, recModel.AdditionalDriversW7);
						}
						else
						{
							this.InsertInRecapitulation(crew, recModel.AdditionalDriversW8);
						}
					}
				}
			}
			this.PrintRecapitulation(worksheet, recModel, currentRow, out currentRow);
			return currentRow;
		}

		private void PrintRecapitulation(ExcelWorksheet worksheet, CrewRecapitulation recModel, int crow, out int currentRow)
		{
			currentRow = crow;

			currentRow++;

			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.DoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.DoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.MedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.MedicalCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDriversW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalMedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalMedicalCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв шоф по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.DisasterDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв шоф по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.DisasterDriversW8, currentRow);

			currentRow++;
		}

		private void PrintRecapitulationDayCount(ExcelWorksheet worksheet, int[] recRow, int currentRow)
		{
			for (int col = 0; col < 31; col++)
			{
				worksheet.Cells[currentRow, col + 7].Value = recRow[col];
			}
		}

		private void InsertInRecapitulation(CrewScheduleListViewModel crew, int[] rec)
		{
			for (int i = 1; i < 32; i++)
			{
				if (crew[i] != 0)
				{
					rec[i - 1]++;
				}
			}
		}

		private void PrintScheduleRow(ExcelWorksheet worksheet, CrewScheduleListViewModel crew, int currentRow, DateTime date)
		{
			worksheet.Cells[currentRow, 1].Value = crew.CrewName;
			worksheet.Cells[currentRow, 2].Value = crew.RegNumber;
			worksheet.Cells[currentRow, 3].Value = crew.CrewDate;
			worksheet.Cells[currentRow, 4].Value = crew.Name;
			worksheet.Cells[currentRow, 5].Value = crew.WorkTime;
			worksheet.Cells[currentRow, 6].Value = crew.Position;
			if (crew.PF == null)
			{
				return;
			}
			int col;
			if (crew.IsTemporary == true)
			{
				DateTime da;
				if (DateTime.TryParse(crew.CrewDate, out da))
				{
					col = da.Day;
					var shift = crew.lstShiftTypes.FirstOrDefault(a => a.id_shiftType == crew[col]);
					if (shift != null)
					{
						worksheet.Cells[currentRow, col + 6].Value = shift.Name;
					}
				}
			}
			else
			{
				for (col = 1; col <= DateTime.DaysInMonth(date.Year, date.Month); col++)
				{
					var shift = crew.lstShiftTypes.FirstOrDefault(a => a.id_shiftType == crew[col]);
					if (shift != null)
					{
						worksheet.Cells[currentRow, col + 6].Value = shift.Name;
					}
				}
			}

			col = DateTime.DaysInMonth(date.Year, date.Month) + 1;

			//Д	Н	изр.ч.	Ч.+	Ч.-
			worksheet.Cells[currentRow, 6 + col + 1].Value = crew.CountDayShifts;
			worksheet.Cells[currentRow, 6 + col + 2].Value = crew.CountNightShifts;
			worksheet.Cells[currentRow, 6 + col + 3].Value = crew.Shifts;
			if (crew.Difference > 0)
			{
				worksheet.Cells[currentRow, 6 + col + 4].Value = crew.Difference;
			}
			else if (crew.Difference < 0)
			{
				worksheet.Cells[currentRow, 6 + col + 5].Value = crew.Difference;
			}
		}

		public void ExportDailyDepartmentSchedule(string fileName, DateTime date)
		{
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

			int currentRow = 3;
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
						if (pf[day] == (int) PresenceTypes.DayShift || pf[day] == (int) PresenceTypes.RegularShift)
						{
							this.PrintOtherRow(worksheet, currentRow, pf, per);
							currentRow++;
						}
					}
					else
					{
						if (pf[day] == (int) PresenceTypes.NightShift)
						{
							this.PrintOtherRow(worksheet, currentRow, pf, per);
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
			                                                                          || d.id_departmentParent == 19
			                                                                          || d.id_departmentParent == 20
			                                                                          || d.id_departmentParent == 21
			                                                                          || d.id_departmentParent == 22).ToList();

			int currentRow = 3;

			ExcelWorksheet worksheet = package.Workbook.Worksheets[6];

			this.PrintStationaryDepartments(date, IsDayShift, lstOtherDepartments, worksheet, currentRow);
		}

		private void PrintOtherRow(ExcelWorksheet worksheet, int currentRow, PFRow pf, HR_Assignments per)
		{
			//Име Специалност Код Раб. време Забележка   Сл.Номер
			worksheet.Cells[currentRow, 1].Value = per.HR_Contracts.UN_Persons.Name;
			worksheet.Cells[currentRow, 2].Value = per.HR_StructurePositions.HR_GlobalPositions.Name;
			worksheet.Cells[currentRow, 3].Value = per.SchedulesCode; //какъв е този код
			worksheet.Cells[currentRow, 4].Value = ""; //not yet available
			// 5 is for notes and will remain empty
			worksheet.Cells[currentRow, 6].Value = per.HR_Contracts.TRZCode;
		}

		private void PrintDailyCrewsAndSchedules(DateTime date, bool IsDayShift, UN_Departments baseDepartment, ref int currentRow, ref int currentRowDrivers, ref int currentRowSisters, ref int currentRowDoctors)
		{
			CalendarRow cRow;

			cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == date.Year && a.date.Month == date.Month);
			if (cRow == null)
			{
				return;
			}

			List<int> lstDepIds = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == baseDepartment.id_department).Select(a => a.id_department).ToList();

			var lstSubDeps = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == baseDepartment.id_department 
																			&& a.id_department != a.id_departmentParent).OrderBy(a => a.TreeOrder).ToList();

			ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = baseDepartment.Name;

			int id_selectedDepartment = lstSubDeps[this.CalculateLeadShift(date, IsDayShift)].id_department;

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
				if (pfRow[date.Day] != (int) PresenceTypes.DayShift
				    && pfRow[date.Day] != (int) PresenceTypes.NightShift
				    && pfRow[date.Day] != (int) PresenceTypes.RegularShift)
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

				this.FillPersonalCrewScheduleModel(date, (int) ScheduleTypes.DailySchedule, lstAssignments, null, cmv, cRow,
					ass.id_assignment);

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
					if (s[date.Day] == (int) PresenceTypes.DayShift
					    || s[date.Day] == (int) PresenceTypes.RegularShift)
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

		private bool IsCrewFull(DateTime date, CrewScheduleListViewModel cmv, bool IsDayShift)
		{
			var ct = (CrewTypes)cmv.id_crewType;

			if (cmv.IsTemporary == true)
			{
				DateTime cd = new DateTime(1900,1,1);
				DateTime.TryParse(cmv.CrewDate, out cd);
				if (cd != date)
				{
					return false;
				}
			}
			switch (ct)
			{
				case CrewTypes.Reanimation:
					if (cmv.LstCrewMembers[0].id_person == 0
					    || cmv.LstCrewMembers[1].id_person == 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv[date.Day] != (int) PresenceTypes.DayShift
						    && cmv[date.Day] != (int) PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int) PresenceTypes.DayShift
						    && cmv.LstCrewMembers[0][date.Day] != (int) PresenceTypes.RegularShift)
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
						if (cmv[date.Day] != (int) PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[1][date.Day] != (int)PresenceTypes.NightShift)
						{
							return false;
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
					if (cmv.LstCrewMembers[2].id_person == 0)
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
							return false;
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
							return false;
						}
					}
					break;
				default:
					return false;
			}
			return true;
		}

		private void PrintDailyCrews(DateTime date, List<GR_Crews> lstDepartmentCrews, List<PersonnelViewModel> lstAssignments, CalendarRow cRow, bool IsDayShift, ref int currentRow)
		{
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();
				var lstAssignmentsToRemove = new List<PersonnelViewModel>();

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(date, (int) ScheduleTypes.DailySchedule, lstAssignments, crew, cmv, cRow,
						crew.id_assignment1);
					lstAssignmentsToRemove.Add(ass);
					var drAmb = lstDriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.MainAmbulance;
						cmv.WorkTime = drAmb.WorkTime;
					}
				}
				else
				{
					continue;
				}

				if (crew.id_assignment2 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, (int) ScheduleTypes.DailySchedule, lstAssignments, crew, cp, cRow,
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
					var ass = this.FillPersonalCrewScheduleModel(date, (int) ScheduleTypes.DailySchedule, lstAssignments, crew, cp, cRow,
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
					var ass = this.FillPersonalCrewScheduleModel(date, (int) ScheduleTypes.DailySchedule, lstAssignments, crew, cp, cRow,
						crew.id_assignment4);
					lstAssignmentsToRemove.Add(ass);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}

				if (this.IsCrewFull(date, cmv, IsDayShift) == true)
				{
					this.PrintCrew(cmv, ref currentRow);
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

			currentRow ++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = cmv.CrewName;
			worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
			worksheet.Cells[currentRow, 3].Value = cmv.Name;
			worksheet.Cells[currentRow, 4].Value = cmv.ShortPosition;
			//worksheet.Cells[currentRow, 5].Value = cmv.;
			//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
			worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
			//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
			worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[0].Name;
			worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[0].ShortPosition;
			//worksheet.Cells[currentRow, 5].Value = cmv.;
			//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
			worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
			//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
			worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[1].Name;
			worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[1].ShortPosition;
			//worksheet.Cells[currentRow, 5].Value = cmv.;
			//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
			worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
			//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;

			currentRow++;
			//No екип	Номер кола	Име	Специалност	Код	натовар-ване	Раб. време	Забележка	Сл. номер
			worksheet.Cells[currentRow, 1].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 2].Value = cmv.RegNumber;
			worksheet.Cells[currentRow, 3].Value = cmv.LstCrewMembers[2].Name;
			worksheet.Cells[currentRow, 4].Value = cmv.LstCrewMembers[2].ShortPosition;
			//worksheet.Cells[currentRow, 5].Value = cmv.;
			//worksheet.Cells[currentRow, 6].Value = cmv.CrewName;
			worksheet.Cells[currentRow, 7].Value = cmv.WorkTime;
			//worksheet.Cells[currentRow, 8].Value = cmv.CrewName;
			//worksheet.Cells[currentRow, 9].Value = cmv.CrewName;
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
					break;
				default:
					currentRowDrivers++;
					currentRow = currentRowDrivers;
					worksheet = package.Workbook.Worksheets[2];
					break;
			}
			worksheet.Cells[currentRow, 1].Value = cmv.Name;
			worksheet.Cells[currentRow, 2].Value = cmv.ShortPosition;
		}
	}
}
