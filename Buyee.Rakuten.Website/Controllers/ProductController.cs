using Buyee.Rakuten.Website.Helpers;
using Buyee.Rakuten.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buyee.Rakuten.Website.Controllers
{
    public class ProductController : Controller
    {
        
        public ActionResult Detail(string id = "", string name = "",int cateid=0,string catename="", int page = 1, string sort = "standard")
        {
            ProductInfo product = ProductUtils.getDetail(1, cateid, sort, "", "", catename, id);
            ViewBag.Title = product.name;
            return View(product);
        }
        // GET: Product
        public ActionResult Category(int id=0,string name="",int page=1,string sort= "standard")
        {
            Pager pager = new Pager(10, 100, 5,Request.Url.AbsoluteUri);
            var list = ProductUtils.getProductList(page, id,sort,"","",name);
            ViewBag.Title = name;
            return View(list);
        }
    }
}