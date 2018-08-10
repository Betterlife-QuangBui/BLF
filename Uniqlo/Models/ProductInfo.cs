using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uniqlo.Models
{
    public class ProductInfo
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string JanCode { get; set; }
        public string Material { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
    }
}