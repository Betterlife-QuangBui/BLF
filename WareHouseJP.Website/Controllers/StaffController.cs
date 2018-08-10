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
    public class StaffController : ManagementSystemController
    {
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách tài khoản";
            ViewBag.key = key;
            ViewBag.sort = sort;
            ViewBag.page = page;
            var status = StatusUtils.GetSettingStatus();
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.IsActive = new SelectList(status, "Value", "Text", sort);

            var item = db.Staffs.OrderByDescending(n => n.CreatedAt);
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name");
            return View(Pager<Staff>.CreatePagging(item, page, 10));
        }
        public ActionResult AjaxIndex(int page = 1,int role=0, string loginname = "", string fullname = "", string email = "", string phone = "", string gender = "0", string status = "0", string data_sort = "")
        {
            ViewBag.Title = "Danh sách tài khoản";
            ViewBag.key = loginname;
            var item = db.Staffs.OrderByDescending(n => n.CreatedAt);
            #region search
            if (role != 0)
            {
                item = item.Where(n => n.RoleId==role).OrderByDescending(n => n.CreatedAt);
            }
            if (loginname != "")
            {
                item = item.Where(n => n.UserName.Contains(loginname)).OrderByDescending(n => n.CreatedAt);
            }
            if (fullname != "")
            {
                item = item.Where(n => n.Name.Contains(fullname)).OrderByDescending(n => n.CreatedAt);
            }
            if (email != "")
            {
                item = item.Where(n => n.Email.Contains(email)).OrderByDescending(n => n.CreatedAt);
            }
            if (phone != "")
            {
                item = item.Where(n => n.Phone.Contains(phone)).OrderByDescending(n => n.CreatedAt);
            }
            if (gender != "0")
            {
                bool gender_flag = gender == "1" ? false : true;
                item = item.Where(n => n.Gender== gender_flag).OrderByDescending(n => n.CreatedAt);
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
                    case "rolename":
                        {
                            item = item.OrderBy(n => n.Role.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Role.Name);
                            }
                            break;
                        }
                    case "loginname":
                        {
                            item = item.OrderBy(n => n.UserName);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.UserName);
                            }
                            break;
                        }
                    case "fullname":
                        {
                            item = item.OrderBy(n => n.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Name);
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
                    case "phone":
                        {
                            item = item.OrderBy(n => n.Phone);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Phone);
                            }
                            break;
                        }
                    case "gender":
                        {
                            item = item.OrderBy(n => n.Gender);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Gender);
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
            var lstReturn = Pager<Staff>.CreatePagging(item, page, 10);
            return PartialView("~/Views/Staff/_ItemOfPage.cshtml", lstReturn);
        }
        public ActionResult Add()
        {
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name");
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text");
            ViewBag.Gender = new SelectList(StatusUtils.GetGender(), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Staff model)
        {
            model.IsDeleted = false;
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {

                try
                {
                    if (model.Email == "" || model.Email == null)
                    {
                        model.Email = model.UserName;
                    }
                    model.Password = SimpleEncrypt.Encrypt(model.Password, true);
                    db.Staffs.Add(model);
                    db.SaveChanges();
                    return Content(javasctipt_add("/Staff", "Thêm dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/Staff", "Thêm dữ liệu thất bại"));
                }
            }
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name", model.RoleId);
            ViewBag.Gender = new SelectList(StatusUtils.GetGender(), "Value", "Text",model.Gender);
            return Content(javasctipt_add("/Staff", "Thêm dữ liệu thất bại"));
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff model = db.Staffs.Find(id.FromBase64());
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name", model.RoleId);
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            ViewBag.Gender = new SelectList(StatusUtils.GetGender(), "Value", "Text", model.Gender);
            return View(model);
        }

        // POST: AgencyAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Staff model, string ReNewPassword)
        {
            model.IsDeleted = false;
            model.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {

                try
                {
                    if (model.Email == "" || model.Email == null)
                    {
                        model.Email = model.UserName;
                    }
                    if (ReNewPassword != null && ReNewPassword.Trim().Length > 0)
                    {
                        model.Password = SimpleEncrypt.Encrypt(ReNewPassword, true);
                    }
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/Staff", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception)
                {
                    return Content(javasctipt_add("/Staff", "Cập nhật dữ liệu thất bại"));
                }
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name", model.RoleId);
            var status = StatusUtils.GetSettingStatus();
            ViewBag.IsActive = new SelectList(status, "Value", "Text", model.IsActive);
            ViewBag.Gender = new SelectList(StatusUtils.GetGender(), "Value", "Text", model.Gender);
            return Content(javasctipt_add("/Staff", "Cập nhật dữ liệu thất bại"));
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                db.Staffs.Remove(db.Staffs.Find(id.FromBase64()));
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
                    db.Staffs.Remove(db.Staffs.Find(id.FromBase64()));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(String returnUrl)
        {
            Session["AccBetterLife"] = null;
            HttpCookie ckiUser = Request.Cookies["CkBetterLife"];
            if (ckiUser != null)
            {
                ckiUser.Expires = DateTime.Now.AddDays(-20);
                Response.Cookies.Add(ckiUser);
            }
            HttpCookie CkAgencyBetterLife = Request.Cookies["CkAgencyBetterLife"];
            if (CkAgencyBetterLife != null)
            {
                CkAgencyBetterLife.Expires = DateTime.Now.AddDays(-20);
                Response.Cookies.Add(CkAgencyBetterLife);
            }
            return RedirectToAction("Index", "Security");
        }


        public ActionResult UpdateInfo(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id.FromBase64());
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name", staff.RoleId);
            return View(staff);
        }

        // POST: AgencyAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(Staff staff, string ReNewPassword)
        {
            staff.IsDeleted = false;
            staff.IsActive = true;
            staff.UpdatedAt = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                try
                {
                    var upImage = Request.Files["upImage"];
                    if (upImage.ContentLength > 0 && upImage != null)
                    {
                        string fileName = upImage.FileName.DoiTenHinh();
                        upImage.SaveAs(Server.MapPath("~/images/Staff/" + fileName));
                        staff.Avatar = fileName;
                    }
                    if (ReNewPassword != null && ReNewPassword.Trim().Length > 0)
                    {
                        staff.Password = SimpleEncrypt.Encrypt(ReNewPassword, true);
                    }
                    db.Entry(staff).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["AccBetterLife"] = staff;
                    #region create cookies
                    HttpCookie ckiUser = new HttpCookie("CkBetterLife");
                    ckiUser["Id"] = staff.UserName;
                    ckiUser["Pw"] = SimpleEncrypt.Encrypt(staff.Password, true);
                    ckiUser.Expires = DateTime.Now.AddHours(24);
                    Response.Cookies.Add(ckiUser);
                    #endregion
                    TempData["Message"] = "Cập nhật thông tin thành công";
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                    TempData["Message"] = "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu";
                }
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(n => n.Name.Contains("Better")), "Id", "Name", staff.RoleId);
            return View(staff);
        }
    }
}