using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Helpers
{
    public class PageUtils
    {
        WareHouseJPDB db = new WareHouseJPDB(); UserPage user = new UserPage();
        public string Category(int id)
        {
            try { return db.WareHouseCategories.Find(id).NameEN; } catch { return ""; }
        }
        public string ProductCode(int id)
        {
            try { return db.ProductTypes.Find(id).Name; } catch { return ""; }
        }
        public string OnRoad(string trackingCode)
        {
            try { return (db.StorageJPs.Where(n => n.TrackingCode.ToLower() == trackingCode.ToLower()).Count() == 0) ? "Kiện đã nhận" : "Kiện đang đến"; } catch { return "Kiện đã nhận"; }
        }
        public string Size(string id)
        {
            try { return db.SizeTables.Find(int.Parse(id)).Name; } catch { return ""; }
        }
        public string DisplayAddress(string s)
        {
            string result = "";
            try
            {
                if (s != null)
                {
                    List<String> str = new List<string>();
                    foreach (var item in s.Split(','))
                    {
                        var add = db.DeliveryAddresses.Find(Guid.Parse(item.Trim()));
                        str.Add(add.AgencyId + "-" + add.DeliveryCode);
                    }
                    if (str.Count > 0) result = String.Join(",", str);
                }
            }
            catch { }
            return result;
        }
        public string MadeIn(int id)
        {
            try { return db.Countries.Find(id).Name; } catch { return ""; }
        }
        public string Status(int id)
        {
            try { return db.StatusWareHouses.Find(id).Name; } catch { return ""; }
        }
        public string PackageStatus(int id, int step = 0)
        {
            try { return StatusUtils.GetStatus(step).Where(n => n.Value == id + "").First().Text; } catch { return ""; }
        }
        public bool AlowEditExportGood(ExportGood model)
        {
            try
            {
                if (db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.ExportGoodId == model.Id).Count() == 0)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
        public string AlowEditStoreJP(StorageJP model)
        {
            try
            {
                //kiem tra co xuat kho khong?
                string s = "";
                var trackingDetails = model.TrackingDetails;
                var exportDetails = db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == user.Agency.Id);
                foreach (var item in trackingDetails)
                {
                    if (exportDetails.Where(n => n.TrackingDetailId == item.Id).Count() > 0) { s = "Đang có liên kết với kiện xuất kho"; break; }
                }
                //kiem tra co tra hang khong?
                var returnPackages = db.ReturnDetails.Where(n => n.PackageReturn.AgencyId == user.Agency.Id);
                foreach (var item in trackingDetails)
                {
                    if (returnPackages.Where(n => n.TrackingDetailId == item.Id).Count() > 0)
                    {
                        s = (s == "") ? "Đang có liên kết với kiện trả hàng" : "Đang có liên kết với kiện xuất kho, trả hàng";
                        break;
                    }
                }
                return s;
            }
            catch { }
            return "";
        }
        public string keyJP = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Deny/KeywordJPs.txt"));
        public string keyEN = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Deny/KeywordENs.txt"));
        //Luu kho
        public string CheckItem(StorageItemJP store)
        {
            if (store.NameEN == null || store.NameEN == "") { return "item-deny"; }
            if (store.NameJP == null || store.NameJP == "") { return "item-deny"; }
            if (store.JanCode == "" || store.JanCode == null) { return "item-deny"; }
            if (store.JanCode.StartsWith("4") == false) { return "item-deny"; }
            foreach (var item in keyJP.ToUpper().Split(','))
            {
                if (store.NameJP.ToUpper().Contains(item) || store.NameEN.ToUpper().Contains(item)) { return "item-deny"; }
            }
            foreach (var item in keyEN.ToUpper().Split(','))
            {
                if (store.NameEN.ToUpper().Contains(item) || store.NameJP.ToUpper().Contains(item)) { return "item-deny"; }
            }
            if (store.Quantity <= 0 || store.Quantity == null) { return "item-deny"; }
            if (store.PriceTax <= 0 || store.PriceTax == null || store.PriceTax < 50) { return "item-deny"; }
            if (store.LinkWeb == "" || store.LinkWeb == null) { return "item-deny"; }
            if (store.ImageBase64 == "" || store.ImageBase64 == null) { return "item-deny"; }
            return "";
        }
        public int CheckItemStoreJPCount(StorageJP items)
        {
            int count = 0;
            foreach (var item in items.StorageItemJPs)
            {
                count += CheckItemCount(item);
            }
            return count;
        }
        public int CheckItemCount(StorageItemJP store)
        {
            int i = 0;
            if (store.NameEN == null) { store.NameEN = ""; }

            if (store.JanCode != null && store.JanCode != "")
            {
                if (store.JanCode.StartsWith("4") == false) { i += 1; }
                else
                {
                    if (store.JanCode.Length != 13) { i += 1; }
                    else
                    {
                        if (store.MadeIn != null && store.MadeIn == "108")
                        {

                            if (store.JanCode.StartsWith("450") == false)
                                if (store.JanCode.StartsWith("451") == false)
                                    if (store.JanCode.StartsWith("452") == false)
                                        if (store.JanCode.StartsWith("453") == false)
                                            if (store.JanCode.StartsWith("454") == false)
                                                if (store.JanCode.StartsWith("455") == false)
                                                    if (store.JanCode.StartsWith("456") == false)
                                                        if (store.JanCode.StartsWith("457") == false)
                                                            if (store.JanCode.StartsWith("458") == false)
                                                                if (store.JanCode.StartsWith("459") == false)
                                                                    if (store.JanCode.StartsWith("490") == false)
                                                                        if (store.JanCode.StartsWith("491") == false)
                                                                            if (store.JanCode.StartsWith("492") == false)
                                                                                if (store.JanCode.StartsWith("493") == false)
                                                                                    if (store.JanCode.StartsWith("494") == false)
                                                                                        if (store.JanCode.StartsWith("495") == false)
                                                                                            if (store.JanCode.StartsWith("496") == false)
                                                                                                if (store.JanCode.StartsWith("497") == false)
                                                                                                    if (store.JanCode.StartsWith("498") == false)
                                                                                                        if (store.JanCode.StartsWith("499") == false) { i += 1; }

                        }
                    }
                }
            }

            if (store.NameJP == null || store.NameJP == "") { i += 1; }
            else
            {
                foreach (var item in keyJP.ToUpper().Split(','))
                {
                    if (store.NameJP.ToUpper().Contains(item)) { i += 1; break; }
                }
            }
            if (store.NameEN == null || store.NameEN == "") { i += 1; }
            else
            {
                foreach (var item in keyJP.ToUpper().Split(','))
                {
                    if (store.NameEN.ToUpper().Contains(item)) { i += 1; break; }
                }
            }
            if (store.Quantity <= 0 || store.Quantity == null) { i += 1; }
            if (store.PriceTax <= 0 || store.PriceTax == null || store.PriceTax < 50) { i += 1; }
            if (store.LinkWeb == "" || store.LinkWeb == null) { i += 1; }
            if (store.ImageBase64 == "" || store.ImageBase64 == null) { i += 1; }
            if (store.CategoryId == null) { i += 1; }
            if (store.MadeIn == null || store.MadeIn == "") { i += 1; }
            return i;
        }
        public string CheckLinkWeb(StorageItemJP store)
        {
            if (store.LinkWeb == "" || store.LinkWeb == null) { return "item-deny"; }
            return "";
        }
        public string CheckImageBase64(StorageItemJP store)
        {
            if (store.ImageBase64 == "" || store.ImageBase64 == null) { return "item-deny"; }
            return "";
        }
        public string CheckPriceTax(StorageItemJP store)
        {
            if (store.PriceTax <= 0 || store.PriceTax == null || store.PriceTax < 50) { return "item-deny"; }
            return "";
        }
        public string CheckQuantity(StorageItemJP store)
        {
            if (store.Quantity <= 0 || store.Quantity == null) { return "item-deny"; }
            return "";
        }
        public string CheckJanCode(StorageItemJP store)
        {
            if (store.JanCode != null && store.JanCode != "")
            {
                if (store.JanCode.StartsWith("4") == false) { return "item-deny"; }
                else
                {
                    if (store.JanCode.Length != 13) { return "item-deny"; }
                    else
                    {
                        if (store.MadeIn != null && store.MadeIn == "108")
                        {
                            if (store.JanCode.StartsWith("450") == false)
                                if (store.JanCode.StartsWith("451") == false)
                                    if (store.JanCode.StartsWith("452") == false)
                                        if (store.JanCode.StartsWith("453") == false)
                                            if (store.JanCode.StartsWith("454") == false)
                                                if (store.JanCode.StartsWith("455") == false)
                                                    if (store.JanCode.StartsWith("456") == false)
                                                        if (store.JanCode.StartsWith("457") == false)
                                                            if (store.JanCode.StartsWith("458") == false)
                                                                if (store.JanCode.StartsWith("459") == false)
                                                                    if (store.JanCode.StartsWith("490") == false)
                                                                        if (store.JanCode.StartsWith("491") == false)
                                                                            if (store.JanCode.StartsWith("492") == false)
                                                                                if (store.JanCode.StartsWith("493") == false)
                                                                                    if (store.JanCode.StartsWith("494") == false)
                                                                                        if (store.JanCode.StartsWith("495") == false)
                                                                                            if (store.JanCode.StartsWith("496") == false)
                                                                                                if (store.JanCode.StartsWith("497") == false)
                                                                                                    if (store.JanCode.StartsWith("498") == false)
                                                                                                        if (store.JanCode.StartsWith("499") == false) { return "item-deny"; }
                        }
                    }
                }
            }
            return "";
        }
        public string CheckNameJP(StorageItemJP store)
        {
            if (store.NameJP == null || store.NameJP == "") { return "item-deny"; }
            else
            {
                foreach (var item in keyJP.ToUpper().Split(','))
                {
                    if (store.NameJP.ToUpper().Contains(item)) { return "item-deny"; }
                }
            }
            return "";
        }
        public string CheckNameEN(StorageItemJP store)
        {
            if (store.NameEN == null || store.NameEN == "") { return "item-deny"; }
            else
            {
                foreach (var item in keyEN.ToUpper().Split(','))
                {
                    if (store.NameEN.ToUpper().Contains(item)) { return "item-deny"; }
                }
            }
            return "";
        }
        public string CheckCategory(StorageItemJP store)
        {
            if (store.CategoryId == null) { return "item-deny"; }
            return "";
        }
        public string CheckMadeIn(StorageItemJP store)
        {
            if (store.MadeIn == null || store.MadeIn == "") { return "item-deny"; }
            return "";
        }
        //database
        public string CheckItem(WareHouseItem store)
        {
            if (store.NameEN == null) { store.NameEN = ""; }
            if (store.NameJP == null) { store.NameJP = ""; }
            if (store.JanCode != null && store.JanCode != "")
            {
                if (store.JanCode.StartsWith("4") == false) { return "item-deny"; }
                else
                {
                    if (store.JanCode.Length != 13) { return "item-deny"; }
                }
            }
            foreach (var item in keyJP.ToUpper().Split(','))
            {
                if (store.NameJP.ToUpper().Contains(item) || store.NameEN.ToUpper().Contains(item)) { return "item-deny"; }
            }
            foreach (var item in keyEN.ToUpper().Split(','))
            {
                if (store.NameEN.ToUpper().Contains(item) || store.NameJP.ToUpper().Contains(item)) { return "item-deny"; }
            }
            if (store.Quantity <= 0 || store.Quantity == null) { return "item-deny"; }
            if (store.PriceTax <= 0 || store.PriceTax == null || store.PriceTax < 50) { return "item-deny"; }
            if (store.LinkWeb == "" || store.LinkWeb == null) { return "item-deny"; }
            if (store.ImageBase64 == "" || store.ImageBase64 == null) { return "item-deny"; }
            return "";
        }
        public string DisplaySize(double? weigh, string sizeInput, int? SizeId, string Size)
        {
            if (weigh == null || weigh == 0) { return ""; }
            if (Size == null || Size == "size-mau")
            {
                try
                {
                    return db.SizeTables.Find(SizeId).Name;
                }
                catch { return ""; }
            }
            else
            {
                try
                {
                    var arrSize = sizeInput.Split(' ', ',');
                    return (double.Parse(arrSize[0]) + double.Parse(arrSize[1]) + double.Parse(arrSize[2])) + "cm";
                }
                catch { return ""; }
            }
        }
        public double DisplaySizeIndex(double? weigh, string sizeInput, int? SizeId, string Size)
        {
            if (weigh == null || weigh == 0) { return 0; }
            string size = "0";
            if (Size == null || Size == "size-mau")
            {
                size = db.SizeTables.Find(SizeId).Name;
                size = size.Replace("cm", "").Trim();
            }
            else
            {
                try
                {
                    var arrSize = sizeInput.Split(' ', ',');
                    size = (double.Parse(arrSize[0]) + double.Parse(arrSize[1]) + double.Parse(arrSize[2])) + "";
                }
                catch { return 0; }
            }
            return double.Parse(size);
        }
        //export - xuat kho
        public SelectListItem Status(Guid? StorejpId)
        {
            var result = new SelectListItem()
            {
                Value = "1",
                Text = "Chưa trộn"
            };
            try
            {
                var tracking = db.StorageJPs.Find(StorejpId);
                int i = 0;
                foreach (var item in tracking.TrackingDetails)
                {
                    if (db.ExportGoodDetails.Where(n => n.TrackingDetailId == item.Id).Count() > 0)
                    {
                        i++;
                    }
                }
                if (i > 0 && i == tracking.TrackingDetails.Count)
                {
                    result = new SelectListItem()
                    {
                        Value = "3",
                        Text = "Đã trộn"
                    };
                }
                else if (i > 0 && i < tracking.TrackingDetails.Count)
                {
                    result = new SelectListItem()
                    {
                        Value = "2",
                        Text = "Đang trộn"
                    };
                }
                return result;
            }
            catch (Exception ex)
            {

                return result;
            }
        }

        //Storage - luu kho
        public SelectListItem StatusJP(StorageJP store, bool flag = false)
        {
            var result = new SelectListItem()
            {
                Value = "1",
                Text = "<ul class='export-return'><li></li></ul>"
            };
            try
            {
                int i = 0;
                foreach (var item in store.TrackingDetails.Where(n => n.TrackingSubCode != "21"))
                {
                    if (db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == user.Agency.Id).Where(n => n.TrackingDetailId == item.Id).Count() > 0)
                    {
                        i++;
                    }
                }
                var export = store.TrackingDetails.Where(n => n.TrackingSubCode != "21").Count();
                return result = new SelectListItem()
                {
                    Value = "2",
                    Text = "<ul class='export-return'><li>" + i + "/" + export + "</li></ul>"
                };
            }
            catch
            {
                return result;
            }
        }
        public SelectListItem StatusReturnJP(StorageJP store, bool flag = false)
        {
            var result = new SelectListItem()
            {
                Value = "1",
                Text = "<ul class='export-return'><li></li></ul>"
            };
            try
            {
                int j = 0;
                //dem tracking co trong kien tra hang hay chua
                foreach (var item in store.TrackingDetails.Where(n => n.TrackingSubCode == "21"))
                {
                    if (db.ReturnDetails.Where(n => n.PackageReturn.AgencyId == user.Agency.Id).Where(n => n.TrackingDetailId == item.Id).Count() > 0)
                    {
                        j++;
                    }
                }
                var returnPackage = store.TrackingDetails.Where(n => n.TrackingSubCode == "21").Count();
                return result = new SelectListItem()
                {
                    Value = "2",
                    Text = "<ul class='export-return'><li>" + j + "/" + returnPackage + "</li></ul>"
                };
            }
            catch
            {
                return result;
            }
        }
        public bool IsExistExport(Guid? TrackingDetailId, string AgencyId)
        {
            if (db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == AgencyId).Where(n => n.TrackingDetailId == TrackingDetailId).Count() > 0)
            {
                return true;
            }
            return false;
        }
        public string Address(StorageJP item)
        {
            if (item.DeliveryAddress == null || item.DeliveryAddress == "") { return ""; }
            else
            {
                string id = item.DeliveryAddress.Split(',')[0];
                if (id != "")
                {
                    var gid = Guid.Parse(id);
                    return db.DeliveryAddresses.Find(gid).DeliveryCode;
                }
                else { return ""; }
            }
        }
        //check update xuat kho
        public string IsUpdateExport(ExportGood export)
        {
            try
            {
                string s = "";
                if (export.StatusId == 10)
                {
                    if (export.Weigh == null || export.Weigh == 0)
                    {
                        s = "Vui lòng cập nhật khối lượng";
                    }
                    if (export.SizeTableId == null && export.SizeInput == null)
                    {
                        s = (s == "" ? "Vui lòng cập nhật kích thước" : s + ",kích thước");
                    }
                    if (export.ExportGoodDetails.Count == 0)
                    {
                        s = (s == "" ? "Nội dung kiện hàng đang trống." : s + ",nội dung kiện hàng");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật khối lượng, kích thước...";
        }
        //check update chuyến hàng
        public string IsUpdateShipping(Shipping shipping)
        {
            try
            {
                string s = "";
                if (shipping.StatusId == 14)
                {
                    //kiem tra co kien hang chua
                    var itemListShipping = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id && n.ShippingHAWB.ShippingId == shipping.Id).OrderByDescending(n => n.CreatedAt);
                    if (itemListShipping.Count()==0)
                    {
                        s = "Kiện hàng đang rỗng";
                    }
                    //kiem tra da booking chua
                    var lstBookingHistory = db.Shippings.Find(shipping.Id).ShippingBookings.OrderByDescending(n => n.CreatedAt);
                    var Status = lstBookingHistory == null || lstBookingHistory.Count() == 0 ? 11 : lstBookingHistory.First().StatusId;
                    if (lstBookingHistory.Count() == 0)
                    {
                        s = (s == "" ? "Chưa có lịch sử booking" : s + ", chưa booking");
                    }
                    else if(Status==11)
                    {
                        s = (s == "" ? "Chưa có lịch sử booking" : s + ", chưa booking");
                    }
                    //kiem tra da nhap pre-ad chua
                    var ShippingHistories = db.ShippingHistories.Where(n => n.AgencyId == user.Agency.Id && n.ShippingId == shipping.Id).OrderByDescending(n => n.CreatedAt);
                    if (ShippingHistories.Count() == 0)
                    {
                        s = (s == "" ? "Chưa nhập thông tin pre-ad" : s + ", chưa có pre-ad");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật thông tin kiện hàng, thông tin booking, thông tin pre-ad.";
        }
        public string IsUpdatePackageReturn(PackageReturn export)
        {
            try
            {
                string s = "";
                if (export.StatusId == 16)
                {
                    if (export.ReturnImage == null || export.ReturnImage == "")
                    {
                        s = "Vui lòng cập nhật hình ảnh";
                    }
                    if (db.ReturnDetails.Where(n=>n.PackageReturnId==export.Id).Count() == 0)
                    {
                        s = (s == "" ? "Nội dung kiện hàng đang trống." : s + ", nội dung kiện hàng");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật hình ảnh, nội dung kiện hàng";
        }
        public string IsPackageReturn(PackageReturn export)
        {
            try
            {
                string s = "";
                if (export.StatusId != 16)
                {
                    if (export.ReturnImage == null || export.ReturnImage == "")
                    {
                        s = "Vui lòng cập nhật hình ảnh";
                    }
                    if (db.ReturnDetails.Where(n => n.PackageReturnId == export.Id).Count() == 0)
                    {
                        s = (s == "" ? "Nội dung kiện hàng đang trống." : s + ", nội dung kiện hàng");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật hình ảnh, nội dung kiện hàng";
        }
        public string IsDisplayExport(Guid? Id)
        {
            try
            {
                var export = db.ExportGoods.Find(Id);
                string s = "";
                if (export.StatusId == 9)
                {
                    if (export.Weigh == null || export.Weigh == 0)
                    {
                        s = "Vui lòng cập nhật khối lượng";
                    }
                    if (export.SizeTableId == null && export.SizeInput == null)
                    {
                        s = (s == "" ? "Vui lòng cập nhật kích thước" : s + ",kích thước");
                    }
                    if (export.ExportGoodDetails.Count == 0)
                    {
                        s = (s == "" ? "Nội dung kiện hàng đang trống." : s + ",nội dung kiện hàng");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật khối lượng, kích thước...";
        }
        //check update luu kho
        public string IsUpdateStoreJP(StorageJP store)
        {
            try
            {
                string s = "";
                if (store.StatusId == 8)
                {
                    if (store.Weigh == null || store.Weigh == 0)
                    {
                        s = "Vui lòng cập nhật khối lượng";
                    }
                    if (store.SizeTableId == null && store.SizeInput == null)
                    {
                        s = (s == "" ? "Vui lòng cập nhật kích thước" : s + ",kích thước");
                    }
                    if (store.Image == null || store.Image == "")
                    {
                        s = (s == "" ? "Vui lòng cập nhật hình ảnh" : s + ",hình ảnh");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật khối lượng, kích thước, hình ảnh.";
        }
        public string IsDisplayStoreJP(Guid? Id)
        {
            try
            {
                var export = db.StorageJPs.Find(Id);
                string s = "";
                if (export.StatusId != 8)
                {
                    if (export.Weigh == null || export.Weigh == 0)
                    {
                        s = "Vui lòng cập nhật khối lượng";
                    }
                    if (export.SizeTableId == null && export.SizeInput == null)
                    {
                        s = (s == "" ? "Vui lòng cập nhật kích thước" : s + ",kích thước");
                    }
                    if (export.Image == null || export.Image == "")
                    {
                        s = (s == "" ? "Vui lòng cập nhật hình ảnh" : s + ",hình ảnh");
                    }
                    return s;
                }
                else return "";
            }
            catch { }
            return "Vui lòng cập nhật khối lượng, kích thước, hình ảnh.";
        }

        public int CheckItemStoreJPCountByOneColumn(StorageJP items, string type = "nameen")
        {
            int count = 0;
            foreach (var item in items.StorageItemJPs)
            {
                count += CheckItemCountOneColumn(item, type);
            }
            return count;
        }
        public int CheckItemCountOneColumn(StorageItemJP store, string type = "nameen")
        {
            int i = 0;
            switch (type)
            {
                case "namejp":
                    {
                        if (store.NameJP == null || store.NameJP == "") { i += 1; }
                        else
                        {
                            foreach (var item in keyJP.ToUpper().Split(','))
                            {
                                if (store.NameJP.ToUpper().Contains(item)) { i += 1; break; }
                            }
                        }
                        break;
                    }
                case "nameen":
                    {
                        if (store.NameEN == null || store.NameEN == "") { i += 1; }
                        else
                        {
                            foreach (var item in keyJP.ToUpper().Split(','))
                            {
                                if (store.NameEN.ToUpper().Contains(item)) { i += 1; break; }
                            }
                        }
                        break;
                    }
                case "category":
                    {
                        if (store.CategoryId == null) { return i += 1; }
                        break;
                    }
                case "image":
                    {
                        if (store.ImageBase64 == "" || store.ImageBase64 == null) { return i += 1; }
                        break;
                    }
                case "jancode":
                    {
                        if (store.JanCode != null && store.JanCode != "")
                        {
                            if (store.JanCode.StartsWith("4") == false) { i += 1; }
                            else
                            {
                                if (store.JanCode.Length != 13) { i += 1; }
                                else
                                {
                                    if (store.MadeIn != null && store.MadeIn == "108")
                                    {

                                        if (store.JanCode.StartsWith("450") == false)
                                            if (store.JanCode.StartsWith("451") == false)
                                                if (store.JanCode.StartsWith("452") == false)
                                                    if (store.JanCode.StartsWith("453") == false)
                                                        if (store.JanCode.StartsWith("454") == false)
                                                            if (store.JanCode.StartsWith("455") == false)
                                                                if (store.JanCode.StartsWith("456") == false)
                                                                    if (store.JanCode.StartsWith("457") == false)
                                                                        if (store.JanCode.StartsWith("458") == false)
                                                                            if (store.JanCode.StartsWith("459") == false)
                                                                                if (store.JanCode.StartsWith("490") == false)
                                                                                    if (store.JanCode.StartsWith("491") == false)
                                                                                        if (store.JanCode.StartsWith("492") == false)
                                                                                            if (store.JanCode.StartsWith("493") == false)
                                                                                                if (store.JanCode.StartsWith("494") == false)
                                                                                                    if (store.JanCode.StartsWith("495") == false)
                                                                                                        if (store.JanCode.StartsWith("496") == false)
                                                                                                            if (store.JanCode.StartsWith("497") == false)
                                                                                                                if (store.JanCode.StartsWith("498") == false)
                                                                                                                    if (store.JanCode.StartsWith("499") == false) { i += 1; }

                                    }
                                }
                            }
                        }
                        break;
                    }
                case "quantity":
                    {
                        if (store.Quantity <= 0 || store.Quantity == null) { i += 1; }
                        break;
                    }
                case "price":
                    {
                        if (store.PriceTax <= 0 || store.PriceTax == null || store.PriceTax < 50) { i += 1; }
                        break;
                    }
                case "linkweb":
                    {
                        if (store.LinkWeb == "" || store.LinkWeb == null) { return i += 1; }
                        break;
                    }
                case "madein":
                    {
                        if (store.MadeIn == null || store.MadeIn == "") { return i += 1; }
                        break;
                    }
            }
            return i;
        }

        //general xuat kho
        public GeneralExportGood GeneralExportGood(StorageJP model)
        {
            GeneralExportGood result = new Models.GeneralExportGood();
            var details = model.TrackingDetails;
            var exports = db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == model.AgencyId);
            var export = new ExportGood();
            bool flag = false;
            foreach (var item in details)
            {
                if (exports.Where(n => n.TrackingDetailId == item.Id).Count() > 0)
                {
                    export = exports.Where(n => n.TrackingDetailId == item.Id).First().ExportGood;
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                result = new Models.GeneralExportGood()
                {
                    ShippingMask = export.ShippingMarkVN,
                    Size = DisplaySize(export.Weigh, export.SizeInput, export.SizeTableId, export.Size),
                    Status = PackageStatus(export.StatusId.Value, 3),
                    Weigh = export.Weigh == null ? 0 : export.Weigh.Value
                };
            }
            return result;
        }
        //gereral chuyen hang
        public GeneralShipping GeneralShipping(StorageJP model)
        {
            GeneralShipping result = new Models.GeneralShipping();
            var details = model.TrackingDetails;
            var exports = db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == model.AgencyId);
            var shippinghawbs = db.ShippingHAWBDetails.Where(n => n.AgencyId == model.AgencyId);
            var shipping = new Shipping();
            bool flag = false;
            foreach (var item in details)
            {
                if (exports.Where(n => n.TrackingDetailId == item.Id).Count() > 0)
                {
                    var export = exports.Where(n => n.TrackingDetailId == item.Id).First().ExportGood;
                    if (shippinghawbs.Where(n => n.ExportGoodId == export.Id).Count() > 0)
                    {
                        shipping = shippinghawbs.Where(n => n.ExportGoodId == export.Id).First().ShippingHAWB.Shipping;
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                result = new Models.GeneralShipping()
                {
                    ShippingCode = shipping.ShippingCode,
                    Status = PackageStatus(shipping.StatusId.Value, 5),
                };
            }

            return result;
        }
        //gereral tra hang
        public GeneralReturn GeneralReturn(StorageJP model)
        {
            GeneralReturn result = new Models.GeneralReturn();
            var returnDetails = db.ReturnDetails.Where(n => n.PackageReturn.AgencyId == model.AgencyId).Where(n => n.StoregeJPId == model.Id);
            if (returnDetails.Count() > 0)
            {
                var item = returnDetails.First();
                result = new Models.GeneralReturn()
                {
                    ReturnCode = item.PackageReturn.ReturnCode,
                    Status = PackageStatus(item.PackageReturn.StatusId.Value, 6)
                };
            }
            return result;
        }
        public GeneralRelation GeneralRelation(StorageJP model)
        {
            GeneralRelation result = new Models.GeneralRelation();

            result.GeneralExportGood = new Models.GeneralExportGood();
            result.GeneralExportGood = GeneralExportGood(model);

            result.GeneralShipping = new Models.GeneralShipping();
            result.GeneralShipping = GeneralShipping(model);

            result.GeneralReturn = new Models.GeneralReturn();
            result.GeneralReturn = GeneralReturn(model);

            return result;
        }
    }
}