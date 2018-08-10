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
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            string url = "http://buyee.jp/rakuten/";
            var listCate = new List<Category>();
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
                    int id = Convert.ToInt32(link.ToList()[0].ToString().Substring(link.ToList()[0].ToString().LastIndexOf('/') + 1));
                    Category cate = new Category()
                    {
                        name = nameCate,
                        url = linkWeb,
                        id = id,
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
                            CateId=id,
                            id = Convert.ToInt32(linkSub.ToList()[0].ToString().Substring(linkSub.ToList()[0].ToString().LastIndexOf('/') + 1)),
                        };
                        listSubCate.Add(cateSub);
                    }
                }

            }
            catch { }
            return PartialView(new MenuPage() { listCate=listCate,listSub= listSubCate });
        }
    }
}