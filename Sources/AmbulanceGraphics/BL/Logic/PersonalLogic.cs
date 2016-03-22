﻿using BL.DB;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class PersonalLogic : BaseLogic
	{
		//internal readonly AmbulanceEntities _databaseContext;

		public PersonalLogic()
		{
			//_databaseContext = new AmbulanceEntities();
			//_databaseContext.Database.Connection.ConnectionString = cs;
		}

		#region Service Methods
		public List<PersonnelViewModel> GetPersonnel(bool IsFired, int id_department = 0, int id_positionType = 0)
		{
			List<PersonnelViewModel> lstPersons = new List<PersonnelViewModel>();
			if (IsFired == true)
			{
				//var query = from person in people
				//			join pet in pets on person equals pet.Owner into gj
				//			from subpet in gj.DefaultIfEmpty()
				//			select new { person.FirstName, PetName = (subpet == null ? String.Empty : subpet.Name) };			

				lstPersons = (from con in this._databaseContext.HR_Contracts
							  join ass in this._databaseContext.HR_Assignments on con.id_contract equals ass.id_contract into ac
							  from acc in ac.DefaultIfEmpty()
							  join per in this._databaseContext.UN_Persons on con.id_person equals per.id_person into pa
							  from spa in pa.DefaultIfEmpty()

							  where con.IsFired == true
							  && acc.IsActive == true

							  select new PersonnelViewModel
							  {
								  id_person = spa.id_person,
								  Name = spa.Name,
								  id_level1 = (acc == null) ? (int?)null :
								  (acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 2) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 1) ? acc.HR_StructurePositions.id_department : (int?)null,

								  id_level2 = (acc == null) ? (int?)null :
									 (acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 2) ? acc.HR_StructurePositions.id_department : (int?)null,

								  id_level3 = (acc == null) ? (int?)null :
									(acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
                                    (acc.HR_StructurePositions.UN_Departments.Level == 3) ? acc.HR_StructurePositions.id_department : (int?)null,

								  id_level4 = (acc == null) ? (int?)null :
									 (acc.HR_StructurePositions.UN_Departments.Level == 4) ? acc.HR_StructurePositions.id_department : (int?)null,

								  Level1 = (acc.HR_StructurePositions.UN_Departments.Level == 4) ? (acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.Name) :
											(acc.HR_StructurePositions.UN_Departments.Level == 3) ? (acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
											(acc.HR_StructurePositions.UN_Departments.Level == 2) ? (acc.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
											(acc.HR_StructurePositions.UN_Departments.Level == 1) ? acc.HR_StructurePositions.UN_Departments.Name : null,

								  Level2 = (acc == null) ? null :
									 (acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((acc == null) ? null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((acc == null) ? null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 2) ? acc.HR_StructurePositions.UN_Departments.Name : null,

								  Level3 = (acc == null) ? null :
									(acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((acc == null) ? null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(acc.HR_StructurePositions.UN_Departments.Level == 3) ? acc.HR_StructurePositions.UN_Departments.Name : null,

								  Level4 = (acc == null) ? null :
									 (acc.HR_StructurePositions.UN_Departments.Level == 4) ? acc.HR_StructurePositions.UN_Departments.Name: null,

								  Position = acc.HR_StructurePositions.HR_GlobalPositions.Name,
							  }).ToList();
			}
			else
			{
				var query = (from spa in this._databaseContext.UN_Persons
							 join con in this._databaseContext.HR_Contracts on spa.id_person equals con.id_person into pa
							 from pas in pa.DefaultIfEmpty()
							 join ass in this._databaseContext.HR_Assignments on pas.id_contract equals ass.id_contract into ac
							 from acc in ac.DefaultIfEmpty()

							 where pas == null
							 || (pas.IsFired == false && acc.IsActive == true)
							 select new { spa, acc });
				if(id_department != 0)
				{
					query = query.Where(s => s.acc.HR_StructurePositions.id_department == id_department);
				}
				if(id_positionType != 0)
				{
					query = query.Where(s => s.acc.HR_StructurePositions.HR_GlobalPositions.id_positionType == id_positionType);
				}
				lstPersons = query.Select(s => new PersonnelViewModel
				{
					id_person = s.spa.id_person,
					Name = s.spa.Name,
					id_contract = (s.acc == null)?(int?) null: s.acc.id_contract,
					id_assignment = (s.acc == null) ? (int?)null : s.acc.id_assignment,
					id_level1 = (s.acc == null) ? (int?)null :
								  (s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((s.acc == null) ? (int?)null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.id_department) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((s.acc == null) ? (int?)null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_department) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 2) ? ((s.acc == null) ? (int?)null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 1) ? s.acc.HR_StructurePositions.id_department : (int?)null,

					id_level2 = (s.acc == null) ? (int?)null :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((s.acc == null) ? (int?)null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_department) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((s.acc == null) ? (int?)null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 2) ? s.acc.HR_StructurePositions.id_department : (int?)null,

					id_level3 = (s.acc == null) ? (int?)null :
									(s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((s.acc == null) ? (int?)null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
									(s.acc.HR_StructurePositions.UN_Departments.Level == 3) ? s.acc.HR_StructurePositions.id_department : (int?)null,

					id_level4 = (s.acc == null) ? (int?)null :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? s.acc.HR_StructurePositions.id_department : (int?)null,

					Level1 = (s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? (s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.Name) :
											(s.acc.HR_StructurePositions.UN_Departments.Level == 3) ? (s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
											(s.acc.HR_StructurePositions.UN_Departments.Level == 2) ? (s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
											(s.acc.HR_StructurePositions.UN_Departments.Level == 1) ? s.acc.HR_StructurePositions.UN_Departments.Name : null,

					Level2 = (s.acc == null) ? null :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((s.acc == null) ? null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((s.acc == null) ? null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 2) ? s.acc.HR_StructurePositions.UN_Departments.Name : null,

					Level3 = (s.acc == null) ? null :
									(s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? ((s.acc == null) ? null : s.acc.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(s.acc.HR_StructurePositions.UN_Departments.Level == 3) ? s.acc.HR_StructurePositions.UN_Departments.Name : null,

					Level4 = (s.acc == null) ? null :
									 (s.acc.HR_StructurePositions.UN_Departments.Level == 4) ? s.acc.HR_StructurePositions.UN_Departments.Name : null,

					Position = (s.acc == null) ? null : 
									s.acc.HR_StructurePositions.HR_GlobalPositions.Name,
				}).ToList();
			}
			return lstPersons;
		}

		public List<PersonnelViewModel> GetPersonnelForParent(int id_departmentParent, int id_positionType = 0)
		{
			List<PersonnelViewModel> lstPersons = new List<PersonnelViewModel>();
			
				var query = (from spa in this._databaseContext.UN_Persons
							 join con in this._databaseContext.HR_Contracts on spa.id_person equals con.id_person into pa
							 from pas in pa.DefaultIfEmpty()
							 join ass in this._databaseContext.HR_Assignments on pas.id_contract equals ass.id_contract into ac
							 from acc in ac.DefaultIfEmpty()

							 where pas == null
							 || (pas.IsFired == false && acc.IsActive == true)
							 select new { spa, acc });
				if (id_departmentParent != 0)
				{
					query = query.Where(s => s.acc.HR_StructurePositions.UN_Departments.id_departmentParent == id_departmentParent);
				}
				if (id_positionType != 0)
				{
					query = query.Where(s => s.acc.HR_StructurePositions.HR_GlobalPositions.id_positionType == id_positionType);
				}
				lstPersons = query.Select(s => new PersonnelViewModel
				{
					id_person = s.spa.id_person,
					Name = s.spa.Name,
					id_contract = (s.acc == null) ? (int?)null : s.acc.id_contract,
					id_assignment = (s.acc == null) ? (int?)null : s.acc.id_assignment,
					
					Position = (s.acc == null) ? null :
									s.acc.HR_StructurePositions.HR_GlobalPositions.Name,
				}).ToList();
			
			return lstPersons;
		}

		public HR_Absence GetAbsenceData(int id_absence)
		{
			return this._databaseContext.HR_Absence.SingleOrDefault(a => a.id_absence == id_absence);
		}

		public void HandleAbsenceSave(HR_Absence absence)
		{
			if (absence.id_absence == 0)
			{
				this._databaseContext.HR_Absence.Add(absence);
			}
			else
			{
				this.HR_Absence.Update(absence);
			}

			this.Save();
		}

		public void ToUpperPersons()
		{
			var lstPersons = this._databaseContext.UN_Persons.ToList();
			foreach(var person in lstPersons)
			{
				person.Name = person.Name.ToUpper();
			}
			this.Save();
		}
		public void HandleAssignmentSave(AssignmentViewModel model)
		{
			if (model.id_contract == 0)
			{
				this.AddContract(model);
			}
			else if (model.id_assignment == 0)
			{
				//handle add new assignment
				this.AddAssignment(model);
			}
			else if (model.IsAdditionalAssignment == false)
			{
				this.EditContract(model);
			}
			else
			{
				this.EditAssignment(model);
			}

			this.Save();
		}

		private void EditAssignment(AssignmentViewModel model)
		{
			var ass = this._databaseContext.HR_Assignments.Single(c => c.id_assignment == model.id_assignment);

			FillAssignmentFromModel(model, ass);
		}

		private static void FillAssignmentFromModel(AssignmentViewModel model, HR_Assignments ass)
		{
			ass.AdditionalHolidays = model.AdditionalHolidays;
			ass.AssignmentDate = model.AssignmentDate;
			ass.BaseSalary = model.BaseSalary;
			ass.ContractDate = model.ContractDate;
			ass.ContractNumber = model.ContractNumber;
			ass.EndContractDate = model.EndContractDate;
			ass.id_contractType = model.id_contractType;
			ass.id_lawType = model.id_lawType;
			ass.id_structurePosition = model.id_structurePosition;
			ass.id_workTime = model.id_workTime;
			ass.NumberHolidays = model.NumHolidays;
			ass.TestContractDate = model.TestContractDate;
			ass.SchedulesCode = model.SchedulesCode;
			ass.id_contract = model.id_contract;
		}

		private void EditContract(AssignmentViewModel model)
		{
			var con = this._databaseContext.HR_Contracts.Single(c => c.id_contract == model.id_contract);
			var ass = this._databaseContext.HR_Assignments.Single(c => c.id_assignment == model.id_assignment);

			con.ContractDate = model.ContractDate;
			//con.ContractID = model.co
			con.ContractNumber = model.ContractNumber;
			con.id_person = model.id_person;
			con.IsFired = false;
			con.TRZCode = model.TRZCode;

			FillAssignmentFromModel(model, ass);
			ass.IsAdditionalAssignment = false;
		}

		private void AddAssignment(AssignmentViewModel model)
		{
			var ass = new HR_Assignments();
			
			var PrevAssignment = this._databaseContext.HR_Assignments.Single(a => a.id_contract == model.id_contract && a.IsActive == true);
			PrevAssignment.IsActive = false;

			FillAssignmentFromModel(model, ass);
			ass.IsActive = true;
			ass.IsAdditionalAssignment = true;

			this._databaseContext.HR_Assignments.Add(ass);

			this.Save();

			if (PrevAssignment.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int)PositionTypes.Driver)
			{
				var newPosition = this._databaseContext.HR_StructurePositions.Single(s => s.id_structurePosition == model.id_structurePosition);

				if (newPosition.HR_GlobalPositions.id_positionType == (int)PositionTypes.Driver)
				{
					var driverAmbulance = this._databaseContext.GR_DriverAmbulances.FirstOrDefault(a => a.id_driverAssignment == PrevAssignment.id_assignment);
					if (driverAmbulance != null)
					{
						driverAmbulance.HR_Assignments = ass;
					}
				}
			}
			model.id_assignment = ass.id_assignment;
		}

		private void AddContract(AssignmentViewModel model)
		{
			var con = new HR_Contracts();
			var ass = new HR_Assignments();
			var yh = new HR_YearHolidays();

			con.ContractDate = model.ContractDate;
			con.ContractNumber = model.ContractNumber;
			con.id_person = model.id_person;
			con.IsFired = false;
			con.TRZCode = model.TRZCode;

			FillAssignmentFromModel(model, ass);
			ass.IsActive = true;
			ass.IsAdditionalAssignment = false;
			ass.HR_Contracts = con;

			yh.HR_Contracts = con;
			yh.IsActive = true;
			if(ass.NumberHolidays == null)
			{
				ass.NumberHolidays = 0;
			}
			if(ass.AdditionalHolidays == null)
			{
				ass.AdditionalHolidays = 0;
			}
			yh.Leftover = (int)ass.NumberHolidays + (int)ass.AdditionalHolidays; //To do To bo changed and recalculated
			yh.Name = ass.AssignmentDate.Value.Year;
			yh.Total = (int)ass.NumberHolidays + (int)ass.AdditionalHolidays;

			this._databaseContext.HR_YearHolidays.Add(yh);
			this._databaseContext.HR_Contracts.Add(con);
			this._databaseContext.HR_Assignments.Add(ass);

			this.Save();

			model.id_contract = con.id_contract;
			model.id_assignment = ass.id_assignment;
		}

		public void AddPerson(PersonViewModel personViewModel)
		{
			UN_Persons per = new UN_Persons();

			per.Name = personViewModel.Name;
			per.Address = personViewModel.Address;
			per.EGN = personViewModel.EGN;
			per.GSM = personViewModel.GSM;

			this._databaseContext.UN_Persons.Add(per);
			this.Save();
			personViewModel.id_person = per.id_person;
		}

		public void UpdatePerson(PersonViewModel personViewModel)
		{
			UN_Persons per = this._databaseContext.UN_Persons.Single(a => a.id_person == personViewModel.id_person);

			per.Name = personViewModel.Name;
			per.Address = personViewModel.Address;
			per.EGN = personViewModel.EGN;
			per.GSM = personViewModel.GSM;
			
			this.Save();
		}

		public GenericPersonViewModel InitGPVM(int id_person)
		{
			GenericPersonViewModel vm = new GenericPersonViewModel();

			this.InitPersonViewModel(id_person, vm);

			this.InitContractsViewModel(id_person, vm);

			this.InitAbsencesViewModel(id_person, vm);

			//this.InitYearHolidaysViewModel(id_person, vm);

			//this.InitScheduleViewModel(id_person, vm);

			

			return vm;
		}

		public void InitScheduleViewModel(int id_person, DateTime selectedMonth, GenericPersonViewModel vm)
		{
			int id_contract = 0;
			if (vm.lstContracts.Where(b => b.IsFired == false).ToList().Count > 0)
			{
				id_contract = vm.lstContracts.FirstOrDefault(b => b.IsFired == false).id_contract;
			}
			else
			{
				return;
			}
			vm.PresenceForm = this._databaseContext.GR_PresenceForms.SingleOrDefault(a => a.id_contract == id_contract
																				&& a.Date.Month == selectedMonth.Month
																				&& a.Date.Year == selectedMonth.Year
																				&& a.id_scheduleType == (int)ScheduleTypes.PresenceForm);

			vm.DailySchedule = this._databaseContext.GR_PresenceForms.SingleOrDefault(a => a.id_contract == id_contract
																				&& a.Date.Month == selectedMonth.Month
																				&& a.Date.Year == selectedMonth.Year
																				&& a.id_scheduleType == (int)ScheduleTypes.DailySchedule);
		}

		private void InitAbsencesViewModel(int id_person, GenericPersonViewModel vm)
		{
			int id_contract = 0;
			if (vm.lstContracts.Where(b => b.IsFired == false).ToList().Count > 0)
			{
				id_contract = vm.lstContracts.FirstOrDefault(b => b.IsFired == false).id_contract;
            }
			else
			{
				return;
			}
			vm.lstAbsences = this._databaseContext.HR_Absence.Where(a => a.id_contract == id_contract)
									.Select(a => new AbsenceListViewModel
									{
										AbsenceType = a.NM_AbsenceTypes.Name,
										CalendarDays = a.CalendarDays,
										EndDate = a.EndDate,
										id_absence = a.id_absence,
										id_absenceType = a.id_absenceType,
										id_contract = a.id_contract,
										id_userLogin = a.id_userLogin,
										OrderDate = a.OrderDate,
										OrderNumber = a.OrderNumber,
										Reason = a.Reason,
										SicknessNapDocs = a.SicknessNapDocs,
										SicknessAdditionalDocs = a.SicknessAdditionalDocs,
										SicknessAttachment7 = a.SicknessAttachment7,
										SicknessIssueDate = a.SicknessIssueDate,
										SicknessMKB = a.SicknessMKB,
										SicknessNumber = a.SicknessNumber,
										SicknessReason = a.SicknessReason,
										SicknessDeclaration39 = a.SicknessDeclaration39,
										StartDate = a.StartDate,
										Timestamp = a.Timestamp,
										WorkDays = a.WorkDays,
										id_yearHoliday = a.id_yearHoliday,
										id_sicknessType = a.id_sicknessType,

									}).ToList();
		}

		private void InitContractsViewModel(int id_person, GenericPersonViewModel vm)
		{
			var lstContracts = this._databaseContext.HR_Contracts.Where(c => c.id_person == id_person);

			vm.lstContracts = new ObservableCollection<ContractsViewModel>();

			foreach(var contract in lstContracts)
			{
				var lstAssignments = this._databaseContext.HR_Assignments.Where(a => a.id_contract == contract.id_contract).ToList();
				var baseAssignment = lstAssignments.Single(a => a.IsAdditionalAssignment == false);

				ContractsViewModel cvm = new ContractsViewModel();
				cvm.Status = (contract.IsFired) ? "Прекратен" : "Действащ";
				cvm.ActiveFrom = baseAssignment.AssignmentDate;
				cvm.ContractDate = baseAssignment.ContractDate;
				cvm.ContractNumber = baseAssignment.ContractNumber;
				
				cvm.Level1 =	(baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.Name) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 2) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 1) ? baseAssignment.HR_StructurePositions.UN_Departments.Name : null;

				cvm.Level2 =	(baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 2) ? baseAssignment.HR_StructurePositions.UN_Departments.Name : null;

				cvm.Level3 =	(baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? baseAssignment.HR_StructurePositions.UN_Departments.Name : null;

				cvm.Level4 =	(baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? baseAssignment.HR_StructurePositions.UN_Departments.Name : null;


				cvm.id_level1 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 2) ? (baseAssignment.HR_StructurePositions.UN_Departments.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 1) ? baseAssignment.HR_StructurePositions.id_department : (int?)null;

				cvm.id_level2 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (baseAssignment.HR_StructurePositions.UN_Departments.UN_Departments2.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? (baseAssignment.HR_StructurePositions.UN_Departments.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 2) ? baseAssignment.HR_StructurePositions.id_department : (int?)null;

				cvm.id_level3 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (baseAssignment.HR_StructurePositions.UN_Departments.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? baseAssignment.HR_StructurePositions.id_department : (int?)null;

				cvm.id_level4 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? baseAssignment.HR_StructurePositions.id_department : (int?)null;

				cvm.StructurePosition = baseAssignment.HR_StructurePositions.HR_GlobalPositions.Name;
				cvm.id_contract = contract.id_contract;
				cvm.id_person = contract.id_person;
				cvm.id_assignment = baseAssignment.id_assignment;

				cvm.lstAdditionalAssignments = new ObservableCollection<ContractsViewModel>();

				var lstAdditionalAssignments = lstAssignments.Where(a => a.IsAdditionalAssignment == true).ToList();

				foreach(var ass in lstAdditionalAssignments)
				{
					ContractsViewModel cam = new ContractsViewModel();
					cam.ActiveFrom = ass.AssignmentDate;
					cam.ContractDate = ass.ContractDate;
					cam.ContractNumber = ass.ContractNumber;

					cam.Level1 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 2) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 1) ? ass.HR_StructurePositions.UN_Departments.Name : null;
					cam.Level2 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 2) ? ass.HR_StructurePositions.UN_Departments.Name : null;

					cam.Level3 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.Name) :
									(ass.HR_StructurePositions.UN_Departments.Level == 3) ? ass.HR_StructurePositions.UN_Departments.Name : null;

					cam.Level4 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? ass.HR_StructurePositions.UN_Departments.Name : null;


					cam.id_level1 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 2) ? (ass.HR_StructurePositions.UN_Departments.id_departmentParent) :
								(baseAssignment.HR_StructurePositions.UN_Departments.Level == 1) ? ass.HR_StructurePositions.id_department : (int?)null;

					cam.id_level2 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.id_departmentParent) :
									(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.id_departmentParent) :
									(baseAssignment.HR_StructurePositions.UN_Departments.Level == 2) ? ass.HR_StructurePositions.id_department : (int?)null;

					cam.id_level3 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.id_departmentParent) :
									(baseAssignment.HR_StructurePositions.UN_Departments.Level == 3) ? ass.HR_StructurePositions.id_department : (int?)null;

					cam.id_level4 = (baseAssignment.HR_StructurePositions.UN_Departments.Level == 4) ? ass.HR_StructurePositions.id_department : (int?)null;

					cam.StructurePosition = ass.HR_StructurePositions.HR_GlobalPositions.Name;
					cam.id_contract = cvm.id_contract;
					cam.id_assignment = ass.id_assignment;
					cam.id_person = contract.id_person;

					cvm.lstAdditionalAssignments.Add(cam);
				}

				vm.lstContracts.Add(cvm);
			}
		}

		public AssignmentViewModel InitAssignmentViewModel(int id_assignment)
		{
			var model = new AssignmentViewModel();
			var ass = this.HR_Assignments.GetById(id_assignment);

			model.AssignmentDate = ass.AssignmentDate;
			model.BaseSalary = ass.BaseSalary;
			model.ContractDate = ass.ContractDate;
			model.ContractNumber = ass.ContractNumber;
			model.EndContractDate = ass.EndContractDate;
			model.id_contract = ass.id_contract;
			model.id_contractType = ass.id_contractType;
			model.id_lawType = ass.id_lawType;
			model.id_assignment = ass.id_assignment;

			model.id_level1 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_departmentParent) :
								(ass.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.id_departmentParent) :
								(ass.HR_StructurePositions.UN_Departments.Level == 2) ? (ass.HR_StructurePositions.UN_Departments.id_departmentParent) :
								(ass.HR_StructurePositions.UN_Departments.Level == 1) ? ass.HR_StructurePositions.id_department : (int?)null;

			model.id_level2 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.UN_Departments2.id_departmentParent) :
							(ass.HR_StructurePositions.UN_Departments.Level == 3) ? (ass.HR_StructurePositions.UN_Departments.id_departmentParent) :
							(ass.HR_StructurePositions.UN_Departments.Level == 2) ? ass.HR_StructurePositions.id_department : (int?)null;

			model.id_level3 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? (ass.HR_StructurePositions.UN_Departments.id_departmentParent) :
							(ass.HR_StructurePositions.UN_Departments.Level == 3) ? ass.HR_StructurePositions.id_department : (int?)null;

			model.id_level4 = (ass.HR_StructurePositions.UN_Departments.Level == 4) ? ass.HR_StructurePositions.id_department : (int?)null;

			model.id_person = ass.HR_Contracts.id_person;
			model.id_structurePosition = ass.id_structurePosition;
			model.id_workTime = ass.id_workTime;
			model.IsActive = ass.IsActive;
			model.IsAdditionalAssignment = ass.IsAdditionalAssignment;
			model.AdditionalHolidays = ass.AdditionalHolidays;
			model.NumHolidays = ass.NumberHolidays;
			model.TestContractDate = ass.TestContractDate;
			model.id_assignment = ass.id_assignment;
			model.TRZCode = ass.HR_Contracts.TRZCode;
			model.SchedulesCode = ass.SchedulesCode;

			return model;
		}

		public AssignmentViewModel InitNextAssignmentViewModel(int id_contract)
		{
			var model = new AssignmentViewModel();

			var ass = this._databaseContext.HR_Assignments.Single(a => a.IsActive == true && a.id_contract == id_contract);

			model = this.InitAssignmentViewModel(ass.id_assignment);
			model.id_assignment = 0;

			return model;
		}

		private void InitPersonViewModel(int id_person, GenericPersonViewModel vm)
		{
			vm.PersonViewModel = this._databaseContext.UN_Persons.Where(a => a.id_person == id_person)
												.Select(a => new PersonViewModel
												{
													Address = a.Address,
													EGN = a.EGN,
													GSM = a.GSM,
													id_person = a.id_person,
													id_gender = a.id_gender,
													Name = a.Name
												}).Single();
		}

		public void CalculateAbsenceDays(DateTime StartDate, DateTime EndDate, out int CalendarDays, out int WorkDays)
		{
			CalendarDays = 0;
			WorkDays = 0;
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