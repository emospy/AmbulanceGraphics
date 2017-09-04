using BL.DB;
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
using System.Windows.Media.TextFormatting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Exceptions;
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
							  && (ass.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int)PositionTypes.Driver
                                || ass.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int)PositionTypes.CorpseSanitar)

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

		private void FillCrewFromModel(GR_Crews2 crew, CrewViewModel model)
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
			crew.DateStart = model.DateStart;
			crew.DateEnd = model.DateEnd;
		}

		public void UpdateCrew(CrewViewModel model)
		{
			var crew = this._databaseContext.GR_Crews2.First(c => c.id_crew == model.id_crew);

			this.FillCrewFromModel(crew, model);

			if (model.IsTemporary == false)
			{
				//this.RemoveCrewPersonnelFromOtherCrews(model);
			}

			this.Save();
		}

		public void AddCrew(CrewViewModel model)
		{
			var crew = new GR_Crews2();

			//this.RemoveCrewPersonnelFromOtherCrews(model);

			this.FillCrewFromModel(crew, model);

			this._databaseContext.GR_Crews2.Add(crew);

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
				var lstCrewAssignments = this._databaseContext.GR_Crews2.Where(a => (a.id_assignment1 == model.id_assignment1
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
				var lstCrewAssignments = this._databaseContext.GR_Crews2.Where(a => (a.id_assignment2 == model.id_assignment2
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
				var lstCrewAssignments = this._databaseContext.GR_Crews2.Where(a => (a.id_assignment1 == model.id_assignment3
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
				var lstCrewAssignments = this._databaseContext.GR_Crews2.Where(a => (a.id_assignment1 == model.id_assignment4
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
			var crew = this._databaseContext.GR_Crews2.FirstOrDefault(a => a.id_crew == id_crew);
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
				cvm.DateStart = crew.DateStart;
				cvm.DateEnd = crew.DateEnd;
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
				//get the last contract if all are fired - not really right
				con = this._databaseContext.HR_Contracts.Where(c => c.id_person == id_person).OrderByDescending(c => c.id_contract).FirstOrDefault();
				if (con == null)
				{
					return null;
				}
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


			var lstCalRows = new List<CalendarRow>();
			lstCalRows.Add(cRow);

			var fass =
				this._databaseContext.HR_Assignments.FirstOrDefault(a => a.IsAdditionalAssignment == false && a.id_contract == ass.id_contract);

			var workDaysNorm = cRow.WorkDays;
			var fad = ass.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false).AssignmentDate;
			if (fass.AssignmentDate != null && fass.AssignmentDate.Year == month.Year && fass.AssignmentDate.Month == month.Month)
			{
				workDaysNorm = this.CalculateWorkDays((DateTime)fass.AssignmentDate, new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month)), lstCalRows);
			}
			else if ((fad.Year != month.Year || fad.Month != month.Month) && ass.HR_Contracts.IsFired && ass.HR_Contracts.DateFired.Value.Year == month.Year && ass.HR_Contracts.DateFired.Value.Month == month.Month)
			{//fired in the current month
				workDaysNorm = this.CalculateWorkDays(new DateTime(month.Year, month.Month, 1), ass.HR_Contracts.DateFired.Value, lstCalRows);
			}
			else if (fad.Year == month.Year && fad.Month == month.Month && ass.HR_Contracts.IsFired && ass.HR_Contracts.DateFired.Value.Year == month.Year && ass.HR_Contracts.DateFired.Value.Month == month.Month)
			{//assigned and fired in the same month
				workDaysNorm = this.CalculateWorkDays(fad, ass.HR_Contracts.DateFired.Value, lstCalRows);
			}

			PF.lstShiftTypes = lstShiftTypes;
			if (ass.HR_WorkTime != null)
			{
				PF.WorkHours = ass.HR_WorkTime.WorkHours;
				PF.Norm = ass.HR_WorkTime.WorkHours * workDaysNorm;
			}
			PF.cRow = cRow;
			if (ass.GR_WorkHours != null)
			{
				PF.IsSumWorkTime = ass.GR_WorkHours.IsSumWorkTime;
			}
			else
			{
				PF.IsSumWorkTime = false;
			}
		    PF.RealDate = month;
			PF.CalculateHours();
			PF.IsDataChanged = false;
			if (PF.PF != null)
			{
				lstResult.Add(PF);
			}
			return lstResult;
		}

		public List<BranchMovementsViewModel> GetBranchMovements(int id_presenceForm)
		{
			var PF = new PFRow();
			PF.PF = this._databaseContext.GR_PresenceForms.FirstOrDefault(a => a.id_presenceForm == id_presenceForm);
			var result = this._databaseContext.GR_BranchMovements.Where(a => a.id_presenceForm == id_presenceForm).Select(a => new BranchMovementsViewModel()
			{
				Date = a.Date,
				id_branch = a.id_departmentTo,
				id_branchMovement = a.id_branchMovement,
				id_presenceForm =  a.id_presenceForm,
			}).ToList();
			foreach (var res in result)
			{
				res.ShiftType = (PF[res.Date.Day] == 13) ? "Дк" : "Нк";
			}
			return result;
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

			var lstAssignments =
				this._databaseContext.HR_Assignments.Where(
					a => a.HR_Contracts.IsFired == false 
					&& a.HR_StructurePositions.id_department == id_dep 
					&& a.ValidTo >= dateStart && a.AssignmentDate <= dateEnd).ToList();

			foreach (var contract in lstAssignments)
			{
				PFRow row = new PFRow();
				row.RealDate = month;
				row.PF = new GR_PresenceForms();
				row.PF.Date = month;
				row.PF.id_contract = contract.id_contract;
				row.PF.id_scheduleType = (int)ScheduleTypes.ForecastMonthSchedule;

				int j = 0;
				int max = days;
				if (contract.ValidTo < dateEnd)
				{
					max = contract.ValidTo.Day;
				}
				if (contract.AssignmentDate > dateStart)
				{
					j = contract.AssignmentDate.Day;
				}

					
				for (; j < max; j++)
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

		public int CalculateStartShift(DateTime date, int id_department)
		{
			date = new DateTime(date.Year, date.Month, 1);
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_department);

			if (dep == null)
			{
				return 0;
			}
			//if (dep.NumberShifts == 0 || dep.NumberShifts < 4)
			//{
			//	return 0;
			//}

			var ns = dep.UN_Departments2.NumberShifts;

			if (ns == 0 || ns < 3)
			{
				return 0;
			}

			var startShift = this._databaseContext.GR_StartShifts.FirstOrDefault(a => a.ShiftsNumber == ns);
			if (startShift == null)
			{
				return 0;
			}
			var startDate = startShift.StartDate;
			if (startDate == null)
			{
				return 0;
			}

			var countDays = (date - (DateTime)startDate).Days;
			var res = (countDays + startShift.StartShift) % ns;
            return ( res== 0)? ns : res;
		}

		public bool? GetShiftTypeByDate(DateTime startDate, int numberShifts, DateTime currentDate, int startShift, int departmentIndex)
		{
			var countDays = (currentDate - (DateTime)startDate).Days;
			var res = (countDays + startShift) % numberShifts;
			if (res == 0)
			{
				res = numberShifts;
			}
			if (res == departmentIndex + 1)
			{
				return true;
			}
			if ((departmentIndex == numberShifts - 1 && res == 1)
				|| res - 1 == departmentIndex + 1)
			{
				return false;
			}
			return null;
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
			    if (id_absenceType == AbsenceTypes.Sickness)
			    {
			        if (row[i] != (int)PresenceTypes.Nothing)
			        {
			            row[i] = (int)PresenceTypes.Sickness;
                    }
			        else
			        {
			            row[i] = (int)PresenceTypes.InactiveSickness;
                    }
			    }
			    else
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
			            case AbsenceTypes.UnpaidHoliday:
			                row[i] = (int) PresenceTypes.UnpaidHoliday;
			                break;
			            case AbsenceTypes.YearPaidHoliday:
			                row[i] = (int) PresenceTypes.YearPaidHoliday;
			                break;
			        }
			    }
			}
		}

		public bool IsMonthlyScheduleAlreadyGenerated(DateTime date, int id_scheduleType, int id_department = 0)
		{
            
			//var res = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == date.Year
			//																		&& p.Date.Month == date.Month
			//																		&& p.id_scheduleType == id_scheduleType);
            //var res = (from con in this._databaseContext.HR_Contracts
            //                 join ass in this._databaseContext.HR_Assignments on con.id_contract equals ass.id_contract into ac
            //                 from acc in ac.DefaultIfEmpty()
            //                 join per in this._databaseContext.UN_Persons on con.id_person equals per.id_person into pa
            //                 from spa in pa.DefaultIfEmpty()

             //where con.IsFired == true
             //&& acc.IsActive == true

             //select new PersonnelViewModel
             //{
             //    id_person = spa.id_person,
             //    Name = spa.Name,

		    var res = (from ass in this._databaseContext.HR_Assignments
		        join contract in this._databaseContext.HR_Contracts on ass.id_contract equals contract.id_contract into ac
		        from acc in ac.DefaultIfEmpty()
		        join sched in this._databaseContext.GR_PresenceForms on acc.id_contract equals sched.id_contract into sac
		        from sacc in sac.DefaultIfEmpty()
		        where sacc.Date.Year == date.Year
		              && sacc.Date.Month == date.Month
		              && ass.HR_StructurePositions.id_department == id_department
		              && sacc.id_scheduleType == id_scheduleType
		        select sacc).Count();
                      


            return res > 0;
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
			var cm1 = this._databaseContext.GR_Crews2.FirstOrDefault(c => (c.id_assignment1 == id_assignment
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

		public void SaveBranchMovements(List<BranchMovementsViewModel> lstVms)
		{
			int id_pf = lstVms.First().id_presenceForm;
            var dbMovements =
				this._databaseContext.GR_BranchMovements.Where(a => a.id_presenceForm == id_pf).ToList();

			foreach (var dbm in dbMovements)
			{
				var vm = lstVms.FirstOrDefault(a => a.id_branchMovement == dbm.id_branchMovement);
				if (vm != null)
				{
					dbm.id_departmentTo = vm.id_branch;
				}
			}
			this.Save();
		}

		public void SavePersonalSchedule(PFRow model)
		{
			if (model.PF != null)
			{
				this.GR_PresenceForms.Update(model.PF);
				this.Save();
			}

			var lstMovements = this._databaseContext.GR_BranchMovements.Where(a => a.id_presenceForm == model.PF.id_presenceForm);

			for (int i = 1; i <= DateTime.DaysInMonth(model.PF.Date.Year, model.PF.Date.Month); i ++)
			{
				var date = new DateTime(model.PF.Date.Year, model.PF.Date.Month, i);
				var move = lstMovements.FirstOrDefault(a => a.Date == date);
				if (model[i] != (int) PresenceTypes.BusinessTripDay && model[i] != (int) PresenceTypes.BusinessTripNight && move != null)
				{
					//get rid of obselete ones
					this._databaseContext.GR_BranchMovements.Remove(move);
				}
				else if (model[i] == (int) PresenceTypes.BusinessTripDay || model[i] == (int) PresenceTypes.BusinessTripNight)
				{
					//if not existing - create
					if (move == null)
					{
						move = new GR_BranchMovements();
						move.Date = date;
						move.id_presenceForm = model.PF.id_presenceForm;
						this._databaseContext.GR_BranchMovements.Add(move);
					}
					//if existing - leave as is
				}
			}
			this.Save();
		}

		public void DeleteCrew(CrewListViewModel model)
		{
			var crewToDelete = this._databaseContext.GR_Crews2.FirstOrDefault(c => c.id_crew == model.id_crew);
			if (crewToDelete != null)
			{
				this._databaseContext.GR_Crews2.Remove(crewToDelete);
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
					a => a.id_scheduleType == id_scheduleType && a.id_contract == contract.id_contract && a.Date.Month == date.Month && a.Date.Year == date.Year);

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
            if (this.IsDepartmentScheduleGenerated(id_department, date, (int)ScheduleTypes.FinalMonthSchedule) == true)
            {
                ThrowZoraException(ErrorCodes.ScheduleAlreadyApproved);
            }

            var lstDepartmnetForecastSchedules = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == date.Year
										&& p.Date.Month == date.Month
										&& p.id_scheduleType == (int)ScheduleTypes.ForecastMonthSchedule
										&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_department && a.IsActive == true) != null).ToList();

			var lstCurrentDepartmentSchedules = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year == date.Year
										&& p.Date.Month == date.Month
										&& (p.id_scheduleType == (int)ScheduleTypes.DailySchedule || p.id_scheduleType == (int)ScheduleTypes.PresenceForm)
										&& p.HR_Contracts.HR_Assignments.FirstOrDefault(a => a.HR_StructurePositions.id_department == id_department && a.IsActive == true) != null).ToList();

			foreach (var ss in lstCurrentDepartmentSchedules)
			{
				this.GR_PresenceForms.Delete(ss);
			}
			this.Save();

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

		//public void FinishMonth(DateTime month)
		//{
		//	CalendarRow cRow;

		//	using (var log = new NomenclaturesLogic())
		//	{
		//		cRow = log.FillCalendarRow(month);
		//	}

		//	var grpOvertime =
		//		this._databaseContext.GR_WorkTimeAbsence.Where(a => a.Date.Month == month.Month).GroupBy(a => new {a.id_contract});

		//	foreach (var gr in grpOvertime)
		//	{
		//		int id_contract = gr.First().id_contract;
		//		var ass =
		//			this._databaseContext.HR_Assignments.FirstOrDefault(a => a.id_contract == id_contract && a.IsActive == true);
		//		if (ass.HR_WorkTime == null)
		//		{
		//			continue;
		//		}
		//		var norm = ass.HR_WorkTime.WorkHours*cRow.WorkDays;

		//		double difference = 0.0;

		//		foreach (var ov in gr)
		//		{
		//			if(ov.WorkHours == null)
		//			{
		//				continue;
		//			}
		//			if (ov.IsPresence == true)
		//			{
		//				difference += (double)ov.WorkHours;
		//			}
		//			else
		//			{
		//				difference -= (double) ov.WorkHours;
		//			}
		//		}
		//		if (difference < 0.1)
		//		{
		//			continue;
		//		}
		//		difference -= norm;
		//		if (difference > 0.2 || difference < -0.2)
		//		{
					
		//			var wot = new GR_WorkTimeAbsence();
					
		//			wot.IsPresence = !(difference < 0);
		//			difference = Math.Abs(difference);

		//			wot.Date = month.AddMonths(1);
		//			wot.StartTime = new TimeSpan(0,0,0);
		//			wot.EndTime = new TimeSpan(0, 0, 0);
		//			wot.Reasons = string.Format("Прехвърляне на часове от {0}.{1}", month.Month, month.Year);
		//			wot.IsPrevMonthTransfer = true;
		//			wot.WorkHours = difference;
		//			wot.id_contract = id_contract;
		//			this._databaseContext.GR_WorkTimeAbsence.Add(wot);
		//		}
		//	}
		//	this.Save();
		//}

		//public void CopyCrews(DateTime date)
		//{
		//	var lstCrews = this._databaseContext.GR_Crews.ToList();
		//	var EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
		//	var StartDate = new DateTime(date.Year, date.Month, 1);
		//	foreach (var crew in lstCrews)
		//	{
		//		var c2 = new GR_Crews2();
		//		if (crew.IsTemporary == true)
		//		{
		//			c2.DateStart = (DateTime)crew.Date;
		//			c2.DateEnd = (DateTime)crew.Date;
		//		}
		//		else
		//		{
		//			c2.DateEnd = EndDate;
		//			c2.DateStart = StartDate;
		//		}
				
		//		c2.IsActive = true;
		//		c2.IsTemporary = crew.IsTemporary;
		//		c2.Name = crew.Name;
		//		c2.id_assignment1 = crew.id_assignment1;
		//		c2.id_assignment2 = crew.id_assignment2;
		//		c2.id_assignment3 = crew.id_assignment3;
		//		c2.id_assignment4 = crew.id_assignment4;
		//		c2.id_crewType = crew.id_crewType;
		//		c2.id_department = crew.id_department;
		//		this._databaseContext.GR_Crews2.Add(c2);
		//	}
		//	this.Save();
		//}

		public void CopyCrews2(DateTime date)
		{
			var refDate = date.AddMonths(-1);
			var lstCrews = this._databaseContext.GR_Crews2.Where(c => c.IsTemporary == false
																	&& c.DateStart.Year == refDate.Year 
																	&& c.DateStart.Month == refDate.Month).ToList();
			var EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			var StartDate = new DateTime(date.Year, date.Month, 1);
			var dp = date.AddMonths(-1);
			var PrevMonthEnd = new DateTime(dp.Year, dp.Month, DateTime.DaysInMonth(dp.Year, dp.Month));
			foreach (var crew in lstCrews)
			{
				if (crew.DateEnd == PrevMonthEnd)
				{
					var c2 = new GR_Crews2();
					c2.DateEnd = EndDate;
					c2.DateStart = StartDate;
					c2.IsActive = true;
					c2.IsTemporary = false;
					c2.Name = crew.Name;
					c2.id_assignment1 = crew.id_assignment1;
					c2.id_assignment2 = crew.id_assignment2;
					c2.id_assignment3 = crew.id_assignment3;
					c2.id_assignment4 = crew.id_assignment4;
					c2.id_crewType = crew.id_crewType;
					c2.id_department = crew.id_department;
					this._databaseContext.GR_Crews2.Add(c2);
				}
				else
				{
					int i = 0;
					i ++;
				}
			}
			this.Save();
		}

	    public void CopyDepartmentCrews(int id_department, DateTime date)
	    {
            var refDate = date.AddMonths(-1);
            var lstCrews = this._databaseContext.GR_Crews2.Where(c => c.IsTemporary == false
                                                                    && c.DateStart.Year == refDate.Year
                                                                    && c.DateStart.Month == refDate.Month
                                                                    && c.id_department == id_department).ToList();

            var EndDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            var StartDate = new DateTime(date.Year, date.Month, 1);
            var dp = date.AddMonths(-1);
            var PrevMonthEnd = new DateTime(dp.Year, dp.Month, DateTime.DaysInMonth(dp.Year, dp.Month));
            foreach (var crew in lstCrews)
            {
                if (crew.DateEnd == PrevMonthEnd)
                {
                    var c2 = new GR_Crews2();
                    c2.DateEnd = EndDate;
                    c2.DateStart = StartDate;
                    c2.IsActive = true;
                    c2.IsTemporary = false;
                    c2.Name = crew.Name;
                    c2.id_assignment1 = crew.id_assignment1;
                    c2.id_assignment2 = crew.id_assignment2;
                    c2.id_assignment3 = crew.id_assignment3;
                    c2.id_assignment4 = crew.id_assignment4;
                    c2.id_crewType = crew.id_crewType;
                    c2.id_department = crew.id_department;
                    this._databaseContext.GR_Crews2.Add(c2);
                }
                else
                {
                    int i = 0;
                    i++;
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

		public void FixWorkHours()
		{
			var lstAssignents = this._databaseContext.HR_Assignments.Where(a => a.id_workHours == null).ToList();

			foreach (var ass in lstAssignents)
			{
				ass.id_workHours = 2;
			}
			this.Save();
		}

	    public bool IsForecastLocked(int id_person, DateTime date)
	    {
            var ds = this._databaseContext.GR_PresenceForms.Any(a => a.Date.Month == date.Month && a.Date.Year == date.Year && a.id_scheduleType == (int)ScheduleTypes.DailySchedule && a.HR_Contracts.id_person == id_person);
	        return ds;
	    }

        public List<DPRowViewModel> GetDepartmentSchedules(int id_department, DateTime date)
        {
            var oneDate = new DateTime(date.Year, date.Month, 1);
            var department = this._databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_department);
            if (department == null)
            {
                return null;
            }
            var numberShifts = department.UN_Departments2.NumberShifts;

            var allDeps = this._databaseContext.UN_Departments.Where(d => d.id_departmentParent == department.id_departmentParent && d.id_departmentParent != d.id_department).ToList();
            var departmentIndex = allDeps.FindIndex(d => d.id_department == id_department);

            int ss = this.CalculateStartShift(oneDate, id_department);

            CalendarRow dRow = new CalendarRow(oneDate);
            CalendarRow nRow = new CalendarRow(oneDate);
            this.DepartmentHasShift(oneDate, ss, ref dRow, departmentIndex, numberShifts, true);
            this.DepartmentHasShift(oneDate, ss, ref nRow, departmentIndex, numberShifts, false);

            var sched = new DPRowViewModel();
            
            sched.Date = date;

            var lstShiftTypes = this._databaseContext.GR_ShiftTypes.Select(a => a).ToList();
            var day = lstShiftTypes.FirstOrDefault(a => a.id_shiftType == (int) PresenceTypes.DayShift);
            var night = lstShiftTypes.FirstOrDefault(a => a.id_shiftType == (int)PresenceTypes.NightShift);

            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                ((DPRow)sched)[i] = dRow[i] ? (int)PresenceTypes.DayShift : (nRow[i] ? (int)PresenceTypes.NightShift : 0);
                sched[i] = dRow[i] ? day.Name : (nRow[i] ? night.Name: "");
            }

            sched.CalculateHours();

            var result = new List<DPRowViewModel>();
            result.Add(sched);
            return result;
        }

        public int GetLeadShiftNumber(DateTime date, int id_department)
        {
            var dep = this._databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_department);

            int ns = 0;
            if (dep.id_department != dep.id_departmentParent)
            {
                ns = dep.UN_Departments2.NumberShifts;
            }

            if (ns < 3)
            {
                return 0;
            }

            var res = this.CalculateLeadDepartmentIndex(date, ns, true);
            return res + 1;
        }

        internal int CalculateLeadDepartmentIndex(DateTime date, int numberShifts, bool isDayShift)
        {
            if (numberShifts == 0 || numberShifts < 4)
            {
                return 0;
            }

            var ns = numberShifts;

            var startShift = this._databaseContext.GR_StartShifts.FirstOrDefault(a => a.ShiftsNumber == ns);
            var startDate = startShift?.StartDate;
            if (startDate == null)
            {
                return 0;
            }

            var countDays = (date - (DateTime)startDate).Days;
            if (isDayShift == false)
            {
                countDays--;
            }
            //remove one from the calculation because it is an index in array
            return (((countDays + startShift.StartShift - 1) % ns));
        }

        internal void DepartmentHasShift(DateTime month, int ss, ref CalendarRow srRow, int ix, int ns, bool IsDayShift)
        {
            for (int i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
            {
                var date = new DateTime(month.Year, month.Month, i);
                srRow[i] = false;
                if (this.CalculateLeadDepartmentIndex(date, ns, IsDayShift) == ix)
                {
                    srRow[i] = true;
                }
            }
        }

        public void HandleAbsenceSave(HR_Absence absence)
        {
            this.CheckAbsenceNoConflict(absence);
            if (absence.id_absence == 0)
            {
                this._databaseContext.HR_Absence.Add(absence);
                this.ProcessNewAbsence(absence);
            }
            else
            {
                using (var oldContext = new AmbulanceEntities())
                {
                    var oldAbsence = oldContext.HR_Absence.FirstOrDefault(a => a.id_absence == absence.id_absence);
                    if (oldAbsence != null)
                    {
                        this.ProcessEditAbsence(oldAbsence);
                        this.ProcessNewAbsence(absence);
                    }
                }
                this.HR_Absence.Update(absence);
            }
            this.Save();
        }

	    private void CheckAbsenceNoConflict(HR_Absence absence)
	    {
	        var lstOtherAbsences = this._databaseContext.HR_Absence.Where(a => a.id_contract == absence.id_contract && a.id_absence != absence.id_absence).ToList();

	        foreach (var abs in lstOtherAbsences)
	        {
	            if (abs.StartDate < absence.EndDate && absence.StartDate < abs.EndDate)
	            {
	                ThrowZoraException(ErrorCodes.OverlappingAbsence);
	            }
	        }
	    }

	    private void ProcessNewAbsence(HR_Absence absence)
	    {
            var contract = this._databaseContext.HR_Contracts.First(a => a.id_contract == absence.id_contract);

            var id_department = contract.HR_Assignments.FirstOrDefault(a => a.IsActive == true)?.HR_StructurePositions.id_department;
            var id_person = contract.id_person;
            
            if (id_department == null)
	        {
	            ThrowZoraException(ErrorCodes.AssignmentNotFoundError);
	            return;
	        }
	        for (var startDate = new DateTime(absence.StartDate.Year, absence.StartDate.Month, 1); 
                startDate.Year < absence.EndDate.Year || (startDate.Year == absence.EndDate.Year && startDate.Month <= absence.EndDate.Month);
                    startDate = startDate.AddMonths(1))
	        {
	            var schedule = this.GetDepartmentSchedules((int)id_department, startDate).FirstOrDefault();
	            //for each month of the absence
	            var lstPFRow = this.GetPersonalSchedule(id_person, startDate, ScheduleTypes.DailySchedule);
	            if (lstPFRow == null || lstPFRow.Count == 0)
	            {//find the forecast schedule
	                lstPFRow = this.GetPersonalSchedule(id_person, startDate, ScheduleTypes.ForecastMonthSchedule);
                }

	            if (lstPFRow == null || lstPFRow.Count == 0)
	            {
	                continue;
	            }

	            int i = 1;
	            if (absence.StartDate.Month < startDate.Month)
	            {
	                i = 1; //start from the beginning of the month
	            }
	            else if(absence.StartDate.Month == startDate.Month)
	            {
	                i = absence.StartDate.Day;
	            }

	            int end = 0;

	            if (absence.EndDate.Month > startDate.Month)
	            {
	                end = DateTime.DaysInMonth(startDate.Year, startDate.Month);
	            }
	            else
	            {
	                end = absence.EndDate.Day;
	            }
                
                List<HR_Absence> lstAbsence = new List<HR_Absence>();
                lstAbsence.Add(new HR_Absence()
                {
                    StartDate = new DateTime(startDate.Year, startDate.Month, i),
                    EndDate = new DateTime(startDate.Year, startDate.Month, end),
                    id_absenceType = absence.id_absenceType
                });
                CalendarRow row = new CalendarRow(startDate, false);
	            var Pf = lstPFRow.First();
	            
	            this.InsertAbsenceInSchedule(Pf, lstAbsence, row);
	            
                this.Save();
	        }
        }

        private void ProcessEditAbsence(HR_Absence oldAbsence)
        {
            var contract = this._databaseContext.HR_Contracts.First(a => a.id_contract == oldAbsence.id_contract);

            var id_department =contract.HR_Assignments.FirstOrDefault(a => a.IsActive == true)?.HR_StructurePositions.id_department;
            var id_person = contract.id_person;
            if (id_department == null)
            {
                ThrowZoraException(ErrorCodes.AssignmentNotFoundError);
                return;
            }
            for (var startDate = new DateTime(oldAbsence.StartDate.Year, oldAbsence.StartDate.Month, 1);
                startDate.Year < oldAbsence.EndDate.Year || (startDate.Year == oldAbsence.EndDate.Year && startDate.Month <= oldAbsence.EndDate.Month);
                    startDate= startDate.AddMonths(1))
            {
                var schedule = this.GetDepartmentSchedules((int)id_department, startDate).FirstOrDefault();
                //for each month of the absence
                var lstPFRow = this.GetPersonalSchedule(id_person, startDate, ScheduleTypes.DailySchedule);
                if (lstPFRow == null || lstPFRow.Count == 0)
                {//find the forecast schedule
                    lstPFRow = this.GetPersonalSchedule(id_person, startDate, ScheduleTypes.ForecastMonthSchedule);
                }

                if (lstPFRow == null || lstPFRow.Count == 0)
                {
                    continue;
                }

                int i = 1;
                if (oldAbsence.StartDate.Month < startDate.Month)
                {
                    i = 1; //start from the beginning of the month
                }
                else if (oldAbsence.StartDate.Month == startDate.Month)
                {
                    i = oldAbsence.StartDate.Day;
                }

                int end = 0;

                if (oldAbsence.EndDate.Month > startDate.Month)
                {
                    end = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                }
                else
                {
                    end = oldAbsence.EndDate.Day;
                }

                var Pf = lstPFRow.First();
                for (; i <= end; i++)
                {
                    Pf[i] = ((DPRow)schedule)[i];
                }
                this.Save();
            }
        }
    }
}
