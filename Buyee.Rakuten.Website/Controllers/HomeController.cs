using Buyee.Rakuten.Website.Helpers;
using Buyee.Rakuten.Website.Models;
using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Buyee.Rakuten.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<ProductInfo> list = ProductUtils.getProductHome();
            return View(list);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Menu()
        {
            string html = "";
            string url = "http://buyee.jp/rakuten/";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                html = dom.Select("#side_category_navi").ToList()[0].InnerHTML;
                html = WebUtility.HtmlDecode(html);
                html = html.Replace("href=\"", "href=\"http://buyee.jp/");
            }
            catch { }
            return PartialView(html);
        }
    }
}