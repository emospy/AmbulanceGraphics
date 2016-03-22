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
	public class CrewSchedulesLogic : SchedulesLogic
	{
		//private List<GR_Crews> lstCrews;
		//private List<GR_PresenceForms> lstPresenceForms;
		//private List<GR_ShiftTypes> lstShiftTypes;
		//private List<PersonnelViewModel> lstAllAssignments;
		//private List<DriverAmbulancesViewModel> lstDriverAmbulances;
		//private List<CalendarRow> lstCalendarRows;
		//public CrewSchedulesLogic()
		//{
		//	var date = DateTime.Now;
		//	DateTime dateStart, dateEnd;

		//	dateStart = new DateTime(date.AddMonths(-6).Year, date.AddMonths(-6).Month, 1);
		//	dateEnd = dateStart.AddYears(1);



		//	this.lstCrews = this._databaseContext.GR_Crews.ToList();

		//	this.lstPresenceForms = this._databaseContext.GR_PresenceForms.Where(p => p.Date.Year >= dateStart.Year
		//																			 && p.Date.Year <= dateEnd.Year
		//																			 ).ToList();

		//	this.lstShiftTypes = this._databaseContext.GR_ShiftTypes.OrderBy(s => s.id_shiftType).ToList();

		//	this.lstAllAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true)
		//		.Select(a => new PersonnelViewModel
		//		{
		//			Name = a.HR_Contracts.UN_Persons.Name,
		//			Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
		//			id_assignment = a.id_assignment,
		//			id_contract = a.id_contract,
		//			id_department = a.HR_StructurePositions.id_department,
		//			WorkHours = a.HR_WorkTime.WorkHours,
		//		}).ToList();

		//	this.lstDriverAmbulances = this._databaseContext.GR_DriverAmbulances.
		//		Select(a => new DriverAmbulancesViewModel()
		//		{
		//			MainAmbulance = a.GR_Ambulances.Name,
		//			WorkTime = a.GR_Ambulances.WorkTime,
		//			id_driverAssignment = a.id_driverAssignment
		//		}
		//		).ToList();

		//	this.lstCalendarRows = new List<CalendarRow>();

		//	var currentDate = dateStart;
		//	using (NomenclaturesLogic logic = new NomenclaturesLogic())
		//	{
		//		while (currentDate <= dateEnd)
		//		{
		//			lstCalendarRows.Add(logic.FillCalendarRow(currentDate));
		//			currentDate = currentDate.AddMonths(1);
		//		}
		//	}
		//}

		//public List<CrewListViewModel> GetDepartmentCrews(int id_selectedDepartment, DateTime? Date = null)
		//{
		//	Stopwatch s17 = new Stopwatch();
		//	Stopwatch s18 = new Stopwatch();
		//	s18.Start();
		//	s17.Start();
		//	var lstCrews = this._databaseContext.GR_Crews.Where(c => c.IsActive == true && c.id_department == id_selectedDepartment)
		//														.OrderBy(c => c.IsTemporary)
		//														.ThenBy(c => c.Name)
		//														.ToList();

		//	s18.Stop();

		//	var lstAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true
		//																			&& a.HR_StructurePositions.id_department == id_selectedDepartment)
		//		.Select(a => new PersonnelViewModel
		//		{
		//			Name = a.HR_Contracts.UN_Persons.Name,
		//			Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
		//			id_assignment = a.id_assignment,
		//		})
		//		.ToList();
		//	s17.Stop();

		//	List<CrewListViewModel> lstCrewModel = new List<CrewListViewModel>();
		//	foreach (var crew in lstCrews)
		//	{
		//		var cmv = new CrewListViewModel();

		//		if (crew.id_assignment1 != null)
		//		{
		//			var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment1);
		//			if (ass == null)
		//			{
		//				continue;
		//			}
					
		//			cmv.CrewName = crew.Name;
		//			cmv.CrewType = crew.NM_CrewTypes.Name;
		//			cmv.id_crew = crew.id_crew;
		//			cmv.id_department = crew.id_department;
		//			cmv.IsActive = crew.IsActive;
		//			cmv.Name = ass.Name;
		//			cmv.Position = ass.Position;
		//			cmv.IsTemporary = crew.IsTemporary;
		//			if (crew.Date.HasValue)
		//			{
		//				cmv.CrewDate = crew.Date.Value.ToShortDateString();
		//			}

		//			var drAmb = this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
		//			if (drAmb != null)
		//			{
		//				cmv.RegNumber = drAmb.GR_Ambulances.Name;
		//				cmv.WorkTime = drAmb.GR_Ambulances.WorkTime;
		//			}
		//		}
		//		else
		//		{
		//			cmv.CrewName = crew.Name;
		//			cmv.CrewType = crew.NM_CrewTypes.Name;
		//			cmv.id_crew = crew.id_crew;
		//			cmv.id_department = crew.id_department;
		//			cmv.IsActive = crew.IsActive;
		//		}

		//		if (crew.id_assignment2 != null)
		//		{
		//			var cp = new CrewListViewModel();

		//			cp.id_crew = crew.id_crew;
		//			cp.id_department = crew.id_department;
		//			cp.IsActive = crew.IsActive;

		//			var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment2);

		//			cp.Name = ass.Name;
		//			cp.Position = ass.Position;

		//			cp.WorkTime = cmv.WorkTime;

		//			cp.IsTemporary = crew.IsTemporary;
		//			if (crew.Date.HasValue)
		//			{
		//				cp.CrewDate = crew.Date.Value.ToShortDateString();
		//			}

		//			lstCrewModel.Add(cp);
					
		//		}

		//		if (crew.id_assignment3 != null)
		//		{
		//			var cp = new CrewListViewModel();

		//			cp.id_crew = crew.id_crew;
		//			cp.id_department = crew.id_department;
		//			cp.IsActive = crew.IsActive;

		//			var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment3);

		//			cp.Name = ass.Name;
		//			cp.Position = ass.Position;

		//			cp.WorkTime = cmv.WorkTime;
		//			cp.IsTemporary = crew.IsTemporary;
		//			if (crew.Date.HasValue)
		//			{
		//				cp.CrewDate = crew.Date.Value.ToShortDateString();
		//			}

		//			lstCrewModel.Add(cp);
		//		}

		//		if (crew.id_assignment4 != null)
		//		{
		//			var cp = new CrewListViewModel();

		//			cp.id_crew = crew.id_crew;
		//			cp.id_department = crew.id_department;
		//			cp.IsActive = crew.IsActive;

		//			var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment4);

		//			cp.Name = ass.Name;
		//			cp.Position = ass.Position;

		//			cp.WorkTime = cmv.WorkTime;
		//			cp.IsTemporary = crew.IsTemporary;
		//			if (crew.Date.HasValue)
		//			{
		//				cp.CrewDate = crew.Date.Value.ToShortDateString();
		//			}

		//			lstCrewModel.Add(cp);
		//		}

		//		lstCrewModel.Add(cmv);
		//	}
		//	return lstCrewModel;
		//}

		//public ObservableCollection<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime date, int id_scheduleType = 1)
		//{
		//	CalendarRow cRow;

		//	cRow = lstCalendarRows.FirstOrDefault(a => a.date.Year == date.Year && a.date.Month == date.Month);
		//	if (cRow == null)
		//	{
		//		return null;
		//	}

		//	var lstAssignments = lstAllAssignments.Where(a => a.id_department == id_selectedDepartment)
		//		.Select(a => new PersonnelViewModel
		//		{
		//			Name = a.Name,
		//			Position = a.Position,
		//			id_assignment = a.id_assignment,
		//			id_contract = a.id_contract,
		//			id_department = a.id_department,
		//			WorkHours = a.WorkHours,
		//		})
		//		.ToList();

		//	var lstDepartmentCrews = this.lstCrews.Where(c => c.id_department == id_selectedDepartment).ToList();

		//	ObservableCollection<CrewScheduleListViewModel> lstCrewModel = new ObservableCollection<CrewScheduleListViewModel>();
		//	foreach (var crew in lstDepartmentCrews)
		//	{
		//		var cmv = new CrewScheduleListViewModel {lstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>()};

		//		if (crew.id_assignment1 != null)
		//		{
		//			var ass = this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cmv, cRow, crew.id_assignment1);
		//			var drAmb = lstDriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == ass.id_assignment);
		//			if (drAmb != null)
		//			{
		//				cmv.RegNumber = drAmb.Name;
		//				cmv.WorkTime = drAmb.WorkTime;
		//			}
		//		}
		//		else
		//		{
		//			cmv.CrewName = crew.Name;
		//			cmv.CrewType = crew.NM_CrewTypes.Name;
		//			cmv.id_crew = crew.id_crew;
		//			cmv.id_department = crew.id_department;
		//			cmv.IsActive = crew.IsActive;
		//			cmv.lstShiftTypes = lstShiftTypes;
		//			cmv.RealDate = date;
		//		}

		//		if (crew.id_assignment2 != null)
		//		{
		//			var cp = new CrewScheduleListViewModel();
		//			this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment2);
		//			cp.WorkTime = cmv.WorkTime;
		//			cmv.lstCrewMembers.Add(cp);
		//		}

		//		if (crew.id_assignment3 != null)
		//		{
		//			var cp = new CrewScheduleListViewModel();
		//			this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment3);
		//			cp.WorkTime = cmv.WorkTime;
		//			cmv.lstCrewMembers.Add(cp);
		//		}

		//		if (crew.id_assignment4 != null)
		//		{
		//			var cp = new CrewScheduleListViewModel();
		//			this.FillPersonalCrewScheduleModel(date, id_scheduleType, lstAssignments, crew, cp, cRow, crew.id_assignment4);
		//			cp.WorkTime = cmv.WorkTime;
		//			cmv.lstCrewMembers.Add(cp);
		//		}
		//		lstCrewModel.Add(cmv);
		//	}
		//	foreach (var ass in lstAssignments)
		//	{
		//		var cmv = new CrewScheduleListViewModel();
		//		cmv.lstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>();

		//		cmv.CrewName = "";
		//		cmv.CrewType = "";
		//		cmv.id_crew = 0;
		//		cmv.id_department = id_selectedDepartment;
		//		cmv.IsActive = true;
		//		cmv.Name = ass.Name;
		//		cmv.Position = ass.Position;
		//		cmv.lstShiftTypes = lstShiftTypes;

		//		cmv.PF = lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
		//																			&& p.Date.Month == date.Month
		//																			&& p.id_contract == ass.id_contract
		//																			&& p.id_scheduleType == id_scheduleType);
		//		cmv.RealDate = date;
		//		cmv.CalculateHours();

		//		cmv.RegNumber = "стационар";
		//		lstCrewModel.Add(cmv);
		//	}
		//	return lstCrewModel;
		//}

		//private PersonnelViewModel FillPersonalCrewScheduleModel(DateTime date, int id_scheduleType, List<PersonnelViewModel> lstAssignments, GR_Crews crew, CrewScheduleListViewModel cmv, CalendarRow cRow, int? id_assignment)
		//{
		//	var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
		//	if (ass == null)
		//	{
		//		ass = lstAllAssignments.FirstOrDefault(a => a.id_assignment == id_assignment);
		//	}
		//	if (ass == null)
		//	{
		//		return null;
		//	}
		//	cmv.CrewName = crew.Name;
		//	cmv.CrewType = crew.NM_CrewTypes.Name;
		//	cmv.id_crew = crew.id_crew;
		//	cmv.id_department = crew.id_department;
		//	cmv.IsActive = crew.IsActive;
		//	cmv.Name = ass.Name;
		//	cmv.Position = ass.Position;
		//	cmv.lstShiftTypes = lstShiftTypes;
		//	cmv.RowPosition = 1;
		//	cmv.RealDate = date;
		//	cmv.IsTemporary = crew.IsTemporary;
		//	if (crew.Date.HasValue)
		//	{
		//		cmv.CrewDate = crew.Date.Value.ToShortDateString();
		//	}
			
		//	cmv.PF = lstPresenceForms.FirstOrDefault(p => p.Date.Year == date.Year
		//                                              && p.Date.Month == date.Month
		//	                                              && p.id_contract == ass.id_contract
		//	                                              && p.id_scheduleType == id_scheduleType);
		//	if (ass.WorkHours == null)
		//	{
		//		ass.WorkHours = 0;
		//	}
		//	cmv.Norm = cRow.WorkDays*(double) ass.WorkHours;
		//	cmv.CalculateHours();
		//	lstAssignments.Remove(ass);
		//	return ass;
		//}
	}
}
