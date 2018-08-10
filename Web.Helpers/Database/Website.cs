using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.Database
{
   public class Website
    {
        public List<String> GetWebsites()
        {
            List<string> lst = new List<string>();
            lst.Add("locondo.jp");
            lst.Add("dena-ec.com");
            lst.Add("aeo.jp");
            lst.Add("forever21.co.jp");
            lst.Add("rakuten.co.jp");
            lst.Add("amazon.co.jp");
            lst.Add("shopping.yahoo.co.jp");
            lst.Add("auctions.yahoo.co.jp");
            lst.Add("uniqlo.com/jp");
            lst.Add("hm.com/ja_jp/");
            lst.Add("shop.adidas.jp");
            lst.Add("wear.jp");
            lst.Add("hikaku.com");
            lst.Add("crocs.co.jp");
            lst.Add("zara.com");
            lst.Add("lacoste.jp");
            lst.Add("gap.co.jp");
            lst.Add("nissen.co.jp");
            return lst;
        }
    }
}
