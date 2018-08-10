using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class AirInfoController : ManagementSystemController
    {
        UserPage user = new UserPage();
        public ActionResult Index(int page = 1, string key = "")
        {
            ViewBag.Title = "Danh sách chuyến bay";
            ViewBag.key = key;
            return View(Pager<AirInfo>.CreatePagging(db.AirInfoes.AsEnumerable().Where(n => String.Format("{0}", n.Name.ChangeUnsigned()).Contains(String.Format("{0}", key.ChangeUnsigned()))).AsQueryable().OrderByDescending(n => n.CreatedAt), page));
        }
        [HttpPost]
        public ActionResult Delete(string id = "0")
        {
            try
            {
                db.AirInfoes.Remove(db.AirInfoes.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        // GET: AirInfo/Create
        public ActionResult Add()
        {
            return View(new AirInfo() {
                FlightDateTo=DateTime.Now,
                FlightHourTo=DateTime.Now.ToString("HH:mm tt"),
                FlightDateFrom = DateTime.Now,
                FlightHourFrom = DateTime.Now.ToString("HH:mm tt")
            });
        }

        // POST: AirInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AirInfo airInfo)
        {
            airInfo.CreatedAt = airInfo.UpdatedAt = DateTime.Now;
            airInfo.UpdatedBy = airInfo.CreatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                 try
                {
                    db.AirInfoes.Add(airInfo);
                    db.SaveChanges();
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }

            return View(airInfo);
        }

        // GET: AirInfo/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AirInfo airInfo = db.AirInfoes.Find(id);
            if (airInfo == null)
            {
                return HttpNotFound();
            }
            return View(airInfo);
        }

        // POST: AirInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AirInfo airInfo)
        {
            airInfo.UpdatedBy = user.Staff.UserName;
            airInfo.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                 try
                {
                    db.Entry(airInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            return View(airInfo);
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
