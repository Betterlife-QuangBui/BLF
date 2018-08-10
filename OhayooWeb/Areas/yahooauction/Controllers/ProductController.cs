using OhayooWeb.Helpers;
using OhayooWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhayooWeb.Areas.yahooauction.Controllers
{
    public class ProductController : Controller
    {
        // GET: yahooauction/Product
        public ActionResult Category(int id = 0, string name = "", int page = 1, string sort = "standard")
        {
            if (sort == "standard") { sort = "Y2JpZHMsYQ%3D%3D"; }
            Pager pager = new Pager(10, 100, 5, Request.Url.AbsoluteUri);
            var list = ProductYahooAuctionUtils.getProductList(page, id, sort, "", "", name);
            ViewBag.Title = name;
            ViewBag.url = Request.Url.AbsoluteUri;
            return View(list);
        }
        public ActionResult Detail(string id = "", string name = "", int cateid = 0, string catename = "", int page = 1, string sort = "standard")
        {
            if (sort == "standard") { sort = "Y2JpZHMsYQ%3D%3D"; }
            ProductInfo product = ProductYahooAuctionUtils.getDetail(1, cateid, sort, "", "", catename, id);
            ViewBag.Title = product.name;
            return View(product);
        }
    }
}