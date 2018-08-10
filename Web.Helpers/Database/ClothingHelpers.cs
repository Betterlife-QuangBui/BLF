using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Web.Helpers.Amazon;

namespace Web.Helpers.Database
{
    public class ClothingHelpers
    {
        #region locondo.jp
        public List<ClothingModel> LocondoJP(string key)
        {
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://www.locondo.jp/shop/search?x=39&y=16&searchWord=" + key;
                var dom = CQ.CreateFromUrl(url);
                CQ divs = dom.Select("#catalog_list div.pro_item");
                foreach (var item in divs.ToList())
                {
                    string name = CQ.Create(item)["h4.product_name"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    string image = CQ.Create(item)["img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                    string linkwebsite = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                    string productId = linkwebsite.Split('/')[linkwebsite.Split('/').Length - 2];
                    double price = 0;
                    string strPrice = CQ.Create(item)[".price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();
                    linkwebsite = "http://www.locondo.jp" + linkwebsite;
                    if (strPrice != null && strPrice.Length > 0)
                    {
                        price = strPrice.ToDouble();
                    }
                    var domDetail = CQ.CreateFromUrl(linkwebsite);
                    CQ divDetail = domDetail.Select("#main");
                    image = divDetail.Select("#product_img .product-image a img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                    string material = "";
                    try
                    {
                        material = divDetail.Select("div.product_table table tbody tr:eq(4) td").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    }
                    catch { }
                    ClothingModel model = new ClothingModel()
                    {
                        NameJP = name,
                        Amount = price,
                        Id = Guid.NewGuid(),
                        Material = material,
                        Image = image,
                        LinkWeb = linkwebsite,
                        JanCode = productId,
                        ProductCode = productId,
                        PriceTax = price,
                        Quantity = 1
                    };
                    lstProducts.Add(model);
                }
            }
            catch { }
            return lstProducts;
        }
        #endregion
        #region dena-ec.com
        public List<ClothingModel> DenaEC(string key)
        {
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://www.dena-ec.com/bep/m/klist3?e_scope=O&at=FP&non_gr=ex&spe_id=c_hd01&e=tsrc_topa_v&ipp=40&keyword=" + key + "&non_keyword=&categ_id=&clow=&chigh=";
                var dom = CQ.CreateFromUrl(url);
                CQ divs = dom.Select("div.conItemList01 ul li");
                if (divs.ToList().Count == 0)
                {
                    divs = dom.Select("ul.js-dfpKeywdSearch li");
                }
                foreach (var item in divs.ToList())
                {
                    string linkwebsite = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                    string productId = linkwebsite.Split('/')[2];
                    linkwebsite = "http://www.dena-ec.com" + linkwebsite;
                    var domDetail = CQ.CreateFromUrl(linkwebsite);
                    CQ divDetail = domDetail.Select("#main");

                    string name = divDetail.Select("#contents h1.name").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    string image = divDetail.Select(".uniMainCarousel .js-mainCarousel_thumbs ul li:eq(0) a img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();


                    double price = 0;
                    string strPrice = divDetail.Select(".price").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();

                    if (strPrice != null && strPrice.Length > 0)
                    {
                        price = strPrice.ToDouble();
                    }
                    string material = "";
                    ClothingModel model = new ClothingModel()
                    {
                        NameJP = name,
                        Amount = price,
                        Id = Guid.NewGuid(),
                        Material = material,
                        Image = image,
                        LinkWeb = linkwebsite,
                        JanCode = productId,
                        ProductCode = productId,
                        PriceTax = price,
                        Quantity = 1
                    };
                    lstProducts.Add(model);
                }
            }
            catch { }
            return lstProducts;
        }
        #endregion
        #region www.aeo.jp
        public List<ClothingModel> Aeo(string key)
        {
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://www.aeo.jp/disp/CSfSearchResults.jsp?q="+key;
                var dom = CQ.CreateFromUrl(url);
                CQ divs = dom.Select(".ProductList .ProductItem");
                foreach (var item in divs.ToList())
                {
                    if (lstProducts.Count > 30) { break; }

                    string linkwebsite = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();

                    var domDetail = CQ.CreateFromUrl(linkwebsite);
                    CQ divDetail = domDetail.Select("#main");

                    string productId = divDetail.Select("#itemcodeinfo").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    if (lstProducts.Where(n => n.ProductCode == productId).Count() == 0)
                    {
                        string name = CQ.Create(item)["span.name"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        string image = "http:"+CQ.Create(item)["img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                        double price = 0;
                        string strPrice = CQ.Create(item)[".price_c"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();
                        if (strPrice != null && strPrice.Length > 0)
                        {
                            price = strPrice.ToDouble();
                        }
                        string material = "";
                        try { material = divDetail.Select(".itemdetailcontent").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim(); }
                        catch { }
                        ClothingModel model = new ClothingModel()
                        {
                            NameJP = name,
                            Amount = price,
                            Id = Guid.NewGuid(),
                            Material = material,
                            Image = image,
                            LinkWeb = linkwebsite,
                            JanCode = productId,
                            ProductCode = productId,
                            PriceTax = price,
                            Quantity = 1
                        };
                        lstProducts.Add(model);
                    }
                }
            }
            catch { }
            return lstProducts;
        }
        #endregion
        #region forever21.co.jp
        public List<ClothingModel> Forever21(string key)
        {
            //return dom.Select(".pdp_zoom a.m_zoomin img.ItemImage").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://www.forever21.co.jp/Search/SearchResultList.aspx?br=f21&filter=" + key + "&SearchText=" + key+"&SearchOption=All";
                var dom = CQ.CreateFromUrl(url);
                CQ divs = dom.Select("div.product_grid div.product_item");
                foreach (var item in divs.ToList())
                {
                    if (lstProducts.Count > 30) { break; }

                    string linkwebsite = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();

                    var domDetail = CQ.CreateFromUrl(linkwebsite);
                    CQ divDetail = domDetail.Select("#main");

                    string productId = divDetail.Select("#itemcodeinfo").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    if (lstProducts.Where(n => n.ProductCode == productId).Count() == 0)
                    {
                        string name = CQ.Create(item)["h1.item_name_c"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        string image = CQ.Create(item)["img:eq(0)"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                        double price = 0;
                        string strPrice = CQ.Create(item)[".price_c"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();
                        if (strPrice != null && strPrice.Length > 0)
                        {
                            price = strPrice.ToDouble();
                        }
                        string material = "";
                        try { material = divDetail.Select(".itemdetailcontent").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim(); }
                        catch { }
                        ClothingModel model = new ClothingModel()
                        {
                            NameJP = name,
                            Amount = price,
                            Id = Guid.NewGuid(),
                            Material = material,
                            Image = image,
                            LinkWeb = linkwebsite,
                            JanCode = productId,
                            ProductCode = productId,
                            PriceTax = price,
                            Quantity = 1
                        };
                        lstProducts.Add(model);
                    }
                }
            }
            catch { }
            return lstProducts;
        }
        #endregion
        #region rakuten.co.jp
        public string Rakuten(string url)
        {
            string imgUrl = "";
            try
            {
                var dom = CQ.CreateFromUrl(url);
                imgUrl = dom.Select("a.rakutenLimitedId_ImageMain1-3:eq(0) img").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
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
                imgUrl = dom.Select("img#landingImage").Select(x => x.Cq().Attr("data-a-dynamic-image")).FirstOrDefault().ToString().Trim();
                imgUrl = WebUtility.HtmlEncode(imgUrl);
                imgUrl = imgUrl.Replace("{&quot;", "");
                imgUrl = imgUrl.Substring(0, imgUrl.IndexOf("&quot;"));
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
                return "http:" + dom.Select("#bigimage .item p img").Select(x => x.Cq().Attr("data-original")).FirstOrDefault().ToString().Trim();
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
        public List<ClothingModel> Crocs(string key)
        {
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://www.crocs.co.jp/on/demandware.store/Sites-crocs_jp-Site/ja_JP/Search-Show?q=" + key;
                var dom = CQ.CreateFromUrl(url);
                CQ divs = dom.Select("div.productCards ul li");
                foreach (var item in divs.ToList())
                {
                    if (lstProducts.Count > 30) { break; }

                    string linkwebsite = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();

                    var domDetail = CQ.CreateFromUrl(linkwebsite);
                    CQ divDetail = domDetail.Select("#mainContent");

                    string productId = divDetail.Select(".itemNum").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    productId = productId.Replace("アイテム#", "");
                    if (lstProducts.Where(n => n.ProductCode == productId).Count() == 0)
                    {
                        string name = divDetail.Select("#pname").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        string image = CQ.Create(item)["img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();


                        double price = 0;
                        string strPrice = CQ.Create(item)[".darkGrayText"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();
                        //strPrice = strPrice.Substring(0, strPrice.IndexOf('円')).Trim();
                        if (strPrice != null && strPrice.Length > 0)
                        {
                            price = strPrice.ToDouble();
                        }
                        string material = divDetail.Select(".productDetailsCopy").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        ClothingModel model = new ClothingModel()
                        {
                            NameJP = name,
                            Amount = price,
                            Id = Guid.NewGuid(),
                            Material = material,
                            Image = image,
                            LinkWeb = linkwebsite,
                            JanCode = productId,
                            ProductCode = productId,
                            PriceTax = price,
                            Quantity = 1
                        };
                        lstProducts.Add(model);
                    }
                }
            }
            catch { }
            return lstProducts;
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
        public List<ClothingModel> Gap(string key)
        {
            //return dom.Select(".pdp_zoom a.m_zoomin img.ItemImage").Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://www.gap.co.jp/browse/product.do?pid=" + key;
                var domDetail = CQ.CreateFromUrl(url);
                CQ divDetail = domDetail.Select("#mainContent");
                WebClient client = new WebClient();
                string content=client.DownloadString(url);
                System.IO.File.WriteAllText(@"D:\test.txt", content);
                string linkwebsite = url;
                string productId = key;
                double price = 0;
                string strPrice = divDetail.Select(".product-price").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();
                if (strPrice != null && strPrice.Length > 0)
                {
                    price = strPrice.ToDouble();
                }
                string name= divDetail.Select("h1").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();

                string image = "";
                //get image
                try
                {
                    image = divDetail.Html();
                    image = image.Substring(image.IndexOf("xlarge")+9);
                    image = image.Substring(0,image.IndexOf("},"));
                }
                catch { }
                
                string material = "";
                try { material = divDetail.Select(".sp_top_sm dash-list").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim(); }
                catch { }
                ClothingModel model = new ClothingModel()
                {
                    NameJP = name,
                    Amount = price,
                    Id = Guid.NewGuid(),
                    Material = material,
                    Image = image,
                    LinkWeb = linkwebsite,
                    JanCode = productId,
                    ProductCode = productId,
                    PriceTax = price,
                    Quantity = 1
                };
                lstProducts.Add(model);
            }
            catch(Exception ex) { }
            return lstProducts;
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

        #region reebok.jp
        public List<ClothingModel> Reebok(string key)
        {
            List<ClothingModel> lstProducts = new List<ClothingModel>();
            try
            {
                string url = "http://reebok.jp/pc/item/index.cgi?q=" + key + "&searchbox=1";
                var dom = CQ.CreateFromUrl(url);
                CQ divs = dom.Select("div#itemList li");
                foreach (var item in divs.ToList())
                {
                    if (lstProducts.Count > 30) { break; }

                    string linkwebsite = CQ.Create(item)["a.plp_item_list_link"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();

                    var domDetail = CQ.CreateFromUrl(linkwebsite);
                    CQ divDetail = domDetail.Select("#itemContents");

                    string productId = divDetail.Select("#selected_article").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    if (lstProducts.Where(n => n.ProductCode == productId).Count() == 0)
                    {
                        string name = CQ.Create(item)["span.name_sub_area"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                        string image = CQ.Create(item)["img:eq(1)"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                        double price = 0;
                        string strPrice = CQ.Create(item)[".itemPrice"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Replace(",", "").Trim();
                        if (strPrice != null && strPrice.Length > 0)
                        {
                            price = strPrice.ToDouble();
                        }
                        string material = "";
                        try { material = divDetail.Select(".bulletArea").Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim(); }
                        catch { }
                        ClothingModel model = new ClothingModel()
                        {
                            NameJP = name,
                            Amount = price,
                            Id = Guid.NewGuid(),
                            Material = material,
                            Image = image,
                            LinkWeb = linkwebsite,
                            JanCode = productId,
                            ProductCode = productId,
                            PriceTax = price,
                            Quantity = 1
                        };
                        lstProducts.Add(model);
                    }
                }
            }
            catch { }
            return lstProducts;
        }
        #endregion


    }
}
