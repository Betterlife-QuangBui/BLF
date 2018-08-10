﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class WareHouseJPDB : DbContext
    {
        public WareHouseJPDB()
            : base("name=WareHouseJPDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<AgencyAccount> AgencyAccounts { get; set; }
        public virtual DbSet<AgencyPackage> AgencyPackages { get; set; }
        public virtual DbSet<AgencyPackageItem> AgencyPackageItems { get; set; }
        public virtual DbSet<AirInfo> AirInfoes { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<ClearanceAir> ClearanceAirs { get; set; }
        public virtual DbSet<ClearanceHAWB> ClearanceHAWBs { get; set; }
        public virtual DbSet<ClearanceMAWB> ClearanceMAWBs { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CustomsClearance> CustomsClearances { get; set; }
        public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual DbSet<DeliveryCom> DeliveryComs { get; set; }
        public virtual DbSet<EmailHistory> EmailHistories { get; set; }
        public virtual DbSet<EmailSend> EmailSends { get; set; }
        public virtual DbSet<ExportGood> ExportGoods { get; set; }
        public virtual DbSet<ExportGoodDetail> ExportGoodDetails { get; set; }
        public virtual DbSet<ExportInvoice> ExportInvoices { get; set; }
        public virtual DbSet<FlightBooking> FlightBookings { get; set; }
        public virtual DbSet<FlightBookingHAWB> FlightBookingHAWBs { get; set; }
        public virtual DbSet<FlightBookingMAWB> FlightBookingMAWBs { get; set; }
        public virtual DbSet<FlightGood> FlightGoods { get; set; }
        public virtual DbSet<FlightRoute> FlightRoutes { get; set; }
        public virtual DbSet<FromAir> FromAirs { get; set; }
        public virtual DbSet<HAWB> HAWBs { get; set; }
        public virtual DbSet<HAWBDetail> HAWBDetails { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<MAWB> MAWBs { get; set; }
        public virtual DbSet<MAWBDetail> MAWBDetails { get; set; }
        public virtual DbSet<PackageReturn> PackageReturns { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<ReturnAddress> ReturnAddresses { get; set; }
        public virtual DbSet<ReturnDetail> ReturnDetails { get; set; }
        public virtual DbSet<ReturnHAWB> ReturnHAWBs { get; set; }
        public virtual DbSet<ReturnItem> ReturnItems { get; set; }
        public virtual DbSet<ReturnTracking> ReturnTrackings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shipment> Shipments { get; set; }
        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<ShippingBooking> ShippingBookings { get; set; }
        public virtual DbSet<ShippingHAWB> ShippingHAWBs { get; set; }
        public virtual DbSet<ShippingHAWBDetail> ShippingHAWBDetails { get; set; }
        public virtual DbSet<ShippingHistory> ShippingHistories { get; set; }
        public virtual DbSet<SizeTable> SizeTables { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StatusWareHouse> StatusWareHouses { get; set; }
        public virtual DbSet<StorageJP> StorageJPs { get; set; }
        public virtual DbSet<ToAir> ToAirs { get; set; }
        public virtual DbSet<TrackingDetail> TrackingDetails { get; set; }
        public virtual DbSet<WareHouseCategory> WareHouseCategories { get; set; }
        public virtual DbSet<WareHouseInfo> WareHouseInfoes { get; set; }
        public virtual DbSet<WareHouseItem> WareHouseItems { get; set; }
        public virtual DbSet<StorageItemJP> StorageItemJPs { get; set; }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual int sp_alterdiagram1(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram1", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram1(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram1", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram1(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram1", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition1_Result> sp_helpdiagramdefinition1(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition1_Result>("sp_helpdiagramdefinition1", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams1_Result> sp_helpdiagrams1(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams1_Result>("sp_helpdiagrams1", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram1(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram1", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams1()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams1");
        }
    
        public virtual ObjectResult<proStoreJPItems_Result> proStoreJPItems(ObjectParameter totalRows, string agencyId, Nullable<int> pageNo, Nullable<int> pageSize, string sortColumn, string sortOrder, string nameJP, string nameEN, Nullable<int> categoryId, string janCode, string productCode, Nullable<double> price, string material, string component)
        {
            var agencyIdParameter = agencyId != null ?
                new ObjectParameter("AgencyId", agencyId) :
                new ObjectParameter("AgencyId", typeof(string));
    
            var pageNoParameter = pageNo.HasValue ?
                new ObjectParameter("PageNo", pageNo) :
                new ObjectParameter("PageNo", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var sortColumnParameter = sortColumn != null ?
                new ObjectParameter("SortColumn", sortColumn) :
                new ObjectParameter("SortColumn", typeof(string));
    
            var sortOrderParameter = sortOrder != null ?
                new ObjectParameter("SortOrder", sortOrder) :
                new ObjectParameter("SortOrder", typeof(string));
    
            var nameJPParameter = nameJP != null ?
                new ObjectParameter("NameJP", nameJP) :
                new ObjectParameter("NameJP", typeof(string));
    
            var nameENParameter = nameEN != null ?
                new ObjectParameter("NameEN", nameEN) :
                new ObjectParameter("NameEN", typeof(string));
    
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("CategoryId", categoryId) :
                new ObjectParameter("CategoryId", typeof(int));
    
            var janCodeParameter = janCode != null ?
                new ObjectParameter("JanCode", janCode) :
                new ObjectParameter("JanCode", typeof(string));
    
            var productCodeParameter = productCode != null ?
                new ObjectParameter("ProductCode", productCode) :
                new ObjectParameter("ProductCode", typeof(string));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(double));
    
            var materialParameter = material != null ?
                new ObjectParameter("Material", material) :
                new ObjectParameter("Material", typeof(string));
    
            var componentParameter = component != null ?
                new ObjectParameter("Component", component) :
                new ObjectParameter("Component", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proStoreJPItems_Result>("proStoreJPItems", totalRows, agencyIdParameter, pageNoParameter, pageSizeParameter, sortColumnParameter, sortOrderParameter, nameJPParameter, nameENParameter, categoryIdParameter, janCodeParameter, productCodeParameter, priceParameter, materialParameter, componentParameter);
        }
    
        public virtual ObjectResult<proWareHouseItems_Result> proWareHouseItems(ObjectParameter totalRows, Nullable<int> pageNo, Nullable<int> pageSize, string sortColumn, string sortOrder, string nameJP, string nameEN, Nullable<int> categoryId, string categoryWeb, string janCode, string productCode, Nullable<double> quantity, Nullable<double> price, Nullable<double> amount, string origin, string flightCode, string tracking, Nullable<int> productTypeId, string material, string component, Nullable<System.Guid> iD)
        {
            var pageNoParameter = pageNo.HasValue ?
                new ObjectParameter("PageNo", pageNo) :
                new ObjectParameter("PageNo", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var sortColumnParameter = sortColumn != null ?
                new ObjectParameter("SortColumn", sortColumn) :
                new ObjectParameter("SortColumn", typeof(string));
    
            var sortOrderParameter = sortOrder != null ?
                new ObjectParameter("SortOrder", sortOrder) :
                new ObjectParameter("SortOrder", typeof(string));
    
            var nameJPParameter = nameJP != null ?
                new ObjectParameter("NameJP", nameJP) :
                new ObjectParameter("NameJP", typeof(string));
    
            var nameENParameter = nameEN != null ?
                new ObjectParameter("NameEN", nameEN) :
                new ObjectParameter("NameEN", typeof(string));
    
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("CategoryId", categoryId) :
                new ObjectParameter("CategoryId", typeof(int));
    
            var categoryWebParameter = categoryWeb != null ?
                new ObjectParameter("CategoryWeb", categoryWeb) :
                new ObjectParameter("CategoryWeb", typeof(string));
    
            var janCodeParameter = janCode != null ?
                new ObjectParameter("JanCode", janCode) :
                new ObjectParameter("JanCode", typeof(string));
    
            var productCodeParameter = productCode != null ?
                new ObjectParameter("ProductCode", productCode) :
                new ObjectParameter("ProductCode", typeof(string));
    
            var quantityParameter = quantity.HasValue ?
                new ObjectParameter("Quantity", quantity) :
                new ObjectParameter("Quantity", typeof(double));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(double));
    
            var amountParameter = amount.HasValue ?
                new ObjectParameter("Amount", amount) :
                new ObjectParameter("Amount", typeof(double));
    
            var originParameter = origin != null ?
                new ObjectParameter("Origin", origin) :
                new ObjectParameter("Origin", typeof(string));
    
            var flightCodeParameter = flightCode != null ?
                new ObjectParameter("FlightCode", flightCode) :
                new ObjectParameter("FlightCode", typeof(string));
    
            var trackingParameter = tracking != null ?
                new ObjectParameter("Tracking", tracking) :
                new ObjectParameter("Tracking", typeof(string));
    
            var productTypeIdParameter = productTypeId.HasValue ?
                new ObjectParameter("ProductTypeId", productTypeId) :
                new ObjectParameter("ProductTypeId", typeof(int));
    
            var materialParameter = material != null ?
                new ObjectParameter("Material", material) :
                new ObjectParameter("Material", typeof(string));
    
            var componentParameter = component != null ?
                new ObjectParameter("Component", component) :
                new ObjectParameter("Component", typeof(string));
    
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proWareHouseItems_Result>("proWareHouseItems", totalRows, pageNoParameter, pageSizeParameter, sortColumnParameter, sortOrderParameter, nameJPParameter, nameENParameter, categoryIdParameter, categoryWebParameter, janCodeParameter, productCodeParameter, quantityParameter, priceParameter, amountParameter, originParameter, flightCodeParameter, trackingParameter, productTypeIdParameter, materialParameter, componentParameter, iDParameter);
        }
    
        public virtual ObjectResult<sp_FindbyALL_Result> sp_FindbyALL(ObjectParameter rowCount, Nullable<int> startRow, Nullable<int> length)
        {
            var startRowParameter = startRow.HasValue ?
                new ObjectParameter("StartRow", startRow) :
                new ObjectParameter("StartRow", typeof(int));
    
            var lengthParameter = length.HasValue ?
                new ObjectParameter("Length", length) :
                new ObjectParameter("Length", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FindbyALL_Result>("sp_FindbyALL", rowCount, startRowParameter, lengthParameter);
        }
    
        public virtual ObjectResult<USP_SEL_Contacts_Result> USP_SEL_Contacts(ObjectParameter totalRows, Nullable<int> pageNo, Nullable<int> pageSize, string sortColumn, string sortOrder, string nameJP, string nameEN, Nullable<int> categoryId, string categoryWeb, string janCode, string productCode, Nullable<double> quantity, Nullable<double> price, Nullable<double> amount, string origin, string flightCode, string tracking, Nullable<int> productTypeId, string material, string component, Nullable<System.Guid> iD)
        {
            var pageNoParameter = pageNo.HasValue ?
                new ObjectParameter("PageNo", pageNo) :
                new ObjectParameter("PageNo", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var sortColumnParameter = sortColumn != null ?
                new ObjectParameter("SortColumn", sortColumn) :
                new ObjectParameter("SortColumn", typeof(string));
    
            var sortOrderParameter = sortOrder != null ?
                new ObjectParameter("SortOrder", sortOrder) :
                new ObjectParameter("SortOrder", typeof(string));
    
            var nameJPParameter = nameJP != null ?
                new ObjectParameter("NameJP", nameJP) :
                new ObjectParameter("NameJP", typeof(string));
    
            var nameENParameter = nameEN != null ?
                new ObjectParameter("NameEN", nameEN) :
                new ObjectParameter("NameEN", typeof(string));
    
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("CategoryId", categoryId) :
                new ObjectParameter("CategoryId", typeof(int));
    
            var categoryWebParameter = categoryWeb != null ?
                new ObjectParameter("CategoryWeb", categoryWeb) :
                new ObjectParameter("CategoryWeb", typeof(string));
    
            var janCodeParameter = janCode != null ?
                new ObjectParameter("JanCode", janCode) :
                new ObjectParameter("JanCode", typeof(string));
    
            var productCodeParameter = productCode != null ?
                new ObjectParameter("ProductCode", productCode) :
                new ObjectParameter("ProductCode", typeof(string));
    
            var quantityParameter = quantity.HasValue ?
                new ObjectParameter("Quantity", quantity) :
                new ObjectParameter("Quantity", typeof(double));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(double));
    
            var amountParameter = amount.HasValue ?
                new ObjectParameter("Amount", amount) :
                new ObjectParameter("Amount", typeof(double));
    
            var originParameter = origin != null ?
                new ObjectParameter("Origin", origin) :
                new ObjectParameter("Origin", typeof(string));
    
            var flightCodeParameter = flightCode != null ?
                new ObjectParameter("FlightCode", flightCode) :
                new ObjectParameter("FlightCode", typeof(string));
    
            var trackingParameter = tracking != null ?
                new ObjectParameter("Tracking", tracking) :
                new ObjectParameter("Tracking", typeof(string));
    
            var productTypeIdParameter = productTypeId.HasValue ?
                new ObjectParameter("ProductTypeId", productTypeId) :
                new ObjectParameter("ProductTypeId", typeof(int));
    
            var materialParameter = material != null ?
                new ObjectParameter("Material", material) :
                new ObjectParameter("Material", typeof(string));
    
            var componentParameter = component != null ?
                new ObjectParameter("Component", component) :
                new ObjectParameter("Component", typeof(string));
    
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<USP_SEL_Contacts_Result>("USP_SEL_Contacts", totalRows, pageNoParameter, pageSizeParameter, sortColumnParameter, sortOrderParameter, nameJPParameter, nameENParameter, categoryIdParameter, categoryWebParameter, janCodeParameter, productCodeParameter, quantityParameter, priceParameter, amountParameter, originParameter, flightCodeParameter, trackingParameter, productTypeIdParameter, materialParameter, componentParameter, iDParameter);
        }
    }
}