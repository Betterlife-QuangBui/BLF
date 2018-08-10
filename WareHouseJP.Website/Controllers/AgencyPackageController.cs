using System;
using System.Collections.Generic;
using System.Data;
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
    public class AgencyPackageController : ManagementSystemController
    {
        // GET: AgencyPackage
        public ActionResult Index(int page = 1, string key = "", string sort = "ngaytao")
        {
            ViewBag.Title = "Danh sách kiện hàng";
            ViewBag.key = key; ViewBag.sort = sort;
            #region backup
            //switch (sort)
            //{
            //    case "ngaygui": return View(Pager<AgencyPackage>.CreatePagging(db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.SentDate), page));
            //    case "ngaynhan": return View(Pager<AgencyPackage>.CreatePagging(db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.ReceivedDate), page));
            //    case "trangthai": return View(Pager<AgencyPackage>.CreatePagging(db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.StatusId), page));
            //    case "matracking": return View(Pager<AgencyPackage>.CreatePagging(db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.TrackingCode), page));
            //    default:
            //        return View(Pager<AgencyPackage>.CreatePagging(db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt), page));
            //}
            #endregion
            return View(Pager<AgencyPackage>.CreatePagging(db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt), page,10));
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.AgencyPackageItems.RemoveRange(db.AgencyPackageItems.Where(n => n.AgencyPackageId == id));
                db.AgencyPackages.Remove(db.AgencyPackages.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult DeleteMultiple(string id)
        {
            try
            {
                foreach (var item in id.Split(','))
                {
                    var ids = Guid.Parse(item);
                    db.AgencyPackageItems.RemoveRange(db.AgencyPackageItems.Where(n => n.AgencyPackageId == ids));
                    db.AgencyPackages.Remove(db.AgencyPackages.Find(ids));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        // GET: AgencyPackage/Create
        public ActionResult Add(Guid id)
        {
            ViewBag.AgencyId = new SelectList(db.Agencies.Where(n => n.Id == user.Agency.Id), "Id", "Name");
            ViewBag.SendHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            ViewBag.AgencyId = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n=>n.IsActive==true), "Id", "Name","");
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(1), "Value", "Text", "");
            return View(new AgencyPackage() {ShipmentId=id, SentDate = DateTime.Now, SendHour = DateTime.Now.ToString("HH:mm"), ReceivedDate = DateTime.Now, ReceivedHour = DateTime.Now.ToString("HH:mm") });
        }
        [HttpPost]
        public ActionResult CheckExistTrackingCode(string tracking)
        {
            try
            {
                if (db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode == tracking).Count() > 0)
                {
                    return Json(new { message = "Xóa dữ liệu thành công !", status = false }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = true }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AgencyPackage agencyPackage)
        {
            agencyPackage.CreatedAt =agencyPackage.UpdatedAt = DateTime.Now;
            agencyPackage.CreatedBy = agencyPackage.UpdatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                agencyPackage.Id = Guid.NewGuid();
                //agencyPackage.StatusId = 2;
                agencyPackage.AgencyId = user.Agency.Id;
                db.AgencyPackages.Add(agencyPackage);
                db.SaveChanges();
                ViewBag.ShipmentId = agencyPackage.ShipmentId;
                return Content(javasctipt_add("/Shipment/Package/"+agencyPackage.ShipmentId, "Thêm dữ liệu thành công"));
            }
            //ViewBag.SendHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", agencyPackage.SendHour);
            //ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", agencyPackage.ReceivedHour);
            //ViewBag.AgencyId = new SelectList(db.Agencies.Where(n => n.Id == user.Agency.Id), "Id", "Name", agencyPackage.AgencyId);
            //ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", agencyPackage.DeliveryId);
            //ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(1), "Value", "Text",agencyPackage.TrackingStatusId);
            return Content(javasctipt_add("/Shipment/Package/" + agencyPackage.ShipmentId, "Thêm dữ liệu thất bại"));
        }

        // GET: AgencyPackage/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgencyPackage agencyPackage = db.AgencyPackages.Find(id);
            if (agencyPackage == null)
            {
                return HttpNotFound();
            }
            ViewBag.SendHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", agencyPackage.SendHour);
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", agencyPackage.ReceivedHour);
            ViewBag.AgencyId = new SelectList(db.Agencies.Where(n => n.Id == user.Agency.Id), "Id", "Name", agencyPackage.AgencyId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(1), "Value", "Text", agencyPackage.TrackingStatusId);
            ViewBag.DeliveryId = new SelectList(db.DeliveryComs, "Id", "Name", agencyPackage.DeliveryId);
            return View(agencyPackage);
        }

        // POST: AgencyPackage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AgencyPackage agencyPackage, string actionlink = "")
        {
            agencyPackage.UpdatedBy = user.Staff.UserName;
            agencyPackage.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                agencyPackage.AgencyId = user.Agency.Id;
                db.Entry(agencyPackage).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ShipmentId = agencyPackage.ShipmentId;
                if (actionlink != "") { return Content(javasctipt_add("/AgencyPackageItem/Index/" + agencyPackage.Id, "Cập nhật dữ liệu thành công")); }
                else return Content(javasctipt_add("/Shipment/Package/" + agencyPackage.ShipmentId, "Cập nhật dữ liệu thành công"));
            }
            ViewBag.SendHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", agencyPackage.SendHour);
            ViewBag.ReceivedHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", agencyPackage.ReceivedHour);
            ViewBag.AgencyId = new SelectList(db.Agencies.Where(n => n.Id == user.Agency.Id), "Id", "Name", agencyPackage.AgencyId);
            ViewBag.DeliveryId = new SelectList(db.DeliveryComs.Where(n => n.IsActive == true), "Id", "Name", agencyPackage.DeliveryId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(1), "Value", "Text", agencyPackage.TrackingStatusId);
            if (actionlink != "") { return Content(javasctipt_add("/AgencyPackageItem/Index/" + agencyPackage.Id, "Cập nhật dữ liệu thất bại")); }
            else return Content(javasctipt_add("/Shipment/Package/" + agencyPackage.ShipmentId, "Cập nhật dữ liệu thất bại"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
