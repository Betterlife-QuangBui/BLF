using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.Helpers.YahooShopping.Models;

namespace Web.Helpers.Zara
{
    public class ZaraUtils
    {
        public List<YSProduct> Search(string key)
        {
            List<YSProduct> items = new List<YSProduct>();
            string url = "http://www2.hm.com/ja_jp/search-results.html?q=" + key + "&sort=ascPrice&offset=0&page-size=80";

            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select("div.product-items-content .product-item");

            foreach (var item in divs.ToList())
            {
                YSProduct model = new YSProduct();
                model.Name = WebUtility.HtmlEncode(CQ.Create(item)["h3.product-item-headline"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                model.Url = CQ.Create(item)["h3.product-item-headline"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                string price = CQ.Create(item)[".product-item-price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                model.Price = Convert.ToDouble(price);
                model.Image = CQ.Create(item)["img.product-item-image"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                try
                {
                    string JanCode = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.Url)[".product-detail-article-code"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                    model.ProductId = model.Code = JanCode;
                    model.Description = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.Url)[".tab-details"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                }
                catch { }
                items.Add(model);
            }
            return items;
        }
    }
}
