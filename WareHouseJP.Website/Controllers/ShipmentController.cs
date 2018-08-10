using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class ShipmentController : ManagementSystemController
    {
        public JsonResult CheckShipmentName(string ShipmentName, Guid? Id)
        {
            var isValid = true;
            if (Id == null)
            {
                if (db.Shipments.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ShipmentName == ShipmentName).Count() > 0)
                {
                    isValid = false;
                }
            }
            else
            {
                if (db.Shipments.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ShipmentName == ShipmentName && x.Id != Id).Count() > 0)
                {
                    isValid = false;
                }
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckTrackingCode(string TrackingCode, Guid? Id)
        {
            var isValid = true;
            if (Id == null)
            {
                if (db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.TrackingCode == TrackingCode).Count() > 0)
                {
                    isValid = false;
                }
            }
            else
            {
                if (db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.TrackingCode == TrackingCode && x.Id != Id).Count() > 0)
                {
                    isValid = false;
                }
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        // GET: Shipment
        public ActionResult Index(int page = 1,int sort = -1)
        {
            ViewBag.Title = "Danh sách lô hàng";
            ViewBag.sort = sort;
            ViewBag.page = page;
            var status = StatusUtils.GetStatus(0);
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.StatusId = new SelectList(status, "Value", "Text", sort);
            if (sort != 0)
            {
                return View(Pager<Shipment>.CreatePagging(db.Shipments.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt), page, 10));
            }
            else
            {
                return View(Pager<Shipment>.CreatePagging(db.Shipments.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt), page, 10));
            }
        }
        public ActionResult Package(Guid id,string key="",int page = 1, int sort = 0)
        {
            var shipment = db.Shipments.Find(id);
            ViewBag.Shipment = shipment;
            ViewBag.Title = shipment.ShipmentName;

            ViewBag.KeyPackage = key;
            ViewBag.SortPackage = sort;
            ViewBag.ShipmentId = id;
            ViewBag.PagePackage = page;
            var status = StatusUtils.GetStatus(1);
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.PackageStatusId = new SelectList(status, "Value", "Text", sort);
            var item = db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n=>n.ShipmentId==id).OrderByDescending(n => n.CreatedAt);
            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }
            if (key != "")
            {
                item = item.Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
            }

            ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", "");
            ViewBag.SendHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.TotalCount = item.Count();
            ViewBag.TotalWeigh = item.Where(n=>n.Weigh!=null).Sum(n => n.Weigh);
            try
            {
                ViewBag.TotalItems = item.Sum(m => m.AgencyPackageItems.Count());
            }catch { ViewBag.TotalItems = 0; }
            try
            {
                ViewBag.TotalItemQuantitys = item.Sum(m => m.AgencyPackageItems.Sum(n => n.ItemQuantity));
            }
            catch { ViewBag.TotalItemQuantitys = 0; }
            return View(Pager<AgencyPackage>.CreatePagging(item, page, 10));
        }
        public ActionResult Add()
        {
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            string code = "LH"+DateTime.Now.ToString("yyyyMMddhhmmss");
            return View(new Shipment() { Code = code, ReceivedDate = DateTime.Now, ReceivedHour = DateTime.Now.ToString("HH:mm tt") });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Shipment model)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.StatusId = -1;
                db.Shipments.Add(model);
                db.SaveChanges();
                return Content(javasctipt_add("/Shipment","Thêm dữ liệu thành công"));
            }
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", model.ReceivedHour);
            return Content(javasctipt_add("/Shipment", "Thêm dữ liệu thất bại"));
        }
        // GET: Shipment/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.Shipments.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(0), "Value", "Text", model.StatusId);
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", model.ReceivedHour);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Shipment model,string actionlink = "")
        {
            model.UpdatedBy = user.Staff.UserName;
            model.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                model.AgencyId = user.Agency.Id;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                if (actionlink != "") { return Content(javasctipt_add("/Shipment/Package/" + model.Id, "Cập nhật dữ liệu thành công")); }
                else return Content(javasctipt_add("/Shipment", "Cập nhật dữ liệu thành công"));
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(0), "Value", "Text", model.StatusId);
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", model.ReceivedHour);
            if (actionlink != "") { return Content(javasctipt_add("/Shipment/Package/" + model.Id, "Cập nhật dữ liệu thất bại")); }
            else return Content(javasctipt_add("/Shipment", "Cập nhật dữ liệu thất bại"));
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var Shipment = db.Shipments.Find(id);
                var AgencyPackages = Shipment.AgencyPackages.ToList();
                foreach (var item in AgencyPackages)
                {
                    var AgencyPackageItems = item.AgencyPackageItems.ToList();
                    foreach (var pack in item.AgencyPackageItems)
                    {
                        db.AgencyPackageItems.Remove(pack);
                    }
                    db.AgencyPackages.Remove(item);
                }
                db.Shipments.Remove(Shipment);
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex){ return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult DeleteMultiple(string id)
        {
            try
            {
                foreach (var items in id.Split(','))
                {
                    var Shipment = db.Shipments.Find(Guid.Parse(items));
                    var AgencyPackages = Shipment.AgencyPackages.ToList();
                    foreach (var item in AgencyPackages)
                    {
                        var AgencyPackageItems = item.AgencyPackageItems.ToList();
                        foreach (var pack in item.AgencyPackageItems)
                        {
                            db.AgencyPackageItems.Remove(pack);
                        }
                        db.AgencyPackages.Remove(item);
                    }
                    db.Shipments.Remove(Shipment);
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        
    }
}