//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ohayoo.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShippingJ_City
    {
        public int id { get; set; }
        public Nullable<int> zoneId { get; set; }
        public string name { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> updatedDate { get; set; }
        public string updatedBy { get; set; }
    
        public virtual ShippingJ_Zone ShippingJ_Zone { get; set; }
    }
}
