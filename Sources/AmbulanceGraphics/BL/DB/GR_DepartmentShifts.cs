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
    
    public partial class GR_DepartmentShifts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GR_DepartmentShifts()
        {
            this.GR_ShiftAssignments = new HashSet<GR_ShiftAssignments>();
        }
    
        public int id_departmentShift { get; set; }
        public int id_department { get; set; }
        public string Name { get; set; }
        public int id_userLogin { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_ShiftAssignments> GR_ShiftAssignments { get; set; }
    }
}
