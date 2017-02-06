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

		//to be used by incomplete member list only
		//internal int numberShifts;
		internal int startShift;
		internal DateTime startDate;
		internal int departmentIndex;

		internal CalendarRow cRow;

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

			this.lstAllAssignments = this._databaseContext.HR_Assignments
				.Select(a => new PersonnelViewModel
				{
					id_person = a.HR_Contracts.id_person,
					Name = a.HR_Contracts.UN_Persons.Name,
					Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
					id_assignment = a.id_assignment,
					id_contract = a.id_contract,
					id_department = a.HR_StructurePositions.id_department,
					Level1 = a.HR_StructurePositions.UN_Departments.Name,
					WorkHours = a.HR_WorkTime.WorkHours,
					WorkZoneDay = a.GR_WorkHours.DayHours,
					WorkZoneNight = a.GR_WorkHours.NightHours,
					id_positionType = a.HR_StructurePositions.HR_GlobalPositions.id_positionType,
					Order = a.HR_StructurePositions.Order,
					ShortPosition = a.HR_StructurePositions.HR_GlobalPositions.NameShort,
					IsSumWorkTime = a.GR_WorkHours.IsSumWorkTime,
					FirstAssignedAt = a.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false).AssignmentDate,
					AssignedAt = a.AssignmentDate,
					ValidTo = (DateTime)a.ValidTo,
					FiredAt = (a.HR_Contracts.IsFired) ? a.HR_Contracts.DateFired : null,
					//PositionOrder = (a.HR_StructurePositions.HR_GlobalPositions.NM_PositionTypes.)
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
			var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment
												&& c.DateStart.Month == Date.Month && c.DateStart.Year == Date.Year)
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
					cmv.DateEnd = crew.DateEnd;
					cmv.DateStart = crew.DateStart;
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
			cmv.DateEnd = crew.DateEnd;
			cmv.DateStart = crew.DateStart;
			cmv.Background = counter % 2 == 0 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Thistle);
		}

		internal bool IsCrewFull(DateTime date, CrewScheduleListViewModel cmv, bool IsDayShift)
		{
			var ct = (CrewTypes)cmv.id_crewType;

			switch (ct)
			{
				case CrewTypes.Reanimation:
				case CrewTypes.Doctor:
					if (cmv.id_person == 0 //if doctor or driver is missing - not full
						|| cmv.LstCrewMembers[0].id_person == 0)
					{
						return false;
					}
					if (IsDayShift)
					{
						if (cmv[date.Day] != (int)PresenceTypes.DayShift //check driver shift
							&& cmv[date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.DayShift //check doctor shift
							&& cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.RegularShift)
						{
							return false;
						}
					}
					else
					{
						if (cmv[date.Day] != (int)PresenceTypes.NightShift) //check driuver shift
						{
							return false;
						}
						if (cmv.LstCrewMembers[0][date.Day] != (int)PresenceTypes.NightShift) //check doctor shift
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

		public List<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd, int id_scheduleType = 1, bool filterOtherShifts = false)
		{
			if (id_selectedDepartment == 0)
			{
				return null;
			}
			DateTime startMonth = new DateTime(dateCurrent.Year, dateCurrent.Month, 1);
			DateTime endMonth = new DateTime(dateCurrent.Year, dateCurrent.Month, DateTime.DaysInMonth(dateCurrent.Year, dateCurrent.Month));
			this.cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == dateCurrent.Year && a.date.Month == dateCurrent.Month);
			if (this.cRow == null)
			{
				return null;
			}

			List<PersonnelViewModel> lstAssignments;

			lstAssignments = lstAllAssignments.Where(a => a.id_department == id_selectedDepartment)
				.ToList();
			lstAssignments = lstAssignments.Where(a => startMonth <= a.ValidTo && a.AssignedAt < endMonth).ToList();
			//bool overlap = a.start < b.end && b.start < a.end;
			var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment
																&& c.DateStart.Month == dateCurrent.Month && c.DateStart.Year == dateCurrent.Year && c.DateStart <= c.DateEnd).OrderBy(c => c.Name).ToList();

			List<CrewScheduleListViewModel> lstCrewModelDraft = new List<CrewScheduleListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(dateBegin, dateCurrent, dateEnd, id_scheduleType, lstAssignments, crew, cmv, crew.id_assignment1);
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
					cmv.RealDate = dateCurrent;
					cmv.DateStart = crew.DateStart;
					cmv.DateEnd = crew.DateEnd;
				}
				lstCrewModelDraft.Add(cmv);

				if (crew.id_assignment2 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(dateBegin, dateCurrent, dateEnd, id_scheduleType, lstAssignments, crew, cp, crew.id_assignment2);
					if (cmv.WorkTime != string.Empty && cmv.WorkTime != null)
					{
						cp.WorkTime = cmv.WorkTime;
					}
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
					}
					cmv.LstCrewMembers.Add(cp);
					//lstCrewModel.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(dateBegin, dateCurrent, dateEnd, id_scheduleType, lstAssignments, crew, cp, crew.id_assignment3);
					if (cmv.WorkTime != string.Empty && cmv.WorkTime != null)
					{
						cp.WorkTime = cmv.WorkTime;
					}
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
					}
					cmv.LstCrewMembers.Add(cp);
					//lstCrewModel.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(dateBegin, dateCurrent, dateEnd, id_scheduleType, lstAssignments, crew, cp, crew.id_assignment4);
					if (cmv.WorkTime != string.Empty && cmv.WorkTime != null)
					{
						cp.WorkTime = cmv.WorkTime;
					}
					if (crew.IsTemporary == false)
					{
						lstAssignments.Remove(ass);
					}
					cmv.LstCrewMembers.Add(cp);
					//lstCrewModel.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}
			}

			AddFreeAssignmentsToListCrewModel(id_selectedDepartment, dateBegin, dateCurrent, dateEnd, id_scheduleType, lstAssignments, lstCrewModelDraft);

			FilterTemporaryCrewsPresences(dateCurrent, lstCrewModelDraft);

			if (filterOtherShifts == false)
			{
				var lstExternal = FindPeopleFromOtherShifts(dateCurrent, lstCrewModelDraft, id_selectedDepartment, id_scheduleType);
				if (lstExternal != null)
				{
					lstCrewModelDraft.AddRange(lstExternal);
				}
			}

			var lstMovements = FindBranchMovements(dateCurrent, lstCrewModelDraft, id_selectedDepartment, id_scheduleType);
			if (lstMovements != null)
			{
				lstCrewModelDraft.AddRange(lstMovements);
			}

			var lstCrewModel = FormDisplayListCrewModel(lstCrewModelDraft);

			//var lstIncompleteMembers = this.CreateIncompleteMembersList(lstCrewModelDraft, id_selectedDepartment, dateCurrent);
			//if (lstIncompleteMembers != null)
			//{
			//	lstCrewModel.AddRange(lstIncompleteMembers);
			//}

			//lstCrewModel = lstCrewModel.OrderBy(a => a.CrewName).ToList();


			return lstCrewModel;
		}

		//public List<CrewScheduleListViewModel> FindBranchMovements(DateTime dateCurrent, int id_selectedDepartment,
		//	int id_scheduleType)
		//{
		//	var dep = _databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_selectedDepartment);
		//	var 
		//	return null;
		//}

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

		public int GetDepartmentScheduledForDate(DateTime date, int id_department, bool IsDayShift)
		{
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_department);
			var AllDpes = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == dep.id_departmentParent && a.id_departmentParent != a.id_department).OrderBy(a => a.TreeOrder).ToList();


			int ns = 0;
			if (dep.id_department != dep.id_departmentParent)
			{
				ns = dep.UN_Departments2.NumberShifts;
			}

			if (ns < 3)
			{
				return 0;
			}

			var res = this.CalculateLeadDepartmentIndex(date, ns, IsDayShift);
			return AllDpes[res].id_department;
		}

		internal int CalculateLeadDepartmentIndex(DateTime date, int NumberShifts, bool isDayShift)
		{
			if (NumberShifts == 0 || NumberShifts < 4)
			{
				return 0;
			}

			var ns = NumberShifts;

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

		private List<CrewScheduleListViewModel> FindBranchMovements(DateTime month, List<CrewScheduleListViewModel> lstCrewModelDraft, int id_department, int id_scheduleType)
		{
			List<CrewScheduleListViewModel> lstExternal = new List<CrewScheduleListViewModel>();

			if (id_department == 0)
			{
				return null;
			}
			int ss = this.CalculateStartShift(month, id_department);
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);

			var allDeps = this._databaseContext.UN_Departments.Where(d => d.id_departmentParent == dep.id_departmentParent && d.id_departmentParent != d.id_department).ToList();
			this.departmentIndex = allDeps.FindIndex(d => d.id_department == id_department);

			CalendarRow dRow = new CalendarRow(month);
			CalendarRow nRow = new CalendarRow(month);
			this.DepartmentHasShift(month, ss, ref dRow, this.departmentIndex, dep.UN_Departments2.NumberShifts, true);
			this.DepartmentHasShift(month, ss, ref nRow, this.departmentIndex, dep.UN_Departments2.NumberShifts, false);
			//Add code for branch movements
			var lstMovements = _databaseContext.GR_BranchMovements.Where(a => a.id_departmentTo == dep.id_departmentParent
																				&& a.Date.Year == month.Year
																				&& a.Date.Month == month.Month).ToList();
			var lstMovementsToRemove = new List<GR_BranchMovements>();

			//filter movements not for this department
			foreach (var move in lstMovements)
			{
				bool remove = true;
				var PF = new PFRow();
				PF.PF = move.GR_PresenceForms;

				int i = move.Date.Day;

				if ((dRow[i] && PF[i] == (int)PresenceTypes.BusinessTripDay)
					|| (nRow[i] && PF[i] == (int)PresenceTypes.BusinessTripNight))
				{
					continue;
				}
				lstMovementsToRemove.Add(move);
			}

			foreach (var rm in lstMovementsToRemove)
			{
				lstMovements.Remove(rm);
			}
			//Distinct the moved people
			var lstContractIds = lstMovements.Select(a => a.GR_PresenceForms.id_contract).ToList();
			lstContractIds = lstContractIds.Distinct().ToList();

			var lstMoveAsses = new List<PersonnelViewModel>();

			foreach (var con in lstContractIds)
			{
				var ass = this.lstAllAssignments.FirstOrDefault(a => a.id_contract == con);
				lstMoveAsses.Add(ass);
			}

			var lstTmpMove = new List<CrewScheduleListViewModel>();
			this.AddFreeAssignmentsToListCrewModel(id_department, month, month, month, id_scheduleType, lstMoveAsses, lstTmpMove);
			lstTmpMove = this.CopyTemporaryList(lstTmpMove);
			this.RemoveNonShiftExternalAssignments(dRow, nRow, lstTmpMove, true);

			//Add them to the list
			lstExternal.AddRange(lstTmpMove);

			return lstExternal;
		}

		private List<CrewScheduleListViewModel> FindPeopleFromOtherShifts(DateTime month, List<CrewScheduleListViewModel> lstCrewModelDraft, int id_department, int id_scheduleType)
		{
			DateTime startMonth = new DateTime(month.Year, month.Month, 1);
			DateTime endMonth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
			List<PersonnelViewModel> lstPvm = new List<PersonnelViewModel>();
			List<CrewScheduleListViewModel> lstExternal = new List<CrewScheduleListViewModel>();

			if (id_department == 0)
			{
				return null;
			}
			int ss = this.CalculateStartShift(month, id_department);
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);

			var allDeps = this._databaseContext.UN_Departments.Where(d => d.id_departmentParent == dep.id_departmentParent && d.id_departmentParent != d.id_department).ToList();
			this.departmentIndex = allDeps.FindIndex(d => d.id_department == id_department);

			CalendarRow dRow = new CalendarRow(month);
			CalendarRow nRow = new CalendarRow(month);
			this.DepartmentHasShift(month, ss, ref dRow, this.departmentIndex, dep.UN_Departments2.NumberShifts, true);
			this.DepartmentHasShift(month, ss, ref nRow, this.departmentIndex, dep.UN_Departments2.NumberShifts, false);

			//search for all employees from the other departments that have shift here
			foreach (var sdep in allDeps)
			{
				if (sdep.id_department == dep.id_department)
				{
					continue;
				}
				var lstTmp = new List<CrewScheduleListViewModel>();

				var lstAssignments = lstAllAssignments.Where(a => a.id_department == sdep.id_department)
				.ToList();
				lstAssignments = lstAssignments.Where(a => startMonth < a.ValidTo && a.AssignedAt < endMonth).ToList();

				this.AddFreeAssignmentsToListCrewModel(id_department, month, month, month, id_scheduleType, lstAssignments, lstTmp);
				lstTmp = this.CopyTemporaryList(lstTmp);
				this.RemoveNonShiftExternalAssignments(dRow, nRow, lstTmp);
				this.ReassembleCrews(lstTmp, month, sdep.id_department);
				//this.ReassembleCrews
				lstExternal.AddRange(lstTmp);
				//return
			}

			

			return lstExternal;
		}

		private List<CrewScheduleListViewModel> CopyTemporaryList(List<CrewScheduleListViewModel> lstTmp)
		{
			List<CrewScheduleListViewModel> lstCopy = new List<CrewScheduleListViewModel>();
			foreach (var l in lstTmp)
			{
				var cp = new CrewScheduleListViewModel();
				this.CopyCrewScheduleListViewModel(out cp, l, 1);
				for (int i = 1; i < 32; i++)
				{
					cp[i] = l[i];
				}
				lstCopy.Add(cp);
			}
			return lstCopy;
		}

		private void ReassembleCrews(List<CrewScheduleListViewModel> lstExternal, DateTime month, int id_department)
		{
			var lstToDel = new List<CrewScheduleListViewModel>();
			var lstDepCrews = this.lstCrews.Where(a => a.DateStart.Year == month.Year && a.DateStart.Month == month.Month && a.id_department == id_department).ToList();
			foreach (var cr in lstExternal)
			{
				cr.DateStart = new DateTime(month.Year, month.Month, 1);
				cr.DateEnd = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
				var crew = lstDepCrews.FirstOrDefault(c => c.id_assignment1 == cr.id_assignment);
				if (crew != null)
				{
					switch ((CrewTypes)crew.id_crewType)
					{
						case CrewTypes.Reanimation:
						case CrewTypes.Doctor:
							var crd = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment2);
							if (crd == null)
							{
								continue; //the crew is not full
							}
							break;
						case CrewTypes.Paramedic:
							var crd2 = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment3);
							if (crd2 == null)
							{
								continue; //the crew is not full
							}
							break;
						case CrewTypes.Corpse:
							var crd3 = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment4);
							if (crd3 == null)
							{
								continue; //the crew is not full
							}
							break;
					}
					cr.id_crewType = crew.id_crewType;
					cr.id_crew = crew.id_crew;
					var c1 = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment2);
					if (c1 != null)
					{
						lstToDel.Add(c1);
						c1.id_crew = cr.id_crew;
						cr.LstCrewMembers.Add(c1);
					}
					else
					{
						cr.LstCrewMembers.Add(new CrewScheduleListViewModel());
					}

					var c2 = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment3);
					if (c2 != null)
					{
						lstToDel.Add(c2);
						c2.id_crew = cr.id_crew;
						cr.LstCrewMembers.Add(c2);
					}
					else
					{
						cr.LstCrewMembers.Add(new CrewScheduleListViewModel());
					}

					var c3 = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment4);
					if (c3 != null)
					{
						lstToDel.Add(c3);
						c3.id_crew = cr.id_crew;
						cr.LstCrewMembers.Add(c3);
					}
					else
					{
						cr.LstCrewMembers.Add(new CrewScheduleListViewModel());
					}
				}
			}

			foreach (var de in lstToDel)
			{
				lstExternal.Remove(de);
			}
		}

		private void RemoveNonShiftExternalAssignments(CalendarRow dRow, CalendarRow nRow, List<CrewScheduleListViewModel> lstExternal, bool IsMovement = false)
		{
			List<CrewScheduleListViewModel> toDel = new List<CrewScheduleListViewModel>();
			foreach (var per in lstExternal)
			{
				bool remFlag = true;
				for (int i = 1; i < 32; i++)
				{
					if (dRow[i])
					{
						if ((per[i] == (int)PresenceTypes.RegularShift || per[i] == (int)PresenceTypes.DayShift) && IsMovement == false)
						{
							remFlag = false;
						}
						else if ((per[i] == (int) PresenceTypes.BusinessTripDay) && IsMovement == true)
						{
							remFlag = false;
						}
						else
						{
							per[i] = 0;
						}
					}
					else if (nRow[i])
					{
						if ((per[i] == (int)PresenceTypes.NightShift) && (IsMovement == false))
						{
							remFlag = false;
						}
						else if ((per[i] == (int)PresenceTypes.BusinessTripNight) && IsMovement == true)
						{
							remFlag = false;
						}
						else
						{
							per[i] = 0;
						}
					}
					else 
					{
						per[i] = 0;
					}
				}
				if (remFlag)
				{
					toDel.Add(per);
				}
			}

			foreach (var ass in toDel)
			{
				lstExternal.Remove(ass);
			}
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

		private List<CrewScheduleListViewModel> CreateIncompleteMembersList(List<CrewScheduleListViewModel> lstCrewModel, int id_department, DateTime date)
		{
			var lstTemporaryCrews = lstCrewModel.Where(c => c.IsTemporary).ToList();
			var lstStationeryCrews = lstCrewModel.Where(c => c.IsTemporary == false).ToList();

			//var dep = this._databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_department);
			//numberShifts = dep.UN_Departments2.NumberShifts;
			//var ss = this._databaseContext.GR_StartShifts.FirstOrDefault(a => a.ShiftsNumber == numberShifts);
			//startDate = ss.StartDate;
			//startShift = ss.StartShift;

			List<CrewScheduleListViewModel> lstIncomplete = new List<CrewScheduleListViewModel>();

			//foreach (var crew in lstStationeryCrews)
			//{
			//	for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
			//	{
			//		if (this.IsCrewFull(new DateTime(date.Year, date.Month, i), crew, true) == false
			//			&& this.IsCrewFull(new DateTime(date.Year, date.Month, i), crew, false) == false)
			//		{
			//			this.AddPersonToIncompleteListIfFree(crew, i, lstTemporaryCrews, date, lstCrewModel, lstIncomplete, id_department);
			//			foreach (var mem in crew.LstCrewMembers)
			//			{
			//				this.AddPersonToIncompleteListIfFree(mem, i, lstTemporaryCrews, date, lstCrewModel, lstIncomplete, id_department);
			//			}
			//		}
			//	}
			//}
			return lstIncomplete;
		}

		private void AddPersonToIncompleteListIfFree(CrewScheduleListViewModel person, int dayIndex, List<CrewScheduleListViewModel> lstTemporaryCrews, DateTime date, List<CrewScheduleListViewModel> lstCrewModel, List<CrewScheduleListViewModel> lstIncomplete, int id_department)
		{
			//if (person.id_person == 0)
			//{
			//	return;
			//}
			//bool HasDayShift = (person[dayIndex] == (int)PresenceTypes.RegularShift || person[dayIndex] == (int)PresenceTypes.DayShift);
			//bool HasNightShift = (person[dayIndex] == (int)PresenceTypes.NightShift);
			//bool? shiftType = this.GetShiftTypeByDate(startDate, numberShifts, new DateTime(date.Year, date.Month, dayIndex), startShift, this.departmentIndex);

			//if ((HasDayShift && shiftType == true)
			//	|| (HasNightShift && shiftType == false))
			//{
			//	int id_p = person.id_person;
			//	var lstTmpPersonCrews = lstTemporaryCrews.Where(c => (c.id_person == id_p || c.LstCrewMembers.Count(p => p.id_person == id_p) > 0)
			//									 && c.DateStart.Day <= dayIndex && c.DateEnd.Day >= dayIndex
			//									 && c.LstCrewMembers.Count > 0).ToList();

			//	if (lstTmpPersonCrews.Count > 0)
			//	{
			//		bool IsTmpCrewFull = false;
			//		//for (int j = 0; j < lstTmpPersonCrews.Count; j++)
			//		//{
			//		//	//If there is at lest one temporary crew that is full it takes advantage
			//		//	if ((this.IsCrewFull(new DateTime(date.Year, date.Month, dayIndex), lstTmpPersonCrews[j], true) && id_department == this.GetDepartmentScheduledForDate(new DateTime(date.Year, date.Month, dayIndex), id_department, true))
			//		//		|| (this.IsCrewFull(new DateTime(date.Year, date.Month, dayIndex), lstTmpPersonCrews[j], false) && id_department == this.GetDepartmentScheduledForDate(new DateTime(date.Year, date.Month, dayIndex), id_department, false)))
			//		//	{
			//		//		IsTmpCrewFull = true;
			//		//		return;
			//		//	}
			//		//}
			//	}

			//	int id_crew = person.id_crew;
			//	var perCrew = lstCrewModel.FirstOrDefault(c => c.id_crew == id_crew && c.LstCrewMembers.Count > 0);
			//	if (perCrew == null)
			//	{
			//		return;
			//	}

			//	CrewScheduleListViewModel ip = lstIncomplete.FirstOrDefault(p => p.id_person == id_p);
			//	if (ip != null)
			//	{
			//		ip[dayIndex] = person[dayIndex];
			//	}
			//	else
			//	{
			//		CopyCrewScheduleListViewModel(out ip, person, dayIndex);
			//		ip.IsIncomplete = true;
			//		lstIncomplete.Add(ip);
			//	}
			//}
		}

		private void CopyCrewScheduleListViewModel(out CrewScheduleListViewModel ip,
			CrewScheduleListViewModel person, int i)
		{
			ip = new CrewScheduleListViewModel();
			ip.BaseDepartment = person.BaseDepartment;
			ip.CrewName = "";
			ip.CrewType = "";
			ip.DateStart = person.DateStart;
			ip.DateEnd = person.DateEnd;
			ip.IsActive = true;
			ip.IsSumWorkTime = person.IsSumWorkTime;
			ip.IsTemporary = true;
			ip.LstCrewMembers = new List<CrewScheduleListViewModel>();
			ip.Name = person.Name;
			ip.PF = new GR_PresenceForms();
			ip.Position = person.Position;
			ip.RegNumber = person.RegNumber;
			ip.RowPosition = person.RowPosition;
			ip.ShortPosition = person.ShortPosition;
			ip.WorkTime = person.WorkTime;
			ip.id_assignment = person.id_assignment;
			ip.id_contract = person.id_contract;
			ip.id_positionType = person.id_positionType;
			ip.id_crew = person.id_crew;
			ip.id_crewType = person.id_crewType;
			ip.id_person = person.id_person;
			ip.id_positionType = person.id_positionType;
			ip.id_department = person.id_department;
			ip[i] = person[i];
			ip.IsIncomplete = false;
			ip.lstShiftTypes = person.lstShiftTypes;
		}

		private void AddFreeAssignmentsToListCrewModel(int id_selectedDepartment, DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd, int id_scheduleType,
			List<PersonnelViewModel> lstAssignments, List<CrewScheduleListViewModel> lstCrewModel)
		{
			DateTime startMonth = new DateTime(dateCurrent.Year, dateCurrent.Month, 1);
			DateTime endMonth = new DateTime(dateCurrent.Year, dateCurrent.Month, DateTime.DaysInMonth(dateCurrent.Year, dateCurrent.Month));
			lstAssignments = lstAssignments.OrderBy(a => a.Order).ThenBy(a => a.Name).ToList();
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();
				cmv.DateStart = startMonth;
				cmv.DateEnd = endMonth;
				this.FillPersonalCrewScheduleModel(dateBegin, dateCurrent, dateEnd, id_scheduleType, lstAssignments, null, cmv, ass.id_assignment);

				cmv.id_department = (int)ass.id_department;
				cmv.IsActive = true;

				lstCrewModel.Add(cmv);
			}
		}

		private static List<CrewScheduleListViewModel> FormDisplayListCrewModel(List<CrewScheduleListViewModel> lstCrewModelDraft)
		{
			List<CrewScheduleListViewModel> lstCrewModel = new List<CrewScheduleListViewModel>();
			foreach (var cr in lstCrewModelDraft)
			{
				lstCrewModel.Add(cr);
				foreach (var cm in cr.LstCrewMembers)
				{
					if (cm.id_person != 0)
					{
						lstCrewModel.Add(cm);
					}
				}
			}
			return lstCrewModel;
		}

		private void FilterTemporaryCrewsPresences(DateTime date, List<CrewScheduleListViewModel> lstCrewModel)
		{
			var lstTemporaryCrews = lstCrewModel.Where(c => c.IsTemporary && c.LstCrewMembers.Count > 0).ToList();

			foreach (var crew in lstTemporaryCrews)
			{
				if (crew.id_person == 0)
				{
					continue;
				}
				int i;

				CrewScheduleListViewModel tmpCrew;

				this.CopyCrewScheduleListViewModel(out tmpCrew, crew, 0);
				tmpCrew.PF = new GR_PresenceForms();

				foreach (var subCrew in crew.LstCrewMembers)
				{
					CrewScheduleListViewModel subMember;
					this.CopyCrewScheduleListViewModel(out subMember, subCrew, 0);
					subMember.PF = new GR_PresenceForms();
					tmpCrew.LstCrewMembers.Add(subMember);
				}
				for (i = crew.DateStart.Day; i <= crew.DateEnd.Day; i++)
				{
					var dt = new DateTime(date.Year, date.Month, i);
					if (this.IsCrewFull(dt, crew, true) || this.IsCrewFull(dt, crew, false))
					{
						tmpCrew[i] = crew[i];
						for (int j = 0; j < crew.LstCrewMembers.Count; j++)
						{
							if (crew.LstCrewMembers[j].id_person != 0)
							{
								tmpCrew.LstCrewMembers[j][i] = crew.LstCrewMembers[j][i];
							}
						}
					}
				}
				crew.PF = tmpCrew.PF;
				for (int j = 0; j < crew.LstCrewMembers.Count; j++)
				{
					crew.LstCrewMembers[j].PF = tmpCrew.LstCrewMembers[j].PF;
				}
			}
		}

		internal PersonnelViewModel FillPersonalCrewScheduleModel(DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd, int id_scheduleType,
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
				cmv.DateStart = crew.DateStart;
				cmv.DateEnd = crew.DateEnd;
			}
			var drAmb = lstDriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
			if (drAmb != null)
			{
				cmv.RegNumber = drAmb.MainAmbulance;

				cmv.WorkTime = drAmb.DayHours + "; " + drAmb.NightHours;
			}
			else
			{
				cmv.WorkTime = ass.WorkZoneDay + "; " + ass.WorkZoneNight;
			}
			cmv.BaseDepartment = ass.Level1;
			cmv.id_positionType = ass.id_positionType;
			cmv.Name = ass.Name;
			cmv.Position = ass.Position;
			cmv.ShortPosition = ass.ShortPosition;
			cmv.RowPosition = 1;
			cmv.RealDate = dateCurrent;
			cmv.id_contract = (int)ass.id_contract;
			cmv.id_assignment = (int)ass.id_assignment;
			if (ass.IsSumWorkTime == null)
			{
				ass.IsSumWorkTime = true;
			}
			DateTime cDate = new DateTime(dateBegin.Year, dateBegin.Month, 1);
			DateTime eDate = new DateTime(dateEnd.Year, dateEnd.Month, 1);
			DateTime mDate = new DateTime(dateCurrent.Year, dateCurrent.Month, 1);

			for (int i = 0; cDate <= eDate; cDate = cDate.AddMonths(1), i++)
			{
				var ccr = new CalendarRow(cDate);
				ccr = this.FillCalendarRow(cDate);

				var end = new DateTime(cDate.Year, cDate.Month, DateTime.DaysInMonth(cDate.Year, cDate.Month));

				PFRow prevMonth = new PFRow();
				var cAss = this.lstAllAssignments.FirstOrDefault(a => a.id_contract == ass.id_contract && cDate < a.ValidTo && a.AssignedAt < end);
				if (cAss == null)
				{
					continue;
				}
				if (cDate < mDate)
				{
					this.FillPfRow(cDate, (int)ScheduleTypes.DailySchedule, prevMonth, cAss, ccr, true);
				}
				else if (cDate.Month == mDate.Month && cDate.Year == mDate.Year)
				{
					this.FillPfRow(cDate, id_scheduleType, prevMonth, cAss, ccr, true);
				}
				else
				{
					this.FillPfRow(cDate, (int)ScheduleTypes.ForecastMonthSchedule, prevMonth, cAss, ccr);
				}

				switch (i)
				{
					case 0:
						cmv.Month1Difference = prevMonth.Difference;
						cmv.Month1OverTime = prevMonth.WorkTimeAbsences;
						break;
					case 1:
						cmv.Month2Difference = prevMonth.Difference;
						cmv.Month2OverTime = prevMonth.WorkTimeAbsences;
						break;
					case 2:
						cmv.Month3Difference = prevMonth.Difference;
						cmv.Month3OverTime = prevMonth.WorkTimeAbsences;
						break;
					case 3:
						cmv.Month4Difference = prevMonth.Difference;
						cmv.Month4OverTime = prevMonth.WorkTimeAbsences;
						break;
					case 4:
						cmv.Month5Difference = prevMonth.Difference;
						cmv.Month5OverTime = prevMonth.WorkTimeAbsences;
						break;
					case 5:
						cmv.Month6Difference = prevMonth.Difference;
						cmv.Month6OverTime = prevMonth.WorkTimeAbsences;
						break;
				}
			}

			this.FillPfRow(mDate, id_scheduleType, cmv, ass, this.cRow);

			return ass;
		}

		internal void FillPfRow(DateTime date, int id_scheduleType, PFRow cmv, PersonnelViewModel ass, CalendarRow cr, bool AllowForecast = false)
		{
			DateTime startMonth = new DateTime(date.Year, date.Month, 1);
			cmv.IsSumWorkTime = ass.IsSumWorkTime ?? true;
			cmv.lstShiftTypes = lstShiftTypes;
			//cmv.IsSumWorkTime =
			cmv.PF = this.lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
																	 && p.Date.Month == date.Month
																	 && p.id_contract == ass.id_contract
																	 && p.id_scheduleType == id_scheduleType);
			if (cmv.PF == null)
			{
				if (id_scheduleType == (int)ScheduleTypes.DailySchedule && AllowForecast)
				{
					cmv.PF = this.lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
																	 && p.Date.Month == date.Month
																	 && p.id_contract == ass.id_contract
																	 && p.id_scheduleType == (int)ScheduleTypes.ForecastMonthSchedule);
				}
				if (cmv.PF == null)
				{
					cmv.PF = new GR_PresenceForms();
					cmv.id_contract = (int)ass.id_contract;
				}
			}

			cmv.LstWorktimeAbsences = this.lstWorktimeAbsenceces.Where(a => a.Date.Year == date.Year
																				  && a.Date.Month == date.Month
																				  && a.id_contract == ass.id_contract).ToList();
			cmv.RealDate = date;
			cmv.cRow = cr;
			if (ass.WorkHours == null)
			{
				ass.WorkHours = 0;
			}
			//if assigned or fired here calculate workdays from date or workdays to date. Or both!
			var workDaysNorm = cr.WorkDays;
			if (ass.FirstAssignedAt != null && ass.FirstAssignedAt.Value.Year == date.Year && ass.FirstAssignedAt.Value.Month == date.Month && ass.FiredAt == null)
			{// assigned in the current month
				workDaysNorm = this.CalculateWorkDays((DateTime)ass.FirstAssignedAt, new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)), this.lstCalendarRows);
			}
			else if (ass.FirstAssignedAt != null && (ass.FirstAssignedAt.Value.Year != date.Year || ass.FirstAssignedAt.Value.Month != date.Month) && ass.FiredAt != null && ass.FiredAt.Value.Year == date.Year && ass.FiredAt.Value.Month == date.Month)
			{//fired in the current month
				workDaysNorm = this.CalculateWorkDays(new DateTime(date.Year, date.Month, 1), ass.FiredAt.Value, this.lstCalendarRows);
			}
			else if (ass.FirstAssignedAt != null && ass.FirstAssignedAt.Value.Year == date.Year && ass.FirstAssignedAt.Value.Month == date.Month && ass.FiredAt != null && ass.FiredAt.Value.Year == date.Year && ass.FiredAt.Value.Month == date.Month)
			{//assigned and fired in the same month
				workDaysNorm = this.CalculateWorkDays(ass.FirstAssignedAt.Value, ass.FiredAt.Value, this.lstCalendarRows);
			}

			cmv.Norm = workDaysNorm * (double)ass.WorkHours;
			cmv.WorkHours = (double)ass.WorkHours;

			cmv.CalculateHours();
		}

		public void FitCrewWorktime()
		{
			//var crews = this.GetDepartmentCrewsAndSchedules(0, DateTime.Now, (int) ScheduleTypes.ForecastMonthSchedule);

			//crews = crews.Where(c => c.LstCrewMembers.Count > 0).ToList();

			//foreach (var cr in crews)
			//{

			//}
		}

		public List<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedulesForDate(int id_selectedDepartment, DateTime dateCurrent, int id_scheduleType = 1)
		{
			var lstCrewModel = this.GetDepartmentCrewsAndSchedules(id_selectedDepartment, dateCurrent, dateCurrent, dateCurrent, id_scheduleType);

			//Filter All schedules for the current date only

			var end = DateTime.DaysInMonth(dateCurrent.Year, dateCurrent.Month);

			for (int i = 1; i <= end; i ++)
			{
				if (i == dateCurrent.Day)
				{
					continue;
				}
				foreach (var cr in lstCrewModel)
				{
					if (cr.id_person != 0)
					{
						cr[i] = 0;
					}
				}
			}
			//Filter all empty schedules
			var lstToDel = new List<CrewScheduleListViewModel>();
			foreach (var cr in lstCrewModel)
			{
				if (cr.id_person == 0)
				{
					lstToDel.Add(cr);
					continue;
				}
				if ((int)cr[dateCurrent.Day] == 0)
				{
					lstToDel.Add(cr);
				}
			}

			foreach (var del in lstToDel)
			{
				lstCrewModel.Remove(del);
			}
			//Join and filter duplicates
			var grpPerson = lstCrewModel.GroupBy(a => a.id_person).Where(a => a.Count() > 1);

			foreach (var pg in grpPerson)
			{
				var tmpCr = pg.FirstOrDefault(a => a.IsTemporary == true);
				if (tmpCr == null)
				{
					tmpCr = pg.First();
				}
				var crToDel = new List<CrewScheduleListViewModel>();
				foreach (var c in pg)
				{
					if (c == tmpCr)
					{
						continue;
					}
					crToDel.Add(c);
				}
				foreach (var c in crToDel)
				{
					lstCrewModel.Remove(c);
				}
			}

			return lstCrewModel;
		}

		//public void FinishMonthAllDepartments(DateTime month)
		//{
		//	var lstDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive == true).ToList();
		//	foreach (var dep in lstDepartments)
		//	{
		//		FinishMonthSingleDepartment(month, dep.id_department);
		//	}
		//}

		//public void FinishMonthSingleDepartment(DateTime month, int id_department)
		//{
		//	DateTime start = new DateTime(month.Year, month.Month, month.Day);
		//	DateTime end = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
		//	//get all assignments for current department 
		//	//var lstAssignments = this._databaseContext.HR_Assignments.Where(a => start < a.ValidTo && a.AssignmentDate < end).ToList();
		//	var lstAssignments =
		//		this.lstAllAssignments.Where(a => a.id_department == id_department && start < a.ValidTo && a.AssignedAt < end)
		//			.ToList();
		//	//get all presence forms filled for each assignment having in mind the assignment start and end date
		//	List<CrewScheduleListViewModel> lstCrewModelDraft = new List<CrewScheduleListViewModel>();

		//	this.cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == month.Year && a.date.Month == month.Month);
		//	if (this.cRow == null)
		//	{
		//		ThrowZoraException(ErrorCodes.NoCalendarRowFound);
		//		return;
		//	}

		//	foreach (var ass in lstAssignments)
		//	{	
		//		var cmv = new CrewScheduleListViewModel();
		//		this.FillPersonalCrewScheduleModel(month, (int) ScheduleTypes.PresenceForm, lstAssignments, null, cmv, ass.id_assignment);
		//		var difference = cmv.Difference;
		//		//transfer to the next month
		//		if (difference > 0.2 || difference < -0.2)
		//		{
		//			var wot = new GR_WorkTimeAbsence();

		//			wot.IsPresence = !(difference < 0);
		//			difference = Math.Abs(difference);

		//			wot.Date = month.AddMonths(1);
		//			wot.StartTime = new TimeSpan(0, 0, 0);
		//			wot.EndTime = new TimeSpan(0, 0, 0);
		//			wot.Reasons = string.Format("Прехвърляне на часове от {0}.{1}", month.Month, month.Year);
		//			wot.IsPrevMonthTransfer = true;
		//			wot.WorkHours = difference;
		//			wot.id_contract = cmv.id_contract;
		//			this._databaseContext.GR_WorkTimeAbsence.Add(wot);
		//		}
		//	}

		//	this.Save();

		//}
	}
}