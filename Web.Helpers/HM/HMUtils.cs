using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.Helpers.YahooShopping.Models;

namespace Web.Helpers.HM
{
    public class HMUtils
    {
        public List<YSProduct> Search(string key)
        {
            List<YSProduct> items = new List<YSProduct>();
            string url = "http://www2.hm.com/ja_jp/search-results.html?q=" + key + "&sort=ascPrice&offset=0&page-size=80";

            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select("div.product-items-content .product-item");

            foreach (var item in divs.ToList())
            {
                try
                {
                    YSProduct model = new YSProduct();
                    model.Name = WebUtility.HtmlEncode(CQ.Create(item)["h3.product-item-headline"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                    model.Url = "http://www2.hm.com" + CQ.Create(item)["h3.product-item-headline a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                    string price = CQ.Create(item)[".product-item-price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                    model.Price = Convert.ToDouble(price);
                    string imgUrl = "";
                    try
                    {
                        var dom1 = CQ.CreateFromUrl(model.Url);
                        imgUrl= "http:" + dom1.Select(".product-detail-main-image-container img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                    }
                    catch { }
                    model.Image = imgUrl;
                    try
                    {
                        string JanCode = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.Url)[".product-detail-article-code"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                        model.ProductId = model.Code = JanCode;
                        model.Description = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.Url)[".tab-details"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                    }
                    catch { }
                    items.Add(model);
                }
                catch { }
            }
            return items;
        }
    }
}
