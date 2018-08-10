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
    public class DeliveryAddressesController : ManagementSystemController
    {
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách địa chỉ nhận hàng";
            ViewBag.key = key;
            ViewBag.sort = sort;
            ViewBag.page = page;
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id");
            var item = db.DeliveryAddresses.OrderByDescending(n => n.CreatedAt);
            return View(Pager<DeliveryAddress>.CreatePagging(item, page, 10));
        }
        public ActionResult AjaxIndex(int page = 1,string agencyCode="", string code = "", string address = "", string data_sort = "")
        {
            ViewBag.Title = "Danh sách địa chỉ nhận hàng";
            ViewBag.key = agencyCode;
            var item = db.DeliveryAddresses.OrderByDescending(n => n.CreatedAt);
            #region search
            if (agencyCode != "")
            {
                item = item.Where(n => n.AgencyId== agencyCode).OrderByDescending(n => n.CreatedAt);
            }
            if (code != "")
            {
                item = item.Where(n => n.DeliveryCode.Contains(code)).OrderByDescending(n => n.CreatedAt);
            }
            if (address != "")
            {
                item = item.Where(n => n.Address.Contains(address)).OrderByDescending(n => n.CreatedAt);
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
                    case "agency":
                        {
                            item = item.OrderBy(n => n.Agency.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Agency.Name);
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
            var lstReturn = Pager<DeliveryAddress>.CreatePagging(item, page, 10);
            return PartialView("~/Views/DeliveryAddresses/_ItemOfPage.cshtml", lstReturn);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.DeliveryAddresses.Remove(db.DeliveryAddresses.Find(id));
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
                    db.DeliveryAddresses.Remove(db.DeliveryAddresses.Find(Guid.Parse(id)));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        // GET: WareHouseInfo/Create
        public ActionResult Add()
        {
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", user.Agency.Id);
            return View();
        }

        // POST: WareHouseInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(DeliveryAddress model)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                db.DeliveryAddresses.Add(model);
                db.SaveChanges();
                return Content(javasctipt_add("/DeliveryAddresses", "Thêm dữ liệu thành công"));
            }
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", model.AgencyId);
            return Content(javasctipt_add("/DeliveryAddresses", "Thêm dữ liệu thất bại"));
        }

        // GET: WareHouseInfo/Edit/5
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryAddress model = db.DeliveryAddresses.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", model.AgencyId);
            return View(model);
        }

        // POST: WareHouseInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DeliveryAddress model)
        {
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = user.Staff.UserName;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/DeliveryAddresses", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/DeliveryAddresses", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu"));
                }
            }
            ViewBag.AgencyId = new SelectList(db.Agencies, "Id", "Id", model.AgencyId);
            return Content(javasctipt_add("/DeliveryAddresses", "Cập nhật dữ liệu thất bại"));
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
