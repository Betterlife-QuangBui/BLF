using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Web.Helpers.Buybid;
using Web.Helpers.Buybid.Model;
using Web.Helpers.library;
using Web.Helpers.Rakuten.Models;

namespace Web.Helpers.Rakuten
{
    public class RakutenUtils
    {
        string appId = "1095695100824025173";
        public List<Models.RaCategory> getCategories()
        {
            List<Models.RaCategory> lst = new List<Models.RaCategory>();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaGenre/Search/20140222?format=xml&genreId=0&applicationId=" + appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("root").Elements("children").Elements("child"))
                {
                    lst.Add(new Models.RaCategory()
                    {
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value),
                        CategoryName = WebUtility.HtmlEncode(element.Element("genreName").Value),
                        CategoryLevel = Convert.ToInt32(element.Element("genreLevel").Value)
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public List<RaSubCategory> getSubCategories(int CategoryId)
        {
            List<RaSubCategory> lst = new List<RaSubCategory>();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaGenre/Search/20140222?format=xml&genreId=" + CategoryId + "&applicationId=" + appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("root").Elements("children").Elements("child"))
                {
                    lst.Add(new RaSubCategory()
                    {
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value),
                        CategoryName = WebUtility.HtmlEncode(element.Element("genreName").Value),
                        CategoryLevel = Convert.ToInt32(element.Element("genreLevel").Value),
                        ParentId= CategoryId
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public ProductPagger getProductsByCategory(int CategoryId, string sort= "standard", int page=1)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaItem/Search/20140222?format=xml&genreId="+CategoryId+"&page="+page+"&sort="+sort+"&applicationId="+appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.Count = Convert.ToInt32(xdoc.Element("root").Element("count").Value);
                lst.Page = Convert.ToInt32(xdoc.Element("root").Element("page").Value);
                lst.First = Convert.ToInt32(xdoc.Element("root").Element("first").Value);
                lst.Last = Convert.ToInt32(xdoc.Element("root").Element("last").Value);
                lst.Hits = Convert.ToInt32(xdoc.Element("root").Element("hits").Value);
                lst.Carrier = Convert.ToInt32(xdoc.Element("root").Element("carrier").Value);
                lst.PageCount = Convert.ToInt32(xdoc.Element("root").Element("pageCount").Value);
                lst.Items = new List<RAProduct>();
                foreach (XElement element in xdoc.Elements("root").Elements("Items").Elements("Item"))
                {
                    List<double> tagIds = new List<double>();
                    foreach (var item in element.Element("tagIds").Elements("value"))
                    {
                        tagIds.Add(Convert.ToDouble(item.Value));
                    }
                    List<SmallImageUrl> SmallImageUrls = new List<SmallImageUrl>();
                    foreach (var item in element.Element("smallImageUrls").Elements("imageUrl"))
                    {
                        SmallImageUrls.Add(new SmallImageUrl { ImageUrl=item.Value });
                    }
                    List<MediumImageUrl> MediumImageUrls = new List<MediumImageUrl>();
                    foreach (var item in element.Element("mediumImageUrls").Elements("imageUrl"))
                    {
                        MediumImageUrls.Add(new MediumImageUrl { ImageUrl = item.Value });
                    }
                    lst.Items.Add(new RAProduct()
                    {
                        TagIds = tagIds,
                        ItemName = WebUtility.HtmlEncode(element.Element("itemName").Value),
                        Catchcopy = WebUtility.HtmlEncode(element.Element("catchcopy").Value),
                        ItemCode = WebUtility.HtmlEncode(element.Element("itemCode").Value),
                        ItemPrice = Convert.ToDouble(element.Element("itemPrice").Value),
                        ItemCaption = WebUtility.HtmlEncode(element.Element("itemCaption").Value),
                        ItemUrl = element.Element("itemUrl").Value,
                        AffiliateUrl = element.Element("affiliateUrl").Value,
                        ShopAffiliateUrl = element.Element("shopAffiliateUrl").Value,
                        ImageFlag = Convert.ToDouble(element.Element("imageFlag").Value),
                        SmallImageUrls = SmallImageUrls,
                        MediumImageUrls = MediumImageUrls,
                        Availability = Convert.ToDouble(element.Element("availability").Value),
                        TaxFlag = Convert.ToDouble(element.Element("taxFlag").Value),
                        PostageFlag = Convert.ToDouble(element.Element("postageFlag").Value),
                        CreditCardFlag = Convert.ToDouble(element.Element("creditCardFlag").Value),
                        ShopOfTheYearFlag = Convert.ToDouble(element.Element("shopOfTheYearFlag").Value),
                        ShipOverseasFlag = Convert.ToDouble(element.Element("shipOverseasFlag").Value),
                        ShipOverseasArea = element.Element("shipOverseasArea").Value,
                        AsurakuFlag = Convert.ToDouble(element.Element("asurakuFlag").Value),
                        AsurakuClosingTime = element.Element("asurakuClosingTime").Value,
                        AsurakuArea = element.Element("asurakuArea").Value,
                        AffiliateRate = Convert.ToDouble(element.Element("affiliateRate").Value),
                        StartTime = element.Element("startTime").Value,
                        EndTime = element.Element("endTime").Value,
                        ReviewCount = Convert.ToDouble(element.Element("reviewCount").Value),
                        ReviewAverage = Convert.ToDouble(element.Element("reviewAverage").Value),
                        PointRate = Convert.ToDouble(element.Element("pointRate").Value),
                        PointRateStartTime = element.Element("pointRateStartTime").Value,
                        PointRateEndTime = element.Element("pointRateEndTime").Value,
                        GiftFlag = Convert.ToDouble(element.Element("giftFlag").Value),
                        ShopName = element.Element("shopName").Value,
                        ShopCode = element.Element("shopCode").Value,
                        ShopUrl = element.Element("shopUrl").Value,
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value),
                        ParentId = CategoryId
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public ProductPagger getProductsDetail(int ItemCode)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaItem/Search/20140222?format=xml&applicationId="+appId+"&itemCode="+ItemCode;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.Count = Convert.ToInt32(xdoc.Element("root").Element("count").Value);
                lst.Page = Convert.ToInt32(xdoc.Element("root").Element("page").Value);
                lst.First = Convert.ToInt32(xdoc.Element("root").Element("first").Value);
                lst.Last = Convert.ToInt32(xdoc.Element("root").Element("last").Value);
                lst.Hits = Convert.ToInt32(xdoc.Element("root").Element("hits").Value);
                lst.Carrier = Convert.ToInt32(xdoc.Element("root").Element("carrier").Value);
                lst.PageCount = Convert.ToInt32(xdoc.Element("root").Element("pageCount").Value);
                lst.Items = new List<RAProduct>();
                foreach (XElement element in xdoc.Elements("root").Elements("Items").Elements("Item"))
                {
                    List<double> tagIds = new List<double>();
                    foreach (var item in element.Element("tagIds").Elements("value"))
                    {
                        tagIds.Add(Convert.ToDouble(item.Value));
                    }
                    List<SmallImageUrl> SmallImageUrls = new List<SmallImageUrl>();
                    foreach (var item in element.Element("smallImageUrls").Elements("imageUrl"))
                    {
                        SmallImageUrls.Add(new SmallImageUrl { ImageUrl = item.Value });
                    }
                    List<MediumImageUrl> MediumImageUrls = new List<MediumImageUrl>();
                    foreach (var item in element.Element("mediumImageUrls").Elements("imageUrl"))
                    {
                        MediumImageUrls.Add(new MediumImageUrl { ImageUrl = item.Value });
                    }
                    lst.Items.Add(new RAProduct()
                    {
                        TagIds = tagIds,
                        ItemName = WebUtility.HtmlEncode(element.Element("itemName").Value),
                        Catchcopy = WebUtility.HtmlEncode(element.Element("catchcopy").Value),
                        ItemCode = WebUtility.HtmlEncode(element.Element("itemCode").Value),
                        ItemPrice = Convert.ToDouble(element.Element("itemPrice").Value),
                        ItemCaption = WebUtility.HtmlEncode(element.Element("itemCaption").Value),
                        ItemUrl = element.Element("itemUrl").Value,
                        AffiliateUrl = element.Element("affiliateUrl").Value,
                        ShopAffiliateUrl = element.Element("shopAffiliateUrl").Value,
                        ImageFlag = Convert.ToDouble(element.Element("imageFlag").Value),
                        SmallImageUrls = SmallImageUrls,
                        MediumImageUrls = MediumImageUrls,
                        Availability = Convert.ToDouble(element.Element("availability").Value),
                        TaxFlag = Convert.ToDouble(element.Element("taxFlag").Value),
                        PostageFlag = Convert.ToDouble(element.Element("postageFlag").Value),
                        CreditCardFlag = Convert.ToDouble(element.Element("creditCardFlag").Value),
                        ShopOfTheYearFlag = Convert.ToDouble(element.Element("shopOfTheYearFlag").Value),
                        ShipOverseasFlag = Convert.ToDouble(element.Element("shipOverseasFlag").Value),
                        ShipOverseasArea = element.Element("shipOverseasArea").Value,
                        AsurakuFlag = Convert.ToDouble(element.Element("asurakuFlag").Value),
                        AsurakuClosingTime = element.Element("asurakuClosingTime").Value,
                        AsurakuArea = element.Element("asurakuArea").Value,
                        AffiliateRate = Convert.ToDouble(element.Element("affiliateRate").Value),
                        StartTime = element.Element("startTime").Value,
                        EndTime = element.Element("endTime").Value,
                        ReviewCount = Convert.ToDouble(element.Element("reviewCount").Value),
                        ReviewAverage = Convert.ToDouble(element.Element("reviewAverage").Value),
                        PointRate = Convert.ToDouble(element.Element("pointRate").Value),
                        PointRateStartTime = element.Element("pointRateStartTime").Value,
                        PointRateEndTime = element.Element("pointRateEndTime").Value,
                        GiftFlag = Convert.ToDouble(element.Element("giftFlag").Value),
                        ShopName = element.Element("shopName").Value,
                        ShopCode = element.Element("shopCode").Value,
                        ShopUrl = element.Element("shopUrl").Value,
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value),
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public ProductRankPagger getProductsByRank(string param,string value)
        {
            ProductRankPagger lst = new ProductRankPagger();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaItem/Ranking/20120927?format=xml&" + param + "=" + value + "&applicationId=" + appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.Title = WebUtility.HtmlEncode(xdoc.Element("root").Element("title").Value);
                lst.LastBuildDate = WebUtility.HtmlEncode(xdoc.Element("root").Element("lastBuildDate").Value);
                lst.Items = new List<RAProductRank>();
                foreach (XElement element in xdoc.Elements("root").Elements("Items").Elements("Item"))
                {
                    List<double> tagIds = new List<double>();
                    foreach (var item in element.Element("tagIds").Elements("value"))
                    {
                        tagIds.Add(Convert.ToDouble(item.Value));
                    }
                    List<SmallImageUrl> SmallImageUrls = new List<SmallImageUrl>();
                    foreach (var item in element.Element("smallImageUrls").Elements("imageUrl"))
                    {
                        SmallImageUrls.Add(new SmallImageUrl { ImageUrl = item.Value });
                    }
                    List<MediumImageUrl> MediumImageUrls = new List<MediumImageUrl>();
                    foreach (var item in element.Element("mediumImageUrls").Elements("imageUrl"))
                    {
                        MediumImageUrls.Add(new MediumImageUrl { ImageUrl = item.Value });
                    }
                    lst.Items.Add(new RAProductRank()
                    {
                        Rank = Convert.ToDouble(element.Element("rank").Value),
                        Carrier = Convert.ToDouble(element.Element("carrier").Value),
                        ItemName = WebUtility.HtmlEncode(element.Element("itemName").Value),
                        Catchcopy = WebUtility.HtmlEncode(element.Element("catchcopy").Value),
                        ItemCode = WebUtility.HtmlEncode(element.Element("itemCode").Value),
                        ItemPrice = Convert.ToDouble(element.Element("itemPrice").Value),
                        ItemCaption = WebUtility.HtmlEncode(element.Element("itemCaption").Value),
                        ItemUrl = element.Element("itemUrl").Value,
                        AffiliateUrl = element.Element("affiliateUrl").Value,
                        ImageFlag = Convert.ToDouble(element.Element("imageFlag").Value),
                        SmallImageUrls = SmallImageUrls,
                        MediumImageUrls = MediumImageUrls,
                        Availability = Convert.ToDouble(element.Element("availability").Value),
                        TaxFlag = Convert.ToDouble(element.Element("taxFlag").Value),
                        PostageFlag = Convert.ToDouble(element.Element("postageFlag").Value),
                        CreditCardFlag = Convert.ToDouble(element.Element("creditCardFlag").Value),
                        ShopOfTheYearFlag = Convert.ToDouble(element.Element("shopOfTheYearFlag").Value),
                        ShipOverseasFlag = Convert.ToDouble(element.Element("shipOverseasFlag").Value),
                        ShipOverseasArea = element.Element("shipOverseasArea").Value,
                        AsurakuFlag = Convert.ToDouble(element.Element("asurakuFlag").Value),
                        AsurakuClosingTime = element.Element("asurakuClosingTime").Value,
                        AsurakuArea = element.Element("asurakuArea").Value,
                        StartTime = element.Element("startTime").Value,
                        EndTime = element.Element("endTime").Value,
                        ReviewCount = Convert.ToDouble(element.Element("reviewCount").Value),
                        PointRate = Convert.ToDouble(element.Element("pointRate").Value),
                        PointRateStartTime = element.Element("pointRateStartTime").Value,
                        PointRateEndTime = element.Element("pointRateEndTime").Value,
                        ShopName = element.Element("shopName").Value,
                        ShopCode = element.Element("shopCode").Value,
                        ShopUrl = element.Element("shopUrl").Value,
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value)
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public ProductPagger getProductsSearch(string key)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaItem/Search/20140222?format=xml&keyword=‎"+key+"&applicationId="+appId+ "&sort=%2BitemPrice";
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.Count = Convert.ToInt32(xdoc.Element("root").Element("count").Value);
                lst.Page = Convert.ToInt32(xdoc.Element("root").Element("page").Value);
                lst.First = Convert.ToInt32(xdoc.Element("root").Element("first").Value);
                lst.Last = Convert.ToInt32(xdoc.Element("root").Element("last").Value);
                lst.Hits = Convert.ToInt32(xdoc.Element("root").Element("hits").Value);
                lst.Carrier = Convert.ToInt32(xdoc.Element("root").Element("carrier").Value);
                lst.PageCount = Convert.ToInt32(xdoc.Element("root").Element("pageCount").Value);
                lst.Items = new List<RAProduct>();
                foreach (XElement element in xdoc.Elements("root").Elements("Items").Elements("Item"))
                {
                    List<double> tagIds = new List<double>();
                    foreach (var item in element.Element("tagIds").Elements("value"))
                    {
                        tagIds.Add(Convert.ToDouble(item.Value));
                    }
                    List<SmallImageUrl> SmallImageUrls = new List<SmallImageUrl>();
                    foreach (var item in element.Element("smallImageUrls").Elements("imageUrl"))
                    {
                        SmallImageUrls.Add(new SmallImageUrl { ImageUrl = item.Value });
                    }
                    List<MediumImageUrl> MediumImageUrls = new List<MediumImageUrl>();
                    foreach (var item in element.Element("mediumImageUrls").Elements("imageUrl"))
                    {
                        MediumImageUrls.Add(new MediumImageUrl { ImageUrl = item.Value });
                    }
                    lst.Items.Add(new RAProduct()
                    {
                        TagIds = tagIds,
                        ItemName = WebUtility.HtmlEncode(element.Element("itemName").Value),
                        Catchcopy = WebUtility.HtmlEncode(element.Element("catchcopy").Value),
                        ItemCode = WebUtility.HtmlEncode(element.Element("itemCode").Value),
                        ItemPrice = Convert.ToDouble(element.Element("itemPrice").Value),
                        ItemCaption = WebUtility.HtmlEncode(element.Element("itemCaption").Value),
                        ItemUrl = element.Element("itemUrl").Value,
                        AffiliateUrl = element.Element("affiliateUrl").Value,
                        ShopAffiliateUrl = element.Element("shopAffiliateUrl").Value,
                        ImageFlag = Convert.ToDouble(element.Element("imageFlag").Value),
                        SmallImageUrls = SmallImageUrls,
                        MediumImageUrls = MediumImageUrls,
                        Availability = Convert.ToDouble(element.Element("availability").Value),
                        TaxFlag = Convert.ToDouble(element.Element("taxFlag").Value),
                        PostageFlag = Convert.ToDouble(element.Element("postageFlag").Value),
                        CreditCardFlag = Convert.ToDouble(element.Element("creditCardFlag").Value),
                        ShopOfTheYearFlag = Convert.ToDouble(element.Element("shopOfTheYearFlag").Value),
                        ShipOverseasFlag = Convert.ToDouble(element.Element("shipOverseasFlag").Value),
                        ShipOverseasArea = element.Element("shipOverseasArea").Value,
                        AsurakuFlag = Convert.ToDouble(element.Element("asurakuFlag").Value),
                        AsurakuClosingTime = element.Element("asurakuClosingTime").Value,
                        AsurakuArea = element.Element("asurakuArea").Value,
                        AffiliateRate = Convert.ToDouble(element.Element("affiliateRate").Value),
                        StartTime = element.Element("startTime").Value,
                        EndTime = element.Element("endTime").Value,
                        ReviewCount = Convert.ToDouble(element.Element("reviewCount").Value),
                        ReviewAverage = Convert.ToDouble(element.Element("reviewAverage").Value),
                        PointRate = Convert.ToDouble(element.Element("pointRate").Value),
                        PointRateStartTime = element.Element("pointRateStartTime").Value,
                        PointRateEndTime = element.Element("pointRateEndTime").Value,
                        GiftFlag = Convert.ToDouble(element.Element("giftFlag").Value),
                        ShopName = element.Element("shopName").Value,
                        ShopCode = element.Element("shopCode").Value,
                        ShopUrl = element.Element("shopUrl").Value,
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value)
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public ProductPagger getProductsSearchWareHouse(string key)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaItem/Search/20140222?format=xml&keyword="+key+"&page=1&sort=standard&applicationId="+appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                
                lst.Items = new List<RAProduct>();
                foreach (XElement element in xdoc.Elements("root").Elements("Items").Elements("Item"))
                {
                   
                    List<SmallImageUrl> SmallImageUrls = new List<SmallImageUrl>();
                    foreach (var item in element.Element("smallImageUrls").Elements("imageUrl"))
                    {
                        SmallImageUrls.Add(new SmallImageUrl { ImageUrl = item.Value });
                    }
                    
                    lst.Items.Add(new RAProduct()
                    {
                        ItemName = WebUtility.HtmlEncode(element.Element("itemName").Value),
                        Catchcopy = WebUtility.HtmlEncode(element.Element("catchcopy").Value),
                        ItemCode = WebUtility.HtmlEncode(element.Element("itemCode").Value),
                        ItemPrice = Convert.ToDouble(element.Element("itemPrice").Value),
                        ItemCaption = WebUtility.HtmlEncode(element.Element("itemCaption").Value),
                        ItemUrl = element.Element("itemUrl").Value,
                        SmallImageUrls = SmallImageUrls,
                        CategoryId = Convert.ToInt32(element.Element("genreId").Value)
                    });
                }

            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public LoadedRatuken Search2(string search,string sortID="10")
        {
            string sortOrder = "-";
            switch (sortID)
            {
                case "00": sortID = "affiliateRate"; sortOrder = "+"; break;
                case "01": sortID = "affiliateRate"; ; break;
                case "10": sortID = "itemPrice"; sortOrder = "+"; break;
                case "11": sortID = "itemPrice"; break;
                case "20": sortID = "updateTimestamp"; sortOrder = "+"; break;
                case "21": sortID = "updateTimestamp"; break;
                case "30": sortID = "reviewCount"; sortOrder = "+"; break;
                case "31": sortID = "reviewCount"; break;
                default:
                    sortID = "updateTimestamp";
                    break;
            }
            string ApiUrl =
            string.Format("https://app.rakuten.co.jp/services/api/IchibaItem/Search/20140222?format=json" +
                            "&keyword={0}" + "&applicationId={1}" + "&page={2}" + "&sort={3}", search, appId, 1, HttpUtility.UrlEncode(sortOrder + sortID));
            string locationsResponse = Common.MakeRequest(ApiUrl);
            var result = JsonConvert.DeserializeObject<LoadedRatuken>(locationsResponse);
            
            return result;

        }
        public ProductPagger getProductsSearchNews(string key)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                LoadedRatuken result = Search2(key);
                lst.Items = new List<RAProduct>();
                lst.Items.AddRange(result.Items.Select(n=>new RAProduct() {

                    ItemName = n.Item.ItemName,
                    Catchcopy = WebUtility.HtmlEncode(n.Item.Catchcopy),
                    ItemCode = WebUtility.HtmlEncode(n.Item.ItemCode),
                    ItemPrice = Convert.ToDouble(n.Item.ItemPrice),
                    ItemCaption = WebUtility.HtmlEncode(n.Item.ItemCaption),
                    ItemUrl = n.Item.ItemUrl,
                    ImageUrl = n.Item.DisplayImage,
                    CategoryId = Convert.ToInt32(n.Item.GenreId),
                    CategoryName=n.Item.GenreName
                }));
                
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
    }
}
