//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tesp.App.db
{
    using System;
    using System.Collections.Generic;
    
    public partial class AgencyPackageItem
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> AgencyPackageId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string ItemCategory { get; set; }
        public Nullable<double> ItemQuantity { get; set; }
        public string ItemUrl { get; set; }
        public string ItemImg { get; set; }
        public string ItemNotes { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual AgencyPackage AgencyPackage { get; set; }
        public virtual WareHouseCategory WareHouseCategory { get; set; }
    }
}
