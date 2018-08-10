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
    
    public partial class proStoreJPItems_Result
    {
        public Nullable<long> ROWNUM { get; set; }
        public Nullable<int> TotalCount { get; set; }
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> StoregeJPId { get; set; }
        public Nullable<System.Guid> TrackingDetailId { get; set; }
        public string NameEN { get; set; }
        public string NameJP { get; set; }
        public string ProductCode { get; set; }
        public string JanCode { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string LinkWeb { get; set; }
        public string Image { get; set; }
        public string ImageLinkWeb { get; set; }
        public string ImageBase64 { get; set; }
        public byte[] ImageBinary { get; set; }
        public string Material { get; set; }
        public string MaterialImage { get; set; }
        public string Component { get; set; }
        public string ComponentImage { get; set; }
        public Nullable<double> Quantity { get; set; }
        public Nullable<double> PriceTax { get; set; }
        public Nullable<double> Amount { get; set; }
        public string MadeIn { get; set; }
        public string Notes { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsExport { get; set; }
        public Nullable<bool> IsDeny { get; set; }
    }
}