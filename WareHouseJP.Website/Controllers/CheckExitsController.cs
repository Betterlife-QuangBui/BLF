using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Controllers
{
    public class CheckExitsController : ManagementSystemController
    {
        #region StoreJP
        public ActionResult StoreJPTrackingCode(string TrackingCode)
        {
            bool isExits = false;
            try
            {
                isExits = db.StorageJPs.Where(n => n.TrackingCode == TrackingCode).Count() > 0 ? true : false;
                return Json(!isExits, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StoreJPDeliveryAddress(string DeliveryAddress)
        {
            bool isExits = false;
            try
            {
                isExits = DeliveryAddress!=null? true : false;
                return Json(!isExits, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region AirInfo
        public ActionResult AirInfoId(string Id)
        {
            bool isExits = false;
            try
            {
                isExits = db.AirInfoes.Where(n => n.Id == Id).Count() > 0 ? true : false;
                return Json(!isExits, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ExportGood
        public ActionResult ExportGoodShippingMarkVN(string ShippingMarkVN)
        {
            bool isExits = false;
            try
            {
                isExits = db.ExportGoods.Where(n => n.ShippingMarkVN == ShippingMarkVN).Count() > 0 ? true : false;
                return Json(!isExits, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region AgencyPackage
        public ActionResult AgencyPackageTrackingCode(string TrackingCode)
        {
            bool isExits = false;
            try
            {
                isExits = db.AgencyPackages.Where(n => n.TrackingCode == TrackingCode).Count() > 0 ? true : false;
                return Json(!isExits, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}