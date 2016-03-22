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
	public class SchedulesLogic : BaseLogic
	{
		public SchedulesLogic()
		{
		}

		#region Service Methods
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
								  WorkTime = spa.GR_Ambulances.WorkTime,
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
			if (model.id_assignment1 != null && model.id_assignment1 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment1 == model.id_assignment1
																					|| a.id_assignment4 == model.id_assignment1)
																					&& a.id_crew != model.id_crew).ToList();
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
																					&& a.id_crew != model.id_crew).ToList();
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
																					&& a.id_crew != model.id_crew).ToList();
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

			if (model.id_assignment1 != null && model.id_assignment1 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment1 == model.id_assignment4
																					|| a.id_assignment2 == model.id_assignment4
																					|| a.id_assignment3 == model.id_assignment4
																					|| a.id_assignment4 == model.id_assignment4)
																					&& a.id_crew != model.id_crew).ToList();
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

		public PFRow GetPersonalSchedule(int id_person, DateTime month)
		{
			var ass = this._databaseContext.HR_Assignments.FirstOrDefault(c => c.IsActive == true
																		&& c.HR_Contracts.id_person == id_person);

			PFRow PF = new PFRow();

			PF.PF = this._databaseContext.GR_PresenceForms.FirstOrDefault(p => p.Date.Year == month.Year && p.Date.Month == month.Month && p.id_contract == ass.id_contract);

			return PF;
		}

		public GR_DriverAmbulances GetActiveDriverAmbulance(int id_driverAssignment)
		{
			var result = this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == id_driverAssignment && a.IsActive == true);
			return result;
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

				List<UN_Departments> lstOrderedDepartments = new List<UN_Departments>();

				for (int i = 0; i < department.NumberShifts; i++)
				{
					int depIndex = 0;
					if (i + startShift >= department.NumberShifts)
					{
						depIndex = i + startShift - department.NumberShifts;
					}
					else
					{
						depIndex = i + startShift;
					}
					lstOrderedDepartments.Add(subDeps[depIndex]);
				}

				var days = DateTime.DaysInMonth(month.Year, month.Month);

				var dateStart = new DateTime(month.Year, month.Month, 1);
				var dateEnd = dateStart.AddMonths(1);

				for (int i = 0; i < department.NumberShifts; i++)
				{
					var id_dep = lstOrderedDepartments[i].id_department;
                    var lstContracts = this._databaseContext.HR_Assignments.Where(a => a.HR_Contracts.IsFired == false && a.HR_StructurePositions.id_department == id_dep).ToList();

					foreach (var contract in lstContracts)
					{
						PFRow row = new PFRow();
						row.RealDate = month;
						row.PF = new GR_PresenceForms();
						row.PF.Date = month;
						row.PF.id_contract = contract.id_contract;
						row.PF.id_scheduleType = (int) ScheduleTypes.ForecastMonthSchedule;

						for (int j = 0; j < days; j++)
						{
							if ((j + department.NumberShifts - i ) % department.NumberShifts == 0)
							{
								row[j + 1] = (int)PresenceTypes.DayShift;
							}
							else if((j + department.NumberShifts - i ) % department.NumberShifts == 1)
							{
								row[j + 1] = (int)PresenceTypes.NightShift;
							}
						}

						var absences = this._databaseContext.HR_Absence.Where(a => a.id_contract == contract.id_contract
																					&& ((a.StartDate <= dateStart && a.EndDate >= dateEnd)
																					|| (a.StartDate >= dateStart && a.StartDate <= dateEnd)
																					|| (a.EndDate >= dateStart && a.EndDate <= dateEnd))).ToList();

						this.InsertAbsenceInSchedule(row, absences, cRow);

						lstScheduleRows.Add(row);
					}
				}
			}
			this.SaveGeneratedSchedules(lstScheduleRows);
		}

		private void SaveGeneratedSchedules(List<PFRow> lstScheduleRows )
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
				count ++;
			}
			this.Save();
		}

		private void InsertAbsenceInSchedule(PFRow row, List<HR_Absence> absences, CalendarRow cRow)
		{
			var DateStart = new DateTime(row.RealDate.Year, row.RealDate.Month, 1);
			var DateEnd = DateStart.AddMonths(1);
			foreach (var absence in absences)
			{
				if (absence.StartDate <= DateStart && absence.EndDate >= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, 1, DateTime.DaysInMonth(row.RealDate.Year, row.RealDate.Month), cRow);
				}
				else if (absence.StartDate >= DateStart && absence.EndDate <= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, DateStart.Day, DateEnd.Day, cRow);
				}
				else if (absence.StartDate <= DateStart && absence.EndDate <= DateEnd)
				{
					this.InitAbsenceInPF(row, (AbsenceTypes)absence.id_absenceType, 1, DateEnd.Day, cRow);
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
				if (cRow[i] == true)
				{
					switch (id_absenceType)
					{
						case AbsenceTypes.BusinessTrip:
							row[i] = (int)PresenceTypes.BusinessTrip;
							break;
						case AbsenceTypes.Education:
							row[i] = (int)PresenceTypes.Education;
							break;
						case AbsenceTypes.Motherhood:
							row[i] = (int)PresenceTypes.Motherhood;
							break;
						case AbsenceTypes.MotherhoodExtend:
							row[i] = (int)PresenceTypes.Motherhood;
							break;
						case AbsenceTypes.MotherhoodSickness:
							row[i] = (int)PresenceTypes.Motherhood;
							break;
						case AbsenceTypes.OtherPaidHoliday:
							row[i] = (int)PresenceTypes.OtherPaidHoliday;
							break;
						case AbsenceTypes.Sickness:
							row[i] = (int)PresenceTypes.Sickness;
							break;
						case AbsenceTypes.UnpaidHoliday:
							row[i] = (int)PresenceTypes.UnpaidHoliday;
							break;
						case AbsenceTypes.YearPaidHoliday:
							row[i] = (int)PresenceTypes.YearPaidHoliday;
							break;
					}
				}
			}
		}

		#endregion
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
