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
    public class ShippingController : ManagementSystemController
    {
        public JsonResult CheckShippingCode(string ShippingCode, Guid? Id)
        {
            var isValid = true;
            if (Id == null)
            {
                if (db.Shippings.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ShippingCode == ShippingCode).Count() > 0)
                {
                    isValid = false;
                }
            }
            else
            {
                if (db.Shippings.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ShippingCode == ShippingCode&&x.Id!=Id).Count() > 0)
                {
                    isValid = false;
                }
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        // GET: Shipping
        public ActionResult Index(int page = 1, string key = "", int sort = 13)
        {
            ViewBag.Title = "Danh sách chuyến bay";
            ViewBag.key = key;
            ViewBag.Page = page;
            ViewBag.sort = sort;
            var status = StatusUtils.GetStatus(5);
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.ShippingStatus = new SelectList(status, "Value", "Text", sort);

            var item = db.Shippings.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.ShippingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }

            //search 
            ViewBag.FromAirId = new SelectList(db.FromAirs.OrderBy(n => n.CreatedAt), "Id", "Name");
            ViewBag.ToAirId = new SelectList(db.ToAirs.OrderBy(n => n.CreatedAt), "Id", "Name");
            //ViewBag.WareHouseInfoId = new SelectList(db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt), "Id", "Name");
            //ViewBag.MaWBId = new SelectList(db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt), "Id", "Name");
            var WareHouseInfoId = db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id + "", Text = n.Name + " / " + n.Address });
            ViewBag.WareHouseInfoId = new SelectList(WareHouseInfoId, "Value", "Text");
            var mawb = db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.MaWBId = new SelectList(mawb, "Value", "Text");
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text");

            return View(Pager<Shipping>.CreatePagging(item, page, 10));
        }
        public ActionResult Add()
        {
            ViewBag.FromAirId = new SelectList(db.FromAirs.OrderBy(n => n.CreatedAt), "Id", "Name");
            ViewBag.ToAirId = new SelectList(db.ToAirs.OrderBy(n => n.CreatedAt), "Id", "Name");
            //var WareHouseInfoId = db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id+"", Text = n.Name + " / " + n.Address });
            //ViewBag.WareHouseInfoId = new SelectList(WareHouseInfoId, "Value", "Text");
            //var mawb = db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            //ViewBag.MaWBId = new SelectList(mawb, "Value", "Text");

            var WareHouseInfoId = db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id + "", Text = n.Name + " / " + n.Address });
            ViewBag.WareHouseInfoId = new SelectList(WareHouseInfoId, "Value", "Text");
            var mawb = db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.MaWBId = new SelectList(mawb, "Value", "Text");


            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text");

            return View(new Shipping() { DateAir = DateTime.Now, HourAir = DateTime.Now.ToString("HH:mm") });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Shipping model)
        {
            model.CreatedAt = model.UpdatedAt = DateTime.Now;
            model.CreatedBy = model.UpdatedBy = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = Guid.NewGuid();
                    db.Shippings.Add(model);
                    db.SaveChanges();
                    return Content(javasctipt_add("/Shipping", "Thêm dữ liệu thành công"));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            ViewBag.FromAirId = new SelectList(db.FromAirs.OrderBy(n => n.CreatedAt), "Id", "Name", model.FromAirId);
            ViewBag.ToAirId = new SelectList(db.ToAirs.OrderBy(n => n.CreatedAt), "Id", "Name", model.ToAirId);
            var WareHouseInfoId = db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id + "", Text = n.Name + " / " + n.Address });
            ViewBag.WareHouseInfoId = new SelectList(WareHouseInfoId, "Value", "Text",model.WareHouseInfoId);
            var mawb = db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.MaWBId = new SelectList(mawb, "Value", "Text",model.MaWBId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text", model.StatusId);
            return Content(javasctipt_add("/ExportGoods", "Thêm dữ liệu thất bại"));
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping model = db.Shippings.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromAirId = new SelectList(db.FromAirs.OrderBy(n => n.CreatedAt), "Id", "Name", model.FromAirId);
            ViewBag.ToAirId = new SelectList(db.ToAirs.OrderBy(n => n.CreatedAt), "Id", "Name", model.ToAirId);
            var WareHouseInfoId = db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id + "", Text = n.Name + " / " + n.Address });
            ViewBag.WareHouseInfoId = new SelectList(WareHouseInfoId, "Value", "Text",model.WareHouseInfoId);
            var mawb = db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.MaWBId = new SelectList(mawb, "Value", "Text", model.MaWBId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text", model.StatusId);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Shipping model,Guid Id)
        {
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = user.Staff.UserName;
            model.AgencyId = user.Agency.Id;
            if (PageUtils.IsUpdateShipping(model) != "")
            {
                return Content(javasctipt_add("/Shipping", PageUtils.IsUpdateShipping(model)));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content(javasctipt_add("/Shipping", "Cập nhật dữ liệu thành công"));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            ViewBag.FromAirId = new SelectList(db.FromAirs.OrderBy(n => n.CreatedAt), "Id", "Name", model.FromAirId);
            ViewBag.ToAirId = new SelectList(db.ToAirs.OrderBy(n => n.CreatedAt), "Id", "Name", model.ToAirId);
            var WareHouseInfoId = db.WareHouseInfoes.Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id + "", Text = n.Name + " / " + n.Address });
            ViewBag.WareHouseInfoId = new SelectList(WareHouseInfoId, "Value", "Text",model.WareHouseInfoId);
            var mawb = db.MAWBs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.IsActive == true).OrderBy(n => n.CreatedAt).Select(n => new SelectListItem() { Value = n.Id, Text = n.Name + " / " + n.Address });
            ViewBag.MaWBId = new SelectList(mawb, "Value", "Text", model.MaWBId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text", model.StatusId);
            return Content(javasctipt_add("/Shipping", "Cập nhật dữ liệu thất bại"));
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var items = db.Shippings.Find(id);
                db.Shippings.Remove(items);
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult DeleteMultiple(String id)
        {
            try
            {
                foreach (var item in id.Split(','))
                {
                    var items = db.Shippings.Find(Guid.Parse(item));
                    db.Shippings.Remove(items);
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        public ActionResult Booking(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping model = db.Shippings.Find(id);

            //lay danh sach lich su booking
            var lstBookingHistory = model.ShippingBookings.OrderByDescending(n => n.CreatedAt);
            ViewBag.BookingHistory = lstBookingHistory;
            //lay danh sach so kien so kg tham khao
            var lstInformation = new List<BookingInformation>();
            //lay danh sach tren duong
            var lstTrenDuong = db.Shipments.Where(n => n.AgencyId == user.Agency.Id && n.StatusId == -1);
            double counttracking = 0; double weigh = 0;
            foreach (var item in lstTrenDuong)
            {
                counttracking += item.AgencyPackages.Count();
                weigh += item.AgencyPackages.Sum(n => n.Weigh).Value;
            }
            var infoTrenDuong = new BookingInformation()
            { Notes = "Trên đường", TrackingCount = counttracking, Weigh = weigh };
            lstInformation.Add(infoTrenDuong);
            ViewBag.Weigh = weigh; ViewBag.TrackingCount = counttracking;
            counttracking = 0; weigh = 0;
            //lay danh sach luu kho va xuat kho
            //+ lay danh sach luu kho khong nam trong xuat kho
            var lstLuuKho = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id);
            lstLuuKho = lstLuuKho.Where(n => n.TrackingDetails.Where(k => db.ExportGoodDetails.Where(m => m.ExportGood.AgencyId == user.Agency.Id).Where(m => m.TrackingDetailId == k.Id).Count() == 0).Count() >= 0);
            weigh += lstLuuKho.Where(n => n.Weigh != null).Sum(n => n.Weigh).Value;
            counttracking += lstLuuKho.Count();
            infoTrenDuong = new BookingInformation()
            { Notes = "Lưu kho", TrackingCount = counttracking, Weigh = weigh };
            lstInformation.Add(infoTrenDuong);
            //+ lay dan sach xuat kho khong nam trong chuyen bay hoac chuyen bay chua thong quan
            var lstXuatKho = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id);
            //lay kien xuat kho co trong chuyen bay
            int ex_counttracking = 0; double ex_weigh = 0;
            var lstXuatKhoInFlight = lstXuatKho.Where(n => db.ShippingHAWBDetails.Where(m => m.AgencyId == user.Agency.Id).Where(m => m.ShippingHAWB.Shipping.StatusId == 14).Where(m => m.ExportGoodId == n.Id).Count() > 0);
            lstXuatKho = lstXuatKho.Where(n => lstXuatKhoInFlight.Where(m => m.Id == n.Id).Count() == 0);
            foreach (var item in lstXuatKho)
            {
                counttracking += item.ExportGoodDetails.Count();
                ex_counttracking+= item.ExportGoodDetails.Count();
            }
            weigh += lstXuatKho.Where(n => n.Weigh != null).Sum(n => n.Weigh).Value;
            ex_weigh += lstXuatKho.Where(n => n.Weigh != null).Sum(n => n.Weigh).Value;
            //thong tin xuat kho
            infoTrenDuong = new BookingInformation()
            { Notes = "Xuất kho", TrackingCount = ex_counttracking, Weigh = ex_weigh };
            lstInformation.Add(infoTrenDuong);

            //thong tin tong cong luu kho + xuat kho
            infoTrenDuong = new BookingInformation()
            { Notes = "Lưu kho & xuất kho chưa thông quan", TrackingCount = counttracking, Weigh = weigh };
            lstInformation.Add(infoTrenDuong);
            ViewBag.Information = lstInformation;
            //lay trang thai cua booking
            var Status = lstBookingHistory == null || lstBookingHistory.Count() == 0 ? 11 : lstBookingHistory.First().StatusId;
            ViewBag.Status = Status;
            ViewBag.Count = lstBookingHistory.Count();
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(4), "Value", "Text", Status);
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateBookingStatus(Guid id, int status)
        {
            try
            {
                Shipping model = db.Shippings.Find(id);

                //lay danh sach lich su booking
                var lstBookingHistory = model.ShippingBookings.OrderByDescending(n => n.CreatedAt);
                if(lstBookingHistory.Count()==0|| lstBookingHistory == null)
                {
                    return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var booking = lstBookingHistory.First();
                    booking.StatusId = status;
                }
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult AddBookingHistory(Guid? id, double trackingCount, double weigh,string notes)
        {
            try
            {
                Shipping model = db.Shippings.Find(id);
                var booking = new ShippingBooking()
                {
                    AgencyId = user.Agency.Id,
                    BookingDate = DateTime.Now,
                    BookingHour = DateTime.Now.ToString("HH:mm"),
                    BookingUser = user.Staff.Email,
                    CreatedAt = DateTime.Now,
                    CreatedBy = user.Staff.UserName,
                    Id = Guid.NewGuid(),
                    ShippingId = model.Id,
                    StatusId = 11,
                    TrackingCount = trackingCount,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = notes,
                    Weigh = weigh
                };
                db.ShippingBookings.Add(booking);
                db.SaveChanges();
                return Json(new { message = "Booking thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Booking thất bại", status = false }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult ShippingPackage(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping model = db.Shippings.Find(id);
            //lay trang thai cua booking
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text", model.StatusId);
            //lay danh sach kien hang VN
            var listVN = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id && n.StatusId == 10);
            //check list VN exist in Chuyen bay Hay khong?
            listVN = listVN.Where(n=>db.ShippingHAWBDetails.Where(m=>m.AgencyId==user.Agency.Id).Where(m=>m.ExportGoodId==n.Id).Count()==0);
            ViewBag.ListVN = Pager<ExportGood>.CreatePagging(listVN.OrderByDescending(n => n.CreatedAt), 1, 10);
            //lay danh sach kiện Shipping
            var itemListShipping = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id && n.ShippingHAWB.ShippingId == model.Id).OrderByDescending(n => n.CreatedAt);
            ViewBag.Weigh = itemListShipping.Sum(n=>n.ExportGood.Weigh);
            ViewBag.ListShipping = Pager<ShippingHAWBDetail>.CreatePagging(itemListShipping, 1, 10);
            //lay danh sach HAWB
            var listHAWB = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt);
            ViewBag.HAWB = new SelectList(listHAWB, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        public ActionResult AddShippingMaskItems(Guid? id, string ids)
        {
            try
            {
                Shipping model = db.Shippings.Find(id);
                var listHAWB = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt);
                if (listHAWB != null && listHAWB.Count() > 0)
                {
                    var hawmb = listHAWB.First();
                    //check exist in table ShippingHAWB
                    var idShippingHAWB = Guid.NewGuid();
                    if (model.ShippingHAWBs.Where(n => n.HAWBId == hawmb.Id).Count() == 0)
                    {
                        //tao moi hawb cho chuyen bay
                        var ShippingHAWB = new ShippingHAWB() {
                            Id=idShippingHAWB,
                            AgencyId=user.Agency.Id,
                            HAWBId=hawmb.Id,
                            CreatedAt=DateTime.Now,
                            CreatedBy=user.Staff.UserName,
                            ShippingId=model.Id,
                            UpdatedAt=DateTime.Now,
                            UpdatedBy=user.Staff.UserName
                        };
                        db.ShippingHAWBs.Add(ShippingHAWB);
                    }
                    else { idShippingHAWB = model.ShippingHAWBs.Single(n => n.HAWBId == hawmb.Id).Id; }
                    //them chi tiet kien chuyen bay
                    foreach (var item in ids.Split(','))
                    {
                        var ShippingHAWBDetail = new ShippingHAWBDetail() {
                            AgencyId=user.Agency.Id,
                            Id=Guid.NewGuid(),
                            CreatedAt=DateTime.Now,
                            CreatedBy=user.Staff.UserName,
                            ExportGoodId=Guid.Parse(item),
                            ShippingHAWBId=idShippingHAWB,
                            ShippingMark= db.ExportGoods.Find(Guid.Parse(item)).ShippingMarkVN,
                            UpdatedAt=DateTime.Now,
                            UpdatedBy=user.Staff.UserName
                        };
                        db.ShippingHAWBDetails.Add(ShippingHAWBDetail);
                    }
                    db.SaveChanges();
                    return Json(new { message = new { mess="Thêm kiện VN thành công !",weigh= db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id && n.ShippingHAWB.ShippingId == model.Id).Sum(n => n.ExportGood.Weigh)
                    }, status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Vui lòng cập nhật danh sách HAWB.", status = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch{ return Json(new { message = "Đã xảy ra lỗi trong quá trình xử lý dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult RemoveHAWB(Guid? id, string ids)
        {
            try
            {
                Shipping model = db.Shippings.Find(id);
                //list ShippingHAWBs
                List<Guid> lstShippingHAWBs = new List<Guid>();
                //xu ly xoa ShippingHAWBDetails
                foreach (var item in ids.Split(','))
                {
                    lstShippingHAWBs.Add(db.ShippingHAWBDetails.Find(Guid.Parse(item)).ShippingHAWBId.Value);
                    db.ShippingHAWBDetails.Remove(db.ShippingHAWBDetails.Find(Guid.Parse(item)));
                }
                //xu ly xoa ShippingHAWBs
                foreach (var item in lstShippingHAWBs.Distinct())
                {
                    if (db.ShippingHAWBs.Find(item).ShippingHAWBDetails.Count == 0)
                    {
                        db.ShippingHAWBs.Remove(db.ShippingHAWBs.Find(item));
                    }
                }
                db.SaveChanges();
                return Json(new
                {
                    message = new
                    {
                        mess = "Xử lý thành công !",
                        weigh = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id && n.ShippingHAWB.ShippingId == model.Id).Sum(n => n.ExportGood.Weigh)
                    },
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xử lý dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult ChangeHAWB(Guid? id, string ids,string hawb)
        {
            try
            {
                Shipping model = db.Shippings.Find(id);
                var model_hawb = db.HAWBs.Find(hawb);
                //list ShippingHAWBs
                List<Guid> lstShippingHAWBs = new List<Guid>();
                var idShippingHAWB = Guid.NewGuid();
                if (model.ShippingHAWBs.Where(n => n.HAWBId == model_hawb.Id).Count() == 0)
                {
                    //tao moi hawb cho chuyen bay
                    var ShippingHAWB = new ShippingHAWB()
                    {
                        Id = idShippingHAWB,
                        AgencyId = user.Agency.Id,
                        HAWBId = model_hawb.Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ShippingId = model.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.ShippingHAWBs.Add(ShippingHAWB);
                }
                else { idShippingHAWB = model.ShippingHAWBs.Single(n => n.HAWBId == model_hawb.Id).Id; }
                //xu ly xoa ShippingHAWBDetails
                foreach (var item in ids.Split(','))
                {
                    var detail = db.ShippingHAWBDetails.Find(Guid.Parse(item));
                    lstShippingHAWBs.Add(detail.ShippingHAWBId.Value);
                    detail.ShippingHAWBId = idShippingHAWB;
                    detail.UpdatedAt = DateTime.Now;
                    detail.UpdatedBy = user.Staff.UserName;
                }
                //xu ly xoa ShippingHAWBs
                foreach (var item in lstShippingHAWBs.Distinct())
                {
                    if (db.ShippingHAWBs.Find(item).ShippingHAWBDetails.Count == 0)
                    {
                        db.ShippingHAWBs.Remove(db.ShippingHAWBs.Find(item));
                    }
                }
                db.SaveChanges();
                return Json(new
                {
                    message = new
                    {
                        mess = "Xử lý thành công !",
                        weigh = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id && n.ShippingHAWB.ShippingId == model.Id).Sum(n => n.ExportGood.Weigh)
                    },
                    status = true
                }, JsonRequestBehavior.AllowGet);

            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xử lý dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult Clearance(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping model = db.Shippings.Find(id);
            //lay trang thai cua booking
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text", model.StatusId);
            var items = db.ShippingHistories.Where(n => n.AgencyId == user.Agency.Id && n.ShippingId == model.Id).OrderByDescending(n => n.CreatedAt);
            ViewBag.ShippingClearance = items;
            ViewBag.CountItem = items.Count();
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateStatusShipping(Guid? id,int statusId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            try
            {
                Shipping model = db.Shippings.Find(id);
                model.StatusId = statusId;
                db.SaveChanges();
                return Json(new { message = "Update trạng thái thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch{ return Json(new { message = "Update trạng thái thất bại", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clearance(Guid? ShippingId,string PreAD_Note="",string PreADFile="",string PreADBase64="")
        {
            if (ShippingId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //xu ly update
            Shipping model = db.Shippings.Find(ShippingId);
            //add shipping clearnace
            var shippingClearnace = new ShippingHistory() {
                Id=Guid.NewGuid(),
                ShippingId=model.Id,
                AgencyId=user.Agency.Id,
                ShippingCode=model.ShippingCode,
                DateAir=model.DateAir,
                HourAir=model.HourAir,
                FromAirId=model.FromAirId,
                ToAirId=model.ToAirId,
                WareHouseInfoId=model.WareHouseInfoId,
                Notes=model.Notes,
                MaWBId=model.MaWBId,
                CreatedAt=DateTime.Now,
                CreatedBy=user.Staff.UserName,
                StatusId=model.StatusId,
                UpdatedAt=DateTime.Now,
                UpdatedBy=user.Staff.UserName
            };
            if (PreAD_Note != "") { shippingClearnace.PreAD_Note = PreAD_Note; }
            //xu ly upload file
            var upImage = Request.Files["upImage"];
            if (upImage.ContentLength > 0 && upImage != null)
            {
                string fileName = upImage.FileName;
                upImage.SaveAs(Server.MapPath("~/Uploads/PreAD/" + fileName));
                shippingClearnace.PreADFile = fileName;
                try
                {
                    shippingClearnace.PreADBase64 = Web.Helpers.Images.ImageUtils.Images(Server.MapPath("~/Uploads/PreAD/" + fileName));
                }
                catch { }
            }
            db.ShippingHistories.Add(shippingClearnace);
            db.SaveChanges();
            TempData["Message"] = "Cập nhật pre-ad thành công";
            //lay trang thai cua booking
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(5), "Value", "Text", model.StatusId);
            ViewBag.ShippingClearance = db.ShippingHistories.Where(n => n.AgencyId == user.Agency.Id && n.ShippingId == model.Id).OrderByDescending(n => n.CreatedAt);
            return Redirect("/shipping/clearance/"+model.Id);
        }
    }
}