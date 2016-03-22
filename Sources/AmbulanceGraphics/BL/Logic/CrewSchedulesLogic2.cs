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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class CrewSchedulesLogic2 : SchedulesLogic
	{
		private readonly List<GR_Crews> lstCrews;
		private readonly List<GR_PresenceForms> lstPresenceForms;
		private readonly List<GR_ShiftTypes> lstShiftTypes;
		private readonly List<PersonnelViewModel> lstAllAssignments;
		private readonly List<DriverAmbulancesViewModel> lstDriverAmbulances;
		private readonly List<CalendarRow> lstCalendarRows;
		public CrewSchedulesLogic2()
		{
			var date = DateTime.Now;

			var dateStart = new DateTime(date.AddMonths(-6).Year, date.AddMonths(-6).Month, 1);
			var dateEnd = dateStart.AddYears(1);

			this.lstCrews = this._databaseContext.GR_Crews.ToList();

			this.lstPresenceForms = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year >= dateStart.Year
																					 && p.Date.Year <= dateEnd.Year
																					 ).ToList();

			this.lstShiftTypes = this._databaseContext.GR_ShiftTypes.OrderBy(s => s.id_shiftType).ToList();

			this.lstAllAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true)
				.Select(a => new PersonnelViewModel
				{
					Name = a.HR_Contracts.UN_Persons.Name,
					Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
					id_assignment = a.id_assignment,
					id_contract = a.id_contract,
					id_department = a.HR_StructurePositions.id_department,
					WorkHours = a.HR_WorkTime.WorkHours,
					id_positionType = a.HR_StructurePositions.HR_GlobalPositions.id_positionType,
				}).ToList();

			this.lstDriverAmbulances = this._databaseContext.GR_DriverAmbulances.
				Select(a => new DriverAmbulancesViewModel()
				{
					MainAmbulance = a.GR_Ambulances.Name,
					WorkTime = a.GR_Ambulances.WorkTime,
					id_driverAssignment = a.id_driverAssignment
				}
				).ToList();

			this.lstCalendarRows = new List<CalendarRow>();

			var currentDate = dateStart;
			using (NomenclaturesLogic logic = new NomenclaturesLogic())
			{
				while (currentDate <= dateEnd)
				{
					lstCalendarRows.Add(logic.FillCalendarRow(currentDate));
					currentDate = currentDate.AddMonths(1);
				}
			}
		}

		public List<CrewListViewModel> GetDepartmentCrews(int id_selectedDepartment, DateTime? Date = null)
		{
			var lstDepartmentCrews = this.lstCrews.Where(c => c.IsActive == true && c.id_department == id_selectedDepartment)
																.OrderBy(c => c.IsTemporary)
																.ThenBy(c => c.Name)
																.ToList();

			List<CrewListViewModel> lstCrewModel = new List<CrewListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewListViewModel();

				if (crew.id_assignment1 != null)
				{
					this.FillCrewListViewModel(crew, cmv, crew.id_assignment1);

					var drAmb = this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == crew.id_assignment1);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.GR_Ambulances.Name;
						cmv.WorkTime = drAmb.GR_Ambulances.WorkTime;
					}
				}
				else
				{
					//int.TryParse(crew.Name, cmv.CrewName)
					cmv.CrewName = crew.Name.ToString();
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
				}
				lstCrewModel.Add(cmv);
				if (crew.id_assignment2 != null)
				{
					var cp = new CrewListViewModel();
					this.FillCrewListViewModel(crew, cp, crew.id_assignment2);
					cp.WorkTime = cmv.WorkTime;
					cp.CrewName = "      " + cp.CrewName;
					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewListViewModel();
					this.FillCrewListViewModel(crew, cp, crew.id_assignment3);
					cp.WorkTime = cmv.WorkTime;
					cp.CrewName = "      " + cp.CrewName;
					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewListViewModel();
					this.FillCrewListViewModel(crew, cp, crew.id_assignment4);
					cp.WorkTime = cmv.WorkTime;
					cp.CrewName = "      " + cp.CrewName;
					lstCrewModel.Add(cp);
				}
			}
			return lstCrewModel;
		}

		private void FillCrewListViewModel(GR_Crews crew, CrewListViewModel cmv, int? id_assignment)
		{
			var ass = lstAllAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
			if (ass == null)
			{
				return;
			}
			cmv.CrewName = crew.Name.ToString();
			cmv.CrewType = crew.NM_CrewTypes.Name;
			cmv.id_crew = crew.id_crew;
			cmv.id_department = crew.id_department;
			cmv.IsActive = crew.IsActive;
			cmv.Name = ass.Name;
			cmv.Position = ass.Position;
			cmv.IsTemporary = crew.IsTemporary;
			if (crew.Date.HasValue)
			{
				cmv.CrewDate = crew.Date.Value.ToShortDateString();
			}
		}

		public List<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime date, int id_scheduleType = 1)
		{
			CalendarRow cRow;

			cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == date.Year && a.date.Month == date.Month);
			if (cRow == null)
			{
				return null;
			}

			var lstAssignments = lstAllAssignments.Where(a => a.id_department == id_selectedDepartment)
				.Select(a => new PersonnelViewModel
				{
					Name = a.Name,
					Position = a.Position,
					id_assignment = a.id_assignment,
					id_contract = a.id_contract,
					id_department = a.id_department,
					WorkHours = a.WorkHours,
				})
				.ToList();

			var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment).ToList();

			List<CrewScheduleListViewModel> lstCrewModel = new List<CrewScheduleListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel();

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cmv, cRow, crew.id_assignment1);
					var drAmb = lstDriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.Name;
						cmv.WorkTime = drAmb.WorkTime;
					}
				}
				else
				{
					cmv.CrewName = crew.Name.ToString();
					cmv.State = crew.Name % 2;
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
					cmv.lstShiftTypes = lstShiftTypes;
					cmv.RealDate = date;
				}
				lstCrewModel.Add(cmv);

				if (crew.id_assignment2 != null)
				{
					var cp = new CrewScheduleListViewModel();
					this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment2);
					cp.WorkTime = cmv.WorkTime;
					cp.CrewName = "      " + cp.CrewName;
					//cmv.lstCrewMembers.Add(cp);
					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewScheduleListViewModel();
					this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment3);
					cp.WorkTime = cmv.WorkTime;
					cp.CrewName = "      " + cp.CrewName;
					//cmv.lstCrewMembers.Add(cp);
					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewScheduleListViewModel();
					this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment4);
					cp.WorkTime = cmv.WorkTime;
					cp.CrewName = "        " + cp.CrewName;
					//cmv.lstCrewMembers.Add(cp);
					lstCrewModel.Add(cp);
				}
				//lstCrewModel.Add(cmv);
			}
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				//cmv.lstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>();

				cmv.CrewName = "";
				cmv.CrewType = "";
				cmv.id_crew = 0;
				cmv.id_department = id_selectedDepartment;
				cmv.IsActive = true;
				cmv.Name = ass.Name;
				cmv.Position = ass.Position;
				cmv.lstShiftTypes = lstShiftTypes;

				cmv.PF = lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
																					&& p.Date.Month == date.Month
																					&& p.id_contract == ass.id_contract
																					&& p.id_scheduleType == id_scheduleType);
				cmv.RealDate = date;
				cmv.CalculateHours();

				cmv.RegNumber = "стационар";
				lstCrewModel.Add(cmv);
			}
			return lstCrewModel;
		}

		private PersonnelViewModel FillPersonalCrewScheduleModel(DateTime date, int id_scheduleType, List<PersonnelViewModel> lstAssignments, GR_Crews crew, CrewScheduleListViewModel cmv, CalendarRow cRow, int? id_assignment)
		{
			var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
			if (ass == null)
			{
				ass = lstAllAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
			}
			if (ass == null)
			{
				return null;
			}
			cmv.CrewName = crew.Name.ToString();
			cmv.State = crew.Name%2;
			cmv.CrewType = crew.NM_CrewTypes.Name;
			cmv.id_crew = crew.id_crew;
			cmv.id_department = crew.id_department;
			cmv.IsActive = crew.IsActive;
			cmv.Name = ass.Name;
			cmv.Position = ass.Position;
			cmv.lstShiftTypes = lstShiftTypes;
			cmv.RowPosition = 1;
			cmv.RealDate = date;
			cmv.IsTemporary = crew.IsTemporary;
			cmv.id_crewType = crew.id_crewType;
			if (crew.Date.HasValue)
			{
				cmv.CrewDate = crew.Date.Value.ToShortDateString();
			}

			cmv.PF = lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
													  && p.Date.Month == date.Month
														  && p.id_contract == ass.id_contract
														  && p.id_scheduleType == id_scheduleType);
			if (ass.WorkHours == null)
			{
				ass.WorkHours = 0;
			}
			cmv.Norm = cRow.WorkDays * (double)ass.WorkHours;
			cmv.CalculateHours();
			lstAssignments.Remove(ass);
			return ass;
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
						worksheet.Cells[1, 1].Value = "№";
						worksheet.Cells[1, 2].Value = "Линейка";
						worksheet.Cells[1, 3].Value = "Име";
						worksheet.Cells[1, 4].Value = "Рв";
						worksheet.Cells[1, 5].Value = "Длъжност";
						int col;
						for (col = 1; col <= DateTime.DaysInMonth(date.Year, date.Month); col++)
						{
							worksheet.Cells[1, col + 5].Value = col.ToString();
						}
						//Д	Н	изр.ч.	Ч.+	Ч.-
						worksheet.Cells[1, 5 + col + 1].Value = "Д";
						worksheet.Cells[1, 5 + col + 2].Value = "Н";
						worksheet.Cells[1, 5 + col + 3].Value = "изр.ч.";
						worksheet.Cells[1, 5 + col + 4].Value = "Ч+";
						worksheet.Cells[1, 5 + col + 5].Value = "Ч-";
						List<UN_Departments> subDeps = lstAllDepartments.Where(d => d.id_departmentParent == department.id_department
																			&& d.NumberShifts < 2)
								.OrderBy(a => a.TreeOrder).ToList();
						foreach (var dep in subDeps)
						{
							currentRow++;
							worksheet.Cells[currentRow, 3].Value = dep.Name;
							currentRow++;

							var recModel = new CrewRecapitulation();
							var lstCrewSchedules = this.GetDepartmentCrewsAndSchedulesByCrew(dep.id_department, date, (int)scheduleType);//.OrderBy(c => c.CrewName).ThenBy(c => c.RowPosition);
							foreach (var crew in lstCrewSchedules)
							{
								currentRow++;
								this.PrintScheduleRow(worksheet, crew, currentRow, date);
								currentRow++;

								foreach (var cr in crew.LstCrewMembers)
								{
									this.PrintScheduleRow(worksheet, cr, currentRow, date);
									currentRow++;
								}

								if (crew.LstCrewMembers.Count > 0)
								{
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
						}
						worksheet.Cells.AutoFitColumns(0);
					}
				}
				package.Save();
			}
		}

		public void PrintRecapitulation(ExcelWorksheet worksheet, CrewRecapitulation recModel, int crow, out int currentRow)
		{
			currentRow = crow;

			currentRow++;

			worksheet.Cells[currentRow, 3].Value = "брой ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.DoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.DoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.MedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.MedicalCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой рез шофьори по дежурства";
			worksheet.Cells[currentRow, 4].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой рез шофьори по дежурства";
			worksheet.Cells[currentRow, 4].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDriversW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой изв. ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой изв. ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой изв. ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalMedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой изв. ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 4].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalMedicalCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой изв шоф по дежурства";
			worksheet.Cells[currentRow, 4].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.DisasterDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 3].Value = "брой изв шоф по дежурства";
			worksheet.Cells[currentRow, 4].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.DisasterDriversW8, currentRow);

			currentRow++;
		}

		private void PrintRecapitulationDayCount(ExcelWorksheet worksheet, int[] recRow, int crow)
		{
			for (int col = 1; col <= 31; col++)
			{
				worksheet.Cells[crow, col + 5].Value = recRow[col];
			}
		}

		public void InsertInRecapitulation(CrewScheduleListViewModel crew, int[] rec)
		{
			for (int i = 1; i < 32; i++)
			{
				if (crew[i] != 0)
				{
					rec[i-1]++;
				}
			}
		}

		public void PrintScheduleRow(ExcelWorksheet worksheet, CrewScheduleListViewModel crew, int currentRow, DateTime date)
		{
			worksheet.Cells[currentRow, 1].Value = crew.CrewName;
			worksheet.Cells[currentRow, 2].Value = crew.RegNumber;
			worksheet.Cells[currentRow, 3].Value = crew.Name;
			worksheet.Cells[currentRow, 4].Value = crew.WorkTime;
			worksheet.Cells[currentRow, 5].Value = crew.Position;
			if (crew.PF == null)
			{
				return;
			}
			int col;
			for (col = 1; col <= DateTime.DaysInMonth(date.Year, date.Month); col++)
			{
				var shift = crew.lstShiftTypes.FirstOrDefault(a => a.id_shiftType == crew[col + 1]);
				if (shift != null)
				{
					worksheet.Cells[currentRow, col + 5].Value = shift.Name;
				}
			}
			//Д	Н	изр.ч.	Ч.+	Ч.-
			//worksheet.Cells[currentRow, 4 + col + 1].Value = "Д";
			//worksheet.Cells[currentRow, 4 + col + 2].Value = "Н";
			worksheet.Cells[currentRow, 5 + col + 3].Value = crew.Shifts;
			//worksheet.Cells[currentRow, 4 + col + 4].Value = "Ч+";
			//worksheet.Cells[currentRow, 4 + col + 5].Value = "Ч-";
		}

		public ObservableCollection<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedulesByCrew(int id_selectedDepartment, DateTime date, int id_scheduleType = 1)
		{
			CalendarRow cRow;

			cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == date.Year && a.date.Month == date.Month);
			if (cRow == null)
			{
				return null;
			}

			var lstAssignments = lstAllAssignments.Where(a => a.id_department == id_selectedDepartment)
				.Select(a => new PersonnelViewModel
				{
					Name = a.Name,
					Position = a.Position,
					id_assignment = a.id_assignment,
					id_contract = a.id_contract,
					id_department = a.id_department,
					WorkHours = a.WorkHours,
				})
				.ToList();

			var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment).ToList();

			ObservableCollection<CrewScheduleListViewModel> lstCrewModel = new ObservableCollection<CrewScheduleListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel { LstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>() };

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cmv, cRow, crew.id_assignment1);
					var drAmb = lstDriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.Name;
						cmv.WorkTime = drAmb.WorkTime;
					}
				}
				else
				{
					cmv.CrewName = crew.Name.ToString();
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
					cmv.lstShiftTypes = lstShiftTypes;
					cmv.RealDate = date;
				}

				if (crew.id_assignment2 != null)
				{
					var cp = new CrewScheduleListViewModel();
					this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment2);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewScheduleListViewModel();
					this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment3);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewScheduleListViewModel();
					this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment4);
					cp.WorkTime = cmv.WorkTime;
					cmv.LstCrewMembers.Add(cp);
				}
				lstCrewModel.Add(cmv);
			}
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>();

				cmv.CrewName = "";
				cmv.CrewType = "";
				cmv.id_crew = 0;
				cmv.id_department = id_selectedDepartment;
				cmv.IsActive = true;
				cmv.Name = ass.Name;
				cmv.Position = ass.Position;
				cmv.lstShiftTypes = lstShiftTypes;

				cmv.PF = lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
																					&& p.Date.Month == date.Month
																					&& p.id_contract == ass.id_contract
																					&& p.id_scheduleType == id_scheduleType);
				cmv.RealDate = date;
				cmv.CalculateHours();

				cmv.RegNumber = "стационар";
				lstCrewModel.Add(cmv);
			}
			return lstCrewModel;
		}
	}
}
