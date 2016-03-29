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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class CrewSchedulesLogic : SchedulesLogic
	{
		private List<GR_Crews> lstCrews;
		private List<GR_PresenceForms> lstPresenceForms;
		private readonly List<GR_ShiftTypes> lstShiftTypes;
		private readonly List<PersonnelViewModel> lstAllAssignments;
		private readonly List<DriverAmbulancesViewModel> lstDriverAmbulances;
		private readonly List<CalendarRow> lstCalendarRows;

		public CrewSchedulesLogic()
		{
			var date = DateTime.Now;

			var dateStart = new DateTime(date.AddMonths(-6).Year, date.AddMonths(-6).Month, 1);
			var dateEnd = dateStart.AddYears(1);

			this.lstCrews = this._databaseContext.GR_Crews.ToList();

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
					id_positionType = a.HR_StructurePositions.HR_GlobalPositions.id_positionType,
					Order = a.HR_StructurePositions.Order,
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

		public void RefreshCrews()
		{
			this._databaseContext = new AmbulanceEntities();
			this.lstCrews = this._databaseContext.GR_Crews.ToList();
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

		public List<CrewListViewModel> GetDepartmentCrews(int id_selectedDepartment, DateTime? Date = null)
		{
			var lstDepartmentCrews = this.lstCrews.Where(c => c.IsActive == true && c.id_department == id_selectedDepartment)
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
						cmv.WorkTime = drAmb.GR_Ambulances.WorkTime;
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

		private void FillCrewListViewModel(GR_Crews crew, CrewListViewModel cmv, int? id_assignment, int counter)
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
			if (crew.Date.HasValue && crew.IsTemporary == true)
			{
				cmv.CrewDate = crew.Date.Value.ToShortDateString();
			}
		}

		public List<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime date,
			int id_scheduleType = 1)
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
					id_person = a.id_person,
					Name = a.Name,
					Position = a.Position,
					id_assignment = a.id_assignment,
					id_contract = a.id_contract,
					id_department = a.id_department,
					WorkHours = a.WorkHours,
					Order = a.Order,
				})
				.ToList();

			var lstDepartmentCrews =
				this.lstCrews.Where(c => c.id_department == id_selectedDepartment).OrderBy(c => c.Name).ToList();

			List<CrewScheduleListViewModel> lstCrewModel = new List<CrewScheduleListViewModel>();
			foreach (var crew in lstDepartmentCrews)
			{
				var cmv = new CrewScheduleListViewModel();

				if (crew.id_assignment1 != null)
				{
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cmv, cRow,
						crew.id_assignment1);
					lstAssignments.Remove(ass);
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
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment2);
					lstAssignments.Remove(ass);
					cp.WorkTime = cmv.WorkTime;

					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment3 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment3);
					lstAssignments.Remove(ass);
					cp.WorkTime = cmv.WorkTime;

					lstCrewModel.Add(cp);
				}

				if (crew.id_assignment4 != null)
				{
					var cp = new CrewScheduleListViewModel();
					var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment4);
					lstAssignments.Remove(ass);
					cp.WorkTime = cmv.WorkTime;

					lstCrewModel.Add(cp);
				}
			}

			//lstCrewModel = lstCrewModel.OrderBy(a => a.CrewName).ToList();

			lstAssignments = lstAssignments.OrderBy(a => a.Order).ThenBy(a => a.Name).ToList();
			var dep = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_selectedDepartment);
			foreach (var ass in lstAssignments)
			{
				var cmv = new CrewScheduleListViewModel();
				cmv.LstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>();

				this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, null, cmv, cRow, ass.id_assignment);

				cmv.id_department = id_selectedDepartment;
				cmv.IsActive = true;
				cmv.CalculateHours();

				if (dep != null)
				{
					cmv.RegNumber = dep.UN_Departments2.Name;
				}
				lstCrewModel.Add(cmv);
			}
			return lstCrewModel;
		}

		private PersonnelViewModel FillPersonalCrewScheduleModel(DateTime date, int id_scheduleType,
			List<PersonnelViewModel> lstAssignments, GR_Crews crew, CrewScheduleListViewModel cmv, CalendarRow cRow,
			int? id_assignment)
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
			cmv.id_person = ass.id_person;
			if (crew != null)
			{
				cmv.CrewName = crew.Name.ToString();
				cmv.State = crew.Name%2;
				cmv.CrewType = crew.NM_CrewTypes.Name;
				cmv.id_crew = crew.id_crew;
				cmv.id_department = crew.id_department;
				cmv.IsActive = crew.IsActive;
				cmv.IsTemporary = crew.IsTemporary;
				cmv.id_crewType = crew.id_crewType;
				if (crew.Date.HasValue && crew.IsTemporary == true)
				{
					cmv.CrewDate = crew.Date.Value.ToShortDateString();
				}
			}
			cmv.Name = ass.Name;
			cmv.Position = ass.Position;
			cmv.lstShiftTypes = lstShiftTypes;
			cmv.RowPosition = 1;
			cmv.RealDate = date;
			cmv.PF = lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
														  && p.Date.Month == date.Month
														  && p.id_contract == ass.id_contract
														  && p.id_scheduleType == id_scheduleType);

			cmv.cRow = cRow;
			if (ass.WorkHours == null)
			{
				ass.WorkHours = 0;
			}
			cmv.Norm = cRow.WorkDays * (double)ass.WorkHours;
			cmv.WorkHours = (double)ass.WorkHours;
			cmv.WorkTime = ass.WorkHours.ToString();
			cmv.CalculateHours();
			return ass;
		}

		
	}
}