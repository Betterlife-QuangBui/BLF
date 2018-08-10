using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Models
{
    public class ProductItem
    {
        public string Id { get; set; }
        public string NameJP { get; set; }
        public string NameEN { get; set; }
        public string ImageUrl { get; set; }
        public string ImageBase64 { get; set; }
        public string CategoryName { get; set; }
        public string Link { get; set; }
        public double Price { get; set; }
        public string ShippingMark { get; set; }
        public string JanCode { get; set; }
        public int Quantity { get; set; }
        public string MadeIn { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public double Amount { get; set; }
        public int SumQuantity { get; set; }
        public string SlipNumber { get; set; }
        public double Weigh { get; set; }
    }
}