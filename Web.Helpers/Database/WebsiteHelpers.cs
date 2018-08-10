using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using System.Net;

namespace Web.Helpers.Database
{
   public class WebsiteHelpers
    {
        #region locondo.jp
        public string LocondoJP(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select("#product_img .product-image a img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch(Exception ex) { }
            return imgUrl;
        }
        #endregion
        #region dena-ec.com
        public string DenaEC(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select(".uniMainCarousel .js-mainCarousel_thumbs ul li:eq(0) a img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region www.aeo.jp
        public string AeoJP(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return "http:"+ dom.Select(".ItemDetailImageThumUL li:eq(0) a").Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region forever21.co.jp
        public string Forever21(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select(".pdp_zoom a.m_zoomin img.ItemImage").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region rakuten.co.jp
        public string Rakuten(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                imgUrl= dom.Select("a.rakutenLimitedId_ImageMain1-3:eq(0) img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                imgUrl = imgUrl.Substring(0, imgUrl.LastIndexOf("?_ex="));
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region amazon.co.jp
        public string Amazon(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                imgUrl= dom.Select("img#landingImage").Select(x => x.Cq().Attr("data-a-dynamic-image")).FirstOrDefault().ToString().Trim();
                imgUrl = WebUtility.HtmlEncode(imgUrl);
                imgUrl = imgUrl.Replace("{&quot;", "");
                imgUrl = imgUrl.Substring(0,imgUrl.IndexOf("&quot;"));
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region shopping.yahoo.co.jp
        public string YahooShopping(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select("li.elNew a img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region auctions.yahoo.co.jp
        public string YahooAuction(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select("div.ProductImage__inner:eq(0) img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region uniqlo.com/jp not_yet
        public string Uniqlo(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select("meta[property=og:image]").Select(x => x.Cq().Attr("content")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region hm.com/ja_jp/
        public string HM(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return "http:" + dom.Select(".product-detail-main-image-container img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region shop.adidas.jp
        public string Adidas(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select("meta[property=og:image]").Select(x => x.Cq().Attr("content")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region wear.jp
        public string Wear(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return "http:"+dom.Select("#bigimage .item p img").Select(x => x.Cq().Attr("data-original")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region hikaku.com
        public string Hikaku(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select(".CompareItemHighBoxItemMiddleImage a img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region crocs.co.jp
        public string Crocs(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select("meta[property=og:image]").Select(x => x.Cq().Attr("content")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region zara.com
        public string Zara(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return "http:" + dom.Select("#main-images ._seoImg").Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim().Replace("&wid=60&hei=72", "");
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region lacoste.jp
        public string Lacoste(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select(".js-jqzoom img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region gap.co.jp
        public string Gap(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return dom.Select(".js-jqzoom img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        #region nissen.co.jp
        public string Nissen(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                return "http://www.nissen.co.jp" + dom.Select("#mainimage_view img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            }
            catch { }
            return imgUrl;
        }
        #endregion
        public string GetImage(string url)
        {
            //url = url.ToLower();
            if (url.Contains("locondo.jp")) { return LocondoJP(url); }
            else if (url.Contains("rakuten.co.jp")) { return Rakuten(url); }
            else if (url.Contains("amazon.co.jp")) { return Amazon(url); }
            else if (url.Contains("shopping.yahoo.co.jp")) { return YahooShopping(url); }
            else if (url.Contains("auctions.yahoo.co.jp")) { return YahooAuction(url); }
            else if (url.Contains("uniqlo.com")) { return Uniqlo(url); }
            else if (url.Contains("hm.com")) { return HM(url); }
            else if (url.Contains("dena-ec.com")) { return DenaEC(url); }
            else if (url.Contains("forever21.co.jp")) { return Forever21(url); }
            else if (url.Contains("shop.adidas.jp")) { return Adidas(url); }
            else if (url.Contains("aeo.jp")) { return AeoJP(url); }
            else if (url.Contains("wear.jp")) { return Wear(url); }
            else if (url.Contains("hikaku.com")) { return Hikaku(url); }
            else if (url.Contains("crocs.co.jp")) { return Crocs(url); }
            else if (url.Contains("zara.com")) { return Zara(url); }
            else if (url.Contains("lacoste.jp")) { return Lacoste(url); }
            else if (url.Contains("gap.co.jp")) { return Gap(url); }
            else if (url.Contains("nissen.co.jp")) { return Nissen(url); }
            return "";
        }
    }
}
