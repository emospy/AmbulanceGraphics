//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class HR_Assignments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HR_Assignments()
        {
            this.GR_AdditionalShiftRequests = new HashSet<GR_AdditionalShiftRequests>();
            this.GR_CrewMembers = new HashSet<GR_CrewMembers>();
            this.GR_DayPlans = new HashSet<GR_DayPlans>();
            this.GR_DriverAmbulances = new HashSet<GR_DriverAmbulances>();
            this.GR_MonthPlans = new HashSet<GR_MonthPlans>();
            this.GR_PresenceForms = new HashSet<GR_PresenceForms>();
            this.GR_ShiftAssignments = new HashSet<GR_ShiftAssignments>();
            this.GR_ShiftsPlan = new HashSet<GR_ShiftsPlan>();
            this.HR_Absence = new HashSet<HR_Absence>();
        }
    
        public int id_assignment { get; set; }
        public int id_contract { get; set; }
        public int id_structurePosition { get; set; }
        public Nullable<int> id_workTime { get; set; }
        public Nullable<double> BaseSalary { get; set; }
        public Nullable<System.DateTime> AssignmentDate { get; set; }
        public Nullable<System.DateTime> TestContractDate { get; set; }
        public Nullable<System.DateTime> EndContractDate { get; set; }
        public string ContractNumber { get; set; }
        public Nullable<System.DateTime> ContractDate { get; set; }
        public Nullable<int> NumberHolidays { get; set; }
        public Nullable<int> AdditionalHolidays { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdditionalAssignment { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_AdditionalShiftRequests> GR_AdditionalShiftRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_CrewMembers> GR_CrewMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_DayPlans> GR_DayPlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_DriverAmbulances> GR_DriverAmbulances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_MonthPlans> GR_MonthPlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_PresenceForms> GR_PresenceForms { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_ShiftAssignments> GR_ShiftAssignments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_ShiftsPlan> GR_ShiftsPlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_Absence> HR_Absence { get; set; }
        public virtual HR_WorkTime HR_WorkTime { get; set; }
        public virtual HR_Contracts HR_Contracts { get; set; }
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        public virtual HR_StructurePositions HR_StructurePositions { get; set; }
    }
}
