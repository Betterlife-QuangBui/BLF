using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.Rakuten.Models
{
    public class RaSubCategory
    {
        public int ParentId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryLevel { get; set; }
    }
}
