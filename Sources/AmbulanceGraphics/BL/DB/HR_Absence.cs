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
    
    public partial class HR_Absence
    {
        public int id_absence { get; set; }
        public int id_absenceType { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public int id_assignment { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int id_userLogin { get; set; }
    
        public virtual NM_AbsenceTypes NM_AbsenceTypes { get; set; }
        public virtual UN_UserLogins UN_UserLogins { get; set; }
        public virtual HR_Assignments HR_Assignments { get; set; }
    }
}
