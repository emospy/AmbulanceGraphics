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
    
    public partial class HR_Contracts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HR_Contracts()
        {
            this.HR_Assignments = new HashSet<HR_Assignments>();
            this.HR_YearHolidays = new HashSet<HR_YearHolidays>();
            this.HR_Absence = new HashSet<HR_Absence>();
        }
    
        public int id_contract { get; set; }
        public int id_person { get; set; }
        public Nullable<System.DateTime> ContractDate { get; set; }
        public string ContractNumber { get; set; }
        public bool IsFired { get; set; }
        public Nullable<int> ContractID { get; set; }
        public int id_userLogin { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        public virtual UN_Persons UN_Persons { get; set; }
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_Assignments> HR_Assignments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_YearHolidays> HR_YearHolidays { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_Absence> HR_Absence { get; set; }
    }
}
