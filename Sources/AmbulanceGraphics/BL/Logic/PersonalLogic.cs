using BL.DB;
using BL.Models;
using System;
using System.Collections.Generic;
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

							  where pas.IsFired == false
							  && acc.IsActive == true

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
