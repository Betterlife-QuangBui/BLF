using OhayooWeb.Helpers;
using OhayooWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OhayooWeb.Areas.yahooshopping.Controllers
{
    public class HomeController : Controller
    {
        // GET: yahooshopping/Home
        public ActionResult Index()
        {
            List<ProductInfo> list = ProductShoppingUtils.getProductHome();
            return View(list);
        }
    }
}