using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace OhayooWeb.Models
{
    public class CateMenu
    {
        public string html { get; set; }
    }
    public class MenuPage
    {
        public List<Category> listCate { get; set; }
        public List<SubCategory> listSub { get; set; }
        public string area { get; set; }
    }
    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        //public string nameEN
        //{
        //    get
        //    {
        //        return WebUtility.HtmlDecode(TranslateUtils.Translate(name));
        //    }
        //}
        public string url { get; set; }
    }
    public class SubCategory
    {
        public int id { get; set; }
        public int CateId { get; set; }
        public string name { get; set; }
        //public string nameEN
        //{
        //    get
        //    {
        //        return WebUtility.HtmlDecode(TranslateUtils.Translate(name));
        //    }
        //}
        public String url { get; set; }
    }
}