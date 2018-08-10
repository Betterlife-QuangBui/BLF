using Buyee.Rakuten.Website.Models;
using CsQuery;
using Ohayoo.DB;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Buyee.Rakuten.Website.Controllers
{
    public class ImportController : Controller
    {
        // GET: Import
        public ActionResult Index()
        {
            return View();
        }
        OhayooDB db = new OhayooDB();
        public MenuPage rakuten(string url)
        {
            var listCate = new List<Buyee.Rakuten.Website.Models.Category>();
            var listSubCate = new List<SubCategory>();
            try
            {
                var dom = CQ.CreateFromUrl(url);
                var divs = dom.Select("#side_category_list_rakuten > li");
                foreach (var item in divs.ToList())
                {
                    var name = CQ.Create(item)["a.p_search_link"].Select(x => x.Cq().Text());
                    var link = CQ.Create(item)["a.p_search_link"].Select(x => x.Cq().Attr("href"));
                    String linkWeb = "http://buyee.jp" + link.ToList()[0].ToString();
                    String nameCate = name.ToList()[0].ToString().Trim();
                    int idCate = Convert.ToInt32(link.ToList()[0].ToString().Substring(link.ToList()[0].ToString().LastIndexOf('/') + 1));
                    Buyee.Rakuten.Website.Models.Category cate = new Buyee.Rakuten.Website.Models.Category()
                    {
                        name = nameCate,
                        url = linkWeb,
                        id = idCate,
                    };
                    listCate.Add(cate);
                    //get subCategory
                    var subDivs = CQ.Create(item)["div.cat_children > ul > li"];
                    foreach (var sub in subDivs.ToList())
                    {
                        var nameSub = CQ.Create(sub)["a.search_link"].Select(x => x.Cq().Text());
                        var linkSub = CQ.Create(sub)["a.search_link"].Select(x => x.Cq().Attr("href"));
                        String linkSubWeb = "http://buyee.jp" + linkSub.ToList()[0].ToString();
                        String nameSubCate = nameSub.ToList()[0].ToString().Trim();
                        SubCategory cateSub = new SubCategory()
                        {
                            name = nameSubCate,
                            url = linkSubWeb,
                            CateId = idCate,
                            id = Convert.ToInt32(linkSub.ToList()[0].ToString().Substring(linkSub.ToList()[0].ToString().LastIndexOf('/') + 1)),
                        };
                        listSubCate.Add(cateSub);
                    }
                }

            }
            catch { }
            return new MenuPage() { listCate = listCate, listSub = listSubCate };
        }

        public MenuPage yahooauction(string url)
        {
            var listCate = new List<Buyee.Rakuten.Website.Models.Category>();
            var listSubCate = new List<SubCategory>();
            try
            {
                var dom = CQ.CreateFromUrl(url);
                var divs = dom.Select("#js_side_category_list > li");
                foreach (var item in divs.ToList())
                {
                    var name = CQ.Create(item)["a.p_search_link"].Select(x => x.Cq().Text());
                    var link = CQ.Create(item)["a.p_search_link"].Select(x => x.Cq().Attr("href"));
                    String linkWeb = "http://buyee.jp" + link.ToList()[0].ToString();
                    String nameCate = name.ToList()[0].ToString().Trim();
                    int idCate = Convert.ToInt32(link.ToList()[0].ToString().Substring(link.ToList()[0].ToString().LastIndexOf('/') + 1));
                    Buyee.Rakuten.Website.Models.Category cate = new Buyee.Rakuten.Website.Models.Category()
                    {
                        name = nameCate,
                        url = linkWeb,
                        id = idCate,
                    };
                    listCate.Add(cate);
                    //get subCategory
                    var subDivs = CQ.Create(item)["div.cat_children > ul > li"];
                    foreach (var sub in subDivs.ToList())
                    {
                        var nameSub = CQ.Create(sub)["a.search_link"].Select(x => x.Cq().Text());
                        var linkSub = CQ.Create(sub)["a.search_link"].Select(x => x.Cq().Attr("href"));
                        String linkSubWeb = "http://buyee.jp" + linkSub.ToList()[0].ToString();
                        String nameSubCate = nameSub.ToList()[0].ToString().Trim();
                        SubCategory cateSub = new SubCategory()
                        {
                            name = nameSubCate,
                            url = linkSubWeb,
                            CateId = idCate,
                            id = Convert.ToInt32(linkSub.ToList()[0].ToString().Substring(linkSub.ToList()[0].ToString().LastIndexOf('/') + 1)),
                        };
                        listSubCate.Add(cateSub);
                    }
                }

            }
            catch { }
            return new MenuPage() { listCate = listCate, listSub = listSubCate };
        }

        public MenuPage zozo(string url)
        {
            var listCate = new List<Buyee.Rakuten.Website.Models.Category>();
            var listSubCate = new List<SubCategory>();
            try
            {
                var dom = CQ.CreateFromUrl(url);
                var divs = dom.Select(".side-area .search-category div.category-each");
                foreach (var item in divs.ToList())
                {
                    var name = CQ.Create(item)["h3.category-name a"].Select(x => x.Cq().Text());
                    var link = CQ.Create(item)["h3.category-name a"].Select(x => x.Cq().Attr("href"));
                    String linkWeb = "https://zozo.buyee.jp" + link.ToList()[0].ToString();
                    String nameCate = name.ToList()[0].ToString().Trim();
                    int idCate = Convert.ToInt32(link.ToList()[0].ToString().Substring(link.ToList()[0].ToString().LastIndexOf('=') + 1));
                    Buyee.Rakuten.Website.Models.Category cate = new Buyee.Rakuten.Website.Models.Category()
                    {
                        name = nameCate,
                        url = linkWeb,
                        id = idCate,
                    };
                    listCate.Add(cate);
                    //get subCategory
                    var subDivs = CQ.Create(item)["ul.subcategory-list li.list"];
                    foreach (var sub in subDivs.ToList())
                    {
                        var nameSub = CQ.Create(sub)["a"].Select(x => x.Cq().Text());
                        var linkSub = CQ.Create(sub)["a"].Select(x => x.Cq().Attr("href"));
                        String linkSubWeb = "https://zozo.buyee.jp" + linkSub.ToList()[0].ToString();
                        String nameSubCate = nameSub.ToList()[0].ToString().Trim();
                        SubCategory cateSub = new SubCategory()
                        {
                            name = nameSubCate,
                            url = linkSubWeb,
                            CateId = idCate,
                            id = Convert.ToInt32(linkSub.ToList()[0].ToString().Substring(linkSub.ToList()[0].ToString().LastIndexOf('=') + 1)),
                        };
                        listSubCate.Add(cateSub);
                    }
                }

            }
            catch(Exception ex) { }
            return new MenuPage() { listCate = listCate, listSub = listSubCate };
        }
        public ActionResult UpdateCategory(string id = "rakuten")
        {
            string url = "http://buyee.jp/rakuten/";
            MenuPage menu = new MenuPage();
            switch (id.ToLower())
            {
                case "rakuten":
                    url = "http://buyee.jp/rakuten/";
                    menu = rakuten(url);
                    break;
                case "yahooauction":
                    url = "http://buyee.jp/yahoo/auction";
                    menu = yahooauction(url);
                    break;
                case "yahooshopping":
                    url = "http://buyee.jp/yahoo/shopping";
                    menu = yahooauction(url);
                    break;
                case "zozo":
                    url = "https://zozo.buyee.jp/?lang=ja";
                    menu = zozo(url);
                    break;
                default:
                    url = "http://buyee.jp/rakuten/"; menu = rakuten(url);
                    break;
            }
            
            foreach (var item in menu.listCate)
            {
                if (db.Ohayoo_Category.SingleOrDefault(n => n.id == item.id.ToString() && n.website == id.ToLower()) == null)
                {
                    Ohayoo_Category ct = new Ohayoo_Category()
                    {
                        id = item.id.ToString(),
                        name_en = item.name,
                        name_jp = item.name,
                        name_vn = item.name,
                        url = item.url.ToLower(),
                        website = id.ToLower()
                    };

                    db.Ohayoo_Category.Add(ct);
                }
            }
            foreach (var item in menu.listSub)
            {
                if (db.Ohayoo_SubCategory.SingleOrDefault(n => n.id == item.id.ToString() && n.website == id.ToLower()) == null)
                {
                    Ohayoo_SubCategory ct = new Ohayoo_SubCategory()
                    {
                        id = item.id.ToString(),
                        name_en = item.name,
                        name_jp = item.name,
                        name_vn = item.name,
                        url = item.url.ToLower(),
                        website = id.ToLower(),
                        cateId = item.CateId.ToString()
                    };
                    db.Ohayoo_SubCategory.Add(ct);
                }
            }
            db.SaveChanges();
            return View("Index");
        }
    }
}