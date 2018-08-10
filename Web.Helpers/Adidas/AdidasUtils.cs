using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.Helpers.YahooShopping.Models;

namespace Web.Helpers.Adidas
{
    public class AdidasUtils
    {
        public List<YSProduct> Search(string key)
        {
            List<YSProduct> items = new List<YSProduct>();
            string url = "http://shop.adidas.jp/pc/item/?order=2&stock=0&limit=80&sscan=&page=1&q=" + key;

            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select("#itemList ul li");

            foreach (var item in divs.ToList())
            {
                YSProduct model = new YSProduct();
                model.Name = WebUtility.HtmlEncode(CQ.Create(item)["span.name_sub_area"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                model.Url = CQ.Create(item)[".plp_item__name a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                string price = CQ.Create(item)[".price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                model.Price = Convert.ToDouble(price);
                string categoryName = "";
                var navs = CQ.CreateFromUrl(model.Url).Select("#breadcrumb li");
                foreach (var bred in navs.ToList())
                {
                    try
                    {
                        string NameNav = CQ.Create(bred)["span[itemprop='name']"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        categoryName = NameNav;
                    }
                    catch { }
                }
                model.CategoryName = categoryName;
                model.Image = "http://shop.adidas.jp" + CQ.Create(item)["img.plp_item__img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                try
                {
                    string JanCode = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.Url)["#selected_article"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                    model.ProductId = model.Code = JanCode;

                    model.Description = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.Url)[".pointArea li:eq(1)"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                }
                catch { }
                items.Add(model);
            }
            return items;
        }
    }
}
