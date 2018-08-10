using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Helpers
{
    public class TimeUtils
    {
        public static List<SelectListItem> TimeHours()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "12-14 AM", Value = "12-14 AM" });
            list.Add(new SelectListItem() { Text = "14-16 AM", Value = "14-16 AM" });
            list.Add(new SelectListItem() { Text = "16-18 AM", Value = "16-18 AM" });
            list.Add(new SelectListItem() { Text = "18-20 AM", Value = "18-20 AM" });
            list.Add(new SelectListItem() { Text = "20-21 AM", Value = "20-21 AM" });
            return list;
        }
    }
}