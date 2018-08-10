using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using System.Web.Configuration;
using static WareHouseJP.Website.Helpers.PaggerUtils;
using System.Net;
using System.Data.Entity;
using System.IO;

namespace WareHouseJP.Website.Controllers
{
    public class PackageReturnController : ManagementSystemController
    {
        public JsonResult CheckReturnCode(string ReturnCode, Guid? Id)
        {
            var isValid = true;
            if (Id == null)
            {
                if (db.PackageReturns.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ReturnCode == ReturnCode).Count() > 0)
                {
                    isValid = false;
                }
            }
            else
            {
                if (db.PackageReturns.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ReturnCode == ReturnCode && x.Id != Id).Count() > 0)
                { 
                        isValid = false;
                }
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        // GET: PackageReturn
        public ActionResult Index(int page = 1, int sort = 15)
        {
            ViewBag.Title = "Danh sách trả hàng";
            ViewBag.Page = page;
            ViewBag.sort = sort;
            var status = StatusUtils.GetStatus(6);
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.ExportGoodStatus = new SelectList(status, "Value", "Text", sort);
            ViewBag.HAWB = new SelectList(status, "Value", "Text", sort);
            var item = db.PackageReturns.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }
            return View(Pager<PackageReturn>.CreatePagging(item, page, 10));
        }
        public ActionResult Add()
        {
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(6), "Value", "Text", "");
            var hawb = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n=>new SelectListItem() {Value=n.Id,Text=n.Name+ " / "+n.Address });
            ViewBag.HAWB = new SelectList(hawb, "Value", "Text");
            //ViewBag.ExportHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            return View(new PackageReturn() { ReturnDate = DateTime.Now, ReturnHour = DateTime.Now.ToString("HH:mm") });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PackageReturn model,string HAWB, string imageWC = "", string address_choose="",string name= "",string PostalCode="",string Address="",string Phone="",bool address_option=false)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    var upImage = Request.Files["upImage"];
                    if (upImage.ContentLength > 0 && upImage != null)
                    {
                        string fileName = upImage.FileName.DoiTenHinh();
                        upImage.SaveAs(Server.MapPath("~/uploads/PackageReturn/" + fileName));
                        model.ReturnImage = fileName;
                        string url = WebConfigurationManager.AppSettings["base64Url"];
                        model.ReturnImageBase64 = Web.Helpers.Images.ImageUtils.Images(url + "uploads/PackageReturn/" + fileName);
                    }
                    else
                    {
                        if (imageWC != null)
                        {
                            model.ReturnImage = imageWC;
                            string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                            model.ReturnImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/PackageReturn/" + imageWC);
                        }
                    }
                    model.Id = Guid.NewGuid();
                    
                    //add deatail address
                    var model_HAWB = new HAWB();
                    ReturnHAWB model_ReturnHAWB = new ReturnHAWB()
                    {
                        Address = Address,
                        CreatedAt = DateTime.Now,
                        AgencyId = user.Agency.Id,
                        CreatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        IsActive = true,
                        IsDefault = true,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        PackageReturnId=model.Id

                    };
                    if (address_choose == "address-list") {
                        model_HAWB = db.HAWBs.Find(HAWB);
                        model.ReciveName = model_HAWB.Name;
                        model_ReturnHAWB.HAWBId = model_HAWB.Id;
                        model_ReturnHAWB.IsChooseList = true;

                        model_ReturnHAWB.Name = model_HAWB.Name;
                        model_ReturnHAWB.Phone = model_HAWB.Phone;
                        model_ReturnHAWB.PostalCode = model_HAWB.PostalCode;
                        model_ReturnHAWB.Address = model_HAWB.Address;
                        model_ReturnHAWB.IsActive = true;
                    }
                    else
                    {
                        model_ReturnHAWB.IsChooseList = false;
                        model_ReturnHAWB.Name = name;
                        model_ReturnHAWB.Phone = Phone;
                        model_ReturnHAWB.PostalCode = PostalCode;
                        model_ReturnHAWB.Address = Address;
                        model_ReturnHAWB.IsActive = true;
                        if (address_option!=false&& address_option != null)
                        {
                            model_HAWB = new Models.HAWB()
                            {
                                Address = Address,
                                CreatedAt = DateTime.Now,
                                AgencyId = user.Agency.Id,
                                CreatedBy = user.Staff.UserName,
                                Id = model.Id + "",
                                IsActive = true,
                                IsDefault = true,
                                IsDeleted = false,
                                Name = name,
                                Phone = Phone,
                                PostalCode = PostalCode,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = user.Staff.UserName
                            };
                            db.HAWBs.Add(model_HAWB);
                            model_ReturnHAWB.HAWBId = model_HAWB.Id;
                        }
                        model.ReciveName = name;
                    }
                    db.PackageReturns.Add(model);
                    //add info recive into database
                    db.ReturnHAWBs.Add(model_ReturnHAWB);
                    db.SaveChanges();
                    return Content(javasctipt_add("/PackageReturn", "Thêm dữ liệu thành công"));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(6), "Value", "Text", model.StatusId);
            var hawb = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.HAWB = new SelectList(hawb, "Value", "Text",HAWB);
            return Content(javasctipt_add("/PackageReturn", "Thêm dữ liệu thất bại"));
        }
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageReturn model = db.PackageReturns.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(6), "Value", "Text", model.StatusId);
            var hawb = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.HAWB = new SelectList(hawb, "Value", "Text", model.ReturnHAWBs.First().HAWBId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PackageReturn model,Guid ReturnHAWBId, string HAWB, string imageWC = "", string address_choose = "", string name = "", string PostalCode = "", string Address = "", string Phone = "", bool address_option = false, string actionlink = "")
        {
            if (PageUtils.IsUpdatePackageReturn(model).Length > 0)
            {
                return Content(javasctipt_add("/PackageReturn", PageUtils.IsUpdatePackageReturn(model)));
            }
            else
            {
                model.UpdatedAt = DateTime.Now;
                model.UpdatedBy = user.Staff.UserName;
                model.AgencyId = user.Agency.Id;

                if (ModelState.IsValid)
                {
                    try
                    {
                        var upImage = Request.Files["upImage"];
                        if (upImage.ContentLength > 0 && upImage != null)
                        {
                            string fileName = upImage.FileName.DoiTenHinh();
                            upImage.SaveAs(Server.MapPath("~/uploads/PackageReturn/" + fileName));
                            model.ReturnImage = fileName;
                            string url = WebConfigurationManager.AppSettings["base64Url"];
                            model.ReturnImageBase64 = Web.Helpers.Images.ImageUtils.Images(url + "uploads/PackageReturn/" + fileName);
                        }
                        else
                        {
                            if (imageWC != null && imageWC != "")
                            {
                                model.ReturnImage = imageWC;
                                string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                                model.ReturnImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/PackageReturn/" + imageWC);
                            }
                        }
                        if (model.ReturnImage != null && model.ReturnImage.Length > 0)
                        {
                            string base64Url = WebConfigurationManager.AppSettings["base64Url"];
                            model.ReturnImageBase64 = Web.Helpers.Images.ImageUtils.Images(base64Url + "Uploads/PackageReturn/" + model.ReturnImageBase64);
                        }
                        var model_HAWB = new HAWB();
                        ReturnHAWB model_ReturnHAWB = db.ReturnHAWBs.Find(ReturnHAWBId);
                        if (address_choose == "address-list")
                        {
                            model_HAWB = db.HAWBs.Find(HAWB);
                            model.ReciveName = model_HAWB.Name;
                            model_ReturnHAWB.HAWBId = model_HAWB.Id;
                            model_ReturnHAWB.IsChooseList = true;

                            model_ReturnHAWB.Name = model_HAWB.Name;
                            model_ReturnHAWB.Phone = model_HAWB.Phone;
                            model_ReturnHAWB.PostalCode = model_HAWB.PostalCode;
                            model_ReturnHAWB.Address = model_HAWB.Address;
                            model_ReturnHAWB.IsActive = true;
                            model_ReturnHAWB.UpdatedAt = DateTime.Now;
                            model_ReturnHAWB.UpdatedBy = user.Staff.UserName;
                            model.ReciveName = model_HAWB.Name;
                        }
                        else
                        {
                            model_ReturnHAWB.HAWBId = null;
                            model_ReturnHAWB.IsChooseList = false;
                            model.ReciveName = name;
                            model_ReturnHAWB.Name = name;
                            model_ReturnHAWB.Phone = Phone;
                            model_ReturnHAWB.PostalCode = PostalCode;
                            model_ReturnHAWB.Address = Address;
                            model_ReturnHAWB.IsActive = true;
                            model_ReturnHAWB.UpdatedAt = DateTime.Now;
                            model_ReturnHAWB.UpdatedBy = user.Staff.UserName;
                        }
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        if (actionlink != "") { return Content(javasctipt_add("/PackageReturn/Detail/" + model.Id, "Cập nhật dữ liệu thành công")); }
                        else return Content(javasctipt_add("/PackageReturn", "Cập nhật dữ liệu thành công"));
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                    }
                }
                ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(6), "Value", "Text", model.StatusId);
                var hawb = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
                ViewBag.HAWB = new SelectList(hawb, "Value", "Text", HAWB);
                if (actionlink != "") { return Content(javasctipt_add("/PackageReturn/Detail/" + model.Id, "Cập nhật dữ liệu thất bại")); }
                else return Content(javasctipt_add("/PackageReturn", "Cập nhật dữ liệu thất bại"));
            }
        }

        [HttpPost]
        public ActionResult DeleteMultiple(string ids)
        {
            try
            {
                foreach (var id in ids.Split(','))
                {
                    var items = db.PackageReturns.Find(Guid.Parse(id));
                    db.PackageReturns.Remove(items);
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var items = db.PackageReturns.Find(id);
                db.PackageReturns.Remove(items);
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult Detail(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageReturn model = db.PackageReturns.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(6), "Value", "Text", model.StatusId);
            var ListJP = db.TrackingDetails.Where(n => n.StorageJP.AgencyId == user.Agency.Id&&n.TrackingSubCode=="21").OrderByDescending(n=>n.CreatedAt);
            ListJP = ListJP.Where(n=>db.ReturnDetails.Where(m=>m.PackageReturn.AgencyId==user.Agency.Id).Where(m=>m.TrackingDetailId==n.Id).Count()==0).OrderByDescending(n => n.CreatedAt);
            ViewBag.ListJP = Pager<TrackingDetail>.CreatePagging(ListJP.OrderByDescending(n => n.CreatedAt), 1, 10);

            var ReturnDetail = db.ReturnDetails.Where(n => n.PackageReturnId == id).Where(n=>n.PackageReturn.AgencyId==user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            ViewBag.ReturnDetail = Pager<ReturnDetail>.CreatePagging(ReturnDetail.OrderByDescending(n => n.CreatedAt), 1, 10);
            ViewBag.Model = model;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddItems(Guid id,string ids)
        {
            try
            {
                PackageReturn model = db.PackageReturns.Find(id);
                foreach (var item in ids.Split(','))
                {
                    var trackingDetail = db.TrackingDetails.Find(Guid.Parse(item));
                    ReturnDetail detail = new ReturnDetail()
                    {
                        CreatedAt=DateTime.Now,
                        CreatedBy=user.Staff.UserName,
                        Id=Guid.NewGuid(),
                        PackageReturnId=model.Id,
                        StoregeJPId=trackingDetail.StoregeJPId,
                        TrackingSubCode=trackingDetail.TrackingSubCode,
                        Weigh=trackingDetail.Weigh,
                        TrackingDetailId=trackingDetail.Id,
                        Size=trackingDetail.Size,
                        UpdatedAt=DateTime.Now,
                        UpdatedBy=user.Staff.UserName
                    };
                    db.ReturnDetails.Add(detail);
                }
                db.SaveChanges();
                return Json(new { message = "Xử lý dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xử lý dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult RemoveItems(Guid id, string ids)
        {
            try
            {
                PackageReturn model = db.PackageReturns.Find(id);
                foreach (var item in ids.Split(','))
                {
                    var detail = db.ReturnDetails.Find(Guid.Parse(item));
                    db.ReturnDetails.Remove(detail);
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
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
                    imagePath = string.Format("~/Uploads/PackageReturn/{0}", imageNameReturn);
                    System.IO.File.WriteAllBytes(Server.MapPath(imagePath), ConvertHexToBytes(hexString));
                    Session["ImagePackageReturn"] = imageNameReturn;
                }
            }
            return Content(imagePath);
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
        [HttpPost]
        public ActionResult UpdateStatus(Guid id, int status)
        {
            try
            {
                var model = db.PackageReturns.Find(id);
                model.StatusId = status;
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult LoadImageUpload()
        {
            return Json(new { message = Session["ImagePackageReturn"], status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}