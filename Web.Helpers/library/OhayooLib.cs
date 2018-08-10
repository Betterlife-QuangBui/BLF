using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Web.Helpers.library
{
    public enum Sort
    {
        /// <summary>
        /// end ：終了時間
        /// </summary>
        終了時間 = 1,//"end",
        /// <summary>
        /// img ：画像の有無
        /// </summary>
        画像の有無 = 2,//"img",
        /// <summary>
        /// bids ：入札数
        /// </summary>
        入札数 = 3,
        /// <summary>
        /// cbids ：現在価格
        /// </summary>
        現在価格 = 4,
        /// <summary>
        /// bidorbuy ：即決価格
        /// </summary>
        即決価格 = 5,
        /// <summary>
        /// affiliate ：アフィリエイト 【NEW】
        /// </summary>
        アフィリエイト = 6
    }
    public static class OhayooLib
    {

        public static String RemoveCData(this string s)
        {
            s = Regex.Replace(s, @"(&lt;\w+&gt;)", "");
            s = Regex.Replace(s, @"(&lt;/\w+&gt;)", "");
            return s;
        }
        public static string Sorter(int sort)
        {
            string result = "end";
            switch (sort)
            {
                case 1: result = "end"; break;
                case 2: result = "img"; break;
                case 3: result = "bids"; break;
                case 4: result = "cbids"; break;
                case 5: result = "bidorbuy"; break;
                case 6: result = "affiliate"; break;
                default:
                    result = "end";
                    break;
            }
            return result;
        }
        public static object ConvertXmlToJsonObject(String xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string json = JsonConvert.SerializeXmlNode(doc);
            return ConvertJsonStringToJsonObject(json);
        }
        public static XmlDocument ConvertJsontoXMLObject(String xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            // To convert JSON text contained in string json into an XML node
            XmlDocument docXML = JsonConvert.DeserializeXmlNode(jsonText);
            return docXML;
        }
        public static XmlDocument JsonToXML(string json)
        {
            XmlDocument doc = new XmlDocument();

            using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
            {
                XElement xml = XElement.Load(reader);
                doc.LoadXml(xml.ToString());
            }
            return doc;
        }
        public static object ConvertJsonStringToJsonObject(String dataJson)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<object>(dataJson);
        }
        public static XmlDocument ConvertStringToXml(String data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return doc;
        }
        public static XDocument getXDocument(string url, string method = "GET")
        {
            String xml = GetDataFromUrl(url, method, null);
            XDocument xdoc = OhayooLib.ConvertStringToXml(xml).ToXDocument();
            return xdoc;
        }
        public static String GetDataFromUrl(String url, String method = "GET", Dictionary<String, Object> param = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            if (param != null)
            {
                String postData = "";
                foreach (var item in param)
                {
                    if (postData == "") { postData = item.Key + "=" + item.Value; }
                    else { postData += "&" + item.Key + "=" + item.Value; }
                }
                var data = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            var response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string content = reader.ReadToEnd();
            reader.Close();
            response.Close();
            content = content.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:yahoo:jp:auc:categoryTree\" xsi:schemaLocation=\"urn:yahoo:jp:auc:categoryTree http://auctions.yahooapis.jp/AuctionWebService/V2/categoryTree.xsd\"", "");
            content = content.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:yahoo:jp:auc:categoryLeaf\" xsi:schemaLocation=\"urn:yahoo:jp:auc:categoryLeaf http://auctions.yahooapis.jp/AuctionWebService/V2/categoryLeaf.xsd\"", "");
            content = content.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:yahoo:jp:auc:auctionItem\" xsi:schemaLocation=\"urn:yahoo:jp:auc:auctionItem http://auctions.yahooapis.jp/AuctionWebService/V2/auctionItem.xsd\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn: yahoo:jp: categorySearch http://shopping.yahooapis.jp/ShoppingWebService/V1/categorySearch.xsd\" xmlns=\"urn:yahoo:jp:categorySearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"","");
            content = content.Replace("xsi:schemaLocation=\"urn:yahoo:jp:categorySearch http://shopping.yahooapis.jp/ShoppingWebService/V1/categorySearch.xsd\" xmlns=\"urn:yahoo:jp:categorySearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn: yahoo:jp: itemSearch http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch.xsd\" xmlns=\"urn:yahoo:jp:itemSearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn: yahoo:jp: itemLookup http://shopping.yahooapis.jp/ShoppingWebService/V1/itemLookup.xsd\" xmlns=\"urn:yahoo:jp:itemLookup\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn:yahoo:jp:itemLookup http://shopping.yahooapis.jp/ShoppingWebService/V1/itemLookup.xsd\" xmlns=\"urn:yahoo:jp:itemLookup\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn: yahoo:jp:itemSearch http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch.xsd\" xmlns=\"urn:yahoo:jp:itemSearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn: yahoo:jp: itemSearch http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch.xsd\" xmlns=\"urn:yahoo:jp:itemSearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace(" xsi:schemaLocation=\"urn: yahoo:jp: itemSearch http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch.xsd\" xmlns=\"urn:yahoo:jp:itemSearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            content = content.Replace("xsi:schemaLocation=\"urn:yahoo:jp:itemSearch http://shopping.yahooapis.jp/ShoppingWebService/V1/itemSearch.xsd\" xmlns=\"urn:yahoo:jp:itemSearch\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");

            content = content.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:yahoo:jp:auc:search\" xsi:schemaLocation=\"urn:yahoo:jp:auc:search http://auctions.yahooapis.jp/AuctionWebService/V2/search.xsd\"", "");
            return Regex.Replace(content, "xmlns:xsi=\"http://w+\"", "");
        }
    }
}
