//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WareHouseJP.Website.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FlightGood
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> FlightBookingId { get; set; }
        public Nullable<System.Guid> ExportGoodId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ExportGood ExportGood { get; set; }
        public virtual FlightBooking FlightBooking { get; set; }
    }
}