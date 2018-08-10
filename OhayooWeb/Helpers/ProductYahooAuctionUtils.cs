using CsQuery;
using Ohayoo.DB;
using Ohayoo.Lib;
using OhayooWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace OhayooWeb.Helpers
{
    public class ProductYahooAuctionUtils
    {
        public static string url = WebConfigurationManager.AppSettings["exchangerates"];

        public static double ExchangeRate()
        {
            OhayooDB db = new OhayooDB();
            double rate = 219.22; 
            try
            {
                if (db.ExchageRates.Count() > 0)
                {
                    rate = db.ExchageRates.OrderByDescending(n => n.id).FirstOrDefault().exRate.Value;
                }

                if (db.ExchageRateCharts.Count() == 0)
                {
                    rate = rate * 1.02;
                }
                else
                {
                    rate = rate * db.ExchageRateCharts.OrderByDescending(n => n.id).FirstOrDefault().exRate.Value;
                }

            }
            catch { }
            return rate;
        }
        public static ProductInfo getDetail(int page = 1, int category = 110729, string sort = "standard", string translationType = "", string query = "", string categoryName = "",string productId="")
        {
            ProductInfo pro = new ProductInfo();
            string url = "http://buyee.jp/item/yahoo/auction/"+productId+"?lang=ja";
            var item = CQ.CreateFromUrl(url).Select("#content").FirstOrDefault();
            pro.name = CQ.Create(item)["#itemHeader h1"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
            string pri = CQ.Create(item)[".current_price_output em"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
            pri = Regex.Matches(pri, @"[0-9]*[\.,]?[0-9]+")[0].Value;
            pro.price=Convert.ToDouble(pri);
            pro.cateName = categoryName;
            pro.image = CQ.Create(item)["ul.slides li a img:first"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            //pro.description = CQ.Create(item)["#shopping_item_detail_container"].Select(x => x.Cq().Document.InnerHTML).FirstOrDefault().ToString().Trim();
            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select("#itemPhoto_sec li");
            List<string> strImages = new List<string>();
            foreach (var img in divs.ToList())
            {
                string image = CQ.Create(img)["img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                strImages.Add(image);
            }
            pro.images = strImages;
            pro.summary = CQ.Create(item)["#itemDetail_sec"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim();
            pro.auction ="<dl>"+ CQ.Create(item)[".current_price"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim()+"</dl>";
            pro.auctionInfo = "<dl>" + CQ.Create(item)[".no_border"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim() + "</dl>";
            //pro.checkAvailable =CQ.Create(item)["#item_inventories"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim().Replace("generalicon-checkmark", "glyphicon glyphicon-ok").Replace("generalicon-remove", "glyphicon glyphicon-remove") ;
            //pro.attribute ="<dl class='attr'>"+ CQ.Create(item)["dl.shopping_input_container"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim()+"</dl>";
            return pro;
        }
        public static List<ProductInfo> getProductHome(string url= "http://buyee.jp/yahoo/auction?lang=ja")
        {
            List<ProductInfo> list = new List<ProductInfo>();
            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select(".rcmd_item_field .rcmd_product_whole");
            foreach (var item in divs.ToList())
            {
                string name = CQ.Create(item)["div.rcmd_product_title a"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                string itemcode = CQ.Create(item)["div.rcmd_product_title a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                itemcode = itemcode.Substring(itemcode.LastIndexOf('/') + 1);
                string image = CQ.Create(item)["img.rcmd_product_image"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                string pri = CQ.Create(item)["div.rcmd_product_price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                pri = Regex.Matches(pri, @"[0-9]*[\.,]?[0-9]+")[0].Value;

                //string auc = CQ.Create(item)["p.product_remaining"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim();
                //auc = auc.Replace("generalicon-clock", "glyphicon glyphicon-ok-circle");
                ProductInfo pro = new ProductInfo()
                {
                    image = image,
                    name = name,
                    price = Convert.ToDouble(pri),
                    itemCode = WebUtility.HtmlDecode(itemcode).Replace("%3A","-")
                    //,auction=auc
                };
                list.Add(pro);
            }
            return list;
        }

        public static ProductPagger getProductList(int page=1,int category= 110729, string sort= "standard",string translationType="",string query="",string categoryName="")
        {
            if (sort == "+itemPrice")
            {
                sort="%2BitemPrice";
            }
            string url = "http://buyee.jp/category/yahoo/auction/"+category+"?sort="+sort+"==&page="+page+"&lang=ja";
            //String url = "http://buyee.jp/rakuten/shopping/search/category/" + category + "?sort=" + sort + "&page=" + page + "&query=" + query + "&translationType=" + translationType;
            ProductPagger list = new ProductPagger();
            list.lstPros = new List<ProductInfo>();
            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select(".product_field .product_whole");
            foreach (var item in divs.ToList())
            {
                string name = CQ.Create(item)["p.product_title"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                string itemcode = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                itemcode = itemcode.Substring(itemcode.LastIndexOf('/') + 1);
                string image = CQ.Create(item)["img.product_image"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                string pri = CQ.Create(item)["p.product_price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                string auc = CQ.Create(item)["p.product_remaining"].Select(x => x.Cq().Html()).FirstOrDefault().ToString().Trim();
                auc = auc.Replace("generalicon-clock", "glyphicon glyphicon-ok-circle");
                pri = Regex.Matches(pri, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                ProductInfo pro = new ProductInfo()
                {
                    image = image,
                    name = name,
                    cateName=categoryName,CateId=category,
                    price = Convert.ToDouble(pri),
                    itemCode = WebUtility.HtmlDecode(itemcode).Replace("%3A", "-"),
                    auction=auc
                };
                list.lstPros.Add(pro);
            }
            //lay tong so trang cua 1 category
            String urlPage = "http://buyee.jp/category/yahoo/auction/"+category+"?sort="+sort+"==&page=1&lang=ja";
            dom = CQ.CreateFromUrl(urlPage);
            String pageCount = dom.Select("nav.search_page_navi .page_navi a:last").Select(x => x.Cq().Attr("onclick")).FirstOrDefault().ToString().Trim();
            pageCount = pageCount.Substring(pageCount.LastIndexOf('=') + 1, pageCount.IndexOf(";")- pageCount.LastIndexOf('=')-1);
            string urlPager = "/yahooauction/product/category/" + category + "/" + categoryName + "?page={0}&sort=" + sort;
            Pager pager = new Pager(20, 20 * Convert.ToInt32(pageCount), 5, urlPager);
            list.nav = pager.Navigation();
            return list;
        }
    }
}