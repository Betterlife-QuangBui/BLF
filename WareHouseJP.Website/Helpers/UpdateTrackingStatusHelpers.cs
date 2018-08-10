using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Helpers
{
    public class UpdateTrackingStatusHelpers
    {
        public void UpdateStatus(WareHouseJPDB db)
        {
            //ExportGood
            var lstEX = db.ExportGoods;
            foreach (var item in lstEX)
            {
                if (db.ExportInvoices.Where(n => n.ExportId == item.Id).Count() > 0)
                {
                    item.StatusId = 5;
                }
            }
            //update StorageJP Status
            var lstSJP = db.StorageJPs;
            foreach (var item in lstSJP)
            {
                //check ListIn
                int statusId = 2;
                if (db.TrackingDetails.ToList().Where(n => n.TrackingSubCode.Substring(0,n.TrackingSubCode.LastIndexOf(" -")).Contains(item.TrackingCode)).Count() > 0)
                {
                    statusId = 3;
                }
                if (db.ExportGoodDetails.Where(n => n.TrackingCode.Contains(item.TrackingCode)).Count() > 0)
                {
                    statusId = 4;
                }
                if (db.ExportGoodDetails.Where(n => n.TrackingCode.Contains(item.TrackingCode)&&db.ExportInvoices.Where(m=>m.ExportGood.Id==n.ExportGoodId).Count()>0).Count() > 0)
                {
                    statusId = 5;
                }
                item.StatusId = statusId;
            }
            //update AgencyPackage
            var lstAP = db.AgencyPackages;
            foreach (var item in lstAP)
            {
                if (lstSJP.Where(n => n.TrackingCode.Contains(item.TrackingCode)).Count() > 0)
                {
                    item.TrackingStatusId = lstSJP.Where(n => n.TrackingCode.Contains(item.TrackingCode.Trim())).First().StatusId;
                }
            }
            db.SaveChanges();
            db.Dispose();
        }
    }
}