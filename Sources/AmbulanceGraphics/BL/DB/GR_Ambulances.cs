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
    
    public partial class GR_Ambulances
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GR_Ambulances()
        {
            this.GR_AmbulanceCrews = new HashSet<GR_AmbulanceCrews>();
            this.GR_DriverAmbulances = new HashSet<GR_DriverAmbulances>();
            this.GR_DriverAmbulances1 = new HashSet<GR_DriverAmbulances>();
        }
    
        public int id_ambulance { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
        public int id_ambulanceType { get; set; }
        public string WorkTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_AmbulanceCrews> GR_AmbulanceCrews { get; set; }
        public virtual NM_AmbulanceTypes NM_AmbulanceTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_DriverAmbulances> GR_DriverAmbulances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GR_DriverAmbulances> GR_DriverAmbulances1 { get; set; }
    }
}