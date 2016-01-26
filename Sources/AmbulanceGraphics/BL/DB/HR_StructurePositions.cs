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
    
    public partial class HR_StructurePositions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HR_StructurePositions()
        {
            this.HR_Assignments = new HashSet<HR_Assignments>();
        }
    
        public int id_structurePosition { get; set; }
        public int id_globalPosition { get; set; }
        public Nullable<double> StaffCount { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
        public bool IsActive { get; set; }
        public int id_department { get; set; }
        public string Code { get; set; }
        public int Order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_Assignments> HR_Assignments { get; set; }
        public virtual HR_GlobalPositions HR_GlobalPositions { get; set; }
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        public virtual UN_Departments UN_Departments { get; set; }
    }
}
