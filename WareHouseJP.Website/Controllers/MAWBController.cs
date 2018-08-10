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
    public class MAWBController : ManagementSystemController
    {
        // GET: WareHouseInfo
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách MAWB";
            ViewBag.key = key;
            ViewBag.sort = sort;
            ViewBag.page = page;
            var status = StatusUtils.GetSettingStatus();
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.IsActive = new SelectList(status, "Value", "Text", sort);
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id");
            var item = db.MAWBs.OrderByDescending(n => n.CreatedAt);
            return View(Pager<MAWB>.CreatePagging(item, page, 10));
        }
        public ActionResult AjaxIndex(int page = 1,string agencyCode="",string code="", string name = "", string address = "", string phone = "", string email = "", string hotline = "", string fax = "", string status = "0", string data_sort = "")
        {
            ViewBag.Title = "Danh sách MAWB";
            ViewBag.key = name;
            var item = db.MAWBs.OrderByDescending(n => n.CreatedAt);
            #region search
            if (agencyCode != "")
            {
                item = item.Where(n => n.AgencyId==agencyCode).OrderByDescending(n => n.CreatedAt);
            }
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
                item = item.Where(n => n.Tel.Contains(hotline)).OrderByDescending(n => n.CreatedAt);
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
                    case "agency":
                        {
                            item = item.OrderBy(n => n.AgencyId);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.AgencyId);
                            }
                            break;
                        }
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
                            item = item.OrderBy(n => n.Tel);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Tel);
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
            var lstReturn = Pager<MAWB>.CreatePagging(item, page, 10);
            return PartialView("~/Views/MAWB/_ItemOfPage.cshtml", lstReturn);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                db.MAWBs.Remove(db.MAWBs.Find(id));
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
                    db.MAWBs.Remove(db.MAWBs.Find(id));
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
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", user.Agency.Id);
            return View();
        }

        // POST: WareHouseInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(MAWB model)
        {
            model.IsDeleted = false;
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = model.CreatedBy = user.Staff.UserName;

            if (ModelState.IsValid)
            {
                db.MAWBs.Add(model);
                db.SaveChanges();
                return Content(javasctipt_add("/MAWB", "Thêm dữ liệu thành công"));
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", model.AgencyId);
            return Content(javasctipt_add("/MAWB", "Thêm dữ liệu thất bại"));
        }

        // GET: WareHouseInfo/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAWB model = db.MAWBs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", model.AgencyId);
            return View(model);
        }

        // POST: WareHouseInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MAWB model)
        {
            model.IsDeleted = false;
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/MAWB", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/MAWB", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu"));
                }
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", model.AgencyId);
            return Content(javasctipt_add("/MAWB", "Cập nhật dữ liệu thất bại"));
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
