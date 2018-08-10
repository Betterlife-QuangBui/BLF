using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ohayoo.Lib
{
   public class ExchangeRateVietComBankUtils
    {
       public static double VietComBank(string url)
       {
           double rate = 0;
           try
           {
               XDocument xdoc = XDocument.Load(url);
               var lv1s = from lv1 in xdoc.Descendants("Exrate")
                          select lv1;
               foreach (var item in lv1s)
               {
                   String CurrencyCode = item.Attributes("CurrencyCode").First().Value;
                   String Sell = item.Attributes("Sell").First().Value;
                   if (CurrencyCode == "JPY")
                   {
                       rate = Convert.ToDouble(Sell);
                       break;
                   }
               }
           }
           catch { }
           return rate;
       }
    }
}
