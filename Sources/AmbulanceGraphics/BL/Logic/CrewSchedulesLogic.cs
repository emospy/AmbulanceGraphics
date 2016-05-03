﻿using BL.DB;
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
using System.Xml.Serialization.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class CrewSchedulesLogic : SchedulesLogic
	{
		internal List<GR_Crews2> lstCrews;
		internal List<GR_PresenceForms> lstPresenceForms;
		internal readonly List<GR_ShiftTypes> lstShiftTypes;
		internal readonly List<PersonnelViewModel> lstAllAssignments;
		internal readonly List<DriverAmbulancesViewModel> lstDriverAmbulances;
		internal readonly List<CalendarRow> lstCalendarRows;
		internal readonly List<GR_WorkTimeAbsence> lstWorktimeAbsenceces;

		internal CalendarRow cRow;
		internal CalendarRow prevCRow;

		public CrewSchedulesLogic()
		{
			var date = DateTime.Now;

			var dateStart = new DateTime(date.AddMonths(-6).Year, date.AddMonths(-6).Month, 1);
			var dateEnd = dateStart.AddYears(1);

			this.lstCrews = this._databaseContext.GR_Crews2.ToList();

			this.lstPresenceForms = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year >= dateStart.Year
																					  && p.Date.Year <= dateEnd.Year
				).ToList();

			this.lstShiftTypes = this._databaseContext.GR_ShiftTypes.OrderBy(s => s.id_shiftType).ToList();

			this.lstAllAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true
																					&& a.HR_Contracts.IsFired == false)
				.Select(a => new PersonnelViewModel
				{
					id_person = a.HR_Contracts.id_person,
					Name = a.HR_Contracts.UN_Persons.Name,
					Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
					id_assignment = a.id_assignment,
					id_contract = a.id_contract,
					id_department = a.HR_StructurePositions.id_department,
					WorkHours = a.HR_WorkTime.WorkHours,
					WorkZoneDay = a.GR_WorkHours.DayHours,
					WorkZoneNight = a.GR_WorkHours.NightHours,
					id_positionType = a.HR_StructurePositions.HR_GlobalPositions.id_positionType,
					Order = a.HR_StructurePositions.Order,
					ShortPosition = a.HR_StructurePositions.HR_GlobalPositions.NameShort,
					IsSumWorkTime = a.GR_WorkHours.IsSumWorkTime,
					AssignedAt =  a.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false).AssignmentDate,
				}).ToList();

			this.lstDriverAmbulances = this._databaseContext.GR_DriverAmbulances.
				Select(a => new DriverAmbulancesViewModel()
				{
					MainAmbulance = a.GR_Ambulances.Name,
					WorkTime = a.GR_Ambulances.GR_WorkHours.DayHours + " " + a.GR_Ambulances.GR_WorkHours.NightHours,
					id_driverAssignment = a.id_driverAssignment,
					DayHours = a.GR_Ambulances.GR_WorkHours.DayHours,
					NightHours = a.GR_Ambulances.GR_WorkHours.NightHours
				}
				).ToList();

			this.lstWorktimeAbsenceces =
				this._databaseContext.GR_WorkTimeAbsence.Where(a => a.Date >= dateStart && a.Date <= dateEnd).ToList();

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

		public void RefreshCrews()
		{
			this._databaseContext = new AmbulanceEntities();
			this.lstCrews = this._databaseContext.GR_Crews2.ToList();
		}

		public void RefreshPresenceFroms()
		{
			this._databaseContext = new AmbulanceEntities();
			var date = DateTime.Now;

			var dateStart = new DateTime(date.AddMonths(-6).Year, date.AddMonths(-6).Month, 1);
			var dateEnd = dateStart.AddYears(1);
			this.lstPresenceForms = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year >= dateStart.Year
																					  && p.Date.Year <= dateEnd.Year
				).ToList();
		}

		public List<CrewListViewModel> GetDepartmentCrews(int id_selectedDepartment, DateTime Date)
		{
			var lstDepartmentCrews = this.lstCrews.Where(c => c.IsActive == true 
												&& c.id_department == id_selectedDepartment
												&& (c.IsTemporary == false 
													|| (c.IsTemporary == true
														&& c.DateStart.Month == Date.Month
														&& c.DateStart.Year == Date.Year)))
				.OrderBy(c => c.IsTemporary)
				.ThenBy(c => c.Name)
				.ToList();

			int counter = 0;
			List<CrewListViewModel> lstCrewModel = new List<CrewListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewListViewModel();

				counter++;
				if (crew.id_assignment1 != null)
				{
					this.FillCrewListViewModel(crew, cmv, crew.id_assignment1, counter);

					var drAmb =
						this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == crew.id_assignment1);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.GR_Ambulances.Name;
						cmv.WorkTime = drAmb.GR_Ambulances.GR_WorkHours?.DayHours + " " + drAmb.GR_Ambulances.GR_WorkHours?.NightHours;
					}
				}
				else
				{
					cmv.CrewName = crew.Name.ToString();
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
					cmv.State = crew.Name % 2 == 0 ? "W" : "G";
				}
				lstCrewModel.Add(cmv);
				if (crew.id_assignment2 != null)
				{
					var cp = new CrewListViewModel();
					this.FillCrewListViewModel(crew, cp, crew.id_assignment2, counter);
					cp.WorkTime = cmv.WorkTime;
					//cp.CrewName = "      " + cp.CrewName;

					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewListViewModel();
					this.FillCrewListViewModel(crew, cp, crew.id_assignment3, counter);
					cp.WorkTime = cmv.WorkTime;
					//cp.CrewName = "      " + cp.CrewName;

					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewListViewModel();
					this.FillCrewListViewModel(crew, cp, crew.id_assignment4, counter);
					cp.WorkTime = cmv.WorkTime;
					//cp.CrewName = "      " + cp.CrewName;
					lstCrewModel.Add(cp);
				}
			}
			lstCrewModel = lstCrewModel.OrderBy(a => a.WorkTime).ToList();
			this.SetCrewModelColors(lstCrewModel);
			return lstCrewModel;
		}

		private void SetCrewModelColors(List<CrewListViewModel> lstModels)
		{
			var groups = lstModels.GroupBy(c => c.CrewName);
			int counter = 0;
			foreach (var group in groups)
			{
				foreach (var elm in group)
				{
					elm.Background = counter % 2 == 0 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Thistle);
				}
				counter++;
			}
		}

		private void FillCrewListViewModel(GR_Crews2 crew, CrewListViewModel cmv, int? id_assignment, int counter)
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
			cmv.State = crew.Name % 2 == 0 ? "W" : "G";
			cmv.Background = counter % 2 == 0 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Thistle);
		}

		public List<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime date,
			int id_scheduleType = 1)
		{
			this.cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == date.Year && a.date.Month == date.Month);
			if (this.cRow == null)
			{
				return null;
			}

			var pDate = date.AddMonths(-1);
			this.prevCRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == pDate.Year && a.date.Month == pDate.Month);
			if (this.prevCRow == null)
			{
				return null;
			}

			//DateTime startDate = new DateTime(date.Year, date.Month, 1);
			//var endDate = startDate.AddMonths(1);

			List<PersonnelViewModel> lstAssignments;

			lstAssignments = lstAllAssignments.Where(a => a.id_department == id_selectedDepartment)
				.ToList();

			var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment
															&& (c.IsTemporary == false
																	||	(c.DateStart.Month == date.Month && c.DateStart.Year == date.Year))).OrderBy(c => c.Name).ToList();

			List<CrewScheduleListViewModel> lstCrewModel = new List<CrewScheduleListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cmv, crew.id_assignment1);
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
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
					cmv.DateStart = crew.DateStart;
					cmv.DateEnd = crew.DateEnd;
				}
				lstCrewModel.Add(cmv);

				if (crew.id_assignment2 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, crew.id_assignment2);
					cp.WorkTime = cmv.WorkTime;
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
					}
					cmv.LstCrewMembers.Add(cp);
					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, crew.id_assignment3);
					cp.WorkTime = cmv.WorkTime;
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
					}
					cmv.LstCrewMembers.Add(cp);
					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, crew.id_assignment4);
					cp.WorkTime = cmv.WorkTime;
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
					}
					cmv.LstCrewMembers.Add(cp);
					lstCrewModel.Add(cp);
				}
			}

			//lstCrewModel = lstCrewModel.OrderBy(a => a.CrewName).ToList();

			lstAssignments = lstAssignments.OrderBy(a => a.Order).ThenBy(a => a.Name).ToList();
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_selectedDepartment);
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();

				this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, null, cmv, ass.id_assignment);

				cmv.id_department = id_selectedDepartment;
				cmv.IsActive = true;

				lstCrewModel.Add(cmv);
			}
			return lstCrewModel;
		}

		internal PersonnelViewModel FillPersonalCrewScheduleModel(DateTime date, int id_scheduleType,
			List<PersonnelViewModel> lstAssignments, GR_Crews2 crew, CrewScheduleListViewModel cmv,
			int? id_assignment, bool IsDayShift = true)
		{
			var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
			if (ass == null)
			{
				ass = lstAllAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
			}
			if (ass?.id_assignment == null || ass?.id_contract == null)
			{
				return null;
			}
			cmv.id_person = ass.id_person;
			if (crew != null)
			{
				cmv.CrewName = crew.Name.ToString();
				cmv.State = crew.Name % 2;
				cmv.CrewType = crew.NM_CrewTypes.Name;
				cmv.id_crew = crew.id_crew;
				cmv.id_department = crew.id_department;
				cmv.IsActive = crew.IsActive;
				cmv.IsTemporary = crew.IsTemporary;
				cmv.id_crewType = crew.id_crewType;
				cmv.id_assignment = (int)ass.id_assignment;
				cmv.DateStart = crew.DateStart;
				cmv.DateEnd = crew.DateEnd;
			}
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
			else
			{
				cmv.WorkTime = IsDayShift ? ass.WorkZoneDay : ass.WorkZoneNight;
			}
			cmv.Name = ass.Name;
			cmv.Position = ass.Position;
			cmv.ShortPosition = ass.ShortPosition;
			cmv.RowPosition = 1;
			cmv.RealDate = date;
			cmv.id_contract = (int)ass.id_contract;
			if (ass.IsSumWorkTime == null)
			{
				ass.IsSumWorkTime = true;
				//ThrowZoraException(ErrorCodes.WorkHoursMissingError, true, "Некоректно зададен часови пояс за служител " + ass.Name, true);
				//return null;
			}
			var pDate = date.AddMonths(-1);
			PFRow prevMonth = new PFRow();
			this.FillPfRow(pDate, id_scheduleType, prevMonth, ass, this.prevCRow);
			if (prevMonth.PF != null)
			{
				cmv.PrevMonthHours = prevMonth.Difference;
			}

			this.FillPfRow(date, id_scheduleType, cmv, ass, this.cRow);

			return ass;
		}

		private void FillPfRow(DateTime date, int id_scheduleType, PFRow cmv, PersonnelViewModel ass, CalendarRow cr)
		{
			cmv.IsSumWorkTime = ass.IsSumWorkTime??true;
			cmv.lstShiftTypes = lstShiftTypes;
			//cmv.IsSumWorkTime =
			cmv.PF = this.lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
			                                                         && p.Date.Month == date.Month
			                                                         && p.id_contract == ass.id_contract
			                                                         && p.id_scheduleType == id_scheduleType);
			if (cmv.PF == null)
			{
				cmv.PF = new GR_PresenceForms();
				cmv.id_contract = (int)ass.id_contract;
			}

			cmv.LstWorktimeAbsences = this.lstWorktimeAbsenceces.Where(a => a.Date.Year == date.Year
			                                                                      && a.Date.Month == date.Month
			                                                                      && a.id_contract == ass.id_contract).ToList();
			cmv.cRow = cr;
			if (ass.WorkHours == null)
			{
				ass.WorkHours = 0;
			}
			//if assigned or fired here calculate workdays from date or workdays to date. Or both!
			var workDaysNorm = cr.WorkDays;
			if (ass.AssignedAt != null && ass.AssignedAt.Value.Year == date.Year && ass.AssignedAt.Value.Month == date.Month)
			{
				workDaysNorm = this.CalculateWorkDays((DateTime)ass.AssignedAt, new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)), this.lstCalendarRows);
			}
			
			cmv.Norm = workDaysNorm * (double)ass.WorkHours;
			cmv.WorkHours = (double) ass.WorkHours;

			cmv.CalculateHours();
			cmv.PrevMonthTotal = cmv.Difference + cmv.PrevMonthHours;
		}

		public void FitCrewWorktime()
		{
			var crews = this.GetDepartmentCrewsAndSchedules(0, DateTime.Now, (int) ScheduleTypes.ForecastMonthSchedule);

			crews = crews.Where(c => c.LstCrewMembers.Count > 0).ToList();

			foreach (var cr in crews)
			{

			}
		}
	}
}