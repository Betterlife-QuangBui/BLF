using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;
using WareHouseJP.Website.Helpers;

namespace WareHouseJP.Website.Controllers
{
    public class ExportGoodDetailsController : ManagementSystemController
    {
        UserPage user = new UserPage();
        // GET: ExportGoodDetails
        public ActionResult Index(Guid id)
        {
            ViewBag.ExportGood = db.ExportGoods.Find(id);
            var exportGoodDetails = db.ExportGoodDetails.Where(n=>n.ExportGoodId==id).Include(e => e.ExportGood).Include(e => e.TrackingDetail);
            return View(exportGoodDetails);
        }
        [HttpPost]
        public ActionResult _Search(string key)
        {
            var items = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id&&n.IsAgencyConfirm==true&&n.IsStaffConfirm==true && n.IsCheck==true);
            if (key != null && key.Trim().Length > 0)
            {
                items = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id&&n.TrackingCode.Contains(key) && n.IsAgencyConfirm == true && n.IsStaffConfirm == true);
            }
            List<TrackingDetail> lst = new List<TrackingDetail>();
            foreach (var item in items)
            {
                lst.AddRange(item.TrackingDetails);
            }
            lst = lst.Where(n => db.ExportGoodDetails.Where(m => m.TrackingDetailId == n.Id).Count() == 0 && !n.TrackingSubCode.Contains(" - 21")).ToList();
            return PartialView(lst.OrderBy(n => n.TrackingSubCode));
        }

        [HttpPost]
        public ActionResult DisplayData(Guid id)
        {
            var items = db.ExportGoodDetails.Find(id).TrackingDetail.StorageItemJPs.OrderBy(n=>n.PriceTax);
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult DeleteExportGoodDetail(Guid id)
        {
            try
            {
                db.ExportGoodDetails.Remove(db.ExportGoodDetails.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public ActionResult DeleteExportGoodDetail2(Guid id)
        {
            try
            {
                var item = db.ExportGoodDetails.Find(id);
                var num = item.ExportGood.ExportGoodDetails.Count();
                double kqOld = item.TrackingDetail.Weigh.Value;
                var kg = item.ExportGood.ExportGoodDetails.Sum(n => n.TrackingDetail.Weigh);
                db.ExportGoodDetails.Remove(item);
                db.SaveChanges();
                return Json(new { message = new { num = num-1, kg = kg- kqOld }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public ActionResult AddNew(string [] array,Guid exportId)
        {
            try
            {
                List<Result> lst = new List<Result>() ;
                foreach (var item in array)
                {
                    Guid id = Guid.NewGuid();
                    
                    TrackingDetail detail = db.TrackingDetails.Find(Guid.Parse(item));
                    lst.Add(new Result() { Id=id,TrackingCode= detail.TrackingSubCode });
                    ExportGoodDetail exportDetail = new ExportGoodDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportGoodId = exportId,
                        Id = id,
                        Notes = "",
                        TrackingCode = detail.TrackingSubCode,
                        TrackingDetailId = detail.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.ExportGoodDetails.Add(exportDetail);
                }
                db.SaveChanges();
                return Json(new { message = lst, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình thêm dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public ActionResult Add2New(string[] array, Guid exportId)
        {
            try
            {
                List<Result> lst = new List<Result>();
                var export=db.ExportGoods.Find(exportId);
                Guid id = Guid.NewGuid();
                foreach (var item in array)
                {
                    

                    TrackingDetail detail = db.TrackingDetails.Find(Guid.Parse(item));
                    lst.Add(new Result() { Id = id, TrackingCode = detail.TrackingSubCode });
                    ExportGoodDetail exportDetail = new ExportGoodDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportGoodId = exportId,
                        Id = id,
                        Notes = "",
                        TrackingCode = detail.TrackingSubCode,
                        TrackingDetailId = detail.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.ExportGoodDetails.Add(exportDetail);
                }
                db.SaveChanges();
                return Json(new { message = new { idNew= id, num = export.ExportGoodDetails.Count(), kg=export.ExportGoodDetails.Sum(n=>n.TrackingDetail.Weigh) }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình thêm dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        
        [HttpPost]
        public ActionResult IsCheckConfirm(Guid id, bool isCheck = false)
        {
            ExportGood item = db.ExportGoods.Find(id);
            List<ExportGoodDetail> details = db.ExportGoodDetails.Where(n => n.ExportGoodId == id).ToList();
            try
            {
                foreach (var d in details)
                {
                    d.StatusId = isCheck == true ? 1 : 0;
                    db.SaveChanges();
                }
                if (isCheck)
                {
                    item.IsStaffConfirm = isCheck;
                    item.StatusId = 10; // đã đóng gói(define=10)
                }
                else
                {
                    item.IsStaffConfirm = isCheck;
                    item.StatusId = 9; // đang đóng gói(define=9)
                }
                db.SaveChanges();

                var PageUtils = new PageUtils();
                return Json(new { message = PageUtils.PackageStatus(item.StatusId.Value, 3), status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
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
    public class Result
    {
        public Guid Id { get; set; }
        public String TrackingCode { get; set; }
    }
}
