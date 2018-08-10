using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Message.Response.Category
{
    public class CategoryResponse
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int level { get; set; }
        public int parent_id { get; set; }
        public bool status { get; set; }
        public bool is_leaf { get; set; }
    }
}