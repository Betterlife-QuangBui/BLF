using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Web.Helpers.library;
using Web.Helpers.YahooShopping.Models;

namespace Web.Helpers.YahooShopping
{
    public class YahooShoppingUtils
    {
        string appId = "dj0zaiZpPXR0U1FYNHBBdnc4YyZzPWNvbnN1bWVyc2VjcmV0Jng9ZWE-";
        public List<YSCategory> getCategories()
        {
            List<YSCategory> lst = new List<YSCategory>();
            try
            {
                string url = "http://shopping.yahooapis.jp/ShoppingWebService/V1/categorySearch?appid="+appId+"&category_id=1";
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("Categories").Elements("Children").Elements("Child"))
                {
                    lst.Add(new YSCategory()
                    {
                        CategoryId = Convert.ToInt32(element.Element("Id").Value),
                        CategoryNameLong = WebUtility.HtmlEncode(element.Element("Title").Element("Long").Value),
                        CategoryNameShort = WebUtility.HtmlEncode(element.Element("Title").Element("Short").Value),
                        CategoryNameMedium = WebUtility.HtmlEncode(element.Element("Title").Element("Medium").Value),
                        CategoryUrl = element.Element("Url").Value,
                        
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        public List<YSSubCategory> getSubsCategory(int CategoryId)
        {
            List<YSSubCategory> lst = new List<YSSubCategory>();
            try
            {
                string url = "http://shopping.yahooapis.jp/ShoppingWebService/V1/categorySearch?appid="+appId+"&category_id="+CategoryId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("Categories").Elements("Children").Elements("Child"))
                {
                    lst.Add(new YSSubCategory()
                    {
                        CategoryId = Convert.ToInt32(element.Element("Id").Value),
                        CategoryNameLong = WebUtility.HtmlEncode(element.Element("Title").Element("Long").Value),
                        CategoryNameShort = WebUtility.HtmlEncode(element.Element("Title").Element("Short").Value),
                        CategoryNameMedium = WebUtility.HtmlEncode(element.Element("Title").Element("Medium").Value),
                        CategoryUrl = element.Element("Url").Value,
                        ParentCategoryId=CategoryId
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
        

        public ProductPagger getProductsByCategory(int CategoryId)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch?appid="+appId+"&category_id="+CategoryId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.firstResultPosition = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("firstResultPosition").Value);
                lst.totalResultsReturned = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("totalResultsReturned").Value);
                lst.totalResultsAvailable = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("totalResultsAvailable").Value);
                lst.Items = new List<YSProduct>();
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("Hit"))
                {
                    lst.Items.Add(new YSProduct()
                    {
                        Code = element.Element("Code").Value,
                        Name = WebUtility.HtmlEncode(element.Element("Name").Value),
                        Url = element.Element("Url").Value,
                        Image = element.Element("Image").Element("Small").Value,
                        CategoryId=Convert.ToInt32(element.Element("Category").Element("Current").Element("Id").Value),
                        CategoryName = WebUtility.HtmlEncode(element.Element("Category").Element("Current").Element("Name").Value),
                        Description= element.Element("Description").Value,
                        Headline= element.Element("Headline").Value,
                        Price=Convert.ToDouble(element.Element("Price").Value)
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }


        public ProductPagger getProductsBySearch(string key)
        {
            ProductPagger lst = new ProductPagger();
            try
            {
                string url = "http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch?appid=" + appId + "&query=" + key;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst.firstResultPosition = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("firstResultPosition").Value);
                lst.totalResultsReturned = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("totalResultsReturned").Value);
                lst.totalResultsAvailable = Convert.ToDouble(xdoc.Element("ResultSet").Attribute("totalResultsAvailable").Value);
                lst.Items = new List<YSProduct>();
                foreach (XElement element in xdoc.Elements("ResultSet").Elements("Result").Elements("Hit"))
                {
                    lst.Items.Add(new YSProduct()
                    {
                        Code = element.Element("JanCode").Value,
                        Name = WebUtility.HtmlEncode(element.Element("Name").Value),
                        Url = element.Element("Url").Value,
                        Image = element.Element("Image").Element("Small").Value,
                        CategoryId = element.Element("Category").Element("Current").Element("Id").Value!=""?Convert.ToInt32(element.Element("Category").Element("Current").Element("Id").Value):0,
                        CategoryName = WebUtility.HtmlEncode(element.Element("Category").Element("Current").Element("Name").Value),
                        Description = element.Element("Description").Value,
                        Headline = element.Element("Headline").Value,
                        Price = Convert.ToDouble(element.Element("Price").Value),
                        ProductId = element.Element("Code").Value
                    });
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }

        public YSProduct getProductDetailVersionBasic(string code)
        {
            YSProduct lst = new YSProduct();
            try
            {
                string url = "http://shopping.yahooapis.jp/ShoppingWebService/V1/itemLookup?appid="+appId+"&itemcode="+code;
                XDocument xdoc = OhayooLib.getXDocument(url);
                lst = new YSProduct()
                {
                    Code = xdoc.Element("ResultSet").Element("Result").Element("Hit").Element("Image").Element("Id").Value,
                    Name = WebUtility.HtmlEncode(xdoc.Element("ResultSet").Element("Result").Element("Hit").Element("Name").Value),
                    Url = xdoc.Element("ResultSet").Element("Result").Element("Hit").Element("Url").Value,
                    Image = xdoc.Element("ResultSet").Element("Result").Element("Hit").Element("Image").Element("Small").Value,
                    Headline = xdoc.Element("ResultSet").Element("Result").Element("Hit").Element("Headline").Value,
                    Price = Convert.ToDouble(xdoc.Element("ResultSet").Element("Result").Element("Hit").Element("Price").Value)
                };
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return lst;
        }
    }
}
