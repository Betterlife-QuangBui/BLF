using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.YahooShopping.Models
{
    public class YSSubCategory
    {
        public int CategoryId { get; set; }
        public string CategoryNameShort { get; set; }
        public string CategoryNameMedium { get; set; }
        public string CategoryNameLong { get; set; }
        public string CategoryUrl { get; set; }
        public int ParentCategoryId { get; set; }
    }
}
