using CsQuery;
using Ohayoo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ohayoo.Lib
{
    public class ShippingJapanUtils
    {
        public static double ShippingPrice(string shopCode = "culture")
        {

            double result = 0;
            try
            {
                OhayooDB db = new OhayooDB();
                result=db.proShippingJapan(RakutenInfo(shopCode)).FirstOrDefault().price.Value;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return result;
        }
        public static string RakutenInfo(string shopCode= "culture")
        {
            string result = "";
            try
            {
                string url = "http://www.rakuten.co.jp/" + shopCode + "/info.html";
                var dom = CQ.CreateFromUrl(url);
                result = dom.Select("blockquote > table:first").ToList()[0].InnerHTML.RemoveHtmlTags();
                result = RakutenPostalCode(result.Trim().Replace("\n",""));
                result = result.Replace("-", "");
            }
            catch { }
            return result;
        }
        public static string RakutenPostalCode(string input)
        {
            Regex emailRegex = new Regex(@"\d{3}-\d{4}",
                RegexOptions.IgnoreCase);
            MatchCollection potalCodes = emailRegex.Matches(input);

            StringBuilder sb = new StringBuilder();

            foreach (Match potalCode in potalCodes)
            {
                return potalCode.Value;
            }
            return "";
        }
    }
}
