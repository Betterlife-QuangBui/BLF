using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Controllers
{
    public class DatabaseSearchController : ManagementSystemController
    {
        #region Database Catalog
        public ActionResult AuComShippingCode(string term = "")
        {
            try
            {
                var items = db.Shippings.Where(n => n.AgencyId == user.Agency.Id)
                    .Where(n => n.StatusId == 14 && n.ShippingCode.Contains(term))
                    .OrderBy(n => n.CreatedAt);
                return Json(items.Select(n => n.ShippingCode), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoadShippingMaskByShipping(string shippingCode = "")
        {
            try
            {
                var shipping = db.Shippings.Where(n => n.AgencyId == user.Agency.Id).Single(n => n.ShippingCode == shippingCode);
                var exports = db.ShippingHAWBDetails.Where(n => n.ShippingHAWB.Shipping.Id == shipping.Id).Select(n => n.ExportGood).Select(n => new
                {
                    Value = n.Id,
                    Text = n.ShippingMarkVN
                }).Distinct();
                return Json(exports, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoadTrackingCodeByShippingMask(Guid ShippingMask)
        {
            try
            {
                var exports= db.ExportGoods.Find(ShippingMask);
                var TrackingDetails = exports.ExportGoodDetails.Select(n=>n.TrackingDetail).Distinct();
                var StorageJP = TrackingDetails.Select(n=>n.StorageJP).Select(n => new
                {
                    Value = n.Id,
                    Text = n.TrackingCode
                }).Distinct();
                return Json(StorageJP, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoadSplitCodeByTrackingCode(Guid TrackingCode)
        {
            try
            {
                var StorageJP = db.StorageJPs.Find(TrackingCode);
                var TrackingDetails = StorageJP.TrackingDetails.OrderBy(n=>n.TrackingSubCode).Select(n => new
                {
                    Value = n.Id,
                    Text = n.TrackingSubCode
                }).Distinct();
                return Json(TrackingDetails, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        // GET: DatabaseSearch
        public ActionResult Index()
        {
            return View();
        }
    }
}