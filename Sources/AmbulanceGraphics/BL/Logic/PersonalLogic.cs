using BL.DB;
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
		public List<PersonnelViewModel> GetPersonnel(bool IsFired)
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
									 (acc.HR_StructurePositions.UN_Departments.Level == 3) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 2) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
									 (acc.HR_StructurePositions.UN_Departments.Level == 1) ? acc.HR_StructurePositions.id_department : (int?)null,

								  id_level3 = (acc == null) ? (int?)null :
									(acc.HR_StructurePositions.UN_Departments.Level == 2) ? ((acc == null) ? (int?)null : acc.HR_StructurePositions.UN_Departments.UN_Departments2.id_department) :
                                    (acc.HR_StructurePositions.UN_Departments.Level == 1) ? acc.HR_StructurePositions.id_department : (int?)null,

								  id_level4 = (acc == null) ? (int?)null :
									 (acc.HR_StructurePositions.UN_Departments.Level == 1) ? acc.HR_StructurePositions.id_department : (int?)null,

								  Position = acc.HR_StructurePositions.HR_GlobalPositions.Name,
							  }).ToList();
			}
			else
			{
				lstPersons = (from per in this._databaseContext.UN_Persons
							  join con in this._databaseContext.HR_Contracts on per.id_person equals con.id_person into pa
							  from pas in pa.DefaultIfEmpty()
							  join ass in this._databaseContext.HR_Assignments on pas.id_contract equals ass.id_contract into ac
							  from acc in ac.DefaultIfEmpty()							  

							  where pas == null || pas.IsFired == false						  

							  select new PersonnelViewModel
							  {
								  id_person = per.id_person,
								  Name = per.Name,
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

								  Position = acc.HR_StructurePositions.HR_GlobalPositions.Name,
							  }).ToList();
			}

			//int id_p = lstPersons[0].id_person;

			//var person = this._databaseContext.UN_Persons.FirstOrDefault(a => a.id_person == id_p);


			return lstPersons;
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

			return vm;
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

				cvm.StructurePosition = baseAssignment.HR_StructurePositions.HR_GlobalPositions.Name;



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

					cam.StructurePosition = ass.HR_StructurePositions.HR_GlobalPositions.Name;

					cvm.lstAdditionalAssignments.Add(cam);
				}

				vm.lstContracts.Add(cvm);
			}
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
