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
    
    public partial class ReturnItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReturnItem()
        {
            this.ReturnAddresses = new HashSet<ReturnAddress>();
            this.ReturnTrackings = new HashSet<ReturnTracking>();
        }
    
        public System.Guid Id { get; set; }
        public string AgencyId { get; set; }
        public string Code { get; set; }
        public Nullable<int> SizeId { get; set; }
        public Nullable<double> Weigh { get; set; }
        public Nullable<double> PackageCount { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public string ReturnHour { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual Agency Agency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReturnAddress> ReturnAddresses { get; set; }
        public virtual SizeTable SizeTable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReturnTracking> ReturnTrackings { get; set; }
    }
}