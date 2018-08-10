using OhayooWeb.Helpers;
using OhayooWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhayooWeb.Areas.rakuten.Controllers
{
    public class HomeController : Controller
    {
        // GET: rakuten/Home
        public ActionResult Index()
        {
            List<ProductInfo> list = ProductRakutenUtils.getProductHome();
            return View(list);
        }
    }
}