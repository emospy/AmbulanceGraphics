﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BL.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AmbulanceEntities : DbContext
    {
        public AmbulanceEntities()
            : base("name=AmbulanceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GR_AdditionalShiftRequests> GR_AdditionalShiftRequests { get; set; }
        public virtual DbSet<GR_ShiftsPlan> GR_ShiftsPlan { get; set; }
        public virtual DbSet<HR_WorkTime> HR_WorkTime { get; set; }
        public virtual DbSet<HR_YearWorkDays> HR_YearWorkDays { get; set; }
        public virtual DbSet<NM_AbsenceTypes> NM_AbsenceTypes { get; set; }
        public virtual DbSet<NM_MedicalSpecialities> NM_MedicalSpecialities { get; set; }
        public virtual DbSet<UN_UserLogins> UN_UserLogins { get; set; }
        public virtual DbSet<GR_DriverAmbulances> GR_DriverAmbulances { get; set; }
        public virtual DbSet<NM_Roles> NM_Roles { get; set; }
        public virtual DbSet<NM_PositionTypes> NM_PositionTypes { get; set; }
        public virtual DbSet<HR_Contracts> HR_Contracts { get; set; }
        public virtual DbSet<HR_IdentityCards> HR_IdentityCards { get; set; }
        public virtual DbSet<HR_NKPD> HR_NKPD { get; set; }
        public virtual DbSet<HR_GlobalPositions> HR_GlobalPositions { get; set; }
        public virtual DbSet<UN_Persons> UN_Persons { get; set; }
        public virtual DbSet<UN_AuditLog> UN_AuditLog { get; set; }
        public virtual DbSet<GR_AmbulanceCrews> GR_AmbulanceCrews { get; set; }
        public virtual DbSet<GR_DayPlans> GR_DayPlans { get; set; }
        public virtual DbSet<GR_DepartmentShifts> GR_DepartmentShifts { get; set; }
        public virtual DbSet<GR_MonthPlans> GR_MonthPlans { get; set; }
        public virtual DbSet<GR_ShiftAssignments> GR_ShiftAssignments { get; set; }
        public virtual DbSet<GR_SixMonthPlans> GR_SixMonthPlans { get; set; }
        public virtual DbSet<UN_Departments> UN_Departments { get; set; }
        public virtual DbSet<HR_Assignments> HR_Assignments { get; set; }
        public virtual DbSet<HR_StructurePositions> HR_StructurePositions { get; set; }
        public virtual DbSet<NM_ContractTypes> NM_ContractTypes { get; set; }
        public virtual DbSet<NM_LawTypes> NM_LawTypes { get; set; }
        public virtual DbSet<NM_AmbulanceTypes> NM_AmbulanceTypes { get; set; }
        public virtual DbSet<HR_StructurePositionTypes> HR_StructurePositionTypes { get; set; }
        public virtual DbSet<GR_Ambulances> GR_Ambulances { get; set; }
        public virtual DbSet<NM_CrewTypes> NM_CrewTypes { get; set; }
        public virtual DbSet<GR_ShiftTypes> GR_ShiftTypes { get; set; }
        public virtual DbSet<HR_YearHolidays> HR_YearHolidays { get; set; }
        public virtual DbSet<HR_Absence> HR_Absence { get; set; }
        public virtual DbSet<NM_ScheduleTypes> NM_ScheduleTypes { get; set; }
        public virtual DbSet<GR_Crews> GR_Crews { get; set; }
        public virtual DbSet<GR_PresenceForms> GR_PresenceForms { get; set; }
        public virtual DbSet<GR_WorkHours> GR_WorkHours { get; set; }
    }
}
