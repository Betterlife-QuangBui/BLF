using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Controllers
{
    public class ClearanceController : ManagementSystemController
    {
        UserPage user = new UserPage();
        public ActionResult _Booking(Guid? id)
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
            Tuple<FlightBooking, AirInfo> tuple = new Tuple<FlightBooking, AirInfo>(model, model.AirInfo == null ? new AirInfo() : model.AirInfo);
            return PartialView(tuple);
        }
        public ActionResult _Clearance(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightBooking FlightBooking = db.FlightBookings.Find(id);
            if (FlightBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(4), "Value", "Text", FlightBooking.StatusId);
            
            ViewBag.MAWB = new SelectList(db.MAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Id");
            ViewBag.HAWB = new SelectList(db.HAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Id");
            ViewBag.FlightBooking = FlightBooking;
            Tuple<Invoice, CustomsClearance> model = new Tuple<Invoice, CustomsClearance>(new Invoice(), new CustomsClearance());
            try
            {
                model = new Tuple<Invoice, CustomsClearance>(FlightBooking.Invoices != null && FlightBooking.Invoices.Count > 0 ? FlightBooking.Invoices.First() : new Invoice(), FlightBooking.CustomsClearances != null && FlightBooking.CustomsClearances.Count > 0 ? FlightBooking.CustomsClearances.First() : new CustomsClearance());
                if (model.Item2.ClearanceAirs == null || model.Item2.ClearanceAirs.Count <= 0)
                {
                    model.Item2.ClearanceAirs.Add(new ClearanceAir() { });
                }
                ViewBag.Size = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name", model.Item2.Size);
            }
            catch { ViewBag.Size = new SelectList(db.SizeTables.OrderBy(n => n.NoOrder), "Id", "Name"); }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult BookingInvoice(Invoice model)
        {
            try
            {
                try
                {
                    if (Guid.Empty != model.Id)
                    {
                        model.UpdatedAt = DateTime.Now;
                        model.UpdatedBy = user.Staff.UserName;
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        Invoice invoice = new Invoice()
                        {
                            AgencyId = user.Agency.Id,
                            CreatedAt = DateTime.Now,
                            Id = Guid.NewGuid(),
                            CreatedBy = user.Staff.UserName,
                            FlightBookingId = model.FlightBookingId,
                            InvoiceCode = model.InvoiceCode,
                            InvoiceDate = model.InvoiceDate,
                            InvoiceHour = model.InvoiceHour,
                            Notes = model.Notes,
                            StaffId = user.Staff.UserName,
                            StaffIdUpdate = user.Staff.UserName,
                            StatusId = model.StatusId,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = user.Staff.UserName
                        };
                        db.Invoices.Add(invoice);
                        db.SaveChanges();
                    }
                }
                catch
                {
                    Invoice invoice = new Invoice()
                    {
                        AgencyId = user.Agency.Id,
                        CreatedAt = DateTime.Now,
                        Id = Guid.NewGuid(),
                        CreatedBy = user.Staff.UserName,
                        FlightBookingId = model.FlightBookingId,
                        InvoiceCode = model.InvoiceCode,
                        InvoiceDate = model.InvoiceDate,
                        InvoiceHour = model.InvoiceHour,
                        Notes = model.Notes,
                        StaffId = user.Staff.UserName,
                        StaffIdUpdate = user.Staff.UserName,
                        StatusId = model.StatusId,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.Invoices.Add(invoice);
                    db.SaveChanges();
                }
                //update staus of booking
                //check exist
                var FlightBooking = db.FlightBookings.Find(model.FlightBookingId);
                FlightBooking.StatusId = model.StatusId;
                db.SaveChanges();
                return Json(new { message = "Lưu invoice thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình lưu dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult SaveClearnace(CustomsClearance model)
        {
            try
            {
                if (Guid.Empty == model.Id)
                {
                    model.Id = Guid.NewGuid();
                    model.AgencyId = user.Agency.Id;
                    model.IsClearance = true;
                    model.UpdatedAt = model.CreatedAt = DateTime.Now;
                    model.UpdatedBy = model.CreatedBy = user.Staff.UserName;
                    model.StaffId = model.StaffIdUpdate = user.Staff.UserName;
                    db.CustomsClearances.Add(model);
                    db.SaveChanges();
                }
                else
                {
                    model.UpdatedAt = DateTime.Now;
                    model.UpdatedBy = user.Staff.UserName;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //update status
                var FlightBooking = db.FlightBookings.Find(model.FlightBookingId);
                FlightBooking.StatusId = model.StatusId; db.SaveChanges();
                return Json(new { message = "Lưu invoice thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình lưu dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }

        }
        [HttpPost]
        public ActionResult SaveClearnaceAir(ClearanceAir model, string[] MAWB, string[] HAWB)
        {
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;
            if (MAWB == null) { MAWB = new string[] { }; }
            else { MAWB = MAWB[0].Split(','); }
            if (HAWB == null) { HAWB = new string[] { }; }
            else { HAWB = HAWB[0].Split(','); }
            if (ModelState.IsValid)
            {
                try
                {
                    Guid id = Guid.NewGuid();
                    if(model.Id!=Guid.Empty)
                    {
                        model.UpdatedAt = DateTime.Now;
                        model.UpdatedBy = user.Staff.UserName;
                        id = model.Id;
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        model.CreatedAt = model.UpdatedAt = DateTime.Now;
                        model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
                        model.Id = id;
                        db.ClearanceAirs.Add(model);
                        db.SaveChanges();
                    }

                    foreach (var ha in db.ClearanceHAWBs.Where(n => n.FlightBookingId == model.FlightBookingId&&n.ClearanceId==model.ClearanceId && n.AgencyId == user.Agency.Id))
                    {
                        db.ClearanceHAWBs.Remove(ha);
                    }
                    foreach (var ma in db.ClearanceMAWBs.Where(n => n.FlightBookingId == model.FlightBookingId && n.ClearanceId == model.ClearanceId&& n.AgencyId == user.Agency.Id))
                    {
                        db.ClearanceMAWBs.Remove(ma);
                    }
                    foreach (var ha in HAWB)
                    {
                        db.ClearanceHAWBs.Add(new ClearanceHAWB()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            FlightBookingId = model.FlightBookingId,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            HAWBId = ha,
                            AgencyId = user.Agency.Id,
                            IsUse = true,
                            ClearanceId=model.ClearanceId
                        });
                    }
                    foreach (var ma in MAWB)
                    {
                        db.ClearanceMAWBs.Add(new ClearanceMAWB()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            FlightBookingId = model.FlightBookingId,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            MAWBId = ma,
                            AgencyId = user.Agency.Id,
                            IsUse = true,
                            ClearanceId = model.ClearanceId
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
    }
}