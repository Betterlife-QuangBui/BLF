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
    
    public partial class WareHouseCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WareHouseCategory()
        {
            this.AgencyPackageItems = new HashSet<AgencyPackageItem>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameJP { get; set; }
        public string NameEN { get; set; }
        public Nullable<int> NoOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgencyPackageItem> AgencyPackageItems { get; set; }
    }
}