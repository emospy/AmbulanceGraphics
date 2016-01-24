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
    
    public partial class UN_Departments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UN_Departments()
        {
            this.GR_AdditionalShiftRequests = new HashSet<GR_AdditionalShiftRequests>();
            this.HR_StructurePositions = new HashSet<HR_StructurePositions>();
            this.UN_DepartmentTree = new HashSet<UN_DepartmentTree>();
        }
    
        public int id_department { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_AdditionalShiftRequests> GR_AdditionalShiftRequests { get; set; }
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_StructurePositions> HR_StructurePositions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UN_DepartmentTree> UN_DepartmentTree { get; set; }
    }
}
