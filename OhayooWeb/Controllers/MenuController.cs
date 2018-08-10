using CsQuery;
using Ohayoo.DB;
using OhayooWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OhayooWeb.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        OhayooDB db = new OhayooDB();
        public ActionResult Index()
        {
            String area = Session["area"] + "";
            string website = "rakuten";
            switch (area)
            {
                case "zozo": website = "zozo"; break;
                case "yahooshopping": website = "yahooshopping"; break;
                case "yahooauction": website = "yahooauction"; break;
                case "rakuten": website = "rakuten"; break;
                default:
                    website = "rakuten";
                    break;
            }
            var listCate = new List<OhayooWeb.Models.Category>();
            var listSubCate = new List<OhayooWeb.Models.SubCategory>();
            foreach (var item in db.Ohayoo_Category.Where(n=>n.website==website))
            {
                listCate.Add(new OhayooWeb.Models.Category() {
                    id=Convert.ToInt32(item.id.Trim()),
                    name=item.name_jp,
                    url=item.url
                });
            }
            foreach (var item in db.Ohayoo_SubCategory.Where(n => n.website == website))
            {
                listSubCate.Add(new OhayooWeb.Models.SubCategory()
                {
                    id = Convert.ToInt32(item.id.Trim()),
                    name = item.name_jp,
                    url = item.url,CateId=Convert.ToInt32(item.cateId)
                });
            }
            return PartialView(new MenuPage() { listCate=listCate,listSub= listSubCate,area= area });
        }
    }
}