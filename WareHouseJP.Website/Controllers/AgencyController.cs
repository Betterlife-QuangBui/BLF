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
    public class AgencyController : ManagementSystemController
    {
        // GET: Agency
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách đại lý";
            ViewBag.key = key;
            ViewBag.sort = sort;
            ViewBag.page = page;
            var status = StatusUtils.GetSettingStatus();
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.IsActive = new SelectList(status, "Value", "Text", sort);

            var item = db.Agencies.OrderByDescending(n => n.CreatedAt);
            return View(Pager<Agency>.CreatePagging(item, page, 10));
        }
        public ActionResult AjaxIndex(int page = 1,string code="", string name = "", string address = "", string phone = "", string email = "", string hotline = "", string fax = "", string status = "0", string data_sort = "")
        {
            ViewBag.Title = "Danh sách đại lý";
            ViewBag.key = name;
            var item = db.Agencies.OrderByDescending(n => n.CreatedAt);
            #region search
            if (code != "")
            {
                item = item.Where(n => n.Id.Contains(code)).OrderByDescending(n => n.CreatedAt);
            }
            if (name != "")
            {
                item = item.Where(n => n.Name.Contains(name)).OrderByDescending(n => n.CreatedAt);
            }
            if (address != "")
            {
                item = item.Where(n => n.Address.Contains(address)).OrderByDescending(n => n.CreatedAt);
            }
            if (email != "")
            {
                item = item.Where(n => n.Email.Contains(email)).OrderByDescending(n => n.CreatedAt);
            }
            if (phone != "")
            {
                item = item.Where(n => n.Phone.Contains(phone)).OrderByDescending(n => n.CreatedAt);
            }
            if (hotline != "")
            {
                item = item.Where(n => n.Hotline.Contains(hotline)).OrderByDescending(n => n.CreatedAt);
            }
            if (fax != "")
            {
                item = item.Where(n => n.Fax.Contains(fax)).OrderByDescending(n => n.CreatedAt);
            }
            if (status != "0")
            {
                bool status_flag = status == "1" ? false : true;
                item = item.Where(n => n.IsActive == status_flag).OrderByDescending(n => n.CreatedAt);
            }
            #endregion
            #region sort
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "code":
                        {
                            item = item.OrderBy(n => n.Id);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Id);
                            }
                            break;
                        }
                    case "name":
                        {
                            item = item.OrderBy(n => n.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Name);
                            }
                            break;
                        }
                    case "address":
                        {
                            item = item.OrderBy(n => n.Address);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Address);
                            }
                            break;
                        }
                    case "phone":
                        {
                            item = item.OrderBy(n => n.Phone);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Phone);
                            }
                            break;
                        }
                    case "hotline":
                        {
                            item = item.OrderBy(n => n.Hotline);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Hotline);
                            }
                            break;
                        }
                    case "email":
                        {
                            item = item.OrderBy(n => n.Email);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Email);
                            }
                            break;
                        }
                    case "fax":
                        {
                            item = item.OrderBy(n => n.Fax);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Fax);
                            }
                            break;
                        }
                    case "status":
                        {
                            item = item.OrderBy(n => n.IsActive);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.IsActive);
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
            var lstReturn = Pager<Agency>.CreatePagging(item, page, 10);
            return PartialView("~/Views/Agency/_ItemOfPage.cshtml", lstReturn);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                db.Agencies.Remove(db.Agencies.Find(id));
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
                    db.Agencies.Remove(db.Agencies.Find(id));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        // GET: WareHouseInfo/Create
        public ActionResult Add()
        {
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text");
            return View();
        }

        // POST: WareHouseInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Agency model)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.IsDeleted = false;

            if (ModelState.IsValid)
            {
                db.Agencies.Add(model);
                db.SaveChanges();
                return Content(javasctipt_add("/Agency", "Thêm dữ liệu thành công"));
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            return Content(javasctipt_add("/Agency", "Thêm dữ liệu thất bại"));
        }

        // GET: WareHouseInfo/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agency model = db.Agencies.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            return View(model);
        }

        // POST: WareHouseInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Agency model)
        {
            model.UpdatedAt = DateTime.Now;
            model.IsDeleted = false;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/Agency", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/Agency", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu"));
                }
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            return Content(javasctipt_add("/Agency", "Cập nhật dữ liệu thất bại"));
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