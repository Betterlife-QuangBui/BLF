using OhayooWeb.Helpers;
using OhayooWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhayooWeb.Areas.yahooauction.Controllers
{
    public class HomeController : Controller
    {
        // GET: yahooauction/Home
        public ActionResult Index()
        {
            List<ProductInfo> list = ProductYahooAuctionUtils.getProductHome();
            return View(list);
        }
    }
}