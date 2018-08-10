using CsQuery;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.Helpers.Rakuten.Models;

namespace Web.Helpers.Amazon
{
    public static class DoubleUtils
    {
        public static double ToDouble(this string source)
        {
            try { return Convert.ToDouble(Regex.Matches(source, @"[0-9]*[\.,]?[0-9]+")[0].Value); } catch { return 0; }
        }
    }
    public class AmazonUtils
    {
        private AmazonAuthentication GetConfig()
        {
            var accessKey = "AKIAICLDWPVAPD6Q2LMA";
            var secretKey = "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";

            var authentication = new AmazonAuthentication();
            authentication.AccessKey = accessKey;
            authentication.SecretKey = secretKey;

            return authentication;
        }
        public Item Detail(string articleNumber)
        {
            try
            {
                var authentication = this.GetConfig();

                var wrapper = new AmazonWrapper(authentication, AmazonEndpoint.JP);
                var result = wrapper.Lookup(articleNumber);

                var item = result.Items?.Item.FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }
        public double Price(string articelNumber)
        {
            double price = 0;
            try {
                string strPrice = Detail(articelNumber).OfferSummary?.LowestNewPrice?.FormattedPrice;
                strPrice = Regex.Matches(strPrice, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                price = Convert.ToDouble(strPrice);
            } catch { }
            return price;
        }
        public Item[] Search(string search)
        {
            var authentication = this.GetConfig();

            var wrapper = new AmazonWrapper(authentication, AmazonEndpoint.JP);
            var responseGroup = AmazonResponseGroup.ItemAttributes | AmazonResponseGroup.Images | AmazonResponseGroup.ItemIds | AmazonResponseGroup.Accessories;

            var result = wrapper.Search(search, AmazonSearchIndex.All, responseGroup);

            return result.Items.Item;
        }

        public ProductPagger SearchBy(string search)
        {
            ProductPagger models = new ProductPagger();
            List<RAProduct> products = new List<RAProduct>();
            try
            {
                string url = String.Format("http://zenmarket.jp/ja/amazon.aspx?c={0}&q={1}&p={2}", "", search, 1);
                var dom = CQ.CreateFromUrl(url);

                CQ divs = dom.Select("#main");
                //Get Pagging
                var pagging = CQ.Create(divs)["#top_page_num"].Text();
                var items = CQ.Create(divs).Select(".product");
                //detail url=
                string urlProductDetail = "http://zenmarket.jp/ja/amazonproduct.aspx?itemCode={0}";
                string categoryName = "";
                //Get Product
                foreach (var item in items.ToList())
                {
                    string name = CQ.Create(item)["h3.product-title"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    string image = CQ.Create(item)["img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                    string productId = CQ.Create(item)["a.product-link"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                    string linkWeb= ""; 
                    productId = productId.Substring(productId.LastIndexOf('=') + 1);
                    double price = 0;
                    string des = "";
                    try
                    {
                        string strPrice = CQ.Create(item)[".amount"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();

                        if (strPrice != null && strPrice.Length > 0)
                        {
                            price = strPrice.ToDouble();
                        }
                        var domDetail = CQ.CreateFromUrl(String.Format(urlProductDetail, productId));
                        CQ divDetail = domDetail.Select("#main");
                        linkWeb = CQ.Create(divDetail)["a#productPage"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                        des = CQ.Create(divDetail)["#txtItemDescription"].Text();
                        //get navaigaton
                        var navs = CQ.Create(divDetail).Select("#breadcrumb a");
                        Dictionary<string, string> dicNavs = new Dictionary<string, string>();
                        int i = 0;
                        foreach (var bred in navs.ToList())
                        {
                            if (i == navs.ToList().Count - 1) { break; }
                            string NameNav = CQ.Create(bred).Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                            categoryName = NameNav;
                            i++;
                        }
                    }
                    catch { }

                    RAProduct p = new RAProduct() { ItemCode = productId, ItemPrice = price, ItemName = name, ImageUrl = image, Catchcopy = des,ItemUrl= linkWeb,CategoryName= categoryName };
                    products.Add(p);
                }
                models.Items = new List<RAProduct>();
                models.Items.AddRange(products);
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return models;
        }
    }
}
