using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Helpers
{
    public class SearchWebsiteUtils
    {
        public static List<SelectListItem> GetWebsite(int cate = 0)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Database", Value = "Database" });
            list.Add(new SelectListItem() { Text = "Rakuten", Value = "Rakuten" });
            list.Add(new SelectListItem() { Text = "Amazon", Value = "Amazon" });
            list.Add(new SelectListItem() { Text = "Yahoo Shopping", Value = "YahooShopping" });
            list.Add(new SelectListItem() { Text = "Yahoo Auction", Value = "YahooAuction" });
            list.Add(new SelectListItem() { Text = "Uniqlo", Value = "Uniqlo" });
            list.Add(new SelectListItem() { Text = "Adidas", Value = "Adidas" });
            list.Add(new SelectListItem() { Text = "Hm", Value = "Hm" });
            list.Add(new SelectListItem() { Text = "Zara", Value = "Zara" });
            return list;
        }
    }
}