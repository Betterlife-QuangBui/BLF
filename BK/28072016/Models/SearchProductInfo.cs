using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Models
{
    public class SearchProductInfo
    {
        public System.Guid Id { get; set; }
        public string NameEN { get; set; }
        public string NameJP { get; set; }
        public string ProductCode { get; set; }
        public string JanCode { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string LinkWeb { get; set; }
        public string Image { get; set; }
        public string Material { get; set; }
        public Nullable<double> Quantity { get; set; }
        public Nullable<double> PriceTax { get; set; }
        public Nullable<double> Amount { get; set; }
        public string MadeIn { get; set; }
        public string Notes { get; set; }
    }
}