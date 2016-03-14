using BL.DB;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
							  && ass.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int)PositonTypes.Driver

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

								  MainAmbulance = (spa == null)? null : spa.GR_Ambulances.Name,
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
			crew.Name = model.CrewName;			
			crew.IsActive = model.IsActive;
			crew.IsTemporary = model.IsTemporary;
		}

		public void UpdateCrew(CrewViewModel model)
		{
			var crew = this._databaseContext.GR_Crews.Single(c => c.id_crew == model.id_crew);

			this.FillCrewFromModel(crew, model);

			this.RemoveCrewPersonnelFromOtherCrews(model);

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
			if(model.id_assignment1 != null && model.id_assignment1 != 0)
			{
				var lstCrewAssignments = this._databaseContext.GR_Crews.Where(a => (a.id_assignment1 == model.id_assignment1 
																					|| a.id_assignment4 == model.id_assignment1)
																					&& a.id_crew != model.id_crew).ToList();
				foreach(var cmtr in lstCrewAssignments)
				{
					if(cmtr.id_assignment1 == model.id_assignment1)
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
					else if(cmtr.id_assignment2 == model.id_assignment4)
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
			if(crew == null)
			{
				return null;
			}
			else
			{
				CrewViewModel cvm = new CrewViewModel();

				cvm.CrewName = crew.Name;
				
				cvm.id_crew = crew.id_crew;
				cvm.id_department = crew.id_department;
				cvm.id_crewType = crew.id_crewType;

				cvm.id_assignment1 = crew.id_assignment1;
				cvm.id_assignment2 = crew.id_assignment2;
				cvm.id_assignment3 = crew.id_assignment3;
				cvm.id_assignment4 = crew.id_assignment4;
				return cvm;
			}
		}

		public ObservableCollection<CrewListViewModel> GetDepartmentCrews(int id_selectedDepartment, DateTime? Date = null)
		{
			Stopwatch s17 = new Stopwatch();
			Stopwatch s18 = new Stopwatch();
			s18.Start();
			s17.Start();
			var lstCrews = this._databaseContext.GR_Crews.Where(c => c.IsActive == true && c.id_department == id_selectedDepartment)
																.OrderBy(c => c.IsTemporary)
																.ThenBy(c => c.Name)
																.ToList();
			
			s18.Stop();
			
			var lstAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true && a.HR_StructurePositions.id_department == id_selectedDepartment)
				.Select(a => new PersonnelViewModel
				{
					Name = a.HR_Contracts.UN_Persons.Name,
					Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
					id_assignment = a.id_assignment,
				})
				.ToList();
			s17.Stop();

			ObservableCollection<CrewListViewModel> lstCrewModel = new ObservableCollection<CrewListViewModel>();
			foreach (var crew in lstCrews)
			{
				Stopwatch s1 = new Stopwatch();
				Stopwatch s2 = new Stopwatch();
				Stopwatch s3 = new Stopwatch();
				Stopwatch s4 = new Stopwatch();
				Stopwatch s5 = new Stopwatch();
				Stopwatch s6 = new Stopwatch();
				Stopwatch s8 = new Stopwatch();

				s1.Start();
				var cmv = new CrewListViewModel();
				cmv.lstCrewMembers = new ObservableCollection<CrewListViewModel>();
				
				if(crew.id_assignment1 != null)
				{
					s2.Start();
					s8.Start();
					var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment1);
					s8.Stop();
					cmv.CrewName = crew.Name;
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
					cmv.Name = ass.Name;
					cmv.Position = ass.Position;
					cmv.IsTemporary = crew.IsTemporary;

					s3.Start();
					var drAmb = this._databaseContext.GR_DriverAmbulances.SingleOrDefault(a => a.id_driverAssignment == ass.id_assignment);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.GR_Ambulances.Name;
						cmv.WorkTime = drAmb.GR_Ambulances.WorkTime;
					}
					s3.Stop();
					s2.Stop();
				}
				else
				{
					cmv.CrewName = crew.Name;
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
				}

				if(crew.id_assignment2 != null)
				{
					s4.Start();
					var cp = new CrewListViewModel();

					cp.id_crew = crew.id_crew;
					cp.id_department = crew.id_department;
					cp.IsActive = crew.IsActive;

					var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment2);

					cp.Name = ass.Name;
					cp.Position = ass.Position;

					cp.WorkTime = cmv.WorkTime;

					cmv.lstCrewMembers.Add(cp);
					s4.Stop();
				}

				if (crew.id_assignment3 != null)
				{
					s5.Start();
					var cp = new CrewListViewModel();

					cp.id_crew = crew.id_crew;
					cp.id_department = crew.id_department;
					cp.IsActive = crew.IsActive;

					var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment3);

					cp.Name = ass.Name;
					cp.Position = ass.Position;

					cp.WorkTime = cmv.WorkTime;

					cmv.lstCrewMembers.Add(cp);
					s5.Stop();
				}

				if (crew.id_assignment4 != null)
				{
					s6.Start();
					var cp = new CrewListViewModel();

					cp.id_crew = crew.id_crew;
					cp.id_department = crew.id_department;
					cp.IsActive = crew.IsActive;

					var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment4);

					cp.Name = ass.Name;
					cp.Position = ass.Position;

					cp.WorkTime = cmv.WorkTime;

					cmv.lstCrewMembers.Add(cp);
					s6.Stop();
				}

				lstCrewModel.Add(cmv);
				s1.Stop();
			}			
            return lstCrewModel;
		}

		public ObservableCollection<CrewScheduleListViewModel> GetDepartmentCrewsAndSchedules(int id_selectedDepartment, DateTime? date)
		{
			Stopwatch s17 = new Stopwatch();
			Stopwatch s18 = new Stopwatch();
			s18.Start();
			s17.Start();
			DateTime dateStart, dateEnd;
			if(date == null)
			{
				dateStart = DateTime.Now;
				dateEnd = dateStart.AddDays(-1);
			}
			else
			{
				dateStart = new DateTime(date.Value.Year, date.Value.Month, 1);
				dateEnd = dateStart.AddMonths(1);
			}
			var lstCrews = this._databaseContext.GR_Crews.Where(c => c.IsActive == true 
																	&& c.id_department == id_selectedDepartment
																	&& (c.IsTemporary == false || (c.Date >= dateStart && c.Date <dateEnd))).ToList();

			var lstShiftTypes = this._databaseContext.GR_ShiftTypes.OrderBy(s => s.id_shiftType).ToList();
			s18.Stop();

			var lstAssignments = this._databaseContext.HR_Assignments.Where(a => a.IsActive == true && a.HR_StructurePositions.id_department == id_selectedDepartment)
				.Select(a => new PersonnelViewModel
				{
					Name = a.HR_Contracts.UN_Persons.Name,
					Position = a.HR_StructurePositions.HR_GlobalPositions.Name,
					id_assignment = a.id_assignment,
				})
				.ToList();
			s17.Stop();

			ObservableCollection<CrewScheduleListViewModel> lstCrewModel = new ObservableCollection<CrewScheduleListViewModel>();
			foreach (var crew in lstCrews)
			{
				Stopwatch s1 = new Stopwatch();
				Stopwatch s2 = new Stopwatch();
				Stopwatch s3 = new Stopwatch();
				Stopwatch s4 = new Stopwatch();
				Stopwatch s5 = new Stopwatch();
				Stopwatch s6 = new Stopwatch();
				Stopwatch s8 = new Stopwatch();

				s1.Start();
				var cmv = new CrewScheduleListViewModel();
				cmv.lstCrewMembers = new ObservableCollection<CrewScheduleListViewModel>();

				if (crew.id_assignment1 != null)
				{
					s2.Start();
					s8.Start();
					var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment1);
					s8.Stop();
					cmv.CrewName = crew.Name;
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
					cmv.Name = ass.Name;
					cmv.Position = ass.Position;
					cmv.lstShiftTypes = lstShiftTypes;

					s3.Start();
					var drAmb = this._databaseContext.GR_DriverAmbulances.SingleOrDefault(a => a.id_driverAssignment == ass.id_assignment);
					if (drAmb != null)
					{
						cmv.RegNumber = drAmb.GR_Ambulances.Name;
						cmv.WorkTime = drAmb.GR_Ambulances.WorkTime;
					}

					cmv.PF = this._databaseContext.GR_PresenceForms.SingleOrDefault(p => p.Date.Year == dateStart.Year && p.Date.Month == dateStart.Month && p.id_contract == ass.id_contract);
					if (date != null)
					{
						cmv.RealDate = date.Value;
					}
					else
					{
						cmv.RealDate = DateTime.Now;
					}
					s3.Stop();
					s2.Stop();
				}
				else
				{
					cmv.CrewName = crew.Name;
					cmv.CrewType = crew.NM_CrewTypes.Name;
					cmv.id_crew = crew.id_crew;
					cmv.id_department = crew.id_department;
					cmv.IsActive = crew.IsActive;
					cmv.lstShiftTypes = lstShiftTypes;
					if (date != null)
					{
						cmv.RealDate = date.Value;
					}
					else
					{
						cmv.RealDate = DateTime.Now;
					}
				}

				//if (crew.id_assignment2 != null)
				//{
				//	s4.Start();
				//	var cp = new CrewScheduleListViewModel();

				//	cp.id_crew = crew.id_crew;
				//	cp.id_department = crew.id_department;
				//	cp.IsActive = crew.IsActive;

				//	var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment2);

				//	cp.Name = ass.Name;
				//	cp.Position = ass.Position;

				//	cp.WorkTime = cmv.WorkTime;

				//	cmv.lstCrewMembers.Add(cp);
				//	s4.Stop();
				//}

				//if (crew.id_assignment3 != null)
				//{
				//	s5.Start();
				//	var cp = new CrewScheduleListViewModel();

				//	cp.id_crew = crew.id_crew;
				//	cp.id_department = crew.id_department;
				//	cp.IsActive = crew.IsActive;

				//	var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment3);

				//	cp.Name = ass.Name;
				//	cp.Position = ass.Position;

				//	cp.WorkTime = cmv.WorkTime;

				//	cmv.lstCrewMembers.Add(cp);
				//	s5.Stop();
				//}

				//if (crew.id_assignment4 != null)
				//{
				//	s6.Start();
				//	var cp = new CrewScheduleListViewModel();

				//	cp.id_crew = crew.id_crew;
				//	cp.id_department = crew.id_department;
				//	cp.IsActive = crew.IsActive;

				//	var ass = lstAssignments.FirstOrDefault(a => a.id_assignment == crew.id_assignment4);

				//	cp.Name = ass.Name;
				//	cp.Position = ass.Position;

				//	cp.WorkTime = cmv.WorkTime;

				//	cmv.lstCrewMembers.Add(cp);
				//	s6.Stop();
				//}

				lstCrewModel.Add(cmv);
				s1.Stop();
			}

			return lstCrewModel;
		}

		public PFRow GetPersonalSchedule(int id_person, DateTime month)
		{
			var ass = this._databaseContext.HR_Assignments.FirstOrDefault(c => c.IsActive == true
																		&& c.HR_Contracts.id_person == id_person);

			PFRow PF = new PFRow();

			PF.PF = this._databaseContext.GR_PresenceForms.SingleOrDefault(p => p.Date.Year == month.Year && p.Date.Month == month.Month && p.id_contract == ass.id_contract);

			return PF;
		}

		public GR_DriverAmbulances GetActiveDriverAmbulance(int id_driverAssignment)
		{
			var result = this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == id_driverAssignment && a.IsActive == true);
			return result;
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
