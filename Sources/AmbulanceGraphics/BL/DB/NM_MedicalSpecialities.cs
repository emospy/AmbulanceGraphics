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
    
    public partial class NM_MedicalSpecialities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NM_MedicalSpecialities()
        {
            this.UN_Persons = new HashSet<UN_Persons>();
        }
    
        public int id_medicalSpeciality { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
    
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UN_Persons> UN_Persons { get; set; }
    }
}
