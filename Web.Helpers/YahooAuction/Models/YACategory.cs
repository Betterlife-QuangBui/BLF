using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.YahooAuction.Models
{
    public class YACategory
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryPath { get; set; }
        public string CategoryIdPath { get; set; }
        public int NumOfAuctions { get; set; }
        public int ParentCategoryId { get; set; }
        public bool IsLeaf { get; set; }
        public int Depth { get; set; }
        public int Order { get; set; }
        public bool IsLink { get; set; }
        public bool IsAdult { get; set; }
        public bool IsLeafToLink { get; set; }
    }
}
