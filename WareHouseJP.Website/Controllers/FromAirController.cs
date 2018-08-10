using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class FromAirController : ManagementSystemController
    {
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách nơi đi";
            ViewBag.key = key;
            ViewBag.sort = sort;
            ViewBag.page = page;
            var item = db.FromAirs.OrderByDescending(n => n.CreatedAt);
            return View(Pager<FromAir>.CreatePagging(item, page, 10));
        }
        public ActionResult AjaxIndex(int page = 1, string name = "", string data_sort = "")
        {
            ViewBag.Title = "Danh sách nơi đi";
            ViewBag.key = name;
            var item = db.FromAirs.OrderByDescending(n => n.CreatedAt);
            #region search
            if (name != "")
            {
                item = item.Where(n => n.Name.Contains(name)).OrderByDescending(n => n.CreatedAt);
            }
            #endregion
            #region sort
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "name":
                        {
                            item = item.OrderBy(n => n.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Name);
                            }
                            break;
                        }
                    case "created":
                        {
                            item = item.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }

            #endregion
            var lstReturn = Pager<FromAir>.CreatePagging(item, page, 10);
            return PartialView("~/Views/FromAir/_ItemOfPage.cshtml", lstReturn);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.FromAirs.Remove(db.FromAirs.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult DeleteMultiple(string ids)
        {
            try
            {
                foreach (var id in ids.Split(','))
                {
                    db.FromAirs.Remove(db.FromAirs.Find(Guid.Parse(id)));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }


        public ActionResult Add()
        {
            return View();
        }

        // POST: WareHouseInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FromAir model)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                db.FromAirs.Add(model);
                db.SaveChanges();
                return Content(javasctipt_add("/FromAir", "Thêm dữ liệu thành công"));
            }
            return Content(javasctipt_add("/FromAir", "Thêm dữ liệu thất bại"));
        }

        // GET: WareHouseInfo/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FromAir model = db.FromAirs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: WareHouseInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FromAir model)
        {
            model.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/FromAir", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/FromAir", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu"));
                }
            }
            return Content(javasctipt_add("/FromAir", "Cập nhật dữ liệu thất bại"));
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
