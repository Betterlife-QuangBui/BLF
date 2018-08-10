using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using Web.Helpers.Database;
using Web.Helpers.Images;

namespace WareHouseJP.Website.Controllers
{
    public class ImportController : ManagementSystemController
    {
        #region Shipment
        // GET: Import
        [HttpGet]
        public ActionResult Shipment(Guid id)
        {
            var shipment = db.Shipments.Find(id);
            return View(shipment);
        }
        [HttpPost]
        public ActionResult Shipment(Guid id, int option = 1, string fileName = "")
        {
            //upload file
            HttpPostedFileBase file = Request.Files[0];
            fileName = file.FileName.DoiTenFile();
            file.SaveAs(Server.MapPath("~/Uploads/Shipment/" + fileName));
            //read file
            var shipment = db.Shipments.Find(id);
            // Opening the Excel template... 
            FileStream fs = new FileStream(Server.MapPath("~/Uploads/Shipment/" + fileName), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            ISheet sheet = null;
            if (fileName.Contains(".xlsx"))
            {
                XSSFWorkbook templateWorkbook = new XSSFWorkbook(Server.MapPath("~/Uploads/Shipment/" + fileName));
                sheet = templateWorkbook.GetSheet("INVOICE");
            }
            else if (fileName.Contains(".xls"))
            {
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                sheet = templateWorkbook.GetSheet("INVOICE");
            }
            if (option == 1) { ImportNew(sheet, shipment); }
            if (option == 2) { ImportOverwrite(sheet, shipment); }
            return View();
        }
        public List<ShipmentImport> readImportShipment(ISheet sheet)
        {
            List<ShipmentImport> lstData = new List<ShipmentImport>();
            string trackingNumber = "";
            for (int row = 10; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row).GetCell(2).StringCellValue != "")
                {
                    trackingNumber = sheet.GetRow(row).GetCell(2).StringCellValue;
                }
                if (sheet.GetRow(row).GetCell(2).StringCellValue == "" && sheet.GetRow(row).GetCell(11).StringCellValue == "") { break; }
                double weigh = 0;
                try
                {
                    weigh = sheet.GetRow(row).GetCell(7).NumericCellValue;
                }
                catch
                {
                    if (sheet.GetRow(row).GetCell(7).StringCellValue != "")
                    {
                        weigh = double.Parse(sheet.GetRow(row).GetCell(7).StringCellValue);
                    }
                }
                int quantity = 0;
                try
                {
                    quantity = (int)sheet.GetRow(row).GetCell(12).NumericCellValue;
                }
                catch
                {
                    if (sheet.GetRow(row).GetCell(12).StringCellValue != "")
                    {
                        quantity = int.Parse(sheet.GetRow(row).GetCell(12).StringCellValue);
                    }
                }
                ShipmentImport item = new ShipmentImport()
                {
                    TrackingNumber = trackingNumber,
                    DeliveryName = sheet.GetRow(row).GetCell(3).StringCellValue,
                    SendDate = sheet.GetRow(row).GetCell(4).StringCellValue != "" ? DateTime.Parse(sheet.GetRow(row).GetCell(4).StringCellValue) : DateTime.Now,
                    RecivedDate = sheet.GetRow(row).GetCell(5).StringCellValue != "" ? DateTime.Parse(sheet.GetRow(row).GetCell(5).StringCellValue) : DateTime.Now,
                    RecivedHour = sheet.GetRow(row).GetCell(6).StringCellValue,
                    Weigh = weigh,
                    Status = sheet.GetRow(row).GetCell(8).StringCellValue,
                    Notes = sheet.GetRow(row).GetCell(9).StringCellValue,
                    ItemName = sheet.GetRow(row).GetCell(10).StringCellValue,
                    ItemCategoryName = sheet.GetRow(row).GetCell(11).StringCellValue,
                    ItemQuantity = quantity,
                    ItemNotes = sheet.GetRow(row).GetCell(13).StringCellValue,
                };
                lstData.Add(item);
            }
            return lstData;
        }
        public List<AgencyPackage> getAgencyPackage(List<ShipmentImport> lst, Shipment shipment)
        {
            List<AgencyPackage> list = new List<AgencyPackage>();
            foreach (var _item in lst)
            {
                var listPackage = lst.Where(n => n.TrackingNumber.Trim() == _item.TrackingNumber.Trim());
                foreach (var pack in listPackage)
                {
                    if (list.Where(n => n.TrackingCode == pack.TrackingNumber.Trim()).Count() > 0) { break; }
                    var packId = Guid.NewGuid();
                    var delivery = getDevilery(pack.DeliveryName);
                    AgencyPackage package = new AgencyPackage()
                    {
                        AgencyId = user.Agency.Id,
                        CreatedAt = DateTime.Now,
                        Id = packId,
                        CreatedBy = user.Staff.UserName,
                        DeliveryId = delivery.Id,
                        DeliveryName = delivery.Name,
                        IsDeclare = false,
                        IsEdit = false,
                        Notes = pack.Notes,
                        ReceivedDate = pack.RecivedDate,
                        ReceivedHour = pack.RecivedHour.Trim(),
                        SentDate = pack.SendDate,
                        SendHour = pack.RecivedHour.Trim(),
                        ShipmentId = shipment.Id,
                        StatusId = int.Parse(getStatus(pack.Status)),
                        TrackingCode = pack.TrackingNumber.Trim(),
                        TrackingStatusId = 2,
                        Weigh = pack.Weigh,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    list.Add(package);
                    break;
                }
            }
            return list;
        }
        public List<AgencyPackageItem> getAgencyPackageItems(List<ShipmentImport> lst, Shipment shipment, AgencyPackage package)
        {
            List<AgencyPackageItem> list = new List<AgencyPackageItem>();
            var listPackage = lst.Where(n => n.TrackingNumber == package.TrackingCode);
            foreach (var pack in listPackage)
            {
                var packId = Guid.NewGuid();
                var cate = getCategory(pack.ItemCategoryName);
                if (cate != null)
                {
                    AgencyPackageItem item = new AgencyPackageItem()
                    {
                        AgencyPackageId = package.Id,
                        CreatedAt = DateTime.Now,
                        Id = packId,
                        CreatedBy = user.Staff.UserName,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        CategoryId = cate.Id,
                        ItemCategory = cate.NameEN,
                        ItemCode = "",
                        ItemName = pack.ItemName,
                        ItemNotes = pack.ItemNotes,
                        ItemQuantity = pack.ItemQuantity,
                        ItemUrl = "",
                        ItemImg = ""
                    };
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion
        public void ImportNew(ISheet sheet, Shipment shipment)
        {
            #region Delete all package & package Items
            var packages = db.AgencyPackages.Where(n => n.ShipmentId == shipment.Id);
            foreach (var item in packages)
            {
                var packageItems = item.AgencyPackageItems;
                db.AgencyPackageItems.RemoveRange(packageItems);
                db.AgencyPackages.Remove(item);
            }
            db.SaveChanges();
            #endregion
            #region Insert New
            //get data
            List<ShipmentImport> lstData = readImportShipment(sheet);
            //Danh sach Package
            List<AgencyPackage> lstPackage = getAgencyPackage(lstData, shipment);
            foreach (var item in lstPackage)
            {
                db.AgencyPackages.Add(item);
                List<AgencyPackageItem> lstPackageItems = getAgencyPackageItems(lstData, shipment, item);
                foreach (var pitem in lstPackageItems)
                {
                    db.AgencyPackageItems.Add(pitem);
                }
            }
            #endregion
            db.SaveChanges();
        }
        public void ImportOverwrite(ISheet sheet, Shipment shipment)
        {
            #region Insert New
            //get data
            List<ShipmentImport> lstData = readImportShipment(sheet);
            //Danh sach Package
            List<AgencyPackage> lstPackage = getAgencyPackage(lstData, shipment);
            foreach (var item in lstPackage)
            {
                db.AgencyPackages.Add(item);
                List<AgencyPackageItem> lstPackageItems = getAgencyPackageItems(lstData, shipment, item);
                foreach (var pitem in lstPackageItems)
                {
                    db.AgencyPackageItems.Add(pitem);
                }
            }
            #endregion
            db.SaveChanges();
        }
        public DeliveryCom getDevilery(string name)
        {
            name = name.Trim();
            return db.DeliveryComs.SingleOrDefault(n => n.Name == name);
        }
        public WareHouseCategory getCategory(string name)
        {
            try
            {
                name = name.Trim();
                return db.WareHouseCategories.ToList().Single(n => n.NameEN.Trim() == name);
            }
            catch
            {
                return db.WareHouseCategories.Find(1);
            }
        }
        public string getStatus(string name)
        {
            string status = "2";
            try
            {
                status = StatusUtils.GetStatus(1).Where(n => n.Text.Trim().ToLower() == name.Trim().ToLower()).ToList()[0].Value;
            }
            catch { }
            return status;
        }
        #region Import StoreJP
        [HttpGet]
        public ActionResult StorageJP(Guid id)
        {
            var StorageJP = db.StorageJPs.Find(id);
            return View(StorageJP);
        }
        public ActionResult StorageJPFull()
        {
            return View();
        }
        public int getIndexColumn(ISheet sheet,string columnName)
        {
            try
            {
                var data_row = sheet.GetRow(2);
                for (int i = 1; i <= data_row.Cells.Count; i++)
                {
                    foreach (var item in columnName.Split(','))
                    {
                        if (data_row.GetCell(i).StringCellValue.Replace(" ", "").ToLower().Contains(item.Replace(" ", "").ToLower()))
                        {
                            return i;
                        }
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        [HttpPost]
        public ActionResult StorageJP(Guid id, string fileName = "")
        {
            var StorageJP = db.StorageJPs.Find(id);
            //upload file
            HttpPostedFileBase file = Request.Files[0];
            fileName = file.FileName.DoiTenFile();
            file.SaveAs(Server.MapPath("~/Uploads/StorageJP/" + fileName));
            //read file
           
            // Opening the Excel template... 
            FileStream fs = new FileStream(Server.MapPath("~/Uploads/StorageJP/" + fileName), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            ISheet sheet = null;
            if (fileName.Contains(".xlsx"))
            {
                XSSFWorkbook templateWorkbook = new XSSFWorkbook(Server.MapPath("~/Uploads/StorageJP/" + fileName));
                sheet = templateWorkbook.GetSheet("例");
            }
            else if (fileName.Contains(".xls"))
            {
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                sheet = templateWorkbook.GetSheet("例");
            }
            //read excel
            WebsiteHelpers websiteHelper = new WebsiteHelpers();
            List<StorageItemJP> lstStorageItemJP = new List<StorageItemJP>();
            List<TrackingDetail> lstTrackingDetail = new List<TrackingDetail>();
            Guid IdTracking = Guid.NewGuid();
            #region read excel

            //check column

            string urlReport = ConfigurationManager.AppSettings["url_report_storejp"];
            for (int row = 4; row <= sheet.LastRowNum; row++)
            {
                double price=0;
                try
                {
                    price = sheet.GetRow(row).GetCell(getIndexColumn(sheet,"GIA")).NumericCellValue;
                }
                catch (Exception ex) {  }
                string subcode = "";
                try
                {
                    subcode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "SO KIEN")).StringCellValue;
                    try
                    {
                        subcode = int.Parse(subcode).ToString("00");
                    }
                    catch (Exception ex) { }
                }
                catch (Exception ex) { subcode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "SO KIEN")).NumericCellValue.ToString("00"); }
                if (price == 0&& subcode!="")
                {
                    IdTracking = Guid.NewGuid();
                    TrackingDetail detail = new TrackingDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Id = IdTracking,
                        StoregeJPId = StorageJP.Id,
                        Weigh = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "KG")).NumericCellValue,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        StatusId = StorageJP.StatusId,
                        TrackingSubCode = subcode
                    };
                    lstTrackingDetail.Add(detail);
                }
                else if (price !=0)
                {
                    string namejp = "";
                    string linkweb = ""; string ImageUrl = "";
                    string ImageBase64 = null;
                    try
                    {
                        namejp = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "TEN HANG_JP")).CellFormula;
                        namejp = namejp.Replace("HYPERLINK(", "").Replace(")", "").Replace("\"", "");
                        linkweb = namejp.Split(',')[0];
                        namejp = namejp.Split(',')[1];
                    }
                    catch (Exception ex)
                    {
                        namejp = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "TEN HANG_JP")).StringCellValue;
                        linkweb = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "WEBLINK")).StringCellValue;
                    }
                    if (linkweb.ToLower().Contains(urlReport.ToLower()))
                    {
                        string idItem = linkweb.Split('/')[linkweb.Split('/').Length-1];
                        try
                        {
                            var item = db.StorageItemJPs.Find(Guid.Parse(idItem));
                            ImageBase64 = item.ImageBase64;
                            linkweb = item.LinkWeb;
                            ImageUrl = item.ImageLinkWeb;
                        }
                        catch (Exception ex) { }
                    }
                    else
                    {
                        try
                        {
                            ImageUrl = websiteHelper.GetImage(linkweb);
                            ImageBase64 = ImageUtils.Images(ImageUrl);
                        }
                        catch (Exception ex) { }
                    }
                    string component = "", material = "", JanCode ="";
                    try
                    {
                        component = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "THANH PHAN")).StringCellValue;
                    }
                    catch (Exception ex) { }
                    try
                    {
                        material = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "CHAT LIEU")).StringCellValue;
                    }
                    catch (Exception ex) { }
                    try
                    {
                        JanCode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Jancode,mavach")).StringCellValue;
                    }
                    catch (Exception ex) { JanCode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Jancode,mavach")).NumericCellValue+""; }
                    StorageItemJP storejpItem = new StorageItemJP()
                    {
                        Amount = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "THANH TIEN")).NumericCellValue,
                        CategoryId = getCategory(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "LOAI HANG")).StringCellValue).Id,
                        CategoryName = getCategory(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "LOAI HANG")).StringCellValue).Name,
                        Component = component,
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ImageBase64= ImageBase64,
                        ImageLinkWeb = ImageUrl,
                        JanCode = JanCode,
                        Material = material,
                        LinkWeb = linkweb,
                        MadeIn = getMadeIn(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "XUAT XU")).StringCellValue).Id + "",
                        NameEN = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "TEN HANG_EN")).StringCellValue,
                        NameJP = namejp,
                        PriceTax = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "GIA")).NumericCellValue,
                        ProductCode = "",
                        Quantity = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "SO LUONG")).NumericCellValue,
                        UpdatedAt = DateTime.Now,
                        StoregeJPId = StorageJP.Id,
                        UpdatedBy = user.Staff.UserName,
                        TrackingDetailId = IdTracking
                    };
                    lstStorageItemJP.Add(storejpItem);
                }

            }
            #endregion

            //delete all data form db
            var details = StorageJP.TrackingDetails.ToList();
            foreach (var item in details)
            {
                var stores = item.StorageItemJPs.ToList();
                foreach (var st in stores)
                {
                    db.StorageItemJPs.Remove(st);
                }
                db.TrackingDetails.Remove(item);
            }
            //Insert database
            foreach (var item in lstTrackingDetail)
            {
                db.TrackingDetails.Add(item);
                foreach (var store in lstStorageItemJP.Where(n=>n.TrackingDetailId==item.Id))
                {
                    db.StorageItemJPs.Add(store); db.SaveChanges();
                }
            }
            db.SaveChanges();
            return View();
        }
        #endregion
        public Country getMadeIn(string name)
        {
            try
            {
                name = name.Trim();
                return db.Countries.ToList().Single(n => n.Name.Trim() == name);
            }
            catch
            {
                return db.Countries.Find(108);
            }
        }

        [HttpPost]
        public ActionResult StorageJPFull(string fileName="")
        {
            //upload file
            HttpPostedFileBase file = Request.Files[0];
            fileName = file.FileName.DoiTenFile();
            file.SaveAs(Server.MapPath("~/Uploads/StorageJP/" + fileName));
            // Opening the Excel template... 
            FileStream fs = new FileStream(Server.MapPath("~/Uploads/StorageJP/" + fileName), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            ISheet sheet = null;
            if (fileName.Contains(".xlsx"))
            {
                XSSFWorkbook templateWorkbook = new XSSFWorkbook(Server.MapPath("~/Uploads/StorageJP/" + fileName));
                sheet = templateWorkbook.GetSheet("例");
            }
            else if (fileName.Contains(".xls"))
            {
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                sheet = templateWorkbook.GetSheet("例");
            }
            //read excel
            WebsiteHelpers websiteHelper = new WebsiteHelpers();
            List<StorageJP> lstStorageJP = new List<StorageJP>();
            List<StorageItemJP> lstStorageItemJP = new List<StorageItemJP>();
            List<TrackingDetail> lstTrackingDetail = new List<TrackingDetail>();
            Guid IdTracking = Guid.NewGuid();
            #region read excel

            //check column
            string nameJP = "";
            string trackingCode = "";
            string urlReport = ConfigurationManager.AppSettings["url_report_storejp"];
            Guid storeId = Guid.NewGuid();
            for (int row = 3; row <= sheet.LastRowNum; row++)
            {
                nameJP = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Tên Hàng Jp(*)")).StringCellValue;
                trackingCode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Mã Tracking(*)")).StringCellValue;
                if (nameJP == "" && trackingCode == "") { break; }
                if (trackingCode != "" && trackingCode != null)
                {
                    IdTracking = Guid.NewGuid();
                    string size = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Size")).StringCellValue;
                    size = size.Replace("cm","").Trim();
                    double sizeInput = double.Parse(size) / 3;
                    sizeInput = sizeInput - Math.Round(sizeInput / 3);
                    double dai = double.Parse(size) - 2 * sizeInput;
                    DateTime revice = new DateTime();
                    try
                    {
                        revice = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Ngày nhận")).DateCellValue;
                    }
                    catch {
                        try
                        {
                            revice = DateTime.ParseExact(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Ngày nhận")).StringCellValue,"yyyy-MM-dd",null);
                        }
                        catch { }
                    }
                    if (lstStorageJP.Where(n => n.TrackingCode == trackingCode.Trim()).Count() == 0)
                    {
                        storeId = Guid.NewGuid();
                        StorageJP store = new Models.StorageJP()
                        {
                            Id = storeId,
                            AgencyId = user.Agency.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            DeliveryAddress = "",
                            StatusId = 6,
                            TrackingCode = trackingCode.Trim(),
                            ReceivedDate = revice,
                            Size = "size-input",
                            SizeInput = dai + " " + sizeInput + " " + sizeInput,
                            Notes = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Ghi chú")).StringCellValue,
                            ReceivedHour = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Giờ nhận")).StringCellValue,
                            Weigh = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Số Kg")).NumericCellValue
                        };
                        lstStorageJP.Add(store);
                    }
                    TrackingDetail detail = new TrackingDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Id = IdTracking,
                        StoregeJPId = storeId,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        TrackingSubCode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Mã Tách Kiện(*)")).StringCellValue
                    };
                    lstTrackingDetail.Add(detail);
                }
                else
                {
                    double price = 0;
                    try
                    {
                        price = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Giá(*)")).NumericCellValue;
                    }
                    catch { }
                    string namejp = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Tên Hàng Jp(*)")).StringCellValue;
                    string nameen = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Tên Hàng En")).StringCellValue;
                    string linkweb = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Weblink")).StringCellValue;
                    string Image = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Hình ảnh")).StringCellValue;
                    string base64Image = "";
                    StorageItemJP storejpItem = new StorageItemJP()
                    {
                        Amount = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Thành tiền")).NumericCellValue,
                        CategoryId = getCategory(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Loại Hàng")).StringCellValue).Id,
                        CategoryName = getCategory(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Loại Hàng")).StringCellValue).Name,
                        Component = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Thành Phần")).StringCellValue,
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ImageBase64 = base64Image,
                        Image = Image,
                        ImageLinkWeb=Image,
                        JanCode = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Jan Code")).StringCellValue,
                        Material = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Chất liệu")).StringCellValue,
                        LinkWeb = linkweb,
                        MadeIn = getMadeIn(sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Xuất xứ(*)")).StringCellValue).Id + "",
                        NameEN = nameen,
                        NameJP = namejp,
                        PriceTax = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Giá(*)")).NumericCellValue,
                        ProductCode = "",
                        Quantity = sheet.GetRow(row).GetCell(getIndexColumn(sheet, "Số Lượng(*)")).NumericCellValue,
                        UpdatedAt = DateTime.Now,
                        StoregeJPId = storeId,
                        UpdatedBy = user.Staff.UserName,
                        TrackingDetailId = IdTracking
                    };
                    lstStorageItemJP.Add(storejpItem);
                }
            }
            #endregion

            //delete all data form db
            var stores = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id);
            foreach (var items in stores)
            {
                if(PageUtils.Status(items.Id).Value=="1")
                {
                    var details = items.TrackingDetails.ToList();
                    foreach (var item in details)
                    {
                        var storeItems = item.StorageItemJPs.ToList();
                        foreach (var st in storeItems)
                        {
                            db.StorageItemJPs.Remove(st);
                        }
                        db.TrackingDetails.Remove(item);
                    }
                    db.StorageJPs.Remove(items);
                }
            }
            db.SaveChanges();
            //Insert database
            foreach (var items in lstStorageJP)
            {
                db.StorageJPs.Add(items);
                var details = lstTrackingDetail.Where(n => n.StoregeJPId == items.Id);
                foreach (var item in details)
                {
                    db.TrackingDetails.Add(item);
                    var itemstores = lstStorageItemJP.Where(n => n.TrackingDetailId == item.Id);
                    foreach (var store in itemstores)
                    {
                        db.StorageItemJPs.Add(store);
                    }
                }
            }
            db.SaveChanges();
            return View();
        }
    }

}