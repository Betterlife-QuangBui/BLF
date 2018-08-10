using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.Rakuten.Models
{

    public class RAProductRank
    {
        public double Rank { get; set; }
        public double Carrier { get; set; }
        public string ItemName { get; set; }
        public string Catchcopy { get; set; }
        public string ItemCode { get; set; }
        public double ItemPrice { get; set; }
        public string ItemCaption { get; set; }
        public string ItemUrl { get; set; }
        public string AffiliateUrl { get; set; }
        public double ImageFlag { get; set; }
        public List<SmallImageUrl> SmallImageUrls { get; set; }
        public List<MediumImageUrl> MediumImageUrls { get; set; }
        public double Availability { get; set; }
        public double TaxFlag { get; set; }
        public double PostageFlag { get; set; }
        public double CreditCardFlag { get; set; }
        public double ShopOfTheYearFlag { get; set; }
        public double ShipOverseasFlag { get; set; }
        public string ShipOverseasArea { get; set; }
        public double AsurakuFlag { get; set; }
        public string AsurakuClosingTime { get; set; }
        public string AsurakuArea { get; set; }
        public string AffiliateRate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double ReviewCount { get; set; }
        public string ReviewAverage { get; set; }
        public double PointRate { get; set; }
        public string PointRateStartTime { get; set; }
        public string PointRateEndTime { get; set; }
        public string ShopName { get; set; }
        public string ShopCode { get; set; }
        public string ShopUrl { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductRankPagger
    {
        public string Title { get; set; }
        public string LastBuildDate { get; set; }
        public List<RAProductRank> Items { get; set; }
    }
}
