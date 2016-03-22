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
    
    public partial class HR_GlobalPositions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HR_GlobalPositions()
        {
            this.HR_StructurePositions = new HashSet<HR_StructurePositions>();
        }
    
        public int id_globalPosition { get; set; }
        public string Name { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
        public bool IsActive { get; set; }
        public int id_positionType { get; set; }
        public Nullable<int> id_nkpd { get; set; }
        public System.DateTime ActiveFrom { get; set; }
        public Nullable<System.DateTime> ActiveTo { get; set; }
    
        public virtual HR_NKPD HR_NKPD { get; set; }
        public virtual NM_PositionTypes NM_PositionTypes { get; set; }
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HR_StructurePositions> HR_StructurePositions { get; set; }
    }
}