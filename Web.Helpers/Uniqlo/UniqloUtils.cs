using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web.Helpers.Uniqlo
{
    public class UniqloSearchProductInfo
    {
        public System.Guid Id { get; set; }
        public string NameEN { get; set; }
        public string NameJP { get; set; }
        public string ProductCode { get; set; }
        public string JanCode { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string LinkWeb { get; set; }
        public string Image { get; set; }
        public string Material { get; set; }
        public Nullable<double> Quantity { get; set; }
        public Nullable<double> PriceTax { get; set; }
        public Nullable<double> Amount { get; set; }
        public string MadeIn { get; set; }
        public string Notes { get; set; }
    }
    public class UniqloUtils
    {
        public List<UniqloSearchProductInfo> returnResult(List<IDomObject> idomOnes)
        {
            List<UniqloSearchProductInfo> items = new List<UniqloSearchProductInfo>();
            foreach (var item in idomOnes)
            {
                UniqloSearchProductInfo model = new UniqloSearchProductInfo();
                model.NameJP = WebUtility.HtmlEncode(CQ.Create(item)[".name"].Select(x => x.Cq().Text()).FirstOrDefault().Trim());
                model.LinkWeb = CQ.Create(item)["a"].Select(x => x.Cq().Attr("href")).FirstOrDefault().ToString().Trim();
                string price = CQ.Create(item)[".price"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
                price = Regex.Matches(price, @"[0-9]*[\.,]?[0-9]+")[0].Value;
                model.PriceTax = Convert.ToDouble(price);
                model.Image = CQ.Create(item)[".thumb img"].Select(x => x.Cq().Attr("src")).FirstOrDefault().ToString().Trim();
                try {
                    string JanCode = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.LinkWeb)["#basic li.number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                    model.JanCode = model.ProductCode = JanCode.Substring(5);

                    model.Material = WebUtility.HtmlEncode(CQ.CreateFromUrl(model.LinkWeb)[".content .spec dd:first"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim());
                } catch { }
                items.Add(model);
            }
            return items;
        }
        public List<IDomObject> IdomObject(string url)
        {
            var dom = CQ.CreateFromUrl(url);
            CQ divs = dom.Select("#blkMainItemList div.unit");
            List<IDomObject> idomOnes = divs.ToList();
            return idomOnes;
        }
        public List<UniqloSearchProductInfo> getSearch(string key)
        {
            List<UniqloSearchProductInfo> items = new List<UniqloSearchProductInfo>();
            try
            {
                try
                {
                    string url1 = "http://www.uniqlo.com/jp/store/search.do?qtext=" + key + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=20#thumbnailSelect";
                    items.AddRange(returnResult(IdomObject(url1)));
                    string url2 = "http://www.uniqlo.com/jp/store/search.do?qtext=" + key + "&x=0&y=0&qstart=0&sort=goods_disp_priority&fid=header_search&qbrand=10#thumbnailSelect";
                    items.AddRange(returnResult(IdomObject(url2)));
                }
                catch { }
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            return items;
        }
    }
}
