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
					AssignedAt =  a.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false).AssignmentDate,
					ValidTo = (DateTime)a.ValidTo,
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

		public List<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime date, int id_scheduleType = 1)
		{
			if (id_selectedDepartment == 0)
			{
				return null;
			}
			DateTime startMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
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

			List<PersonnelViewModel> lstAssignments;

			lstAssignments = lstAllAssignments.Where(a => a.id_department == id_selectedDepartment )
				.ToList();
			lstAssignments = lstAssignments.Where(a => startMonth < a.ValidTo && a.AssignedAt < endMonth).ToList();
			//bool overlap = a.start < b.end && b.start < a.end;
			var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment
																&& c.DateStart.Month == date.Month && c.DateStart.Year == date.Year).OrderBy(c => c.Name).ToList();

			List<CrewScheduleListViewModel> lstCrewModelDraft = new List<CrewScheduleListViewModel>();
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
				lstCrewModelDraft.Add(cmv);

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
					//lstCrewModel.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
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
					//lstCrewModel.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
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
					//lstCrewModel.Add(cp);
				}
				else
				{
					cmv.LstCrewMembers.Add(new CrewScheduleListViewModel());
				}
			}

			AddFreeAssignmentsToListCrewModel(id_selectedDepartment, date, id_scheduleType, lstAssignments, lstCrewModelDraft);

			var lstExternal = FindPeopleFromOtherShifts(date, lstCrewModelDraft, id_selectedDepartment, id_scheduleType);
			if (lstExternal != null)
			{
				lstCrewModelDraft.AddRange(lstExternal);
			}
			FilterTemporaryCrewsPresences(date, lstCrewModelDraft);

			var lstCrewModel = FormDisplayListCrewModel(lstCrewModelDraft);

			var lstIncompleteMembers = this.CreateIncompleteMembersList(lstCrewModelDraft);
			if (lstIncompleteMembers != null)
			{
				lstCrewModel.AddRange(lstIncompleteMembers);
			}

			//lstCrewModel = lstCrewModel.OrderBy(a => a.CrewName).ToList();
			

			return lstCrewModel;
		}

		internal int CalculateLeadDepartment(DateTime date, int NumberShifts, bool isDayShift)
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
			int ix = allDeps.FindIndex(d => d.id_department == id_department);

			CalendarRow dRow = new CalendarRow(month);
			CalendarRow nRow = new CalendarRow(month);
			this.DepartmentHasShift(month, ss, ref dRow, ix, dep.UN_Departments2.NumberShifts, true);
			this.DepartmentHasShift(month, ss, ref nRow, ix, dep.UN_Departments2.NumberShifts, false);

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

				this.AddFreeAssignmentsToListCrewModel(id_department, month, id_scheduleType, lstAssignments, lstTmp);
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
				for (int i = 1; i < 32; i ++)
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
					switch ((CrewTypes) crew.id_crewType)
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
					var c1 = lstExternal.FirstOrDefault(a => a.id_assignment == crew.id_assignment2);
					if (c1 != null)
					{
						lstToDel.Add(c1);
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

		private void RemoveNonShiftExternalAssignments(CalendarRow dRow, CalendarRow nRow, List<CrewScheduleListViewModel> lstExternal)
		{
			List<CrewScheduleListViewModel> toDel = new List<CrewScheduleListViewModel>();
			foreach (var per in lstExternal)
			{
				bool remFlag = true;
				for (int i = 1; i < 32; i++)
				{
					if (dRow[i])
					{
						if (per[i] == (int) PresenceTypes.RegularShift
						    || per[i] == (int) PresenceTypes.DayShift)
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
						if (per[i] == (int) PresenceTypes.NightShift)
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

		private void DepartmentHasShift(DateTime month, int ss, ref CalendarRow srRow, int ix, int ns, bool IsDayShift)
		{
			for (int i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
			{
				var date = new DateTime(month.Year, month.Month, i);
				srRow[i] = false;
				if (this.CalculateLeadDepartment(date, ns, IsDayShift) == ix)
				{
					srRow[i] = true;
				}
            }
		}

		private List<CrewScheduleListViewModel> CreateIncompleteMembersList(List<CrewScheduleListViewModel> lstCrewModel)
		{
			var lstTemporaryCrews = lstCrewModel.Where(c => c.IsTemporary).ToList();
			var lstStationeryCrews = lstCrewModel.Where(c => c.IsTemporary == false).ToList();

			List <CrewScheduleListViewModel> lstIncomplete = new List<CrewScheduleListViewModel>();

			foreach (var person in lstStationeryCrews)
			{
				for (int i = 1; i <= DateTime.DaysInMonth(person.DateStart.Year, person.DateStart.Month); i ++)
				{
					if (person.id_person == 0)
					{
						continue;
					}
					if (person[i] == (int) PresenceTypes.RegularShift
					    || person[i] == (int) PresenceTypes.DayShift
					    || person[i] == (int) PresenceTypes.NightShift)
					{
						int id_p = person.id_person;
						var lstTmpPersonCrews =
							lstTemporaryCrews.Where(c => c.id_person == id_p 
															&& c.DateStart.Day <= i && c.DateEnd.Day >= i 
															&& c.LstCrewMembers.Count > 0).ToList();

						if (lstTmpPersonCrews.Count > 0)
						{
							bool IsTmpCrewFull = false;
							for (int j = 0; j < lstTmpPersonCrews.Count; j++)
							{
								//If there is at lest one temporary crew that is full it takes advantage
								if(this.IsCrewFull(new DateTime(person.DateStart.Year, person.DateStart.Month, i), lstTmpPersonCrews[j], true)
								|| this.IsCrewFull(new DateTime(person.DateStart.Year, person.DateStart.Month, i), lstTmpPersonCrews[j], false))
								{
									IsTmpCrewFull = true;
									break;
								}
							}
							if (IsTmpCrewFull == false)
							{
								lstTmpPersonCrews = new List<CrewScheduleListViewModel>();
							}
						}
						if (lstTmpPersonCrews.Count == 0)
						{
							int id_crew = person.id_crew;
							var perCrew = lstCrewModel.FirstOrDefault(c => c.id_crew == id_crew && c.LstCrewMembers.Count > 0);
							if (perCrew == null)
							{
								continue;
							}
							if (this.IsCrewFull(new DateTime(person.DateStart.Year, person.DateStart.Month, i), perCrew, true)
							    || this.IsCrewFull(new DateTime(person.DateStart.Year, person.DateStart.Month, i), perCrew, false))
							{
							}
							else
							{
								CrewScheduleListViewModel ip = lstIncomplete.FirstOrDefault(p => p.id_person == id_p);
								if (ip != null)
								{
									ip[i] = person[i];
								}
								else
								{
									CopyCrewScheduleListViewModel(out ip, person, i);
									lstIncomplete.Add(ip);
								}
							}
						}
					}
                }
			}
			
			return lstIncomplete;
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
			ip[i] = person[i];
			ip.lstShiftTypes = person.lstShiftTypes;
		}

		private void AddFreeAssignmentsToListCrewModel(int id_selectedDepartment, DateTime date, int id_scheduleType,
			List<PersonnelViewModel> lstAssignments, List<CrewScheduleListViewModel> lstCrewModel)
		{
			DateTime startMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			lstAssignments = lstAssignments.OrderBy(a => a.Order).ThenBy(a => a.Name).ToList();
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_selectedDepartment);
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new List<CrewScheduleListViewModel>();
				cmv.DateStart = startMonth;
				cmv.DateEnd = endMonth;
				this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, null, cmv, ass.id_assignment);

				cmv.id_department = id_selectedDepartment;
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
						for(int j = 0; j < crew.LstCrewMembers.Count; j ++)
						{
							if (crew.LstCrewMembers[j].id_person != 0)
							{
								tmpCrew.LstCrewMembers[j][i] = crew.LstCrewMembers[j][i];
							}
						}
					}
				}
				crew.PF = tmpCrew.PF;
				for(int j = 0; j < crew.LstCrewMembers.Count; j ++)
				{
					crew.LstCrewMembers[j].PF = tmpCrew.LstCrewMembers[j].PF;
				}
			}
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
			cmv.BaseDepartment = ass.Level1;
			cmv.Name = ass.Name;
			cmv.Position = ass.Position;
			cmv.ShortPosition = ass.ShortPosition;
			cmv.RowPosition = 1;
			cmv.RealDate = date;
			cmv.id_contract = (int)ass.id_contract;
			cmv.id_assignment = (int)ass.id_assignment;
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

		public void FinishMonthSingleDepartment(DateTime month, int id_department)
		{
			DateTime start = new DateTime(month.Year, month.Month, month.Day);
			DateTime end = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
			//get all assignments for current department 
			var lstAssignments = this._databaseContext.HR_Assignments.Where(a => start < a.ValidTo && a.AssignmentDate < end).ToList();
			//get all presence forms filled for each assignment having in mind the assignment start and end date
			List<CrewScheduleListViewModel> lstCrewModelDraft = new List<CrewScheduleListViewModel>();

			//this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, null, cmv, ass.id_assignment);
			//get overtime
			//calculate hours
			//transfer to the next month
		}
	}
}