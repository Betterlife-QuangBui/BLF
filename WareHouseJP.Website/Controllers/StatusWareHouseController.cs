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
    public class StatusWareHouseController : ManagementSystemController
    {
        UserPage user = new UserPage();
        // GET: StatusWareHouse
        public ActionResult Index(int page = 1, string key = "")
        {
            ViewBag.Title = "Danh sách trạng thái";
            ViewBag.key = key;
            return View(Pager<StatusWareHouse>.CreatePagging(db.StatusWareHouses.AsEnumerable().Where(n => String.Format("{0}", n.Name.ChangeUnsigned()).Contains(String.Format("{0}", key.ChangeUnsigned()))).AsQueryable().OrderByDescending(n => n.CreatedAt), page));
        }
        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            try
            {
                db.StatusWareHouses.Remove(db.StatusWareHouses.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        // GET: StatusWareHouse/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: StatusWareHouse/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StatusWareHouse statusWareHouse)
        {
            statusWareHouse.CreatedAt = statusWareHouse.UpdatedAt = DateTime.Now;
            statusWareHouse.CreatedBy = statusWareHouse.UpdatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                 try
                {
                    db.StatusWareHouses.Add(statusWareHouse);
                    db.SaveChanges();
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }

            return View(statusWareHouse);
        }

        // GET: StatusWareHouse/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusWareHouse statusWareHouse = db.StatusWareHouses.Find(id);
            if (statusWareHouse == null)
            {
                return HttpNotFound();
            }
            return View(statusWareHouse);
        }

        // POST: StatusWareHouse/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StatusWareHouse statusWareHouse)
        {
            statusWareHouse.UpdatedAt = DateTime.Now;
            statusWareHouse.UpdatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                 try
                {
                    db.Entry(statusWareHouse).State = EntityState.Modified;
                    db.SaveChanges();
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            return View(statusWareHouse);
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
