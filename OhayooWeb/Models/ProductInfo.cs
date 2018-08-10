using OhayooWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhayooWeb.Models
{
    public class PageInfo
    {
        public int page { get; set; }
        public string sort { get; set; }
        public string pageNav { get; set; }
    }
    public class ProductInfo
    {
        public string itemCode { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public double price { get; set; }
        public int CateId { get; set; }
        public string cateName { get; set; }
        public double priceVN { get {
                return ProductRakutenUtils.ExchangeRate()* price;
        } }
        public PageInfo pageInfo { get; set; }
        public string attribute { get; set; }
        public string description { get; set; }
        public List<String> images { get; set; }
        public string checkAvailable { get; set; }
        public string auction { get; set; }
        public string summary { get; set; }
        public string auctionInfo { get; set; }
        public string urlPageMain { get; set; }
    }
    public class ProductPagger
    {
        public List<ProductInfo> lstPros { get; set; }
        public string nav { get; set; }
    }
}