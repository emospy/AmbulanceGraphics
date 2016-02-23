using BL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class BaseLogic : CoreLogic , IDisposable
	{
		internal readonly AmbulanceEntities _databaseContext;

		public BaseLogic()
		{
			_databaseContext = new AmbulanceEntities();
			//_databaseContext.Database.Connection.ConnectionString = cs;
		}

		#region Repository tables
		public IRepository<GR_AdditionalShiftRequests> GR_AdditionalShiftRequests
		{
			get { return new Repository<GR_AdditionalShiftRequests>(_databaseContext); }
		}

		public IRepository<GR_Ambulances> GR_Ambulances
		{
			get { return new Repository<GR_Ambulances>(_databaseContext); }
		}
		public IRepository<GR_DriverAmbulances> GR_DriverAmbulances
		{
			get { return new Repository<GR_DriverAmbulances>(_databaseContext); }
		}
		public IRepository<GR_PresenceForms> GR_PresenceForms
		{
			get { return new Repository<GR_PresenceForms>(_databaseContext); }
		}
		public IRepository<GR_ShiftsPlan> GR_ShiftsPlan
		{
			get { return new Repository<GR_ShiftsPlan>(_databaseContext); }
		}
		public IRepository<HR_Absence> HR_Absence
		{
			get { return new Repository<HR_Absence>(_databaseContext); }
		}
		public IRepository<HR_Assignments> HR_Assignments
		{
			get { return new Repository<HR_Assignments>(_databaseContext); }
		}
		public IRepository<HR_Contracts> HR_Contracts
		{
			get { return new Repository<HR_Contracts>(_databaseContext); }
		}
		public IRepository<HR_GlobalPositions> HR_GlobalPositions
		{
			get { return new Repository<HR_GlobalPositions>(_databaseContext); }
		}
		public IRepository<HR_StructurePositions> HR_StructurePositions
		{
			get { return new Repository<HR_StructurePositions>(_databaseContext); }
		}
		public IRepository<HR_WorkTime> HR_WorkTime
		{
			get { return new Repository<HR_WorkTime>(_databaseContext); }
		}
		public IRepository<HR_YearWorkDays> HR_YearWorkDays
		{
			get { return new Repository<HR_YearWorkDays>(_databaseContext); }
		}
		public IRepository<NM_AbsenceTypes> NM_AbsenceTypes
		{
			get { return new Repository<NM_AbsenceTypes>(_databaseContext); }
		}
		public IRepository<NM_MedicalSpecialities> NM_MedicalSpecialities
		{
			get { return new Repository<NM_MedicalSpecialities>(_databaseContext); }
		}
		public IRepository<NM_ShiftTypes> NM_ShiftTypes
		{
			get { return new Repository<NM_ShiftTypes>(_databaseContext); }
		}

		public IRepository<NM_PositionTypes> NM_PositionTypes
		{
			get { return new Repository<NM_PositionTypes>(_databaseContext); }
		}

		public IRepository<UN_Departments> UN_Departments
		{
			get { return new Repository<UN_Departments>(_databaseContext); }
		}

		public IRepository<UN_Persons> UN_Persons
		{
			get { return new Repository<UN_Persons>(_databaseContext); }
		}
		#endregion

		#region Repository Methods

		#endregion
		public void Save()
		{
			_databaseContext.SaveChanges();
		}

		public void Dispose()
		{
			_databaseContext.Dispose();
		}
	}
}
