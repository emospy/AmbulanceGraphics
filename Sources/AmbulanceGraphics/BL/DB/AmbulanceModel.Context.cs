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
        public virtual DbSet<GR_Ambulances> GR_Ambulances { get; set; }
        public virtual DbSet<GR_PresenceForms> GR_PresenceForms { get; set; }
        public virtual DbSet<GR_ShiftsPlan> GR_ShiftsPlan { get; set; }
        public virtual DbSet<HR_Assignments> HR_Assignments { get; set; }
        public virtual DbSet<HR_GlobalPositions> HR_GlobalPositions { get; set; }
        public virtual DbSet<HR_StructurePositions> HR_StructurePositions { get; set; }
        public virtual DbSet<HR_WorkTime> HR_WorkTime { get; set; }
        public virtual DbSet<HR_YearWorkDays> HR_YearWorkDays { get; set; }
        public virtual DbSet<NM_AbsenceTypes> NM_AbsenceTypes { get; set; }
        public virtual DbSet<NM_MedicalSpecialities> NM_MedicalSpecialities { get; set; }
        public virtual DbSet<NM_ShiftTypes> NM_ShiftTypes { get; set; }
        public virtual DbSet<UN_Departments> UN_Departments { get; set; }
        public virtual DbSet<UN_UserLogins> UN_UserLogins { get; set; }
        public virtual DbSet<GR_DriverAmbulances> GR_DriverAmbulances { get; set; }
        public virtual DbSet<UN_Persons> UN_Persons { get; set; }
        public virtual DbSet<NM_Roles> NM_Roles { get; set; }
        public virtual DbSet<NM_PositionTypes> NM_PositionTypes { get; set; }
        public virtual DbSet<HR_Absence> HR_Absence { get; set; }
        public virtual DbSet<UN_DepartmentTree> UN_DepartmentTree { get; set; }
    }
}
