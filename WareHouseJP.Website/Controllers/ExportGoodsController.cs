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
    public class ExportGoodsController : ManagementSystemController
    {
        public JsonResult CheckShippingMarkVN(string ShippingMarkVN, Guid? Id)
        {
            var isValid = true;
            if (Id == null)
            {
                if (db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ShippingMarkVN == ShippingMarkVN).Count() > 0)
                {
                    isValid = false;
                }
            }
            else
            {
                if (db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id).Where(x => x.ShippingMarkVN == ShippingMarkVN && x.Id != Id).Count() > 0)
                {
                    isValid = false;
                }
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        // GET: ExportGoods
        public ActionResult Index(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.Title = "Danh sách kiện hàng Việt Nam";
            ViewBag.key = key;
            ViewBag.Page = page;
            ViewBag.sort = sort;
            var status = StatusUtils.GetStatus(3);
            status.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            ViewBag.ExportGoodStatus = new SelectList(status, "Value", "Text", sort);

            var item = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            var exportShipping = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id);
            item = item.Where(m => exportShipping.Where(n=>n.ExportGoodId==m.Id).Count()==0)
                .OrderByDescending(n => n.CreatedAt);


            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }
            if (key != "")
            {
                item = item.Where(n => n.ShippingMarkVN.Contains(key)).OrderByDescending(n => n.CreatedAt);
            }
            return View(Pager<ExportGood>.CreatePagging(item, page, 10));
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var items = db.ExportGoods.Find(id);
                foreach (var item in items.ExportGoodDetails.ToList())
                {
                    db.ExportGoodDetails.Remove(item);
                }
                db.ExportGoods.Remove(items);
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        // GET: ExportGoods/Create
        public ActionResult Add()
        {
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(3), "Value", "Text", "");
            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
            ViewBag.SizeTableId = new SelectList(size, "Value", "Text");
            //ViewBag.ExportHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", "");
            return View(new ExportGood() { ExportDate = DateTime.Now, ExportHour = DateTime.Now.ToString("HH:mm") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ExportGood exportGood)
        {
            exportGood.CreatedAt = exportGood.UpdatedAt = DateTime.Now;
            exportGood.CreatedBy = exportGood.UpdatedBy = user.Staff.UserName;
            exportGood.AgencyId = user.Agency.Id; exportGood.StaffId = user.Staff.UserName;

            if (ModelState.IsValid)
            {
                try
                {
                    //var upImage = Request.Files["upImage"];
                    //if (upImage.ContentLength > 0 && upImage != null)
                    //{
                    //    string fileName = upImage.FileName.DoiTenHinh();
                    //    upImage.SaveAs(Server.MapPath("~/images/ExportGoods/" + fileName));
                    //    exportGood.Image = fileName;
                    //}
                    exportGood.Id = Guid.NewGuid();
                    db.ExportGoods.Add(exportGood);
                    db.SaveChanges();
                    TempData["Message"] = "Thêm dữ liệu thành công";
                    return Content(javasctipt_add("/ExportGoods", "Thêm dữ liệu thành công"));
                }
                catch (Exception)
                {
                    TempData["Message"] = "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu";
                    ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                }
            }
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(3), "Value", "Text", exportGood.StatusId);
            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
            ViewBag.SizeTableId = new SelectList(size, "Value", "Text", exportGood.SizeTableId);
            //ViewBag.ExportHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", exportGood.ExportHour);
            TempData["Message"] = "Thêm dữ liệu thất bại";
            return Content(javasctipt_add("/ExportGoods", "Thêm dữ liệu thất bại"));
        }

        // GET: ExportGoods/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExportGood exportGood = db.ExportGoods.Find(id);
            if (exportGood == null)
            {
                return HttpNotFound();
            }
            var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
            ViewBag.SizeTableId = new SelectList(size, "Value", "Text", exportGood.SizeTableId);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(3), "Value", "Text", exportGood.StatusId);
            //ViewBag.ExportHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", exportGood.ExportHour);

            return View(exportGood);
        }

        // POST: ExportGoods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExportGood exportGood, string actionlink = "")
        {
            if (PageUtils.IsUpdateExport(exportGood).Length > 0)
            {
                if (exportGood.ExportGoodDetails.Count() == 0) { return Content(javasctipt_add("/ExportGoods", PageUtils.IsUpdateExport(exportGood))+", nội dung kiện hàng."); }
                else return Content(javasctipt_add("/ExportGoods", PageUtils.IsUpdateExport(exportGood)));
            }
            else
            {
                exportGood.UpdatedAt = DateTime.Now;
                exportGood.UpdatedBy = user.Staff.UserName;
                exportGood.StaffId = user.Staff.UserName;
                exportGood.AgencyId = user.Agency.Id;

                if (ModelState.IsValid)
                {
                    try
                    {
                        //var upImage = Request.Files["upImage"];
                        //if (upImage.ContentLength > 0 && upImage != null)
                        //{
                        //    string fileName = upImage.FileName.DoiTenHinh();
                        //    upImage.SaveAs(Server.MapPath("~/images/ExportGoods/" + fileName));
                        //    exportGood.Image = fileName;
                        //}
                        db.Entry(exportGood).State = EntityState.Modified;
                        TempData["Message"] = "Cập nhật dữ liệu thành công";
                        db.SaveChanges();
                        
                        if (actionlink != "") { return Content(javasctipt_add("/ExportGoods/Detail/" + exportGood.Id, "Cập nhật dữ liệu thành công")); }
                        else return Content(javasctipt_add("/ExportGoods", "Cập nhật dữ liệu thành công"));
                    }
                    catch (Exception)
                    {
                        TempData["Message"] = "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu";
                        ModelState.AddModelError("error", "Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu");
                    }
                }
                var size = db.SizeTables.OrderBy(n => n.NoOrder).Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
                size.Insert(0, new SelectListItem() { Value = " ", Text = " " });
                ViewBag.SizeTableId = new SelectList(size, "Value", "Text", exportGood.SizeTableId);
                ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(3), "Value", "Text", exportGood.StatusId);
                //ViewBag.ExportHour = new SelectList(TimeUtils.TimeHours(), "Value", "Text", exportGood.ExportHour);
                if (actionlink != "") { return Content(javasctipt_add("/ExportGoods/Detail/" + exportGood.Id, "Cập nhật dữ liệu thất bại")); }
                else return Content(javasctipt_add("/ExportGoods", "Cập nhật dữ liệu thất bại"));
            }
        }
        //detail
        public ActionResult Detail(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExportGood exportGood = db.ExportGoods.Find(id);
            //LIST KIEN JP
            var listjp = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.StatusId == 8).OrderByDescending(n => n.CreatedAt);
            List<TrackingDetail> lstDetail = new List<TrackingDetail>();
            foreach (var item in listjp)
            {
                lstDetail.AddRange(item.TrackingDetails.Where(n => n.TrackingSubCode != "21"));
            }
            lstDetail = lstDetail.Where(n => PageUtils.IsExistExport(n.Id, user.Agency.Id) == false).OrderByDescending(n => n.CreatedAt).ToList();
            var lstReturn = Pager<TrackingDetail>.CreatePagging(lstDetail.AsQueryable(), 1, 10);
            ViewBag.ListJP = lstReturn;
            ViewBag.ListVN = Pager<ExportGoodDetail>.CreatePagging(exportGood.ExportGoodDetails.OrderByDescending(n => n.CreatedAt).AsQueryable(), 1, 10);
            ViewBag.StatusId = new SelectList(StatusUtils.GetStatus(3), "Value", "Text", exportGood.StatusId);
            ViewBag.ExportGoodStatus = exportGood.StatusId;
            return View(exportGood);
        }
        [HttpPost]
        public ActionResult AddItems(Guid id, string ids = "")
        {
            try
            {
                var export = db.ExportGoods.Find(id);
                foreach (var item in ids.Split(','))
                {
                    var tracking = db.TrackingDetails.Find(Guid.Parse(item));
                    ExportGoodDetail detail = new ExportGoodDetail()
                    {
                        CreatedAt = DateTime.Now,
                        ExportGoodId = export.Id,
                        Id = Guid.NewGuid(),
                        Notes = "",
                        CreatedBy = user.Staff.UserName,
                        TrackingCode = tracking.TrackingSubCode,
                        TrackingDetailId = tracking.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.ExportGoodDetails.Add(detail);
                }
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình cập nhật dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult RemveItems(string ids = "")
        {
            try
            {
                foreach (var item in ids.Split(','))
                {
                    var export = db.ExportGoodDetails.Find(Guid.Parse(item));
                    db.ExportGoodDetails.Remove(export);
                }
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình cập nhật dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        public ActionResult UpdateStatus(Guid id, int status)
        {
            try
            {
                var store = db.ExportGoods.Find(id);
                store.StatusId = status;
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Autocomplete(string term = "")
        {
            try
            {
                var listjp = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.StatusId == 8 && n.TrackingCode.Contains(term)).OrderBy(n => n.TrackingCode);
                return Json(listjp.Select(n => n.TrackingCode), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AutocompleteVN(string term = "")
        {
            try
            {
                var listjp = db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == user.Agency.Id).Where(n => n.TrackingDetail.StorageJP.TrackingCode.Contains(term)).OrderBy(n => n.TrackingDetail.StorageJP.TrackingCode);
                db.SaveChanges();
                return Json(listjp.Select(n => n.TrackingDetail.StorageJP.TrackingCode), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddBarCode(Guid id, string barcode = "")
        {
            try
            {
                var export = db.ExportGoods.Find(id);
                //check status of barcode
                string[] trackingCodes = barcode.Split('-');
                string split = trackingCodes[trackingCodes.Length - 1];
                string trackingcode = barcode.Substring(0, barcode.LastIndexOf('-'));
                //check tracking code exist
                try
                {
                    var storeJP = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).SingleOrDefault(n => n.TrackingCode == trackingcode);
                    if (storeJP != null)
                    {
                        //kiem tra trạng thái
                        if (storeJP.StatusId == 8)
                        {
                            //kiem tra kiện con đã trộn hay chưa
                            var trackingdetails = storeJP.TrackingDetails;
                            var tracking = trackingdetails.SingleOrDefault(n => n.TrackingSubCode == split);
                            if (tracking != null)
                            {
                                if (db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == user.Agency.Id).Where(n => n.TrackingDetailId == tracking.Id).Count() == 0)
                                {
                                    ExportGoodDetail detail = new ExportGoodDetail()
                                    {
                                        CreatedAt = DateTime.Now,
                                        ExportGoodId = export.Id,
                                        Id = Guid.NewGuid(),
                                        Notes = "",
                                        CreatedBy = user.Staff.UserName,
                                        TrackingCode = tracking.TrackingSubCode,
                                        TrackingDetailId = tracking.Id,
                                        UpdatedAt = DateTime.Now,
                                        UpdatedBy = user.Staff.UserName
                                    };
                                    db.ExportGoodDetails.Add(detail);
                                    db.SaveChanges();
                                    return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
                                }
                                else return Json(new { message = "Mã tách kiện đã được thêm.", status = false }, JsonRequestBehavior.AllowGet);

                            }
                            else return Json(new { message = "Mã tách kiện không tồn tại.", status = false }, JsonRequestBehavior.AllowGet);
                        }
                        else return Json(new { message = "Kiện hàng này chưa được phép trộn.", status = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "Không tìm thấy tracking này.", status = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình cập nhật dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình cập nhật dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public ActionResult DeleteMultiple(string ids)
        {
            try
            {
                foreach (var id in ids.Split(','))
                {
                    var items = db.ExportGoods.Find(Guid.Parse(id));
                    foreach (var item in items.ExportGoodDetails.ToList())
                    {
                        db.ExportGoodDetails.Remove(item);
                    }
                    db.ExportGoods.Remove(items);
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
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
