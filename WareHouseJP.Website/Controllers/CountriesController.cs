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
    public class CountriesController : ManagementSystemController
    {

        // GET: WareHouseInfo
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách quốc gia";
            ViewBag.key = key;
            ViewBag.sort = sort;
            ViewBag.page = page;

            var item = db.Countries.OrderByDescending(n => n.Id);
            return View(Pager<Country>.CreatePagging(item, page, 10));
        }
        public ActionResult AjaxIndex(int page = 1, string shortname = "", string name = "", string data_sort = "")
        {
            ViewBag.Title = "Danh sách quốc gia";
            ViewBag.key = name;
            var item = db.Countries.OrderByDescending(n => n.Id);
            #region search
            if (name != "")
            {
                item = item.Where(n => n.Name.Contains(name)).OrderByDescending(n => n.Id);
            }
            if (shortname != "")
            {
                item = item.Where(n => n.NameShore.Contains(shortname)).OrderByDescending(n => n.Id);
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
                    case "shortName":
                        {
                            item = item.OrderBy(n => n.NameShore);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.NameShore);
                            }
                            break;
                        }
                    case "id":
                        {
                            item = item.OrderBy(n => n.Id);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Id);
                            }
                            break;
                        }
                }
            }
            #endregion
            var lstReturn = Pager<Country>.CreatePagging(item, page, 10);
            return PartialView("~/Views/countries/_ItemOfPage.cshtml", lstReturn);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                db.Countries.Remove(db.Countries.Find(id));
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
                    db.Countries.Remove(db.Countries.Find(int.Parse(id)));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        // GET: WareHouseInfo/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: WareHouseInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Country model)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Add(model);
                db.SaveChanges();
                return Content(javasctipt_add("/countries", "Thêm dữ liệu thành công"));
            }
            return Content(javasctipt_add("/countries", "Thêm dữ liệu thất bại"));
        }

        // GET: WareHouseInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country model = db.Countries.Find(id);
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
        public ActionResult Edit(Country model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/countries", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/countries", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu"));
                }
            }
            return Content(javasctipt_add("/countries", "Cập nhật dữ liệu thất bại"));
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
