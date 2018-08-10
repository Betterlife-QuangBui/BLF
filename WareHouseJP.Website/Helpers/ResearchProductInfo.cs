using CsQuery;
using Nager.AmazonProductAdvertising.Model;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using WareHouseJP.Website.Models;
using Web.Helpers.Amazon;
using Web.Helpers.library;
using Web.Helpers.YahooAuction;
using Web.Helpers.YahooShopping;

namespace WareHouseJP.Website.Helpers
{
    public class ResearchProductInfo
    {
        public void Reseach(string SearchCode, int SearchQuantity, Guid StoregeJPId)
        {
            try
            {
                try
                {
                    var item = db.WareHouseItems.First(n => n.ProductCode == SearchCode);
                    #region create model
                    StorageItemJP model = new StorageItemJP()
                    {
                        Amount = item.PriceTax * SearchQuantity,
                        CategoryId = item.CategoryId,
                        CategoryName = item.CategoryName,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        Image = item.Image,
                        JanCode = item.JanCode,
                        LinkWeb = item.LinkWeb,
                        MadeIn = item.MadeIn,
                        Material = item.Material,
                        NameEN = item.NameEN,
                        NameJP = item.NameJP,
                        Notes = item.Notes,
                        PriceTax = item.PriceTax,
                        ProductCode = item.ProductCode,
                        Quantity = SearchQuantity,
                        StatusId = 1,
                        StoregeJPId = StoregeJPId,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    #endregion
                    db.StorageItemJPs.Add(model);
                    db.SaveChanges();
                }
                catch
                {
                    #region Research Product Internet
                    //rakuten
                    try { getRakutenProductsDetail(SearchCode, SearchQuantity, StoregeJPId); }
                    catch
                    {
                        //amazon
                        try { getAmazonJPProductsDetail(SearchCode, SearchQuantity, StoregeJPId); }
                        catch
                        {
                            //yahoo shopping
                            try { getYahooShoppingProductsDetail(SearchCode, SearchQuantity, StoregeJPId); }
                            catch
                            {
                                //Uniqlo
                                try { getUniqloProductsDetail(SearchCode, SearchQuantity, StoregeJPId); }
                                catch
                                {
                                    //yahoo Auction
                                    try { getYahooAuctionProductsDetail(SearchCode, SearchQuantity, StoregeJPId); }
                                    catch
                                    {
                                        getOtherProductsDetail(SearchCode, SearchQuantity, StoregeJPId);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        public WareHouseJPDB db = new WareHouseJPDB();
        #region Rakuten
        public string getJanCode(string LinkWeb)
        {
            string result = "";
            try
            {
                result=CQ.CreateFromUrl(LinkWeb)[".item_number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
            }
            catch
            {

                
            }
            return result;
        }

        public void SearchAndSave(SearchProductInfo item,string ItemCode, int SearchQuantity = 1, Guid? StoregeJPId = null)
        {
            StorageItemJP model = new StorageItemJP()
            {
                Amount = item.PriceTax * SearchQuantity,
                CategoryId = item.CategoryId,
                CategoryName = item.CategoryName,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Staff.UserName,
                Id = Guid.NewGuid(),
                Image = item.Image,
                JanCode = item.JanCode,
                LinkWeb = item.LinkWeb,
                MadeIn = "JAPAN",
                Material = item.Material,
                NameEN = item.NameEN,
                NameJP = item.NameJP,
                Notes = item.Notes,
                PriceTax = item.PriceTax,
                ProductCode = item.ProductCode,
                Quantity = SearchQuantity,
                StatusId = 1,
                StoregeJPId = StoregeJPId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = user.Staff.UserName
            };
            if (db.StorageItemJPs.Where(n => n.ProductCode == model.ProductCode && n.StoregeJPId == StoregeJPId).Count() == 0)
            {
                db.StorageItemJPs.Add(model);
            }

            WareHouseItem items = new WareHouseItem()
            {
                Amount = item.PriceTax * SearchQuantity,
                CategoryId = item.CategoryId,
                CategoryName = item.CategoryName,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Staff.UserName,
                Id = Guid.NewGuid(),
                Image = item.Image,
                JanCode = item.JanCode,
                LinkWeb = item.LinkWeb,
                MadeIn = "JAPAN",
                Material = item.Material,
                NameEN = item.NameEN,
                NameJP = item.NameJP,
                Notes = item.Notes,
                PriceTax = item.PriceTax,
                ProductCode = item.ProductCode,
                Quantity = SearchQuantity,
                UpdatedAt = DateTime.Now,
                UpdatedBy = user.Staff.UserName
            };
            if (db.WareHouseItems.Where(n => n.ProductCode == model.ProductCode).Count() == 0)
            {
                db.WareHouseItems.Add(items);
            }
            db.SaveChanges();
        }
        UserPage user = new UserPage();
        public String getRakutenCategories(int CategoryId)
        {
            string appId = "1095695100824025173";
            List<String> lst = new List<String>();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaGenre/Search/20140222?format=xml&genreId=" + CategoryId + "&applicationId=" + appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                return WebUtility.HtmlEncode(xdoc.Element("root").Element("current").Element("genreName").Value);
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        public void getRakutenProductsDetail(string ItemCode,int SearchQuantity=1,Guid? StoregeJPId=null)
        {
            string appId = "1095695100824025173";
            SearchProductInfo item = new SearchProductInfo();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaItem/Search/20140222?format=xml&applicationId=" + appId + "&itemCode=" + ItemCode;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("root").Elements("Items").Elements("Item"))
                {
                    string img = "";
                    foreach (var ig in element.Element("smallImageUrls").Elements("imageUrl"))
                    {
                        img = ig.Value;
                        break;
                    }
                    item = new SearchProductInfo()
                    {
                        JanCode = getJanCode(element.Element("itemUrl").Value),
                        Image = img,
                        LinkWeb = element.Element("itemUrl").Value,
                        NameJP = WebUtility.HtmlEncode(element.Element("itemName").Value),
                        NameEN = TranslateUtils.TranslateGoogleTextEN(WebUtility.HtmlEncode(element.Element("itemName").Value)),
                        PriceTax = Convert.ToDouble(element.Element("itemPrice").Value),
                        Quantity = SearchQuantity,
                        Amount = Convert.ToDouble(element.Element("itemPrice").Value)* SearchQuantity,
                        ProductCode = WebUtility.HtmlEncode(element.Element("itemCode").Value),
                        Notes = WebUtility.HtmlEncode(element.Element("itemCaption").Value),
                        Material = WebUtility.HtmlEncode(element.Element("catchcopy").Value),
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value),
                        CategoryName = getRakutenCategories(Convert.ToInt32(element.Element("genreId").Value)),
                        MadeIn = "Japan",
                    };
                    SearchAndSave(item, ItemCode, SearchQuantity, StoregeJPId);
                    break;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        #endregion

        #region Yahoo Shopping
        public void getYahooShoppingProductsDetail(string ItemCode, int SearchQuantity = 1, Guid? StoregeJPId = null)
        {
            SearchProductInfo item = new SearchProductInfo();
            try
            {
                YahooShoppingUtils util = new YahooShoppingUtils();
                var result = util.getProductDetailVersionBasic(ItemCode);
                item = new SearchProductInfo()
                {
                    JanCode = result.Code,
                    Image = result.Image,
                    LinkWeb = result.Url,
                    NameJP = result.Name,
                    NameEN = TranslateUtils.TranslateGoogleTextEN(result.Name),
                    PriceTax = result.Price,
                    Quantity = SearchQuantity,
                    Amount = result.Price * SearchQuantity,
                    ProductCode = result.Code,
                    Notes = "",
                    Material = "",
                    MadeIn = "Japan",
                };
                SearchAndSave(item, ItemCode, SearchQuantity, StoregeJPId);


            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        #endregion

        #region Yahoo Auction
        public void getYahooAuctionProductsDetail(string ItemCode, int SearchQuantity = 1, Guid? StoregeJPId = null)
        {
            SearchProductInfo item = new SearchProductInfo();
            try
            {
                YahooAuctionUtils util = new YahooAuctionUtils();
                var result=util.getProductDetailVersionBasic(ItemCode);
                item = new SearchProductInfo()
                {
                    JanCode = result.AuctionID,
                    Image = result.Img.FirstOrDefault(),
                    LinkWeb = result.AuctionItemUrl,
                    NameJP = result.Title,
                    NameEN = TranslateUtils.TranslateGoogleTextEN(result.Title),
                    PriceTax = result.Price,
                    Quantity = SearchQuantity,
                    Amount = result.Price * SearchQuantity,
                    ProductCode = result.AuctionID,
                    Notes = "",
                    Material = "",
                    CategoryId = Convert.ToInt32(result.CategoryID),
                    CategoryName = util.getCategoryDetail(Convert.ToInt32(result.CategoryID)).CategoryName,
                    MadeIn = result.Location,
                };
                SearchAndSave(item, ItemCode, SearchQuantity, StoregeJPId);
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        #endregion

        #region Amazon JP
        public void getAmazonJPProductsDetail(string ItemCode, int SearchQuantity = 1, Guid? StoregeJPId = null)
        {
            SearchProductInfo item = new SearchProductInfo();
            try
            {
                AmazonUtils util = new AmazonUtils();
                Item result = util.Detail(ItemCode);

                string strPrice = result.OfferSummary?.LowestNewPrice?.FormattedPrice;
                strPrice = Regex.Matches(strPrice, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                double price = Convert.ToDouble(strPrice);

                item = new SearchProductInfo()
                {
                    JanCode = result.ASIN,
                    Image = result.LargeImage!=null?result.LargeImage.URL:result.MediumImage.URL,
                    LinkWeb = result.DetailPageURL,
                    NameJP = result.ItemAttributes.Title,
                    NameEN = TranslateUtils.TranslateGoogleTextEN(result.ItemAttributes.Title),
                    PriceTax = price,
                    Quantity = SearchQuantity,
                    Amount = price * SearchQuantity,
                    ProductCode = result.ASIN,
                    Notes = "",
                    Material = "",
                    MadeIn = "Japan",
                };
                SearchAndSave(item, ItemCode, SearchQuantity, StoregeJPId);

            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        #endregion

        #region Uniqlo
        public void getUniqloProductsDetail(string ItemCode, int SearchQuantity = 1, Guid? StoregeJPId = null)
        {
            SearchProductInfo item = new SearchProductInfo();
            try
            {

                string url = "http://www.uniqlo.com/jp/store/search.do?qtext=" + ItemCode + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=10#thumbnailSelect";
                var dom = CQ.CreateFromUrl(url).Select("#blkMainItemList .unit:first .info .name").FirstOrDefault();
                if (dom == null)
                {
                    url = "http://www.uniqlo.com/jp/store/search.do?qtext=" + ItemCode + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=20#thumbnailSelect";
                    dom = CQ.CreateFromUrl(url).Select("#blkMainItemList .unit:first .info .name").FirstOrDefault();
                }
                if (dom != null)
                {
                    string href = CQ.Create(dom)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                    item.LinkWeb = href;
                    string price = CQ.CreateFromUrl(url)[".price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                    price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                    item.PriceTax = Convert.ToDouble(price);
                    var domDetail = CQ.CreateFromUrl(href).Select("#content").FirstOrDefault();
                    item.Image = CQ.CreateFromUrl(href).Select("meta[property=og:image]").Select(x => x.Cq().Attr("content")).FirstOrDefault().ToString().Trim();
                    if (domDetail != null)
                    {
                        item.NameJP = WebUtility.HtmlEncode(CQ.Create(domDetail)["#goodsNmArea"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                        item.NameEN = TranslateUtils.TranslateGoogleTextEN(item.NameJP);
                        string JanCode = WebUtility.HtmlEncode(CQ.Create(domDetail)["#basic li.number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                        item.JanCode =item.ProductCode = JanCode.Substring(5);
                        item.MadeIn = "Japan";
                        item.Material = WebUtility.HtmlEncode(CQ.Create(domDetail)[".content .spec dd:first"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                    }
                }
                SearchAndSave(item, ItemCode, SearchQuantity, StoregeJPId);
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        #endregion

        #region Other
        public void getOtherProductsDetail(string ItemCode, int SearchQuantity = 1, Guid? StoregeJPId = null)
        {
            try
            {
                    StorageItemJP model = new StorageItemJP()
                    {
                        CreatedAt = DateTime.Now,
                        ProductCode=ItemCode,JanCode=ItemCode,
                        CreatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        Quantity = SearchQuantity,
                        StatusId = 1,
                        StoregeJPId = StoregeJPId,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.StorageItemJPs.Add(model);
                    
                    db.SaveChanges();
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        #endregion
    }
}