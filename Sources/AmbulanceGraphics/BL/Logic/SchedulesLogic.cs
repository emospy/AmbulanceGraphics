﻿using BL.DB;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class SchedulesLogic : BaseLogic
	{
		public SchedulesLogic()
		{
		}

		public List<DriverAmbulancesViewModel> GetDriverAmbulances(bool ShowHistory)
		{
			if (ShowHistory == false)
			{
				List<DriverAmbulancesViewModel> lstDrivers = new List<DriverAmbulancesViewModel>();

				lstDrivers = (from ass in this._databaseContext.HR_Assignments
							  join dri in this._databaseContext.GR_DriverAmbulances on ass.id_assignment equals dri.id_driverAssignment into pa
							  from spa in pa.DefaultIfEmpty()

							  where (spa == null || spa.IsActive == true)
							  && ass.IsActive == true
							  && ass.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int)PositionTypes.Driver

							  select new DriverAmbulancesViewModel
							  {
								  id_driverAssignment = ass.id_assignment,
								  Name = ass.HR_Contracts.UN_Persons.Name,

								  Level1 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.Name) :
											(ass.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
											(ass.HR_StructurePositions.UN_Departments.Level == 2) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
											(ass.HR_StructurePositions.UN_Departments.Level == 1) ? ass.HR_StructurePositions.UN_Departments.Name : null,

								  Level2 = (ass == null) ? null :
									 (ass.HR_StructurePositions.UN_Departments.Level == 4) ? ((ass == null) ? null : ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
									 (ass.HR_StructurePositions.UN_Departments.Level == 3) ? ((ass == null) ? null : ass.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									 (ass.HR_StructurePositions.UN_Departments.Level == 2) ? ass.HR_StructurePositions.UN_Departments.Name : null,

								  Level3 = (ass == null) ? null :
									(ass.HR_StructurePositions.UN_Departments.Level == 4) ? ((ass == null) ? null : ass.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 3) ? ass.HR_StructurePositions.UN_Departments.Name : null,

								  Level4 = (ass == null) ? null :
									 (ass.HR_StructurePositions.UN_Departments.Level == 4) ? ass.HR_StructurePositions.UN_Departments.Name : null,

								  MainAmbulance = (spa == null) ? null : spa.GR_Ambulances.Name,
								  id_mainAmbulance = (spa == null) ? (int?)null : spa.GR_Ambulances.id_ambulance,
								  SecondaryAmbulance = (spa == null) ? null : spa.GR_Ambulances1.Name,
								  WorkTime = spa.GR_Ambulances.GR_WorkHours.DayHours + " " + spa.GR_Ambulances.GR_WorkHours.NightHours,
							  }).ToList();

				return lstDrivers;
			}
			else
			{
				List<DriverAmbulancesViewModel> lstDrivers = new List<DriverAmbulancesViewModel>();

				lstDrivers = (from dri in this._databaseContext.GR_DriverAmbulances
							  join ass in this._databaseContext.HR_Assignments on dri.id_driverAssignment equals ass.id_assignment into pa
							  from spa in pa.DefaultIfEmpty()

							  select new DriverAmbulancesViewModel
							  {
								  id_driverAssignment = spa.id_assignment,
								  Name = spa.HR_Contracts.UN_Persons.Name,

								  Level1 = (spa.HR_StructurePositions.UN_Departments.Level == 4) ? (spa.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.Name) :
											(spa.HR_StructurePositions.UN_Departments.Level == 3) ? (spa.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
											(spa.HR_StructurePositions.UN_Departments.Level == 2) ? (spa.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
											(spa.HR_StructurePositions.UN_Departments.Level == 1) ? spa.HR_StructurePositions.UN_Departments.Name : null,

								  Level2 = (spa == null) ? null :
									 (spa.HR_StructurePositions.UN_Departments.Level == 4) ? ((spa == null) ? null : spa.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
									 (spa.HR_StructurePositions.UN_Departments.Level == 3) ? ((spa == null) ? null : spa.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									 (spa.HR_StructurePositions.UN_Departments.Level == 2) ? spa.HR_StructurePositions.UN_Departments.Name : null,

								  Level3 = (spa == null) ? null :
									(spa.HR_StructurePositions.UN_Departments.Level == 4) ? ((spa == null) ? null : spa.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(spa.HR_StructurePositions.UN_Departments.Level == 3) ? spa.HR_StructurePositions.UN_Departments.Name : null,

								  Level4 = (spa == null) ? null :
									 (spa.HR_StructurePositions.UN_Departments.Level == 4) ? spa.HR_StructurePositions.UN_Departments.Name : null,

								  MainAmbulance = dri.GR_Ambulances.Name,
								  SecondaryAmbulance = dri.GR_Ambulances1.Name,
							  }).ToList();
				return lstDrivers;
			}
		}

		private void FillCrewFromModel(GR_Crews crew, CrewViewModel model)
		{
			crew.id_assignment1 = model.id_assignment1;
			crew.id_assignment2 = model.id_assignment2;
			crew.id_assignment3 = model.id_assignment3;
			crew.id_assignment4 = model.id_assignment4;
			crew.id_crewType = model.id_crewType;
			crew.id_department = model.id_department;
			int cn;
			if (int.TryParse(model.CrewName, out cn) == true)
			{
				crew.Name = cn;
			}
			crew.IsActive = model.IsActive;
			crew.IsTemporary = model.IsTemporary;
			crew.Date = model.Date;
		}

		public void UpdateCrew(CrewViewModel model)
		{
			var crew = this._databaseContext.GR_Crews.First(c => c.id_crew == model.id_crew);

			this.FillCrewFromModel(crew, model);

			if (model.IsTemporary == false)
			{
				this.RemoveCrewPersonnelFromOtherCrews(model);
			}

			this.Save();
		}

		public void AddCrew(CrewViewModel model)
		{
			var crew = new GR_Crews();

			this.RemoveCrewPersonnelFromOtherCrews(model);

			this.FillCrewFromModel(crew, model);

			this._databaseContext.GR_Crews.Add(crew);

			this.Save();

			model.id_crew = crew.id_crew;
		}

		private void RemoveCrewPersonnelFromOtherCrews(CrewViewModel model)
		{
			if (model.IsTemporary == true)
			{
				return;
			}
			if (model.id_assignment1 != null && model.id_assignment1 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment1 == model.id_assignment1
																					|| a.id_assignment4 == model.id_assignment1)
																					&& a.id_crew != model.id_crew
																					&& a.IsTemporary == false).ToList();
				foreach (var cmtr in lstCrewAssignments)
				{
					if (cmtr.id_assignment1 == model.id_assignment1)
					{
						cmtr.id_assignment1 = null;
					}
					else
					{
						cmtr.id_assignment4 = null;
					}
				}
			}

			if (model.id_assignment2 != null && model.id_assignment2 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment2 == model.id_assignment2
																					|| a.id_assignment4 == model.id_assignment2)
																					&& a.id_crew != model.id_crew
																					&& a.IsTemporary == false).ToList();
				foreach (var cmtr in lstCrewAssignments)
				{
					if (cmtr.id_assignment2 == model.id_assignment2)
					{
						cmtr.id_assignment2 = null;
					}
					else
					{
						cmtr.id_assignment4 = null;
					}
				}
			}

			if (model.id_assignment3 != null && model.id_assignment3 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment1 == model.id_assignment3
																					|| a.id_assignment4 == model.id_assignment3)
																					&& a.id_crew != model.id_crew
																					&& a.IsTemporary == false).ToList();
				foreach (var cmtr in lstCrewAssignments)
				{
					if (cmtr.id_assignment3 == model.id_assignment3)
					{
						cmtr.id_assignment3 = null;
					}
					else
					{
						cmtr.id_assignment4 = null;
					}
				}
			}

			if (model.id_assignment4 != null && model.id_assignment4 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment1 == model.id_assignment4
																					|| a.id_assignment2 == model.id_assignment4
																					|| a.id_assignment3 == model.id_assignment4
																					|| a.id_assignment4 == model.id_assignment4)
																					&& a.id_crew != model.id_crew
																					&& a.IsTemporary == false).ToList();
				foreach (var cmtr in lstCrewAssignments)
				{
					if (cmtr.id_assignment1 == model.id_assignment4)
					{
						cmtr.id_assignment1 = null;
					}
					else if (cmtr.id_assignment2 == model.id_assignment4)
					{
						cmtr.id_assignment2 = null;
					}
					else if (cmtr.id_assignment3 == model.id_assignment3)
					{
						cmtr.id_assignment3 = null;
					}
					else
					{
						cmtr.id_assignment4 = null;
					}
				}
			}
		}

		public CrewViewModel GetCrewViewModel(int id_crew)
		{
			var crew = this._databaseContext.GR_Crews.FirstOrDefault(a => a.id_crew == id_crew);
			if (crew == null)
			{
				return null;
			}
			else
			{
				CrewViewModel cvm = new CrewViewModel();

				cvm.CrewName = crew.Name.ToString();

				cvm.id_crew = crew.id_crew;
				cvm.id_department = crew.id_department;
				cvm.id_crewType = crew.id_crewType;
				//cvm.id_departmentParent = crew.UN_Departments.id_departmentParent;

				cvm.id_assignment1 = crew.id_assignment1;
				cvm.id_assignment2 = crew.id_assignment2;
				cvm.id_assignment3 = crew.id_assignment3;
				cvm.id_assignment4 = crew.id_assignment4;
				cvm.IsActive = crew.IsActive;
				cvm.Date = crew.Date;
				cvm.IsTemporary = crew.IsTemporary;
				return cvm;
			}
		}

		public List<PFRow> GetPersonalSchedule(int id_person, DateTime month, ScheduleTypes id_scheduleType)
		{
			List<PFRow> lstResult = new List<PFRow>();
			CalendarRow cRow;
			using (var logic = new NomenclaturesLogic())
			{
				cRow = logic.FillCalendarRow(month);
			}
			var con = this._databaseContext.HR_Contracts.FirstOrDefault(c => c.IsFired == false
																			&& c.id_person == id_person);

			if (con == null)
			{
				return null;
			}

			var lstShiftTypes = this._databaseContext.GR_ShiftTypes.ToList();

			var ass = this._databaseContext.HR_Assignments.FirstOrDefault(c => c.id_contract == con.id_contract
																			   && c.IsActive == true);

			if (ass == null)
			{
				return null;
			}

			PFRow PF = new PFRow();

			PF.PF = this._databaseContext.GR_PresenceForms.FirstOrDefault(p => p.Date.Year == month.Year
																			&& p.Date.Month == month.Month
																			&& p.id_contract == con.id_contract
																			&& p.id_scheduleType == (int)id_scheduleType);

			PF.LstWorktimeAbsences = this._databaseContext.GR_WorkTimeAbsence.Where(a => a.Date.Year == month.Year
																			&& a.Date.Month == month.Month
																			&& a.id_contract == con.id_contract).ToList();

			PF.lstShiftTypes = lstShiftTypes;
			if (ass.HR_WorkTime != null)
			{
				PF.WorkHours = ass.HR_WorkTime.WorkHours;
				PF.Norm = ass.HR_WorkTime.WorkHours * cRow.WorkDays;
			}
			PF.cRow = cRow;
			PF.CalculateHours();
			PF.IsDataChanged = false;
			if (PF.PF != null)
			{
				lstResult.Add(PF);
			}
			return lstResult;
		}

		public GR_DriverAmbulances GetActiveDriverAmbulance(int id_driverAssignment)
		{
			var result = this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == id_driverAssignment && a.IsActive == true);
			return result;
		}

		public void GenerateSingleDepartmentSchedule(DateTime month, int id_department, int startShift = 0)
		{
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);
			if (dep == null)
			{
				return;
			}
			var lstDepartments = this._databaseContext.UN_Departments.Where(d => d.id_departmentParent == dep.id_departmentParent && d.id_department != d.id_departmentParent).OrderBy(d => d.TreeOrder).ToList();
			CalendarRow cRow;

			using (var nomlogic = new NomenclaturesLogic())
			{
				cRow = nomlogic.FillCalendarRow(month);
			}

			var lstOrderedDepartments = OrderDepartmentsByStartShift(startShift, dep.UN_Departments2.NumberShifts, lstDepartments);

			int di = 0;
			for (int i = 0; i < lstDepartments.Count; i++)
			{
				if (dep.id_department == lstOrderedDepartments[i].id_department)
				{
					di = i;
					break;
				}
			}

			List<PFRow> lstScheduleRows = new List<PFRow>();
			this.GenerateScheduleForDepartment(month, id_department, dep.UN_Departments2.NumberShifts, di, cRow, lstScheduleRows);
			this.SaveGeneratedDepartmentSchedules(lstScheduleRows, id_department);
		}

		public void GenerateSchedules(DateTime month, int startShift = 0)
		{
			var lstDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive == true && d.NumberShifts > 1).ToList();

			CalendarRow cRow;

			using (var nomlogic = new NomenclaturesLogic())
			{
				cRow = nomlogic.FillCalendarRow(month);
			}

			List<PFRow> lstScheduleRows = new List<PFRow>();

			foreach (var department in lstDepartments)
			{
				List<UN_Departments> subDeps = this._databaseContext.UN_Departments.Where(d => d.id_departmentParent == department.id_department
																								&& d.NumberShifts < 2)
																								.OrderBy(a => a.TreeOrder).ToList();

				var lstOrderedDepartments = OrderDepartmentsByStartShift(startShift, department.NumberShifts, subDeps);

				for (int i = 0; i < department.NumberShifts; i++)
				{
					var id_dep = lstOrderedDepartments[i].id_department;
					this.GenerateScheduleForDepartment(month, id_dep, department.NumberShifts, i, cRow, lstScheduleRows);
				}
			}
			this.SaveGeneratedSchedules(lstScheduleRows);
		}

		private static List<UN_Departments> OrderDepartmentsByStartShift(int startShift, int NumberShifts, List<UN_Departments> subDeps)
		{
			List<UN_Departments> lstOrderedDepartments = new List<UN_Departments>();

			for (int i = 0; i < NumberShifts; i++)
			{
				int depIndex = 0;
				if (i + startShift >= NumberShifts)
				{
					depIndex = i + startShift - NumberShifts;
				}
				else
				{
					depIndex = i + startShift;
				}
				lstOrderedDepartments.Add(subDeps[depIndex]);
			}
			return lstOrderedDepartments;
		}

		private void GenerateScheduleForDepartment(DateTime month, int id_dep, int NumberShifts, int i,
			CalendarRow cRow, List<PFRow> lstScheduleRows)
		{
			var days = DateTime.DaysInMonth(month.Year, month.Month);

			var dateStart = new DateTime(month.Year, month.Month, 1);
			var dateEnd = dateStart.AddMonths(1);

			var lstContracts =
				this._databaseContext.HR_Assignments.Where(
					a => a.HR_Contracts.IsFired == false && a.HR_StructurePositions.id_department == id_dep).ToList();

			foreach (var contract in lstContracts)
			{
				PFRow row = new PFRow();
				row.RealDate = month;
				row.PF = new GR_PresenceForms();
				row.PF.Date = month;
				row.PF.id_contract = contract.id_contract;
				row.PF.id_scheduleType = (int)ScheduleTypes.ForecastMonthSchedule;

				for (int j = 0; j < days; j++)
				{
					if ((j + NumberShifts - i) % NumberShifts == 0)
					{
						row[j + 1] = (int)PresenceTypes.DayShift;
					}
					else if ((j + NumberShifts - i) % NumberShifts == 1)
					{
						row[j + 1] = (int)PresenceTypes.NightShift;
					}
				}

				var absences = this._databaseContext.HR_Absence.Where(a => a.id_contract == contract.id_contract
																		   && ((a.StartDate <= dateStart && a.EndDate >= dateEnd)
																			   || (a.StartDate >= dateStart && a.StartDate <= dateEnd)
																			   || (a.EndDate >= dateStart && a.EndDate <= dateEnd)))
					.ToList();

				this.InsertAbsenceInSchedule(row, absences, cRow);

				lstScheduleRows.Add(row);
			}
		}

		private void SaveGeneratedDepartmentSchedules(List<PFRow> lstScheduleRows, int id_department)
		{
			if (lstScheduleRows.Count == 0)
			{
				return;
			}
			var month = lstScheduleRows.First().RealDate;
			var id_st = lstScheduleRows.First().PF.id_scheduleType;

			var lstRowsToDelete = this._databaseContext.GR_PresenceForms.Where(p =>
													p.Date.Year == month.Year
													&& p.Date.Month == month.Month
													&& p.id_scheduleType == id_st
													&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_department && a.IsActive == true) != null)
													.ToList();

			this._databaseContext.GR_PresenceForms.RemoveRange(lstRowsToDelete);
			int count = 0;
			foreach (var row in lstScheduleRows)
			{
				this._databaseContext.GR_PresenceForms.Add(row.PF);
				count++;
			}
			this.Save();
		}

		public int CalculateLeadShift(DateTime date, bool IsDayShift)
		{
			
			var countDays = (date - Constants.StartScheduleDate).Days;
			if (IsDayShift == false)
			{
				countDays --;
			}
			return (countDays + Constants.StartShift) % 5;
		}

		private void SaveGeneratedSchedules(List<PFRow> lstScheduleRows)
		{
			if (lstScheduleRows.Count == 0)
			{
				return;
			}
			var month = lstScheduleRows.First().RealDate;
			var id_st = lstScheduleRows.First().PF.id_scheduleType;

			var lstRowsToDelete = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == month.Year
																					&& p.Date.Month == month.Month
																					&& p.id_scheduleType == id_st).ToList();

			this._databaseContext.GR_PresenceForms.RemoveRange(lstRowsToDelete);
			int count = 0;
			foreach (var row in lstScheduleRows)
			{
				this._databaseContext.GR_PresenceForms.Add(row.PF);
				count++;
			}
			this.Save();
		}

		private void InsertAbsenceInSchedule(PFRow row, List<HR_Absence> absences, CalendarRow cRow)
		{
			var DateStart = new DateTime(row.RealDate.Year, row.RealDate.Month, 1);
			var DateEnd = DateStart.AddMonths(1).AddDays(-1);
			foreach (var absence in absences)
			{
				if (absence.StartDate <= DateStart && absence.EndDate >= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, 1, DateTime.DaysInMonth(row.RealDate.Year, row.RealDate.Month), cRow);
				}
				else if (absence.StartDate >= DateStart && absence.EndDate <= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, absence.StartDate.Day, absence.EndDate.Day, cRow);
				}
				else if (absence.StartDate <= DateStart && absence.EndDate <= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, 1, absence.EndDate.Day, cRow);
				}
				else if (absence.StartDate >= DateStart && absence.EndDate >= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, DateStart.Day, DateTime.DaysInMonth(row.RealDate.Year, row.RealDate.Month), cRow);
				}
			}
		}

		private void InitAbsenceInPF(PFRow row, AbsenceTypes id_absenceType, int dayStart, int dayEnd, CalendarRow cRow)
		{
			for (int i = dayStart; i <= dayEnd; i++)
			{
				switch (id_absenceType)
				{
					case AbsenceTypes.BusinessTrip:
						row[i] = (int) PresenceTypes.BusinessTrip;
						break;
					case AbsenceTypes.Education:
						row[i] = (int) PresenceTypes.Education;
						break;
					case AbsenceTypes.Motherhood:
						row[i] = (int) PresenceTypes.Motherhood;
						break;
					case AbsenceTypes.MotherhoodExtend:
						row[i] = (int) PresenceTypes.Motherhood;
						break;
					case AbsenceTypes.MotherhoodSickness:
						row[i] = (int) PresenceTypes.Motherhood;
						break;
					case AbsenceTypes.OtherPaidHoliday:
						row[i] = (int) PresenceTypes.OtherPaidHoliday;
						break;
					case AbsenceTypes.Sickness:
						row[i] = (int) PresenceTypes.Sickness;
						break;
					case AbsenceTypes.UnpaidHoliday:
						row[i] = (int) PresenceTypes.UnpaidHoliday;
						break;
					case AbsenceTypes.YearPaidHoliday:
						row[i] = (int) PresenceTypes.YearPaidHoliday;
						break;
				}
			}
		}

		public bool IsMonthlyScheduleAlreadyGenerated(DateTime date, int id_scheduleType)
		{
			var count = this._databaseContext.GR_PresenceForms.Count(p => p.Date.Year == date.Year
																					&& p.Date.Month == date.Month
																					&& p.id_scheduleType == id_scheduleType);
			return count > 0;
		}

		public List<string> CheckReassignPersonalCrew(CrewViewModel crewModel)
		{
			List<string> lstResults = new List<string>();
			CheckCrewByAssignment(crewModel, lstResults, crewModel.id_assignment1);
			CheckCrewByAssignment(crewModel, lstResults, crewModel.id_assignment2);
			CheckCrewByAssignment(crewModel, lstResults, crewModel.id_assignment3);
			CheckCrewByAssignment(crewModel, lstResults, crewModel.id_assignment4);
			return lstResults;
		}

		private void CheckCrewByAssignment(CrewViewModel crewModel, List<string> lstResults, int? id_assignment)
		{
			var cm1 = this._databaseContext.GR_Crews.FirstOrDefault(c => (c.id_assignment1 == id_assignment
																		 || c.id_assignment2 == id_assignment
																		 || c.id_assignment3 == id_assignment
																		 || c.id_assignment4 == id_assignment)
																		 && c.IsTemporary == false
																		 && c.id_crew != crewModel.id_crew);
			if (cm1 != null)
			{
				var per = this._databaseContext.HR_Assignments.FirstOrDefault(a => a.id_assignment == id_assignment);
				if (per != null)
				{
					lstResults.Add(per.HR_Contracts.UN_Persons.Name + " от екип " + cm1.Name + ", " + cm1.UN_Departments.Name);
				}
			}
		}

		public void SavePersonalSchedule(PFRow model)
		{
			if (model.PF != null)
			{
				this.GR_PresenceForms.Update(model.PF);
				this.Save();
			}
		}

		public void DeleteCrew(CrewListViewModel model)
		{
			var crewToDelete = this._databaseContext.GR_Crews.FirstOrDefault(c => c.id_crew == model.id_crew);
			if (crewToDelete != null)
			{
				this._databaseContext.GR_Crews.Remove(crewToDelete);
				this.Save();
			}
		}

		public void CreatePersonalScheduleForMonth(int id_person, DateTime date, int id_scheduleType)
		{
			var contract =
				this._databaseContext.HR_Contracts.Where(a => a.id_person == id_person && a.IsFired == false).FirstOrDefault();
			if (contract == null)
			{
				return;
			}
			var pfE =
				this._databaseContext.GR_PresenceForms.FirstOrDefault(
					a => a.id_scheduleType == id_scheduleType && a.id_contract == contract.id_contract);

			if (pfE != null)
			{
				return;
			}
			var sch = new GR_PresenceForms();
			sch.Date = date;
			sch.id_contract = contract.id_contract;
			sch.id_scheduleType = id_scheduleType;

			this.GR_PresenceForms.Add(sch);
			this.Save();
		}

		private bool IsDepartmentScheduleGenerated(int id_department, DateTime date, int id_scheduleType)
		{
			var count = this._databaseContext.GR_PresenceForms.Count(p => p.Date.Year == date.Year
			                            && p.Date.Month == date.Month
			                            && p.id_scheduleType == id_scheduleType
			                            && p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_department && a.IsActive == true) != null);

			return count > 0;
		}

		public void ApproveForecastScheduleForDepartment(int id_department, DateTime date)
		{
			if (this.IsDepartmentScheduleGenerated(id_department, date, (int) ScheduleTypes.FinalMonthSchedule) == true)
			{
				ThrowZoraException(ErrorCodes.ScheduleAlreadyApproved);
			}

			var lstDepartmnetForecastSchedules = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == date.Year
										&& p.Date.Month == date.Month
										&& p.id_scheduleType == (int)ScheduleTypes.ForecastMonthSchedule
										&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_department && a.IsActive == true) != null).ToList();

			foreach (var sched in lstDepartmnetForecastSchedules)
			{
				var sh = this.CreateCopySchedule(sched);
				sh.id_scheduleType = (int) ScheduleTypes.FinalMonthSchedule;
				this.GR_PresenceForms.Add(sh);

				var dsh = this.CreateCopySchedule(sched);
				dsh.id_scheduleType = (int)ScheduleTypes.DailySchedule;
				this.GR_PresenceForms.Add(dsh);

				var pf = new GR_PresenceForms();
				pf.id_contract = sched.id_contract;
				pf.id_scheduleType = (int) ScheduleTypes.PresenceForm;
				pf.Date = date;
				this.GR_PresenceForms.Add(pf);
			}

			this.Save();
		}

		public GR_PresenceForms CreateCopySchedule(GR_PresenceForms source)
		{
			GR_PresenceForms destination = new GR_PresenceForms();

			destination.Date = source.Date;
			destination.id_contract = source.id_contract;
			destination.id_day1 = source.id_day1;
			destination.id_day2 = source.id_day2;
			destination.id_day3 = source.id_day3;
			destination.id_day4 = source.id_day4;
			destination.id_day5 = source.id_day5;
			destination.id_day6 = source.id_day6;
			destination.id_day7 = source.id_day7;
			destination.id_day8 = source.id_day8;
			destination.id_day9 = source.id_day9;
			destination.id_day10 = source.id_day10;
			destination.id_day11 = source.id_day11;
			destination.id_day12 = source.id_day12;
			destination.id_day13 = source.id_day13;
			destination.id_day14 = source.id_day14;
			destination.id_day15 = source.id_day15;
			destination.id_day16 = source.id_day16;
			destination.id_day17 = source.id_day17;
			destination.id_day18 = source.id_day18;
			destination.id_day19 = source.id_day19;
			destination.id_day20 = source.id_day20;
			destination.id_day21 = source.id_day21;
			destination.id_day22 = source.id_day22;
			destination.id_day23 = source.id_day23;
			destination.id_day24 = source.id_day24;
			destination.id_day25 = source.id_day25;
			destination.id_day26 = source.id_day26;
			destination.id_day27 = source.id_day27;
			destination.id_day28 = source.id_day28;
			destination.id_day29 = source.id_day29;
			destination.id_day30 = source.id_day30;
			destination.id_day31 = source.id_day31;

			return destination;
		}

		public void CopyScheduleToPF(int id_selectedDepartment, DateTime date, DateTime dateTo)
		{
			var lstDepartmnetDailySchedules = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == date.Year
										&& p.Date.Month == date.Month
										&& p.id_scheduleType == (int)ScheduleTypes.DailySchedule
										&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_selectedDepartment && a.IsActive == true) != null).ToList();

			var lstDepartmnetPresenceForms = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == date.Year
										&& p.Date.Month == date.Month
										&& p.id_scheduleType == (int)ScheduleTypes.PresenceForm
										&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_selectedDepartment && a.IsActive == true) != null).ToList();

			foreach (var ds in lstDepartmnetDailySchedules)
			{
				var pf = lstDepartmnetPresenceForms.FirstOrDefault(a => a.id_contract == ds.id_contract);
				if (pf == null)
				{
					continue;
				}
				DateTime t = date;
				for (int i = t.Day; i <= dateTo.Day; i++)
				{
					switch (t.Day)
					{
						case 1:
							pf.id_day1 = ds.id_day1;
							break;
						case 2:
							pf.id_day2 = ds.id_day2;
							break;
						case 3:
							pf.id_day3 = ds.id_day3;
							break;
						case 4:
							pf.id_day4 = ds.id_day4;
							break;
						case 5:
							pf.id_day5 = ds.id_day5;
							break;
						case 6:
							pf.id_day6 = ds.id_day6;
							break;
						case 7:
							pf.id_day7 = ds.id_day7;
							break;
						case 8:
							pf.id_day8 = ds.id_day8;
							break;
						case 9:
							pf.id_day9 = ds.id_day9;
							break;
						case 10:
							pf.id_day10 = ds.id_day10;
							break;
						case 11:
							pf.id_day11 = ds.id_day11;
							break;
						case 12:
							pf.id_day12 = ds.id_day12;
							break;
						case 13:
							pf.id_day13 = ds.id_day13;
							break;
						case 14:
							pf.id_day14 = ds.id_day14;
							break;
						case 15:
							pf.id_day15 = ds.id_day15;
							break;
						case 16:
							pf.id_day16 = ds.id_day16;
							break;
						case 17:
							pf.id_day17 = ds.id_day17;
							break;
						case 18:
							pf.id_day18 = ds.id_day18;
							break;
						case 19:
							pf.id_day19 = ds.id_day19;
							break;
						case 20:
							pf.id_day20 = ds.id_day20;
							break;
						case 21:
							pf.id_day21 = ds.id_day21;
							break;
						case 22:
							pf.id_day22 = ds.id_day22;
							break;
						case 23:
							pf.id_day23 = ds.id_day23;
							break;
						case 24:
							pf.id_day24 = ds.id_day24;
							break;
						case 25:
							pf.id_day25 = ds.id_day25;
							break;
						case 26:
							pf.id_day26 = ds.id_day26;
							break;
						case 27:
							pf.id_day27 = ds.id_day27;
							break;
						case 28:
							pf.id_day28 = ds.id_day28;
							break;
						case 29:
							pf.id_day29 = ds.id_day29;
							break;
						case 30:
							pf.id_day30 = ds.id_day30;
							break;
						case 31:
							pf.id_day31 = ds.id_day31;
							break;
					}
					t = t.AddDays(1);
				}
			}
			this.Save();
		}

		public new void Save()
		{
			try
			{
				_databaseContext.SaveChanges();
			}
			catch (EntityException)
			{
				ThrowZoraException(ErrorCodes.NoDb);
			}
			catch (DbUpdateException ex)
			{
				Exception exp = ex.InnerException;
				while (exp.InnerException != null)
				{
					exp = exp.InnerException;
				}

				if (exp is SqlException)
				{
					var sqexp = exp as SqlException;
					if (sqexp.Number == 547)
					{
						ThrowZoraException(ErrorCodes.DeleteRecordAlreadyReferred);
					}
					else if (sqexp.Number == 2627)
					{
						ThrowZoraException(ErrorCodes.DuplicateName);
					}
					else
					{
						throw exp;
					}
				}
				ThrowZoraException(ErrorCodes.NoDb);
			}
			catch (Exception ex)
			{
				ThrowZoraException(ErrorCodes.NoDb);
			}
		}
	}
}
