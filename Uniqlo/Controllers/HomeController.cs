using CsQuery;
using CsQuery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Uniqlo.Models;

namespace Uniqlo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            ProductInfo pro = new ProductInfo();
            if (code.Contains("-")) { code = code.Substring(0, code.IndexOf('-')); }
            try
            {
                if (code.Contains("http://"))
                {
                    string codeSpit = code;
                    if (code.Contains("?"))
                    {
                        codeSpit = code.Substring(code.LastIndexOf('/') + 1, code.IndexOf('?'));
                    }
                    else
                    {
                        codeSpit = code.Substring(code.LastIndexOf('/') + 1);
                    }
                    string url = "http://www.uniqlo.com/jp/store/search.do?qtext=" + codeSpit + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=10#thumbnailSelect";
                    var dom = CQ.CreateFromUrl(url).Select("#blkMainItemList .unit:first .info .name").FirstOrDefault();
                    if (dom == null)
                    {
                        url = "http://www.uniqlo.com/jp/store/search.do?qtext=" + code + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=20#thumbnailSelect";
                        dom = CQ.CreateFromUrl(url).Select("#blkMainItemList .unit:first .info .name").FirstOrDefault();
                    }
                    if (dom != null)
                    {
                        string href = CQ.Create(dom)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                        pro.Url = href;
                        string price = CQ.Create(dom)[".price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                        pro.Price = Convert.ToDouble(price);
                        var domDetail = CQ.CreateFromUrl(href).Select("#content").FirstOrDefault();
                        pro.Image = CQ.CreateFromUrl(href).Select("meta[property=og:image]").Select(x => x.Cq().Attr("content")).FirstOrDefault().ToString().Trim();
                        if (domDetail != null)
                        {
                            pro.Name = WebUtility.HtmlEncode(CQ.Create(domDetail)["#goodsNmArea"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                            string JanCode = WebUtility.HtmlEncode(CQ.Create(domDetail)["#basic li.number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                            pro.JanCode = JanCode.Substring(5);
                            pro.Material = WebUtility.HtmlEncode(CQ.Create(domDetail)[".content .spec dd:first"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                        }
                    }
                }
                else
                {
                    string url = "http://www.uniqlo.com/jp/store/search.do?qtext="+code+"&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=10#thumbnailSelect";
                    var dom = CQ.CreateFromUrl(url).Select("#blkMainItemList .unit:first .info .name").FirstOrDefault();
                    if (dom == null)
                    {
                        url = "http://www.uniqlo.com/jp/store/search.do?qtext=" + code + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=20#thumbnailSelect";
                        dom = CQ.CreateFromUrl(url).Select("#blkMainItemList .unit:first .info .name").FirstOrDefault();
                    }
                    if (dom!=null)
                    {
                        string href=CQ.Create(dom)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                        pro.Url = href;
                        string price = CQ.CreateFromUrl(url)[".price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                        pro.Price = Convert.ToDouble(price);
                        var domDetail = CQ.CreateFromUrl(href).Select("#content").FirstOrDefault();
                        pro.Image = CQ.CreateFromUrl(href).Select("meta[property=og:image]").Select(x => x.Cq().Attr("content")).FirstOrDefault().ToString().Trim();
                        if (domDetail != null)
                        {
                            pro.Name = WebUtility.HtmlEncode(CQ.Create(domDetail)["#goodsNmArea"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                            string JanCode = WebUtility.HtmlEncode(CQ.Create(domDetail)["#basic li.number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                            pro.JanCode = JanCode.Substring(5);
                            pro.Material = WebUtility.HtmlEncode(CQ.Create(domDetail)[".content .spec dd:first"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            ViewBag.ProductInfo = pro;
            return View();
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
    }
}