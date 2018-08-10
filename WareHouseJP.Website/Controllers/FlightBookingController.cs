using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class FlightBookingController : ManagementSystemController
    {
        UserPage user = new UserPage();
        public ActionResult Index(int page = 1, string key = "")
        {
            ViewBag.Title = "Danh sách booking"; 
            ViewBag.key = key;
            if (key.Length == 0) { return View(Pager<FlightBooking>.CreatePagging(db.FlightBookings.OrderByDescending(n => n.CreatedAt), page)); }
            else
            {
                return View(Pager<FlightBooking>.CreatePagging(db.FlightBookings.Where(n => n.Code.Contains(key)).OrderByDescending(n => n.CreatedAt), page));
            }
        }
        [HttpPost]
        public ActionResult EditAir(FlightBooking model, string[] MAWB, string[] HAWB)
        {
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = user.Staff.UserName;
            model.StaffId = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;
            if (MAWB == null) { MAWB = new string[] { }; }
            else { MAWB = MAWB[0].Split(','); }
            if (HAWB == null) { HAWB = new string[] { }; }
            else { HAWB = HAWB[0].Split(','); }
            if (ModelState.IsValid)
            {
                try
                {
                    model.StaffIdUpdate = user.Staff.UserName;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    foreach (var ha in db.FlightBookingHAWBs.Where(n => n.FlightBookingId == model.Id && n.AgencyId == user.Agency.Id))
                    {
                        db.FlightBookingHAWBs.Remove(ha);
                    }
                    foreach (var ma in db.FlightBookingMAWBs.Where(n => n.FlightBookingId == model.Id && n.AgencyId == user.Agency.Id))
                    {
                        db.FlightBookingMAWBs.Remove(ma);
                    }

                    foreach (var ha in HAWB)
                    {
                        db.FlightBookingHAWBs.Add(new FlightBookingHAWB()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            FlightBookingId = model.Id,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            HAWBId = ha,
                            AgencyId = user.Agency.Id,
                            IsUse = true
                        });
                    }
                    foreach (var ma in MAWB)
                    {
                        db.FlightBookingMAWBs.Add(new FlightBookingMAWB()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            FlightBookingId = model.Id,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            MAWBId = ma,
                            AgencyId = user.Agency.Id,
                            IsUse = true
                        });
                    }
                    db.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            return Json(new { message = "Lưu chuyến bay thành công", status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveAirBooking(AirInfo model,string FlightBookingId)
        {
            try
            {
                //check exist
                var FlightBooking = db.FlightBookings.Find(Guid.Parse(FlightBookingId));
                try
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user.Staff.UserName;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    model.CreatedAt = model.UpdatedAt = DateTime.Now;
                    model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
                    db.AirInfoes.Add(model);
                    FlightBooking.AirId = model.Id;
                    db.SaveChanges();
                }
                return Json(new { message = "Lưu chuyến bay thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình lưu dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult Upload()
        {
            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    fileName = fileName.DoiTenHinh();
                    var path = Path.Combine(Server.MapPath("~/images/FlightBooking/"), fileName);
                    file.SaveAs(path);
                    var FlightBooking = db.FlightBookings.Find(Guid.Parse(Request["FlightBookingId"]));
                    FlightBooking.Image = fileName;
                    db.SaveChanges();
                }
                return Json(new { message = "Upload hình ảnh thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình upload dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.FlightBookings.Remove(db.FlightBookings.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult Add()
        {
            ViewBag.Size = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name", "");
            return View(new FlightBooking() { BookingDate = DateTime.Now, BookingHour = DateTime.Now.ToString("HH:mm tt"), Size = "7" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FlightBooking model)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
            model.AgencyId = user.Agency.Id; model.StaffId = user.Staff.UserName;

            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = Guid.NewGuid();
                    model.StatusId = 13;
                    model.AutomaticTrackingCount = 0;
                    model.AutomaticWeigh = 0;
                    model.EnterManuallyTrackingCount = 0;
                    model.EnterManuallyWeigh = 0;
                    db.FlightBookings.Add(model);
                    db.SaveChanges();
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            ViewBag.Size = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name", model.Size);
            return View(model);
        }

        // GET: FlightBooking/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightBooking model = db.FlightBookings.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.AirId = new SelectList(db.AirInfoes.Select(n => new { Id = n.Id, Name = n.Id + " - " + n.Name }), "Id", "Name", model.AirId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(4), "Value", "Text", model.StatusId);
            ViewBag.FlightRouteId = new SelectList(db.FlightRoutes.OrderBy(n => n.CreatedAt), "Id", "Name", model.FlightRouteId);
            ViewBag.Size = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name", model.Size);
            ViewBag.MAWB = new SelectList(db.MAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Id");
            ViewBag.HAWB = new SelectList(db.HAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Id");
            return View(model);
        }

        // POST: FlightBooking/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FlightBooking model, string[] MAWB, string[] HAWB)
        {
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = user.Staff.UserName;
            model.StaffId = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;
            if (MAWB == null) { MAWB = new string[] { }; }
            if (HAWB == null) { HAWB = new string[] { }; }
            if (ModelState.IsValid)
            {
                try
                {
                    var upImage = Request.Files["upImage"];
                    if (upImage.ContentLength > 0 && upImage != null)
                    {
                        string fileName = upImage.FileName.DoiTenHinh();
                        upImage.SaveAs(Server.MapPath("~/images/FlightBooking/" + fileName));
                        model.Image = fileName;
                    }
                    model.StaffIdUpdate = user.Staff.UserName;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return PartialView("~/views/Message/Add.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            ViewBag.MAWB = new SelectList(db.MAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Id");
            ViewBag.HAWB = new SelectList(db.HAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Id");
            ViewBag.AirId = new SelectList(db.AirInfoes.Select(n => new { Id = n.Id, Name = n.Id + " - " + n.Name }), "Id", "Name", model.AirId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(4), "Value", "Text", model.StatusId);
            ViewBag.FlightRouteId = new SelectList(db.FlightRoutes.OrderBy(n => n.CreatedAt), "Id", "Name", model.FlightRouteId);
            ViewBag.Size = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name", model.Size);
            return View(model);
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
