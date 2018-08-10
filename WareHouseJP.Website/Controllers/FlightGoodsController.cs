using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Controllers
{
    public class FlightGoodsController : ManagementSystemController
    {
        UserPage user = new UserPage();
        // GET: FlightGoods
        public ActionResult Index(Guid id)
        {
            var FlightBooking = db.FlightBookings.Find(id);
            ViewBag.FlightBooking = FlightBooking;
            var model = db.FlightGoods.Where(n => n.FlightBookingId == id);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(4), "Value", "Text", FlightBooking.StatusId);
            return View(model);
        }
        [HttpPost]
        public ActionResult _Search(string key)
        {
            var items = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id);
            if (key != null && key.Trim().Length > 0)
            {
                items = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id && n.ShippingMarkVN.Contains(key));
            }
            //Check vn package is exist
            items = items.Where(n => db.FlightGoods.Where(m => m.ExportGoodId == n.Id).Count() == 0);
            return PartialView(items.OrderBy(n => n.ShippingMarkVN));
        }
        [HttpPost]
        public ActionResult Add2New(string[] array, Guid FlightBookingId)
        {
            try
            {
                var FlightBooking = db.FlightBookings.Find(FlightBookingId);
                Guid id = Guid.NewGuid();
                foreach (var item in array)
                {
                    ExportGood ExportGood = db.ExportGoods.Find(Guid.Parse(item));
                    FlightGood FlightGood = new FlightGood()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportGoodId = ExportGood.Id,
                        Id = id,
                        FlightBookingId= FlightBookingId,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.FlightGoods.Add(FlightGood);
                }
                FlightBooking.AutomaticTrackingCount = FlightBooking.FlightGoods.Sum(n => n.ExportGood.ExportGoodDetails.Count);
                FlightBooking.AutomaticWeigh = FlightBooking.FlightGoods.Sum(n => n.ExportGood.Weigh);
                db.SaveChanges();
                return Json(new { message = new { idNew = id, num = FlightBooking.FlightGoods.Sum(n=>n.ExportGood.ExportGoodDetails.Count), kg = FlightBooking.FlightGoods.Sum(n => n.ExportGood.Weigh) }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình thêm dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public ActionResult DeleteFlightGoods(Guid id)
        {
            try
            {
                var item = db.FlightGoods.Find(id);
                var FlightBooking = item.FlightBooking;
                var num = item.FlightBooking.FlightGoods.Sum(n => n.ExportGood.ExportGoodDetails.Count);
                var numCurrent = item.ExportGood.ExportGoodDetails.Count;
                double kqOld = item.ExportGood.Weigh.Value;
                var kg = item.FlightBooking.FlightGoods.Sum(n => n.ExportGood.Weigh);
                db.FlightGoods.Remove(item);
                FlightBooking.AutomaticTrackingCount = num - numCurrent;
                FlightBooking.AutomaticWeigh = kg - kqOld;
                db.SaveChanges();
                return Json(new { message = new { num = num - numCurrent, kg = kg - kqOld }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
    }
}