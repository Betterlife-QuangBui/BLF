using CsQuery;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using Web.Helpers.Adidas;
using Web.Helpers.Amazon;
using Web.Helpers.Database;
using Web.Helpers.HM;
using Web.Helpers.library;
using Web.Helpers.Rakuten;
using Web.Helpers.Uniqlo;
using Web.Helpers.YahooAuction;
using Web.Helpers.YahooShopping;
using System.Web.Configuration;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class StorageJPController : ManagementSystemController
    {
        public JsonResult CheckTrackingCode(string TrackingCode, Guid? Id)
        {
            var isValid = true;
            if (Id == null)
            {
                if (db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.TrackingCode == TrackingCode).Count() > 0)
                {
                    isValid = false;
                }
            }
            else
            {
                if (db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.TrackingCode == TrackingCode && x.Id != Id).Count() > 0)
                {
                    isValid = false;
                }
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateWeigh(Guid id, double weigh)
        {
            TrackingDetail item = db.TrackingDetails.Find(id);
            try
            {
                item.Weigh = weigh; db.SaveChanges();
                return Json(new { message = new { weigh = item.StorageJP.TrackingDetails.Sum(n => n.Weigh) }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateIsCheck(Guid id, bool isCheck = false)
        {
            StorageJP item = db.StorageJPs.Find(id);
            try
            {
                if (isCheck) { item.StatusId = 8; }
                else
                {
                    if (item.StorageItemJPs.Count > 0 && item.TrackingDetails.Count > 1) { item.StatusId = 7; }
                    else if (item.StorageItemJPs.Count > 0 && item.TrackingDetails.Count <= 1) { item.StatusId = 6; }
                    else { item.StatusId = 5; }
                }
                item.IsCheck = isCheck;
                db.SaveChanges();
                var PageUtils = new PageUtils();
                return Json(new { message = PageUtils.PackageStatus(item.StatusId.Value, 2), status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách lưu kho JP";
            ViewBag.key = key;
            ViewBag.page = page;
            ViewBag.sort = sort;
            var status = StatusUtils.GetStatus(2);
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.StatusId = new SelectList(status, "Value", "Text", sort);
            var item = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            ViewBag.ItemError = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id && n.StatusId != 8).OrderByDescending(n => n.CreatedAt);
            ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", "");
            ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");
            ViewBag.ReceivedDate = DateTime.Now.ToString("yyyy-MM-dd"); ViewBag.ReceivedHour = DateTime.Now.ToString("HH:mm");
            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            ViewBag.Size = new SelectList(size, "Value", "Text");

            //check item not exist XUAT KHO & TRA HANG
            var exports = db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == user.Agency.Id);
            var returns = db.ReturnDetails.Where(n => n.PackageReturn.AgencyId == user.Agency.Id);
            item = item.Where(m => m.TrackingDetails.Count() == 0 || m.TrackingDetails.Count() != (exports.Where(n => n.TrackingDetail.StorageJP.Id == m.Id).Count() + returns.Where(n => n.TrackingDetail.StorageJP.Id == m.Id).Count()))
                .OrderByDescending(n => n.CreatedAt);

            if (key != "")
            {
                item = item.Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
            }
            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }
            ViewBag.Count = item.Count();
            return View(Pager<StorageJP>.CreatePagging(item, page, 10));
        }
        [HttpPost]
        public ActionResult WebcamUpload()
        {
            string imagePath = ""; Session["imagePath"] = null;
            if (Request.InputStream.Length > 0)
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    string hexString = Server.UrlEncode(reader.ReadToEnd());
                    string imageName = DateTime.Now.ToString("dd-MM-yy-hh-mm-ss");
                    string imageNameReturn = (imageName + ".png").DoiTenHinh();
                    imagePath = string.Format("~/Uploads/StoreHourse/{0}", imageNameReturn);
                    System.IO.File.WriteAllBytes(Server.MapPath(imagePath), ConvertHexToBytes(hexString));
                    Session["imagePath"] = imageNameReturn;
                }
            }
            return Content(imagePath);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.StorageJPs.Remove(db.StorageJPs.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        private static byte[] ConvertHexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public ActionResult Add()
        {
            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
            ViewBag.SizeTableId = new SelectList(size, "Value", "Text");

            ViewBag.DeliveryAddressValue = new string[] { "" };
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text");
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");
            return View(new StorageJP() { IsStaffConfirm = true, IsAgencyConfirm = true, StatusId = 5, ReceivedDate = DateTime.Now, ReceivedHour = DateTime.Now.ToString("HH:mm") });
        }
        // POST: StorageJP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StorageJP storageJP, string[] DeliveryAddress, string imageWC = "")
        {
            storageJP.CreatedAt = storageJP.UpdatedAt = DateTime.Now;
            storageJP.CreatedBy = storageJP.UpdatedBy = user.Staff.UserName;
            storageJP.AgencyId = user.Agency.Id; storageJP.StaffId = user.Staff.UserName;
            //storageJP.WeighInput = WeighInput(storageJP.SizeInput);
            if (DeliveryAddress != null)
            {
                if (DeliveryAddress.Length > 0 && DeliveryAddress != null)
                {
                    storageJP.DeliveryAddress = String.Join(",", DeliveryAddress);
                }
                else
                {
                    storageJP.DeliveryAddress = "";
                }
            }
            else
            {
                storageJP.DeliveryAddress = "";
                DeliveryAddress = new string[] { };
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var upImage = Request.Files["upImage"];
                    if (upImage != null)
                    {
                        if (upImage.ContentLength > 0)
                        {
                            string fileName = upImage.FileName;
                            upImage.SaveAs(Server.MapPath("~/Uploads/StoreHourse/" + fileName));
                            storageJP.Image = fileName;
                            string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                            storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + fileName);
                        }
                    }
                    else
                    {
                        if (imageWC != null)
                        {
                            storageJP.Image = imageWC;
                            string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                            storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + imageWC);
                        }
                    }
                    storageJP.Id = Guid.NewGuid();
                    db.StorageJPs.Add(storageJP);
                    db.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("error", "Mã tracking đã tồn tại");
                    ViewBag.SizeTableId = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name", storageJP.SizeTableId);
                    ViewBag.DeliveryAddressValue = DeliveryAddress;
                    ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");
                    return Content(javasctipt_add("/StorageJP", "Thêm dữ liệu thất bại"));
                }
                return Content(javasctipt_add("/StorageJP", "Thêm dữ liệu thành công"));
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text", storageJP.StatusId);

            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
            ViewBag.SizeTableId = new SelectList(size, "Value", "Text", storageJP.SizeTableId);

            ViewBag.DeliveryAddressValue = DeliveryAddress;
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", storageJP.ReceivedHour);
            ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");

            return Content(javasctipt_add("/StorageJP", "Thêm dữ liệu thất bại"));
        }
        // GET: StorageJP/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StorageJP storageJP = db.StorageJPs.Find(id);
            if (storageJP == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text", storageJP.StatusId);
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", storageJP.ReceivedHour);

            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
            ViewBag.SizeTableId = new SelectList(size, "Value", "Text", storageJP.SizeTableId);
            ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", storageJP.DeliveryId);
            ViewBag.DeliveryAddressValue = (storageJP.DeliveryAddress == null ? "" : storageJP.DeliveryAddress).Split(',');
            ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");
            return View(storageJP);
        }
        // POST: StorageJP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StorageJP storageJP, Guid Id, string[] DeliveryAddress, string imageWC = "", string actionlink = "")
        {
            int count = db.TrackingDetails.Where(n => n.StoregeJPId == Id).Count();
            if (storageJP.StatusId == 8)
            {
                if (PageUtils.IsUpdateStoreJP(storageJP).Length > 0)
                {
                    if (PageUtils.CheckItemStoreJPCount(storageJP) > 0)
                    {
                        return Content(javasctipt_add("/StorageJP", PageUtils.IsUpdateStoreJP(storageJP)) + ", số lỗi: " + PageUtils.CheckItemStoreJPCount(storageJP));
                    }
                    else
                    {
                        return Content(javasctipt_add("/StorageJP", PageUtils.IsUpdateStoreJP(storageJP)));
                    }
                }
                else if (PageUtils.CheckItemStoreJPCount(storageJP) > 0)
                {
                    return Content(javasctipt_add("/StorageJP", "Vui lòng cập nhật lỗi nội dung. Số lỗi: " + PageUtils.CheckItemStoreJPCount(storageJP)));
                }
                else if (PageUtils.CheckItemStoreJPCount(storageJP) > 0)
                {
                    return Content(javasctipt_add("/StorageJP", "Vui lòng cập nhật lỗi nội dung. Số lỗi: " + PageUtils.CheckItemStoreJPCount(storageJP)));
                }
                else if (count == 0)
                {
                    return Content(javasctipt_add("/StorageJP", "Kiện hàng đang rỗng. Không thể đổi sang trạng thái đã kiểm"));
                }
                else
                {
                    string[] a = new string[] { };
                    storageJP.UpdatedAt = DateTime.Now;
                    storageJP.UpdatedBy = user.Staff.UserName;
                    storageJP.StaffId = user.Staff.UserName;
                    storageJP.AgencyId = user.Agency.Id;
                    if (DeliveryAddress != null)
                    {
                        if (DeliveryAddress.Length > 0)
                        {
                            storageJP.DeliveryAddress = String.Join(",", DeliveryAddress);
                        }
                        else
                        {
                            storageJP.DeliveryAddress = "";
                        }
                    }
                    else { storageJP.DeliveryAddress = ""; DeliveryAddress = new string[] { }; }


                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var upImage = Request.Files["upImage"];
                            if (upImage.ContentLength > 0 && upImage != null)
                            {
                                string fileName = upImage.FileName;
                                upImage.SaveAs(Server.MapPath("~/Uploads/StoreHourse/" + fileName));
                                storageJP.Image = fileName;
                                string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                                storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + fileName);
                            }
                            else
                            {
                                if (imageWC != null && imageWC != "")
                                {
                                    storageJP.Image = imageWC;
                                    string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                                    storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + imageWC);
                                }
                            }
                            if (storageJP.Image != null && storageJP.Image.Length > 0)
                            {
                                string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                                storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + storageJP.Image);
                            }
                            db.Entry(storageJP).State = EntityState.Modified;
                            db.SaveChanges();
                            if (actionlink != "") { return Content(javasctipt_add("/StorageJP/Detail/" + storageJP.Id, "Cập nhật dữ liệu thành công")); }
                            else return Content(javasctipt_add("/StorageJP", "Cập nhật dữ liệu thành công"));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                            if (actionlink != "") { return Content(javasctipt_add("/StorageJP/Detail/" + storageJP.Id, "Cập nhật dữ liệu thất bại")); }
                            else return Content(javasctipt_add("/StorageJP", "Cập nhật dữ liệu thất bại"));
                        }
                    }
                    else
                    {
                        ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text", storageJP.StatusId);

                        var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
                        size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
                        ViewBag.SizeTableId = new SelectList(size, "Value", "Text", storageJP.SizeTableId);
                        ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", storageJP.DeliveryId);
                        ViewBag.DeliveryAddressValue = DeliveryAddress;
                        ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", storageJP.ReceivedHour);
                        ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");
                        if (actionlink != "") { return Content(javasctipt_add("/StorageJP/Detail/" + storageJP.Id, "Cập nhật dữ liệu thất bại")); }
                        else return Content(javasctipt_add("/StorageJP", "Cập nhật dữ liệu thất bại"));
                    }
                }
            }
            else if (count == 0 && storageJP.StatusId == 7)
            {
                return Content(javasctipt_add("/StorageJP", "Kiện hàng đang rỗng. Không thể đổi sang trạng thái đang kiểm"));
            }
            else if (count > 0 && storageJP.StatusId == 6)
            {
                return Content(javasctipt_add("/StorageJP", "Kiện hàng đang có hàng. Không thể đổi trạng thái đã nhận"));
            }
            else
            {

                string[] a = new string[] { };
                storageJP.UpdatedAt = DateTime.Now;
                storageJP.UpdatedBy = user.Staff.UserName;
                storageJP.StaffId = user.Staff.UserName;
                storageJP.AgencyId = user.Agency.Id;
                if (DeliveryAddress != null)
                {
                    if (DeliveryAddress.Length > 0)
                    {
                        storageJP.DeliveryAddress = String.Join(",", DeliveryAddress);
                    }
                    else
                    {
                        storageJP.DeliveryAddress = "";
                    }
                }
                else { storageJP.DeliveryAddress = ""; DeliveryAddress = new string[] { }; }


                if (ModelState.IsValid)
                {
                    try
                    {
                        var upImage = Request.Files["upImage"];
                        if (upImage.ContentLength > 0 && upImage != null)
                        {
                            string fileName = upImage.FileName;
                            upImage.SaveAs(Server.MapPath("~/Uploads/StoreHourse/" + fileName));
                            storageJP.Image = fileName;
                            string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                            storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + fileName);
                        }
                        else
                        {
                            if (imageWC != null && imageWC != "")
                            {
                                storageJP.Image = imageWC;
                                string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                                storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + imageWC);
                            }
                        }
                        if (storageJP.Image != null && storageJP.Image.Length > 0)
                        {
                            string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                            storageJP.ImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/StoreHourse/" + storageJP.Image);
                        }
                        db.Entry(storageJP).State = EntityState.Modified;
                        db.SaveChanges();
                        if (actionlink != "") { return Content(javasctipt_add("/StorageJP/Detail/" + storageJP.Id, "Cập nhật dữ liệu thành công")); }
                        else return Content(javasctipt_add("/StorageJP", "Cập nhật dữ liệu thành công"));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                        if (actionlink != "") { return Content(javasctipt_add("/StorageJP/Detail/" + storageJP.Id, "Cập nhật dữ liệu thất bại")); }
                        else return Content(javasctipt_add("/StorageJP", "Cập nhật dữ liệu thất bại"));
                    }
                }
                else
                {
                    ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text", storageJP.StatusId);

                    var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
                    size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
                    ViewBag.SizeTableId = new SelectList(size, "Value", "Text", storageJP.SizeTableId);
                    ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", storageJP.DeliveryId);
                    ViewBag.DeliveryAddressValue = DeliveryAddress;
                    ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", storageJP.ReceivedHour);
                    ViewBag.DeliveryAddress = new SelectList(db.DeliveryAddresses.Where(n => n.AgencyId == user.Agency.Id).Select(n => new { n.Id, DeliveryCode = n.AgencyId + "-" + n.DeliveryCode }).OrderByDescending(n => n.Id), "Id", "DeliveryCode", "");
                    if (actionlink != "") { return Content(javasctipt_add("/StorageJP/Detail/" + storageJP.Id, "Cập nhật dữ liệu thất bại")); }
                    else return Content(javasctipt_add("/StorageJP", "Cập nhật dữ liệu thất bại"));
                }
            }
        }
        [HttpPost]
        public ActionResult CheckExistTrackingCode(string tracking)
        {
            try
            {
                if (db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode == tracking).Count() > 0)
                {
                    return Json(new { message = "", status = false }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { message = "", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "", status = true }, JsonRequestBehavior.AllowGet); }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult WareHouse(Guid id)
        {
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name", 108);
            ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(id).TrackingCode), "Value", "Text");
            try
            {
                if (db.StorageJPs.Find(id).TrackingDetails.Count > 0)
                {
                    ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(id).TrackingCode), "Value", "Text");
                }
                else
                {
                    ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(id).TrackingCode).Where(n => n.Value.Contains(" - 01")), "Value", "Text");
                }
                return PartialView(db.StorageJPs.Find(id).TrackingDetails);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult Detail(Guid id, string TrackingSearchCode = "", string jan = "")
        {
            var storejp = db.StorageJPs.Find(id);
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories.OrderBy(n => n.NameEN), "Id", "NameEN", "");
            ViewBag.MadeIn = new SelectList(db.Countries.OrderBy(n => n.Name), "Id", "Name", "");

            var lst = SelectListUtils.TrackingList(storejp.TrackingCode);
            ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(storejp.TrackingCode), "Value", "Text");
            List<SelectListItem> search = new List<SelectListItem>();
            search.AddRange(storejp.TrackingDetails.OrderBy(n => n.TrackingSubCode).Select(n => new SelectListItem() { Value = n.TrackingSubCode.Trim(), Text = n.TrackingSubCode.Trim() }));
            search.Insert(0, new SelectListItem() { Value = "", Text = "Tất cả" });
            ViewBag.TrackingSearchCode = new SelectList(search, "Value", "Text", TrackingSearchCode);
            ViewBag.jan = jan;
            ViewBag._TrackingSearchCode = TrackingSearchCode;
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text", storejp.StatusId);
            return View(storejp);
        }
        public ActionResult DetailPackage(Guid id)
        {
            var storejp = db.StorageJPs.Find(id);
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name", 108);

            ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(storejp.TrackingCode), "Value", "Text");
            ViewBag.searchCate = new string[] { "Rakuten" };
            ViewBag.listResultSearch = new List<SearchProductInfo>();
            return View(storejp);
        }

        //search Item
        string getJanCode(string LinkWeb)
        {
            string result = "";
            try
            {
                result = CQ.CreateFromUrl(LinkWeb)[".item_number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return result;
        }
        public String getRakutenCategories(int CategoryId)
        {
            string appId = "1095695100824025173";
            List<String> lst = new List<String>();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaGenre/Search/20140222?format=xml&genreId=" + CategoryId + "&applicationId=" + appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                return WebUtility.HtmlEncode(xdoc.Element("root").Element("current").Element("genreName").Value);
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        ClothingHelpers clothing = new ClothingHelpers();
        [HttpPost]
        public ActionResult DetailPackage(Guid id, string SearchCode, string[] Website, Guid StoregeJPId)
        {
            var storejp = db.StorageJPs.Find(StoregeJPId);
            ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(storejp.TrackingCode), "Value", "Text");
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories.OrderBy(n => n.NameEN), "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries.OrderBy(n => n.Name), "Id", "Name", 108);
            ViewBag.StatusId = new SelectList(db.StatusWareHouses, "Id", "Name");
            ViewBag.TrackingStatusId = new SelectList(StatusUtils.GetStatus(1), "Value", "Text", "");
            ViewBag.StorageJP = db.StorageJPs.Find(StoregeJPId);
            var searchCate = Website;
            List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
            if (searchCate == null) { searchCate = new string[] { "database" }; }
            foreach (var item in searchCate)
            {
                if (item.ToLower().Contains("Rakuten".ToLower()))
                {
                    #region Rakuten
                    RakutenUtils rakuten = new RakutenUtils();
                    try
                    {
                        listSearch.AddRange(rakuten.getProductsSearchWareHouse(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.SmallImageUrls[0].ImageUrl,
                            JanCode = n.ItemCode,//getJanCode(n.ItemUrl),
                            ProductCode = n.ItemCode,
                            Amount = n.ItemPrice * 1,
                            LinkWeb = n.ItemUrl,
                            NameJP = n.ItemName,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.ItemPrice,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                            CategoryId = n.CategoryId,
                            CategoryName = ""//getRakutenCategories(n.CategoryId)
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("YahooShopping".ToLower()))
                {
                    #region Yahoo Shopping
                    YahooShoppingUtils yahooShop = new YahooShoppingUtils();
                    try
                    {
                        listSearch.AddRange(yahooShop.getProductsBySearch(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.Code,
                            ProductCode = n.ProductId.Split('_')[1],
                            Amount = n.Price * 1,
                            LinkWeb = n.Url,
                            NameJP = n.Name,
                            //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Name),
                            PriceTax = n.Price,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags(),
                            CategoryId = n.CategoryId,
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else if (item.ToLower().Contains("YahooAuction".ToLower()))
                {
                    #region YahooAuction
                    YahooAuctionUtils yahooAuction = new YahooAuctionUtils();
                    try
                    {
                        listSearch.AddRange(yahooAuction.getProductsBySarch(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.AuctionID,
                            ProductCode = n.AuctionID,
                            Amount = n.CurrentPrice * 1,
                            LinkWeb = n.AuctionItemUrl,
                            NameJP = n.Title,
                            //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Title),
                            PriceTax = n.CurrentPrice,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags(),
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Amazon".ToLower()))
                {
                    #region Amazon
                    AmazonUtils amazon = new AmazonUtils();
                    try
                    {
                        listSearch.AddRange(amazon.SearchBy(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.ImageUrl,
                            JanCode = n.ItemCode,
                            ProductCode = n.ItemCode,
                            Amount = n.ItemPrice * 1,
                            LinkWeb = n.ItemUrl,
                            NameJP = n.ItemName,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.ItemPrice,
                            Quantity = 1,
                            CategoryName = n.CategoryName,
                            Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                        }));
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else if (item.ToLower().Contains("Uniqlo".ToLower()))
                {
                    #region Uniqlo
                    UniqloUtils uniqlo = new UniqloUtils();
                    try
                    {
                        listSearch.AddRange(uniqlo.getSearch(SearchCode).Select(n => new Models.SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductCode,
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            //NameEN = TranslateUtils.TranslateGoogleTextEN(n.NameJP),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = n.Material,
                            CategoryName = ""
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Adidas".ToLower()))
                {
                    #region Adidas
                    AdidasUtils adidas = new AdidasUtils();
                    try
                    {
                        listSearch.AddRange(adidas.Search(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductId,
                            ProductCode = n.ProductId,
                            Amount = n.Price * 1,
                            LinkWeb = n.Url,
                            NameJP = n.Name,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.Price,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Hm".ToLower()))
                {
                    #region Hm
                    HMUtils hm = new HMUtils();
                    try
                    {
                        listSearch.AddRange(hm.Search(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductId,
                            ProductCode = n.ProductId,
                            Amount = n.Price * 1,
                            LinkWeb = n.Url,
                            NameJP = n.Name,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.Price,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Locondo".ToLower()))
                {
                    #region Locondo
                    try
                    {
                        listSearch.AddRange(clothing.LocondoJP(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = "",
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Crocs".ToLower()))
                {
                    #region Crocs
                    try
                    {
                        listSearch.AddRange(clothing.Crocs(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = "",
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                        }));
                    }
                    catch { }
                    #endregion
                }
                //not yet
                else if (item.ToLower().Contains("Dena-ec".ToLower()))
                {
                    #region Dena-ec
                    try
                    {
                        listSearch.AddRange(clothing.DenaEC(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = "",
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Reebok".ToLower()))
                {
                    #region Reebok
                    try
                    {
                        listSearch.AddRange(clothing.Reebok(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = "",
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Forever21".ToLower()))
                {
                    #region Forever21
                    try
                    {
                        listSearch.AddRange(clothing.Forever21(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = "",
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Gap".ToLower()))
                {
                    #region Gap
                    try
                    {
                        listSearch.AddRange(clothing.Gap(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = "",
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("database".ToLower()))
                {
                    #region Database
                    try
                    {
                        listSearch.AddRange(db.WareHouseItems.Where(n => n.ProductCode.Contains(SearchCode) || n.JanCode.Contains(SearchCode) || n.NameJP.Contains(SearchCode) || n.NameEN.Contains(SearchCode)).Take(50).Select(n => new Models.SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductCode,
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = n.NameEN,
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = n.Material,
                            CategoryName = n.CategoryName,
                            MadeIn = n.MadeIn.ToString(),
                        }));
                    }
                    catch { }
                    #endregion
                }
            }
            ViewBag.listResultSearch = listSearch.OrderBy(n => n.PriceTax);
            Session["searchResult"] = listSearch.OrderBy(n => n.PriceTax).ToList();
            ViewBag.Website = new SelectList(SearchWebsiteUtils.GetWebsite(), "Value", "Text");
            ViewBag.SearchCode = SearchCode;
            ViewBag.searchCate = searchCate;
            return View("DetailPackage", storejp);
        }
        [HttpPost]
        public ActionResult AddNewItem(Guid StoregeJPId, string IdItem, string TrackingCode, int Quantity, int CategoryId, int MadeIn, int StatusId = 2, string ComponentImage = "")
        {
            //add tracking detail
            TrackingCode = TrackingCode.Trim();
            Guid idTracking = Guid.NewGuid();
            if (db.TrackingDetails.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingSubCode == TrackingCode).Count() == 0)
            {
                //add new
                TrackingDetail tracking = new TrackingDetail()
                {
                    Id = idTracking,
                    StoregeJPId = StoregeJPId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = user.Staff.UserName,
                    SaveDate = DateTime.Now,
                    SaveHour = "",
                    Size = "",
                    TrackingSubCode = TrackingCode,
                    Weigh = 0,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = user.Staff.UserName
                };
                db.TrackingDetails.Add(tracking);
            }
            else
            {
                idTracking = db.TrackingDetails.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingSubCode == TrackingCode).FirstOrDefault().Id;
            }
            //add product into tracking details
            var lst = Session["searchResult"] as List<SearchProductInfo>;
            foreach (var id in IdItem.Split(','))
            {
                if (id.Trim().Length > 0)
                {
                    SearchProductInfo search = lst.FirstOrDefault(n => n.Id == Guid.Parse(id));
                    //if (db.StorageItemJPs.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).Count() == 0)
                    //{
                    //add new StorageItem
                    StorageItemJP model = new StorageItemJP()
                    {
                        Amount = search.PriceTax * Quantity,
                        CategoryId = CategoryId,
                        CategoryName = search.CategoryName,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        Image = search.Image,
                        ImageBase64 = search.ImageBase64,
                        JanCode = search.JanCode,
                        LinkWeb = search.LinkWeb,
                        HSCode = search.HSCode,
                        DescriptionOfGoods = search.DescriptionOfGoods,
                        MadeIn = MadeIn + "",
                        Component = search.Component,
                        ComponentImage = ComponentImage,
                        Material = search.Material,
                        NameEN = TranslateUtils.TranslateGoogleTextEN(search.NameJP),
                        NameJP = search.NameJP,
                        Notes = "",
                        PriceTax = search.PriceTax,
                        ProductCode = search.ProductCode,
                        Quantity = Quantity,
                        StoregeJPId = StoregeJPId,
                        UpdatedAt = DateTime.Now,
                        TrackingDetailId = idTracking,
                        UpdatedBy = user.Staff.UserName,
                        IsDeny = false,
                    };
                    if (model.ProductCode == null || model.ProductCode.Trim().Length == 0)
                    {
                        model.ProductCode = model.JanCode;
                    }
                    if (model.JanCode == null || model.JanCode.Trim().Length == 0)
                    {
                        model.JanCode = model.ProductCode;
                    }
                    db.StorageItemJPs.Add(model);

                    //add warehouse
                    WareHouseItem warehouse = new WareHouseItem();
                    if (db.WareHouseItems.Where(n => n.JanCode == model.JanCode && n.ProductCode == model.ProductCode).Count() == 0)
                    {
                        warehouse = new WareHouseItem()
                        {
                            Amount = model.Amount,
                            CategoryId = model.CategoryId,
                            CategoryName = model.CategoryName,
                            CreatedAt = model.CreatedAt,
                            CreatedBy = model.CreatedBy,
                            Id = Guid.NewGuid(),
                            Image = model.Image,
                            JanCode = model.JanCode,
                            LinkWeb = model.LinkWeb,
                            HSCode = model.HSCode,
                            DescriptionOfGoods = model.DescriptionOfGoods,
                            ImageBase64 = model.ImageBase64,
                            Component = model.Component,
                            ComponentImage = model.ComponentImage,
                            MadeIn = model.MadeIn,
                            Material = model.Material,
                            NameEN = TranslateUtils.TranslateGoogleTextEN(model.NameJP),
                            NameJP = model.NameJP,
                            Notes = model.Notes,
                            PriceTax = model.PriceTax,
                            ProductCode = model.ProductCode,
                            Quantity = model.Quantity,
                            UpdatedAt = model.UpdatedAt,
                            UpdatedBy = model.UpdatedBy,
                            ProductTypeId = 1,
                            IsDeny = false
                        };
                        db.WareHouseItems.Add(warehouse);
                    }
                }

                else
                {
                    SearchProductInfo search = lst.FirstOrDefault(n => n.Id == Guid.Parse(id));

                    var storageItem = db.StorageItemJPs.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).FirstOrDefault();
                    storageItem.Quantity = Quantity + storageItem.Quantity;
                    storageItem.Amount = storageItem.PriceTax * (Quantity + storageItem.Quantity);
                }
            }
            db.SaveChanges();
            Session["searchResult"] = null;
            return Redirect("/StorageJP/Detail/" + StoregeJPId);
        }
        [HttpPost]
        public ActionResult UpdateStatus(Guid id, int status)
        {
            try
            {
                var store = db.StorageJPs.Find(id);
                if (PageUtils.IsUpdateStoreJP(store).Length > 0)
                {
                    return Content(javasctipt_add("/StorageJP", PageUtils.IsUpdateStoreJP(store)));
                }
                store.StatusId = status;
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        //database
        public ActionResult Database(Guid id, int page = 1, string SearchCategoryId = "", string key = "", int TypeSearch = 0)
        {
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN", "");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name", "");
            ViewBag.StoreJP = db.StorageJPs.Find(id);
            ViewBag.StoreJPId = id;
            int pageno = 1;
            int pageSize = 15; int totalCount = 0;

            var storejp = db.StorageJPs.Find(id);
            var spOutput = new SqlParameter
            {
                ParameterName = "TotalRows",
                SqlDbType = System.Data.SqlDbType.BigInt,
                Direction = System.Data.ParameterDirection.Output
            };
            var spPageNo = new SqlParameter
            {
                ParameterName = "PageNo",
                Value = pageno,
                SqlDbType = System.Data.SqlDbType.BigInt,
                Direction = System.Data.ParameterDirection.Input
            };
            var spPageSize = new SqlParameter
            {
                ParameterName = "PageSize",
                Value = pageSize,
                SqlDbType = System.Data.SqlDbType.BigInt,
                Direction = System.Data.ParameterDirection.Input
            };
            var spAgency = new SqlParameter
            {
                ParameterName = "AgencyId",
                Value = user.Agency.Id,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var data = db.Database.SqlQuery<StorageItemJP>("exec proStoreJPItems @TotalRows Out, @AgencyId,@PageNo, @PageSize",
                spOutput, spAgency, spPageNo, spPageSize).ToList();

            if (spOutput.Value != null)
            {
                totalCount = int.Parse(spOutput.Value.ToString());
            }

            //PagerCus<WareHouseItem> pager = new PagerCus<WareHouseItem>(data.AsQueryable(), pageno, pageSize, totalCount);

            //var item = db.WareHouseItems.OrderByDescending(n => n.CreatedAt);
            ViewBag.Pagging = Pager<int>.CreatePagging(totalCount, page, 15);
            return View(Pager<StorageItemJP>.CreatePagging(data.AsQueryable(), page, 15));
            //return View("Database", pager);
        }
        public float WeighInput(string sizeInput)
        {
            if (sizeInput == null) return 0;
            return 0;
        }
        //edit one item
        public ActionResult EditItem(Guid id)
        {
            var storeItem = db.StorageItemJPs.Find(id);
            ViewBag.StoreJP = storeItem.StorageJP;
            return View(storeItem);
        }
        public ActionResult MoreQuickly(Guid id)
        {
            var storejp = db.StorageJPs.Find(id);
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name", 108);
            ViewBag.Website = new SelectList(SelectListUtils.WebsiteList(), "Value", "Text", "Rakuten");
            return View(storejp);
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
        public int getCategoryAdd(string name)
        {
            try
            {
                name = name.Trim();
                return db.WareHouseCategories.ToList().Single(n => n.NameEN.Trim() == name).Id;
            }
            catch
            {
                return 0;
            }
        }
        public int getMadeInAdd(string name)
        {
            try
            {
                name = name.Trim();
                return db.Countries.ToList().Single(n => n.Name.Trim() == name).Id;
            }
            catch
            {
                return 0;
            }
        }
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
        public ActionResult AddMoreQuickly(Guid Id, string Jancode = "", string Productcode = "", string CategoryId = "", string MadeIn = "", string Website = "", string quantity = "")
        {
            try
            {
                List<MoreQuickly> lst = new List<Models.MoreQuickly>();
                bool flag = true;
                int i = 0;
                if (Jancode == "") { Jancode = Productcode; flag = false; }
                foreach (string item in Jancode.Split(','))
                {
                    #region backup
                    //if (lst.Where(n => n.Jancode == item).Count() == 0)
                    //{
                    //    try
                    //    {
                    //        lst.Add(new Models.MoreQuickly()
                    //        {
                    //            CategoryId = getCategory((CategoryId.Split(',').Length == 1 ? CategoryId.Split(',')[0] : CategoryId.Split(',')[i]).Trim()).Id,
                    //            Jancode = item,
                    //            MadeIn = getMadeIn((MadeIn.Split(',').Length == 1 ? MadeIn.Split(',')[0] : MadeIn.Split(',')[i]).Trim()).Id,
                    //            quantity = int.Parse((quantity.Split(',').Length == 1 ? quantity.Split(',')[0] : quantity.Split(',')[i]).Trim()),
                    //            Website = Website
                    //        });
                    //    }
                    //    catch
                    //    {
                    //        lst.Add(new Models.MoreQuickly()
                    //        {
                    //            CategoryId = 0,
                    //            Jancode = item,
                    //            MadeIn = 0,
                    //            quantity = 0,
                    //            Website = Website
                    //        });
                    //    }
                    //}
                    #endregion
                    //int quantity = 0;
                    //try { } catch { }
                    try
                    {
                        lst.Add(new Models.MoreQuickly()
                        {
                            CategoryId = getCategoryAdd((CategoryId.Split(',').Length == 1 ? CategoryId.Split(',')[0] : CategoryId.Split(',')[i]).Trim()),
                            Jancode = item,
                            MadeIn = getMadeInAdd((MadeIn.Split(',').Length == 1 ? MadeIn.Split(',')[0] : MadeIn.Split(',')[i]).Trim()),
                            quantity = int.Parse((quantity.Split(',').Length == 1 ? quantity.Split(',')[0] : quantity.Split(',')[i]).Trim()),
                            Website = Website
                        });
                    }
                    catch (Exception ex)
                    {
                        lst.Add(new Models.MoreQuickly()
                        {
                            CategoryId = 0,
                            Jancode = item,
                            MadeIn = 0,
                            quantity = 0,
                            Website = Website
                        });
                    }
                    i++;
                }
                List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
                #region Search
                foreach (var item in lst)
                {
                    var listSearchProductInfo = new List<SearchProductInfo>();
                    if (item.Jancode != "")
                    {
                        #region search item
                        if (item.Website.ToLower().Contains("Rakuten".ToLower()))
                        {
                            #region Rakuten
                            RakutenUtils rakuten = new RakutenUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(rakuten.getProductsSearchWareHouse(item.Jancode).Items.Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.SmallImageUrls[0].ImageUrl,
                                    JanCode = n.ItemCode,//getJanCode(n.ItemUrl),
                                    ProductCode = n.ItemCode,
                                    Amount = n.ItemPrice * item.quantity,
                                    LinkWeb = n.ItemUrl,
                                    NameJP = n.ItemName,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.ItemPrice,
                                    Quantity = item.quantity,
                                    Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    CategoryName = ""//getRakutenCategories(n.CategoryId),

                                }).Take(1));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("YahooShopping".ToLower()))
                        {
                            #region Yahoo Shopping
                            YahooShoppingUtils yahooShop = new YahooShoppingUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(yahooShop.getProductsBySearch(item.Jancode).Items.Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.Image,
                                    JanCode = n.Code,
                                    ProductCode = n.ProductId.Split('_')[1],
                                    Amount = n.Price * item.quantity,
                                    LinkWeb = n.Url,
                                    NameJP = n.Name,
                                    //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Name),
                                    PriceTax = n.Price,
                                    Quantity = item.quantity,
                                    Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags(),
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    CategoryName = n.CategoryName
                                }).Take(1));
                            }
                            catch (Exception ex) { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("YahooAuction".ToLower()))
                        {
                            #region YahooAuction
                            YahooAuctionUtils yahooAuction = new YahooAuctionUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(yahooAuction.getProductsBySarch(item.Jancode).Items.Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.Image,
                                    JanCode = n.AuctionID,
                                    ProductCode = n.AuctionID,
                                    Amount = n.CurrentPrice * item.quantity,
                                    LinkWeb = n.AuctionItemUrl,
                                    NameJP = n.Title,
                                    //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Title),
                                    PriceTax = n.CurrentPrice,
                                    Quantity = item.quantity,
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags(),
                                    CategoryName = n.CategoryName
                                }).Take(1));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Amazon".ToLower()))
                        {
                            #region Amazon
                            AmazonUtils amazon = new AmazonUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(amazon.SearchBy(item.Jancode).Items.Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.ImageUrl,
                                    JanCode = n.ItemCode,
                                    ProductCode = n.ItemCode,
                                    Amount = n.ItemPrice * item.quantity,
                                    LinkWeb = n.ItemUrl,
                                    NameJP = n.ItemName,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.ItemPrice,
                                    Quantity = item.quantity,
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    CategoryName = n.CategoryName,
                                    Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                                }).Take(1));
                            }
                            catch (Exception ex) { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Uniqlo".ToLower()))
                        {
                            #region Uniqlo
                            UniqloUtils uniqlo = new UniqloUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(uniqlo.getSearch(item.Jancode).Select(n => new Models.SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.Image,
                                    JanCode = n.ProductCode,
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * item.quantity,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    //NameEN = TranslateUtils.TranslateGoogleTextEN(n.NameJP),
                                    PriceTax = n.PriceTax,
                                    Quantity = item.quantity,
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    Material = n.Material,
                                    CategoryName = ""
                                }).OrderBy(n => n.PriceTax).Take(1));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Adidas".ToLower()))
                        {
                            #region Adidas
                            AdidasUtils adidas = new AdidasUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(adidas.Search(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.Image,
                                    JanCode = n.ProductId,
                                    ProductCode = n.ProductId,
                                    Amount = n.Price * item.quantity,
                                    LinkWeb = n.Url,
                                    NameJP = n.Name,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.Price,
                                    Quantity = item.quantity,
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                                    CategoryName = n.CategoryName
                                }).OrderBy(n => n.PriceTax).Take(1));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Hm".ToLower()))
                        {
                            #region Hm
                            HMUtils hm = new HMUtils();
                            try
                            {
                                listSearchProductInfo.AddRange(hm.Search(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    MoreQuickly = item,
                                    Image = n.Image,
                                    JanCode = n.ProductId,
                                    ProductCode = n.ProductId,
                                    Amount = n.Price * item.quantity,
                                    LinkWeb = n.Url,
                                    NameJP = n.Name,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.Price,
                                    Quantity = item.quantity,
                                    CategoryId = item.CategoryId,
                                    MadeIn = item.MadeIn.ToString(),
                                    Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                                    CategoryName = n.CategoryName
                                }).OrderBy(n => n.PriceTax).Take(1));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Locondo".ToLower()))
                        {
                            #region Locondo
                            try
                            {
                                listSearchProductInfo.AddRange(clothing.LocondoJP(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    Image = n.Image,
                                    JanCode = "",
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * 1,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.PriceTax,
                                    Quantity = 1,
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                                }));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Crocs".ToLower()))
                        {
                            #region Crocs
                            try
                            {
                                listSearchProductInfo.AddRange(clothing.Crocs(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    Image = n.Image,
                                    JanCode = "",
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * 1,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.PriceTax,
                                    Quantity = 1,
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                                }));
                            }
                            catch { }
                            #endregion
                        }
                        //not yet
                        else if (item.Website.ToLower().Contains("Dena-ec".ToLower()))
                        {
                            #region Dena-ec
                            try
                            {
                                listSearchProductInfo.AddRange(clothing.DenaEC(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    Image = n.Image,
                                    JanCode = "",
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * 1,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.PriceTax,
                                    Quantity = 1,
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                                }));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Reebok".ToLower()))
                        {
                            #region Reebok
                            try
                            {
                                listSearchProductInfo.AddRange(clothing.Reebok(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    Image = n.Image,
                                    JanCode = "",
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * 1,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.PriceTax,
                                    Quantity = 1,
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                                }));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Forever21".ToLower()))
                        {
                            #region Forever21
                            try
                            {
                                listSearchProductInfo.AddRange(clothing.Forever21(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    Image = n.Image,
                                    JanCode = "",
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * 1,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.PriceTax,
                                    Quantity = 1,
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                                }));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("Gap".ToLower()))
                        {
                            #region Gap
                            try
                            {
                                listSearchProductInfo.AddRange(clothing.Gap(item.Jancode).Select(n => new SearchProductInfo()
                                {
                                    Id = Guid.NewGuid(),
                                    Image = n.Image,
                                    JanCode = "",
                                    ProductCode = n.ProductCode,
                                    Amount = n.PriceTax * 1,
                                    LinkWeb = n.LinkWeb,
                                    NameJP = n.NameJP,
                                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                    PriceTax = n.PriceTax,
                                    Quantity = 1,
                                    Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags().SplitMaritalRakuten()
                                }));
                            }
                            catch { }
                            #endregion
                        }
                        else if (item.Website.ToLower().Contains("database".ToLower()))
                        {
                            #region Database
                            try
                            {
                                var item_ = db.WareHouseItems.Where(n => n.ProductCode.Contains(item.Jancode) || n.JanCode.Contains(item.Jancode) || n.NameJP.Contains(item.Jancode) || n.NameEN.Contains(item.Jancode)).Take(1);
                                if (item_.Count() > 0)
                                {
                                    var n = item_.Single();
                                    var insItem = new Models.SearchProductInfo();
                                    insItem.MoreQuickly = new Models.MoreQuickly();
                                    insItem.Id = Guid.NewGuid();
                                    insItem.MoreQuickly = item;
                                    insItem.Image = n.Image;
                                    insItem.JanCode = n.ProductCode;
                                    insItem.HSCode = n.HSCode;
                                    insItem.DescriptionOfGoods = n.DescriptionOfGoods;
                                    insItem.ProductCode = n.ProductCode;
                                    insItem.Amount = n.PriceTax * item.quantity;
                                    insItem.LinkWeb = n.LinkWeb;
                                    insItem.NameJP = n.NameJP;
                                    insItem.NameEN = n.NameEN;
                                    insItem.PriceTax = n.PriceTax;
                                    insItem.Quantity = item.quantity;
                                    insItem.CategoryId = item.CategoryId;
                                    insItem.MadeIn = n.MadeIn.ToString();
                                    insItem.Material = n.Material;
                                    insItem.CategoryName = n.CategoryName;
                                    listSearchProductInfo.Add(insItem);
                                }

                            }
                            catch (Exception ex) { }
                            #endregion
                        }
                        #endregion
                    }

                    if (listSearchProductInfo.Count == 0)
                    {
                        listSearchProductInfo.Add(new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = "",
                            JanCode = item.Jancode,//getJanCode(n.ItemUrl),
                            HSCode = "",
                            DescriptionOfGoods = "",
                            ProductCode = "",
                            Amount = 0,
                            LinkWeb = "",
                            NameJP = "",
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = 0,
                            Quantity = item.quantity,
                            Material = "",
                            CategoryId = item.CategoryId,
                            MadeIn = item.MadeIn + "",
                            CategoryName = "CLOTHING"//getRakutenCategories(n.CategoryId),

                        });
                    }
                    listSearch.AddRange(listSearchProductInfo);
                }
                #endregion
                #region SaveDB
                //add tracking detail
                string TrackingCode = "01";
                Guid idTracking = Guid.NewGuid();
                if (db.TrackingDetails.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Where(n => n.StoregeJPId == Id && n.TrackingSubCode == TrackingCode).Count() == 0)
                {
                    //add new
                    TrackingDetail tracking = new TrackingDetail()
                    {
                        Id = idTracking,
                        StoregeJPId = Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        SaveDate = DateTime.Now,
                        SaveHour = "",
                        Size = "",
                        TrackingSubCode = TrackingCode,
                        Weigh = 0,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.TrackingDetails.Add(tracking);
                }
                else
                {
                    idTracking = db.TrackingDetails.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Where(n => n.StoregeJPId == Id && n.TrackingSubCode == TrackingCode).FirstOrDefault().Id;
                }
                //add product into tracking details
                foreach (var id in listSearch)
                {
                    SearchProductInfo search = id;
                    //if (db.StorageItemJPs.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Where(n => n.StoregeJPId == Id && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).Count() == 0)
                    //{
                    //add new StorageItem
                    StorageItemJP model = new StorageItemJP()
                    {
                        Amount = search.PriceTax * search.Quantity,
                        CategoryId = search.CategoryId == 0 ? null : search.CategoryId,
                        CategoryName = search.CategoryName,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        Image = search.Image,
                        ImageBase64 = search.ImageBase64,
                        JanCode = flag ? search.JanCode : "",
                        LinkWeb = search.LinkWeb,
                        HSCode = search.HSCode,
                        DescriptionOfGoods = search.DescriptionOfGoods,
                        MadeIn = search.MadeIn == "0" ? null : search.MadeIn + "",
                        Component = search.Component,
                        ComponentImage = "",
                        Material = search.Material,
                        NameEN = TranslateUtils.TranslateGoogleTextEN(search.NameJP),
                        NameJP = search.NameJP,
                        Notes = "",
                        PriceTax = search.PriceTax,
                        ProductCode = search.ProductCode,
                        Quantity = search.Quantity,
                        StoregeJPId = Id,
                        UpdatedAt = DateTime.Now,
                        TrackingDetailId = idTracking,
                        UpdatedBy = user.Staff.UserName,
                        IsDeny = false,
                    };
                    if (model.ProductCode == null || model.ProductCode.Trim().Length == 0)
                    {
                        model.ProductCode = model.JanCode;
                    }
                    if (model.JanCode == null || model.JanCode.Trim().Length == 0)
                    {
                        model.JanCode = flag ? search.JanCode : "";
                    }
                    db.StorageItemJPs.Add(model);
                    //add warehouse
                    //WareHouseItem warehouse = new WareHouseItem();
                    //if (db.WareHouseItems.Where(n => n.JanCode == model.JanCode && n.ProductCode == model.ProductCode).Count() == 0)
                    //{
                    //    warehouse = new WareHouseItem()
                    //    {
                    //        Amount = model.Amount,
                    //        CategoryId = model.CategoryId,
                    //        CategoryName = model.CategoryName,
                    //        CreatedAt = model.CreatedAt,
                    //        CreatedBy = model.CreatedBy,
                    //        Id = Guid.NewGuid(),
                    //        Image = model.Image,
                    //        JanCode = model.JanCode,
                    //        LinkWeb = model.LinkWeb,
                    //        ImageBase64 = model.ImageBase64,
                    //        Component = model.Component,
                    //        ComponentImage = model.ComponentImage,
                    //        MadeIn = model.MadeIn,
                    //        Material = model.Material,
                    //        NameEN = TranslateUtils.TranslateGoogleTextEN(model.NameJP),
                    //        NameJP = model.NameJP,
                    //        Notes = model.Notes,
                    //        PriceTax = model.PriceTax,
                    //        ProductCode = model.ProductCode,
                    //        Quantity = model.Quantity,
                    //        UpdatedAt = model.UpdatedAt,
                    //        UpdatedBy = model.UpdatedBy,
                    //        ProductTypeId = 1,
                    //        IsDeny = false
                    //    };
                    //    db.WareHouseItems.Add(warehouse);
                    //}
                    //}

                    //else
                    //{
                    //    var storageItem = db.StorageItemJPs.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Where(n => n.StoregeJPId == Id && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).FirstOrDefault();
                    //    storageItem.Quantity = search.Quantity + storageItem.Quantity;
                    //    storageItem.Amount = storageItem.PriceTax * (search.Quantity + storageItem.Quantity);
                    //}
                }
                db.SaveChanges();
                #endregion
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult RemoveMoreQuickly(string Jancode)
        {
            try
            {
                List<MoreQuickly> lst = new List<Models.MoreQuickly>();
                if (Session["MoreQuickly"] != null)
                {
                    lst = Session["MoreQuickly"] as List<Models.MoreQuickly>;
                }
                if (lst.Where(n => n.Jancode == Jancode).Count() > 0)
                {
                    var item = lst.Single(n => n.Jancode == Jancode);
                    lst.Remove(item);
                }
                Session["MoreQuickly"] = lst;
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SaveMoreQuickly(Guid Id)
        {
            try
            {
                List<MoreQuickly> lst = new List<Models.MoreQuickly>();
                if (Session["MoreQuickly"] != null)
                {
                    lst = Session["MoreQuickly"] as List<Models.MoreQuickly>;
                }
                List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
                #region Search
                foreach (var item in lst)
                {
                    if (item.Website.ToLower().Contains("Rakuten".ToLower()))
                    {
                        #region Rakuten
                        RakutenUtils rakuten = new RakutenUtils();
                        try
                        {
                            listSearch.AddRange(rakuten.getProductsSearchWareHouse(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.SmallImageUrls[0].ImageUrl,
                                JanCode = n.ItemCode,//getJanCode(n.ItemUrl),
                                ProductCode = n.ItemCode,
                                Amount = n.ItemPrice * item.quantity,
                                LinkWeb = n.ItemUrl,
                                NameJP = n.ItemName,
                                NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                PriceTax = n.ItemPrice,
                                Quantity = item.quantity,
                                Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                CategoryName = ""//getRakutenCategories(n.CategoryId),

                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("YahooShopping".ToLower()))
                    {
                        #region Yahoo Shopping
                        YahooShoppingUtils yahooShop = new YahooShoppingUtils();
                        try
                        {
                            listSearch.AddRange(yahooShop.getProductsBySearch(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.Code,
                                ProductCode = n.ProductId.Split('_')[1],
                                Amount = n.Price * item.quantity,
                                LinkWeb = n.Url,
                                NameJP = n.Name,
                                //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Name),
                                PriceTax = n.Price,
                                Quantity = item.quantity,
                                Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags(),
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("YahooAuction".ToLower()))
                    {
                        #region YahooAuction
                        YahooAuctionUtils yahooAuction = new YahooAuctionUtils();
                        try
                        {
                            listSearch.AddRange(yahooAuction.getProductsBySarch(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.AuctionID,
                                ProductCode = n.AuctionID,
                                Amount = n.CurrentPrice * item.quantity,
                                LinkWeb = n.AuctionItemUrl,
                                NameJP = n.Title,
                                //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Title),
                                PriceTax = n.CurrentPrice,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags(),
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("Amazon".ToLower()))
                    {
                        #region Amazon
                        AmazonUtils amazon = new AmazonUtils();
                        try
                        {
                            listSearch.AddRange(amazon.SearchBy(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.ImageUrl,
                                JanCode = n.ItemCode,
                                ProductCode = n.ItemCode,
                                Amount = n.ItemPrice * item.quantity,
                                LinkWeb = n.ItemUrl,
                                NameJP = n.ItemName,
                                NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                PriceTax = n.ItemPrice,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                CategoryName = n.CategoryName,
                                Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("Uniqlo".ToLower()))
                    {
                        #region Uniqlo
                        UniqloUtils uniqlo = new UniqloUtils();
                        try
                        {
                            listSearch.AddRange(uniqlo.getSearch(item.Jancode).Select(n => new Models.SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.ProductCode,
                                ProductCode = n.ProductCode,
                                Amount = n.PriceTax * item.quantity,
                                LinkWeb = n.LinkWeb,
                                NameJP = n.NameJP,
                                //NameEN = TranslateUtils.TranslateGoogleTextEN(n.NameJP),
                                PriceTax = n.PriceTax,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = n.Material,
                                CategoryName = ""
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("Adidas".ToLower()))
                    {
                        #region Adidas
                        AdidasUtils adidas = new AdidasUtils();
                        try
                        {
                            listSearch.AddRange(adidas.Search(item.Jancode).Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.ProductId,
                                ProductCode = n.ProductId,
                                Amount = n.Price * item.quantity,
                                LinkWeb = n.Url,
                                NameJP = n.Name,
                                NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                PriceTax = n.Price,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("Hm".ToLower()))
                    {
                        #region Hm
                        HMUtils hm = new HMUtils();
                        try
                        {
                            listSearch.AddRange(hm.Search(item.Jancode).Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.ProductId,
                                ProductCode = n.ProductId,
                                Amount = n.Price * item.quantity,
                                LinkWeb = n.Url,
                                NameJP = n.Name,
                                NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                PriceTax = n.Price,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("database".ToLower()))
                    {
                        #region Database
                        try
                        {
                            listSearch.AddRange(db.WareHouseItems.Where(n => n.ProductCode.Contains(item.Jancode) || n.JanCode.Contains(item.Jancode) || n.NameJP.Contains(item.Jancode) || n.NameEN.Contains(item.Jancode)).Take(50).Select(n => new Models.SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                //MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.ProductCode,
                                ProductCode = n.ProductCode,
                                Amount = n.PriceTax * item.quantity,
                                LinkWeb = n.LinkWeb,
                                NameJP = n.NameJP,
                                NameEN = n.NameEN,
                                PriceTax = n.PriceTax,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = n.Material,
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax).Take(1));
                        }
                        catch { }
                        #endregion
                    }
                }
                #endregion
                #region SaveDB
                //add tracking detail
                string TrackingCode = "01";
                Guid idTracking = Guid.NewGuid();
                if (db.TrackingDetails.Where(n => n.StoregeJPId == Id && n.TrackingSubCode == TrackingCode).Count() == 0)
                {
                    //add new
                    TrackingDetail tracking = new TrackingDetail()
                    {
                        Id = idTracking,
                        StoregeJPId = Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        SaveDate = DateTime.Now,
                        SaveHour = "",
                        Size = "",
                        TrackingSubCode = TrackingCode,
                        Weigh = 0,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.TrackingDetails.Add(tracking);
                }
                else
                {
                    idTracking = db.TrackingDetails.Where(n => n.StoregeJPId == Id && n.TrackingSubCode == TrackingCode).FirstOrDefault().Id;
                }
                //add product into tracking details
                foreach (var id in listSearch)
                {
                    SearchProductInfo search = id;
                    if (db.StorageItemJPs.Where(n => n.StoregeJPId == Id && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).Count() == 0)
                    {
                        //add new StorageItem
                        StorageItemJP model = new StorageItemJP()
                        {
                            Amount = search.PriceTax * search.Quantity,
                            CategoryId = search.CategoryId,
                            CategoryName = search.CategoryName,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            Image = search.Image,
                            ImageBase64 = search.ImageBase64,
                            JanCode = search.JanCode,
                            LinkWeb = search.LinkWeb,
                            MadeIn = search.MadeIn + "",
                            Component = search.Component,
                            ComponentImage = "",
                            Material = search.Material,
                            NameEN = search.NameEN,
                            NameJP = search.NameJP,
                            Notes = "",
                            PriceTax = search.PriceTax,
                            ProductCode = search.ProductCode,
                            Quantity = search.Quantity,
                            StoregeJPId = Id,
                            UpdatedAt = DateTime.Now,
                            TrackingDetailId = idTracking,
                            UpdatedBy = user.Staff.UserName,
                            IsDeny = false,
                        };
                        if (model.ProductCode == null || model.ProductCode.Trim().Length == 0)
                        {
                            model.ProductCode = model.JanCode;
                        }
                        if (model.JanCode == null || model.JanCode.Trim().Length == 0)
                        {
                            model.JanCode = search.JanCode;
                        }
                        db.StorageItemJPs.Add(model);
                        //add warehouse
                        WareHouseItem warehouse = new WareHouseItem();
                        //if (db.WareHouseItems.Where(n => n.JanCode == model.JanCode && n.ProductCode == model.ProductCode).Count() == 0)
                        //{
                        //    warehouse = new WareHouseItem()
                        //    {
                        //        Amount = model.Amount,
                        //        CategoryId = model.CategoryId,
                        //        CategoryName = model.CategoryName,
                        //        CreatedAt = model.CreatedAt,
                        //        CreatedBy = model.CreatedBy,
                        //        Id = Guid.NewGuid(),
                        //        Image = model.Image,
                        //        JanCode = model.JanCode,
                        //        LinkWeb = model.LinkWeb,
                        //        ImageBase64 = model.ImageBase64,
                        //        Component = model.Component,
                        //        ComponentImage = model.ComponentImage,
                        //        MadeIn = model.MadeIn,
                        //        Material = model.Material,
                        //        NameEN = TranslateUtils.TranslateGoogleTextEN(model.NameJP),
                        //        NameJP = model.NameJP,
                        //        Notes = model.Notes,
                        //        PriceTax = model.PriceTax,
                        //        ProductCode = model.ProductCode,
                        //        Quantity = model.Quantity,
                        //        UpdatedAt = model.UpdatedAt,
                        //        UpdatedBy = model.UpdatedBy,
                        //        ProductTypeId = 1,
                        //        IsDeny = false
                        //    };
                        //    db.WareHouseItems.Add(warehouse);
                        //}
                    }

                    else
                    {
                        var storageItem = db.StorageItemJPs.Where(n => n.StoregeJPId == Id && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).FirstOrDefault();
                        storageItem.Quantity = search.Quantity + storageItem.Quantity;
                        storageItem.Amount = storageItem.PriceTax * (search.Quantity + storageItem.Quantity);
                    }
                }
                db.SaveChanges();
                #endregion
                Session["MoreQuickly"] = null;
                return Json(new { message = "Thêm dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Thêm dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete multiple
        [HttpPost]
        public ActionResult DeleteMultiple(string id)
        {
            try
            {
                foreach (var item in id.Split(','))
                {
                    db.StorageJPs.Remove(db.StorageJPs.Find(Guid.Parse(item.Trim())));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult Autocomplete(string term = "")
        {
            try
            {
                var listjp = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.StatusId == 8 && n.TrackingCode.Contains(term)).OrderBy(n => n.TrackingCode);
                db.SaveChanges();
                return Json(listjp.Select(n => n.TrackingCode), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }
        //review result
        [HttpPost]
        public ActionResult MoreQuickly(Guid Id, string Jancode = "", string Productcode = "", string CategoryId = "", string MadeIn = "", string Website = "", string quantity = "")
        {
            try
            {
                List<MoreQuickly> lst = new List<Models.MoreQuickly>();
                bool flag = true;
                if (Jancode == "") { Jancode = Productcode; flag = true; }
                int i = 0;
                foreach (string item in Jancode.Split(','))
                {
                    if (lst.Where(n => n.Jancode == item).Count() == 0)
                    {
                        try
                        {
                            lst.Add(new Models.MoreQuickly()
                            {
                                CategoryId = getCategory((CategoryId.Split(',').Length == 1 ? CategoryId.Split(',')[0] : CategoryId.Split(',')[i]).Trim()).Id,
                                Jancode = item,
                                MadeIn = getMadeIn((MadeIn.Split(',').Length == 1 ? MadeIn.Split(',')[0] : MadeIn.Split(',')[i]).Trim()).Id,
                                quantity = int.Parse((quantity.Split(',').Length == 1 ? quantity.Split(',')[0] : quantity.Split(',')[i]).Trim()),
                                Website = Website
                            });
                        }
                        catch
                        {
                            lst.Add(new Models.MoreQuickly()
                            {
                                CategoryId = 0,
                                Jancode = item,
                                MadeIn = 0,
                                quantity = 0,
                                Website = Website
                            });
                        }
                    }
                    i++;
                }
                List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
                #region Search
                foreach (var item in lst)
                {
                    var listSearchProductInfo = new List<SearchProductInfo>();
                    #region search item
                    if (item.Website.ToLower().Contains("Rakuten".ToLower()))
                    {
                        #region Rakuten
                        RakutenUtils rakuten = new RakutenUtils();
                        try
                        {
                            listSearchProductInfo.AddRange(rakuten.getProductsSearchWareHouse(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                MoreQuickly = item,
                                Image = n.SmallImageUrls[0].ImageUrl,
                                JanCode = n.ItemCode,//getJanCode(n.ItemUrl),
                                ProductCode = n.ItemCode,
                                Amount = n.ItemPrice * item.quantity,
                                LinkWeb = n.ItemUrl,
                                NameJP = n.ItemName,
                                NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                PriceTax = n.ItemPrice,
                                Quantity = item.quantity,
                                Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                CategoryName = ""//getRakutenCategories(n.CategoryId),

                            }).OrderBy(n => n.PriceTax));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("YahooShopping".ToLower()))
                    {
                        #region Yahoo Shopping
                        YahooShoppingUtils yahooShop = new YahooShoppingUtils();
                        try
                        {
                            listSearchProductInfo.AddRange(yahooShop.getProductsBySearch(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.Code,
                                ProductCode = n.ProductId.Split('_')[1],
                                Amount = n.Price * item.quantity,
                                LinkWeb = n.Url,
                                NameJP = n.Name,
                                //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Name),
                                PriceTax = n.Price,
                                Quantity = item.quantity,
                                Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags(),
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax));
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("YahooAuction".ToLower()))
                    {
                        #region YahooAuction
                        YahooAuctionUtils yahooAuction = new YahooAuctionUtils();
                        try
                        {
                            listSearchProductInfo.AddRange(yahooAuction.getProductsBySarch(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.AuctionID,
                                ProductCode = n.AuctionID,
                                Amount = n.CurrentPrice * item.quantity,
                                LinkWeb = n.AuctionItemUrl,
                                NameJP = n.Title,
                                //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Title),
                                PriceTax = n.CurrentPrice,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags(),
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax));
                        }
                        catch { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("Amazon".ToLower()))
                    {
                        #region Amazon
                        AmazonUtils amazon = new AmazonUtils();
                        try
                        {
                            listSearchProductInfo.AddRange(amazon.SearchBy(item.Jancode).Items.Select(n => new SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                MoreQuickly = item,
                                Image = n.ImageUrl,
                                JanCode = n.ItemCode,
                                ProductCode = n.ItemCode,
                                Amount = n.ItemPrice * item.quantity,
                                LinkWeb = n.ItemUrl,
                                NameJP = n.ItemName,
                                NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                                PriceTax = n.ItemPrice,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                CategoryName = n.CategoryName,
                                Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                            }).OrderBy(n => n.PriceTax));
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    else if (item.Website.ToLower().Contains("database".ToLower()))
                    {
                        #region Database
                        try
                        {
                            listSearchProductInfo.AddRange(db.WareHouseItems.Where(n => n.ProductCode.Contains(item.Jancode) || n.JanCode.Contains(item.Jancode) || n.NameJP.Contains(item.Jancode) || n.NameEN.Contains(item.Jancode)).Take(50).Select(n => new Models.SearchProductInfo()
                            {
                                Id = Guid.NewGuid(),
                                MoreQuickly = item,
                                Image = n.Image,
                                JanCode = n.ProductCode,
                                ProductCode = n.ProductCode,
                                Amount = n.PriceTax * item.quantity,
                                LinkWeb = n.LinkWeb,
                                NameJP = n.NameJP,
                                NameEN = n.NameEN,
                                PriceTax = n.PriceTax,
                                Quantity = item.quantity,
                                CategoryId = item.CategoryId,
                                MadeIn = item.MadeIn.ToString(),
                                Material = n.Material,
                                CategoryName = n.CategoryName
                            }).OrderBy(n => n.PriceTax));
                        }
                        catch { }
                        #endregion
                    }
                    #endregion
                    #region No data
                    if (listSearchProductInfo.Count == 0)
                    {
                        listSearchProductInfo.Add(new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = "",
                            MoreQuickly = item,
                            JanCode = item.Jancode,
                            ProductCode = "",
                            Amount = 0,
                            LinkWeb = "",
                            NameJP = "",
                            NameEN = "",
                            PriceTax = 0,
                            Quantity = item.quantity,
                            Material = "",
                            CategoryId = item.CategoryId,
                            MadeIn = item.MadeIn + "",
                            CategoryName = "CLOTHING"

                        });
                    }
                    #endregion
                    listSearch.AddRange(listSearchProductInfo);
                }
                #endregion
                Session["resultQuickly"] = listSearch;
                Session["searchQuickly"] = lst;
                return Json(new { message = "Tìm dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Tìm dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult LoadImageUpload()
        {
            return Json(new { message = Session["imagePath"], status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveMoreQuicklyMain(Guid Id, string searchMoreQuickyId = "")
        {
            try
            {
                List<SearchProductInfo> list = Session["resultQuickly"] as List<SearchProductInfo>;
                List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
                string[] addItems = searchMoreQuickyId.Split(',');
                foreach (var item in list)
                {
                    foreach (var idPro in addItems)
                    {
                        if (item.Id == Guid.Parse(idPro))
                        {
                            listSearch.Add(item); break;
                        }
                    }
                }
                #region SaveDB
                //add tracking detail
                string TrackingCode = "01";
                Guid idTracking = Guid.NewGuid();
                if (db.TrackingDetails.Where(n => n.StoregeJPId == Id && n.TrackingSubCode == TrackingCode).Count() == 0)
                {
                    //add new
                    TrackingDetail tracking = new TrackingDetail()
                    {
                        Id = idTracking,
                        StoregeJPId = Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        SaveDate = DateTime.Now,
                        SaveHour = "",
                        Size = "",
                        TrackingSubCode = TrackingCode,
                        Weigh = 0,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.TrackingDetails.Add(tracking);
                }
                else
                {
                    idTracking = db.TrackingDetails.Where(n => n.StoregeJPId == Id && n.TrackingSubCode == TrackingCode).FirstOrDefault().Id;
                }
                //add product into tracking details
                foreach (var id in listSearch)
                {
                    SearchProductInfo search = id;
                    if (db.StorageItemJPs.Where(n => n.StoregeJPId == Id && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).Count() == 0)
                    {
                        //add new StorageItem
                        StorageItemJP model = new StorageItemJP()
                        {
                            Amount = search.PriceTax * search.Quantity,
                            CategoryId = search.CategoryId,
                            CategoryName = search.CategoryName,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            Image = search.Image,
                            ImageBase64 = search.ImageBase64,
                            JanCode = search.JanCode,
                            LinkWeb = search.LinkWeb,
                            MadeIn = search.MadeIn + "",
                            Component = search.Component,
                            ComponentImage = "",
                            Material = search.Material,
                            NameEN = search.NameEN,
                            NameJP = search.NameJP,
                            Notes = "",
                            PriceTax = search.PriceTax,
                            ProductCode = search.ProductCode,
                            Quantity = search.Quantity,
                            StoregeJPId = Id,
                            UpdatedAt = DateTime.Now,
                            TrackingDetailId = idTracking,
                            UpdatedBy = user.Staff.UserName,
                            IsDeny = false,
                        };
                        if (model.ProductCode == null || model.ProductCode.Trim().Length == 0)
                        {
                            model.ProductCode = model.JanCode;
                        }
                        if (model.JanCode == null || model.JanCode.Trim().Length == 0)
                        {
                            model.JanCode = search.JanCode;
                        }
                        db.StorageItemJPs.Add(model);
                        //add warehouse

                        //if (db.WareHouseItems.Where(n => n.JanCode == search.JanCode && n.ProductCode == search.ProductCode).Count() == 0)
                        //{
                        //    WareHouseItem warehouse = new WareHouseItem()
                        //    {
                        //        Amount = search.PriceTax * search.Quantity,
                        //        CategoryId = search.CategoryId,
                        //        CategoryName = search.CategoryName,
                        //        CreatedAt = DateTime.Now,
                        //        CreatedBy = user.Staff.UserName,
                        //        Id = Guid.NewGuid(),
                        //        Image = search.Image,
                        //        JanCode = search.JanCode,
                        //        LinkWeb = search.LinkWeb,
                        //        ImageBase64 = search.ImageBase64,
                        //        Component = search.Component,
                        //        ComponentImage = search.ComponentImage,
                        //        MadeIn = search.MadeIn,
                        //        Material = search.Material,
                        //        NameEN = TranslateUtils.TranslateGoogleTextEN(search.NameJP),
                        //        NameJP = search.NameJP,
                        //        Notes = search.Notes,
                        //        PriceTax = search.PriceTax,
                        //        ProductCode = search.ProductCode,
                        //        Quantity = search.Quantity,
                        //        UpdatedAt = DateTime.Now,
                        //        UpdatedBy = user.Staff.UserName,
                        //        ProductTypeId = 1,
                        //        IsDeny = false
                        //    };
                        //    db.WareHouseItems.Add(warehouse);
                        //}
                    }

                    else
                    {
                        var storageItem = db.StorageItemJPs.Where(n => n.StoregeJPId == Id && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).FirstOrDefault();
                        storageItem.Quantity = search.Quantity + storageItem.Quantity;
                        storageItem.Amount = storageItem.PriceTax * (search.Quantity + storageItem.Quantity);
                    }
                }
                db.SaveChanges();
                #endregion
                Session["MoreQuickly"] = null; Session["resultQuickly"] = null;
                Session["searchQuickly"] = null;
                return Json(new { message = "Thêm dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = "Thêm dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
