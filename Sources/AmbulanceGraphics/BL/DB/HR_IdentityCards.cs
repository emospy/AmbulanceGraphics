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
    
    public partial class HR_IdentityCards
    {
        public int id_identityCard { get; set; }
        public string IDCardNumber { get; set; }
        public Nullable<System.DateTime> IDCardIssueDate { get; set; }
        public string IDCardIssuedBy { get; set; }
        public string Address { get; set; }
        public Nullable<int> id_city { get; set; }
        public Nullable<System.DateTime> IDCardValidDate { get; set; }
        public int id_identityCardType { get; set; }
    }
}
