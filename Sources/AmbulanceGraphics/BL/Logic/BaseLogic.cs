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
		internal AmbulanceEntities _databaseContext;

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
		public IRepository<GR_ShiftTypes> GR_ShiftTypes
		{
			get { return new Repository<GR_ShiftTypes>(_databaseContext); }
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

		public IRepository<NM_LawTypes> NM_LawTypes
		{
			get { return new Repository<NM_LawTypes>(_databaseContext); }
		}

		public IRepository<NM_ContractTypes> NM_ContractTypes
		{
			get { return new Repository<NM_ContractTypes>(_databaseContext); }
		}

		public IRepository<NM_AmbulanceTypes> NM_AmbulanceTypes
		{
			get { return new Repository<NM_AmbulanceTypes>(_databaseContext); }
		}

		public IRepository<NM_CrewTypes> NM_CrewTypes
		{
			get { return new Repository<NM_CrewTypes>(_databaseContext); }
		}

		public IRepository<NM_ScheduleTypes> NM_ScheduleTypes
		{
			get { return new Repository<NM_ScheduleTypes>(_databaseContext); }
		}

		public IRepository<GR_WorkHours> GR_WorkHours
		{
			get { return new Repository<GR_WorkHours>(_databaseContext); }
		}

		public IRepository<GR_WorkTimeAbsence> GR_WorkTimeAbsence
		{
			get { return new Repository<GR_WorkTimeAbsence>(_databaseContext); }
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
