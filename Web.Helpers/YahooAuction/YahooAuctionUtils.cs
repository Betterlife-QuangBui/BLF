using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Web.Helpers.library;
using Web.Helpers.YahooAuction.Models;

namespace Web.Helpers.YahooAuction
{
    public class YahooAuctionUtils
    {
        string appId = "dj0zaiZpPUdDY3ZwYTh6dkVKMSZzPWNvbnN1bWVyc2VjcmV0Jng9MTQ-";
        public List<YACategory> getCategories()
        {
            List<YACategory> lst = new List<YACategory>();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/categoryTree?APPID=" + appId + "&category=0";
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("ChildCategory"))
                {
                    lst.Add(new YACategory()
                    {
                        CategoryId = element.Element("CategoryId").Value,
                        CategoryName = WebUtility.HtmlEncode(element.Element("CategoryName").Value),
                        CategoryIdPath = element.Element("CategoryIdPath").Value,
                        CategoryPath = WebUtility.HtmlEncode(element.Element("CategoryPath").Value),
                        Depth = Convert.ToInt32(element.Element("Depth").Value),
                        IsAdult = Convert.ToBoolean(element.Element("IsAdult").Value),
                        IsLeaf = Convert.ToBoolean(element.Element("IsLeaf").Value),
                        IsLeafToLink = Convert.ToBoolean(element.Element("IsLeafToLink").Value),
                        IsLink = Convert.ToBoolean(element.Element("IsLink").Value),
                        NumOfAuctions = Convert.ToInt32(element.Element("NumOfAuctions").Value),
                        Order = Convert.ToInt32(element.Element("Order").Value),
                        ParentCategoryId = Convert.ToInt32(element.Element("ParentCategoryId").Value),
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public List<YASubCategory> getSubsCategory(int CategoryId)
        {
            List<YASubCategory> lst = new List<YASubCategory>();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/categoryTree?APPID=" + appId + "&category=" + CategoryId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("ChildCategory"))
                {
                    lst.Add(new YASubCategory()
                    {
                        CategoryId = element.Element("CategoryId").Value,
                        CategoryName = WebUtility.HtmlEncode(element.Element("CategoryName").Value),
                        CategoryIdPath = element.Element("CategoryIdPath").Value,
                        CategoryPath = WebUtility.HtmlEncode(element.Element("CategoryPath").Value),
                        Depth = Convert.ToInt32(element.Element("Depth").Value),
                        IsAdult = Convert.ToBoolean(element.Element("IsAdult").Value),
                        IsLeaf = Convert.ToBoolean(element.Element("IsLeaf").Value),
                        IsLeafToLink = Convert.ToBoolean(element.Element("IsLeafToLink").Value),
                        IsLink = Convert.ToBoolean(element.Element("IsLink").Value),
                        NumOfAuctions = Convert.ToInt32(element.Element("NumOfAuctions").Value),
                        Order = Convert.ToInt32(element.Element("Order").Value),
                        ParentCategoryId = Convert.ToInt32(element.Element("ParentCategoryId").Value),
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public YASubCategory getCategoryDetail(int CategoryId)
        {
            YASubCategory lst = new YASubCategory();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/categoryTree?APPID=" + appId + "&category=" + CategoryId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst = new YASubCategory()
                {
                    CategoryId = xdoc.Element("ResultSet").Element("Result").Element("CategoryId").Value,
                    CategoryName = WebUtility.HtmlEncode(xdoc.Element("ResultSet").Element("Result").Element("CategoryName").Value),
                    CategoryIdPath = xdoc.Element("ResultSet").Element("Result").Element("CategoryIdPath").Value,
                    CategoryPath = WebUtility.HtmlEncode(xdoc.Element("ResultSet").Element("Result").Element("CategoryPath").Value),
                    Depth = Convert.ToInt32(xdoc.Element("ResultSet").Element("Result").Element("Depth").Value),
                    IsAdult = Convert.ToBoolean(xdoc.Element("ResultSet").Element("Result").Element("IsAdult").Value),
                    IsLeaf = Convert.ToBoolean(xdoc.Element("ResultSet").Element("Result").Element("IsLeaf").Value),
                    IsLeafToLink = Convert.ToBoolean(xdoc.Element("ResultSet").Element("Result").Element("IsLeafToLink").Value),
                    IsLink = Convert.ToBoolean(xdoc.Element("ResultSet").Element("Result").Element("IsLink").Value),
                    //NumOfAuctions = Convert.ToInt32(xdoc.Element("ResultSet").Element("Result").Element("NumOfAuctions").Value),
                    Order = Convert.ToInt32(xdoc.Element("ResultSet").Element("Result").Element("Order").Value),
                    ParentCategoryId = Convert.ToInt32(xdoc.Element("ResultSet").Element("Result").Element("ParentCategoryId").Value),
                };
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public ProductPagger getProductsByCategory(int CategoryId, int sort, int page)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/categoryLeaf?appid=" + appId + "&category=" + CategoryId + "&page=" + page + "&sort=" + OhayooLib.Sorter(sort);
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.CategoryPath = WebUtility.HtmlEncode(xdoc.Element("ResultSet").Element("Result").Element("CategoryPath").Value);
                lst.CategoryIdPath = WebUtility.HtmlEncode(xdoc.Element("ResultSet").Element("Result").Element("CategoryIdPath").Value);
                lst.firstResultPosition = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("firstResultPosition").Value);
                lst.totalResultsReturned = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("totalResultsReturned").Value);
                lst.totalResultsAvailable = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("totalResultsAvailable").Value);
                lst.Items = new List<YAProduct>();
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("Item"))
                {
                    lst.Items.Add(new YAProduct()
                    {
                        AuctionID = element.Element("AuctionID").Value,
                        Title = WebUtility.HtmlEncode(element.Element("Title").Value),
                        ItemUrl = element.Element("ItemUrl").Value,
                        AuctionItemUrl = WebUtility.HtmlEncode(element.Element("AuctionItemUrl").Value),
                        Image = element.Element("Image").Value,
                        OriginalImageNum = Convert.ToInt32(element.Element("OriginalImageNum").Value),
                        CurrentPrice = Convert.ToDouble(element.Element("CurrentPrice").Value),
                        Bids = Convert.ToInt32(element.Element("Bids").Value),
                        EndTime = Convert.ToDateTime(element.Element("EndTime").Value),
                        BidOrBuy = Convert.ToDouble(element.Element("BidOrBuy") == null ? "0.0" : element.Element("BidOrBuy").Value),
                        IsReserved = Convert.ToBoolean(element.Element("IsReserved").Value),
                        IsAdult = Convert.ToBoolean(element.Element("IsAdult").Value),
                        Seller = new Seller() { Id = element.Element("Seller").Element("Id").Value, ItemListUrl = element.Element("Seller").Element("ItemListUrl").Value, RatingUrl = element.Element("Seller").Element("RatingUrl").Value },
                        Option = new Option()
                        {
                            //BuynowIcon = element.Element("Option").Element("BuynowIcon")==null?"": element.Element("Option").Element("BuynowIcon").Value,
                            //EasyPaymentIcon = element.Element("Option").Element("EasyPaymentIcon").Value,
                            // FeaturedIcon = element.Element("Option").Element("FeaturedIcon").Value,
                            IsBackGroundColor = Convert.ToBoolean(element.Element("Option").Element("IsBackGroundColor").Value),
                            IsBold = Convert.ToBoolean(element.Element("Option").Element("IsBold").Value),
                            IsCharity = Convert.ToBoolean(element.Element("Option").Element("IsCharity").Value),
                            IsOffer = Convert.ToBoolean(element.Element("Option").Element("IsOffer").Value)

                        }
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public YAProductDetail getProductDetail(string auctionID)
        {
            YAProductDetail lst = new YAProductDetail();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/auctionItem?appid=" + appId + "&auctionID=" + auctionID;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result"))
                {
                    HighestBidders HighestBidders = new HighestBidders();
                    HighestBidders.IsMore = Convert.ToBoolean(element.Element("HighestBidders").Element("IsMore").Value);
                    HighestBidders.totalHighestBidders = Convert.ToInt32(element.Element("HighestBidders").Attribute("totalHighestBidders").Value);
                    HighestBidders.lstBidder = new List<Bidder>();
                    foreach (XElement HighestBidder in element.Element("HighestBidders").Elements("Bidder"))
                    {
                        
                        HighestBidders.lstBidder.Add(new Bidder()
                        {
                            Id = HighestBidder.Element("Id").Value,
                            Rating = new Rating()
                            {
                                Point = Convert.ToInt32(HighestBidder.Element("Rating").Element("Point").Value),
                                IsSuspended = Convert.ToBoolean(HighestBidder.Element("Rating").Element("IsSuspended").Value),
                                IsDeleted = Convert.ToBoolean(HighestBidder.Element("Rating").Element("IsDeleted").Value),
                            }
                        });
                    }

                    lst = new YAProductDetail()
                    {
                        AuctionID = element.Element("AuctionID").Value,
                        CategoryID = element.Element("CategoryID").Value,
                        CategoryFarm = element.Element("CategoryFarm").Value,
                        CategoryIdPath = element.Element("CategoryFarm").Value,
                        CategoryPath = WebUtility.HtmlEncode(element.Element("CategoryPath").Value),
                        Title = WebUtility.HtmlEncode(element.Element("Title").Value),
                        Seller = new SellerDetail()
                        {
                            Id = element.Element("Seller").Element("Id").Value,
                            ItemListURL = element.Element("Seller").Element("ItemListURL").Value,
                            RatingURL = element.Element("Seller").Element("RatingURL").Value,
                            Rating = new Rating()
                            {
                                IsDeleted = Convert.ToBoolean(element.Element("Seller").Element("Rating").Element("IsDeleted").Value),
                                IsSuspended = Convert.ToBoolean(element.Element("Seller").Element("Rating").Element("IsSuspended").Value),
                                Point = Convert.ToInt32(element.Element("Seller").Element("Rating").Element("Point").Value),
                                TotalBadRating = Convert.ToInt32(element.Element("Seller").Element("Rating").Element("TotalBadRating").Value),
                                TotalGoodRating = Convert.ToInt32(element.Element("Seller").Element("Rating").Element("TotalGoodRating").Value),
                                TotalNormalRating = Convert.ToInt32(element.Element("Seller").Element("Rating").Element("TotalNormalRating").Value)
                            }
                        }
                    ,
                        AuctionItemUrl = element.Element("AuctionItemUrl").Value,
                        Img = new List<string>() {
                            element.Element("Img").Element("Image1").Value,
                            element.Element("Img").Element("Image2").Value,
                            element.Element("Img").Element("Image3").Value
                        },
                        Initprice = Convert.ToDouble(element.Element("Initprice").Value),
                        Price = Convert.ToDouble(element.Element("Price").Value),
                        Quantity = Convert.ToInt32(element.Element("Quantity").Value),
                        Bids = Convert.ToDouble(element.Element("Bids").Value),
                        HighestBidders = HighestBidders,
                        ItemStatus = new ItemStatus() { Condition = element.Element("ItemStatus").Element("Condition").Value },
                        ItemReturnable = new ItemReturnable() { Allowed = Convert.ToBoolean(element.Element("ItemReturnable").Element("Allowed").Value) },
                        AvailableQuantity = Convert.ToInt32(element.Element("AvailableQuantity").Value),
                        StartTime = Convert.ToDateTime(element.Element("StartTime").Value),
                        EndTime = Convert.ToDateTime(element.Element("EndTime").Value),
                        TaxRate = Convert.ToInt32(element.Element("TaxRate").Value),
                        IsBidCreditRestrictions = Convert.ToBoolean(element.Element("IsBidCreditRestrictions").Value),
                        IsBidderRestrictions = Convert.ToBoolean(element.Element("IsBidderRestrictions").Value),
                        IsBidderRatioRestrictions = Convert.ToBoolean(element.Element("IsBidderRatioRestrictions").Value),
                        IsEarlyClosing = Convert.ToBoolean(element.Element("IsEarlyClosing").Value),
                        IsAutomaticExtension = Convert.ToBoolean(element.Element("IsAutomaticExtension").Value),
                        IsOffer = Convert.ToBoolean(element.Element("IsOffer").Value),
                        HasOfferAccept = Convert.ToBoolean(element.Element("HasOfferAccept").Value),
                        IsCharity = Convert.ToBoolean(element.Element("IsCharity").Value),
                        //SalesContract = Convert.ToBoolean(element.Element("SalesContract").Value),
                        IsFleaMarket = Convert.ToBoolean(element.Element("IsFleaMarket").Value),
                        Description = WebUtility.HtmlEncode(element.Element("Description").Value.RemoveCData()),
                        SeoKeywords = WebUtility.HtmlEncode(element.Element("SeoKeywords").Value.RemoveCData()),
                        BlindBusiness = element.Element("BlindBusiness").Value,
                        SevenElevenReceive = element.Element("SevenElevenReceive").Value,
                        ChargeForShipping = element.Element("ChargeForShipping").Value,
                        Location = WebUtility.HtmlEncode(element.Element("Location").Value),
                        IsWorldwide = Convert.ToBoolean(element.Element("IsWorldwide").Value),
                        ShipTime = element.Element("ShipTime").Value,
                        //ShippingInput = element.Element("ShippingInput").Value,
                        BaggageInfo = new BaggageInfo()
                        {
                            Size = WebUtility.HtmlEncode(element.Element("BaggageInfo").Element("Size").Value),
                            SizeIndex = WebUtility.HtmlEncode(element.Element("BaggageInfo").Element("SizeIndex").Value),
                            Weight = WebUtility.HtmlEncode(element.Element("BaggageInfo").Element("Weight").Value),
                            WeightIndex = WebUtility.HtmlEncode(element.Element("BaggageInfo").Element("WeightIndex").Value),
                        },
                        IsAdult = Convert.ToBoolean(element.Element("IsAdult").Value),
                        IsCreature = Convert.ToBoolean(element.Element("IsCreature").Value),
                        IsSpecificCategory = Convert.ToBoolean(element.Element("IsSpecificCategory").Value),
                        IsCharityCategory = Convert.ToBoolean(element.Element("IsCharityCategory").Value),
                        CharityOption = new CharityOption() {
                                Proportion= Convert.ToInt32(element.Element("CharityOption").Element("Proportion").Value)
                        },
                        AnsweredQAndANum = Convert.ToInt32(element.Element("AnsweredQAndANum").Value),
                        Status = element.Element("Status").Value,
                    };
                }
            }
            catch (Exception ex) { }
            return lst;
        }

        public YAProductDetail getProductDetailVersionBasic(string auctionID)
        {
            YAProductDetail lst = new YAProductDetail();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/auctionItem?appid=" + appId + "&auctionID=" + auctionID;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result"))
                {
                    

                    lst = new YAProductDetail()
                    {
                        AuctionID = element.Element("AuctionID").Value,
                        CategoryID = element.Element("CategoryID").Value,
                        
                        Title = WebUtility.HtmlEncode(element.Element("Title").Value),
                        
                        AuctionItemUrl = element.Element("AuctionItemUrl").Value,
                        Img = new List<string>() {
                            element.Element("Img").Element("Image1").Value
                        },
                        Price = Convert.ToDouble(element.Element("Price").Value),
                        Location = WebUtility.HtmlEncode(element.Element("Location").Value),
                    };
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public ProductPagger getProductsBySarch(string key)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "http://auctions.yahooapis.jp/AuctionWebService/V2/search?appid="+appId+ "&page=1&sort=popular&type=any&output=xml&store=0&query=" + key;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.Items = new List<YAProduct>();
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("Item"))
                {
                    lst.Items.Add(new YAProduct()
                    {
                        AuctionID = element.Element("AuctionID").Value,
                        Title = WebUtility.HtmlEncode(element.Element("Title").Value),
                        ItemUrl = element.Element("ItemUrl").Value,
                        AuctionItemUrl = WebUtility.HtmlEncode(element.Element("AuctionItemUrl").Value),
                        Image = element.Element("Image").Value,
                        CurrentPrice = Convert.ToDouble(element.Element("CurrentPrice").Value),
                        CategoryName= "",//getCategoryDetail(Convert.ToInt32(element.Element("CategoryId").Value)).CategoryName,
                        Material= ""//getProductDetail(element.Element("AuctionID").Value)==null?"": getProductDetail(element.Element("AuctionID").Value).Description

                    });
                }
            }
            catch (Exception ex) {  }
            return lst;
        }


    }
}
