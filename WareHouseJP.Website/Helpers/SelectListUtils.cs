using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Helpers
{
    public class SelectListUtils
    {
        public static List<SelectListItem> TrackingList(string trackingcode, bool flag = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 1; i <= 20; i++)
            {
                if (true)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.ToString("00"),
                        Text = i.ToString("00")
                    });
                }
                else
                {
                    //list.Add(new SelectListItem()
                    //{
                    //    Value = trackingcode + " - " + i.ToString("00"),
                    //    Text = trackingcode + " - " + i.ToString("00")
                    //});
                }
                
            }
            list.Add(new SelectListItem() { Value = "21", Text = "TH" });
            return list;
        }
        public static List<SelectListItem> WebsiteList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text="Database",Value="Database"});
            list.Add(new SelectListItem() { Text = "Rakuten", Value = "Rakuten" });
            list.Add(new SelectListItem() { Text = "Amazon", Value = "Amazon" });
            list.Add(new SelectListItem() { Text = "YahooShopping", Value = "YahooShopping" });
            list.Add(new SelectListItem() { Text = "YahooAuction", Value = "YahooAuction" });
            list.Add(new SelectListItem() { Text = "Uniqlo", Value = "Uniqlo" });
            list.Add(new SelectListItem() { Text = "Zara", Value = "Zara" });
            list.Add(new SelectListItem() { Text = "Adidas", Value = "Adidas" });
            list.Add(new SelectListItem() { Text = "Hm", Value = "Hm" });
            return list;
        }
        public static List<SelectListItem> RoleLogin()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem() { Value = null, Text = "Chọn quyền đăng nhập" });
            list.Add(new SelectListItem() { Value = "1", Text = "Đại lý" });
            list.Add(new SelectListItem() { Value = "2", Text = "BLF" });
            return list;
        }
    }
}