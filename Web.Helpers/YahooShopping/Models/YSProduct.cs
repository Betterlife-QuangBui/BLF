using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.YahooShopping.Models
{
    public class ItemResult
    {
        public double totalResultsAvailable { get; set; }
        public double totalResultsReturned { get; set; }
        public double firstResultPosition { get; set; }

    }
    public class Seller
    {
        public string Id { get; set; }
        public string ItemListUrl { get; set; }
        public string RatingUrl { get; set; }
    }
    public class Option
    {
        public string FeaturedIcon { get; set; }
        public string BuynowIcon { get; set; }
        public string EasyPaymentIcon { get; set; }
        public bool IsBold { get; set; }
        public bool IsBackGroundColor { get; set; }
        public bool IsOffer { get; set; }
        public bool IsCharity { get; set; }
    }
    public class ProductPagger : ItemResult
    {
        public List<YSProduct> Items { get; set; }
    }
    public class YSProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Headline { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public Double Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }
        public string ProductId { get; set; }
    }

    public class Rating
    {
        public int Point { get; set; }
        public int TotalGoodRating { get; set; }
        public int TotalNormalRating { get; set; }
        public int TotalBadRating { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class SellerDetail
    {
        public string Id { get; set; }
        public Rating Rating { get; set; }
        public string ItemListURL { get; set; }
        public string RatingURL { get; set; }
    }
    public class Bidder
    {
        public string Id { get; set; }
        public Rating Rating { get; set; }
    }
    public class HighestBidders
    {
        public int totalHighestBidders { get; set; }
        public List<Bidder> lstBidder { get; set; }
        public bool IsMore { get; set; }
    }
    public class ItemStatus
    {
        public string Condition { get; set; }
    }
    public class ItemReturnable
    {
        public bool Allowed { get; set; }
    }
    public class OptionDetail
    {
        public bool IsTradingNaviAuction { get; set; }
    }
    public class BaggageInfo
    {
        public string Size { get; set; }
        public string SizeIndex { get; set; }
        public string Weight { get; set; }
        public string WeightIndex { get; set; }
    }
    public class CharityOption
    {
        public int Proportion { get; set; }
    }
    public class YAProductDetail
    {
        public string AuctionID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryFarm { get; set; }
        public string CategoryIdPath { get; set; }
        public string CategoryPath { get; set; }
        public string Title { get; set; }
        public SellerDetail Seller { get; set; }
        public string AuctionItemUrl { get; set; }
        public List<String> Img { get; set; }
        public double Initprice { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double AvailableQuantity { get; set; }
        public double Bids { get; set; }
        public HighestBidders HighestBidders { get; set; }
        public ItemStatus ItemStatus { get; set; }
        public ItemReturnable ItemReturnable { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TaxRate { get; set; }
        public bool IsBidCreditRestrictions { get; set; }
        public bool IsBidderRestrictions { get; set; }
        public bool IsBidderRatioRestrictions { get; set; }
        public bool IsEarlyClosing { get; set; }
        public bool IsAutomaticExtension { get; set; }
        public bool IsOffer { get; set; }
        public bool HasOfferAccept { get; set; }
        public bool IsCharity { get; set; }
        public bool SalesContract { get; set; }
        public bool IsFleaMarket { get; set; }
        public OptionDetail Option { get; set; }
        public string Description { get; set; }
        public string SeoKeywords { get; set; }
        public string BlindBusiness { get; set; }
        public string SevenElevenReceive { get; set; }
        public string ChargeForShipping { get; set; }
        public string Location { get; set; }
        public bool IsWorldwide { get; set; }
        public string ShipTime { get; set; }
        public string ShippingInput { get; set; }
        public BaggageInfo BaggageInfo { get; set; }
        public bool IsAdult { get; set; }
        public bool IsCreature { get; set; }
        public bool IsSpecificCategory { get; set; }
        public bool IsCharityCategory { get; set; }
        public int AnsweredQAndANum { get; set; }
        public CharityOption CharityOption { get; set; }
        public string Status { get; set; }
    }
}
