using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* To work eith EPPlus library */
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

/* For I/O purpose */
using System.IO;

/* For Diagnostics */
using System.Diagnostics;
using System.Data;
using System.Drawing;
using OfficeOpenXml.Style;
using CsQuery;
using System.Text.RegularExpressions;
using Web.Helpers.YahooAuction;
using Web.Helpers.library;
using Nager.AmazonProductAdvertising.Model;
using Nager.AmazonProductAdvertising;
using Web.Helpers.Rakuten;

namespace Ohayoo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            RakutenUtils json = new RakutenUtils();
            //StreamWriter write = new StreamWriter(@"E:\Project\abc.txt", true, Encoding.UTF8);
            var items = json.getProductsByCategory(559887);
            foreach (var item in items.Items)
            {
                System.Console.WriteLine(item.ItemCode);
            }
            //write.Close();
            System.Console.ReadLine();
        }
        static void amazon()
        {
            //var authentication = new AmazonAuthentication();
            //authentication.AccessKey = "AKIAJOIZSJM4RORGRCCA";
            //authentication.SecretKey = "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";

            //var wrapper = new AmazonWrapper(authentication, AmazonEndpoint.JP, "nagerat-21");
            //var searchOperation = wrapper.ItemSearchOperation("canon eos", AmazonSearchIndex.Electronics);
            //searchOperation.Sort(AmazonSearchSort.Price, AmazonSearchSortOrder.Descending);
            //searchOperation.Skip(2);
            //var xml = wrapper.Request();

            //var result = Nager.AmazonProductAdvertising.XmlHelper.ParseXml<ItemSearchResponse>(xml);
        }
        //static void excel()
        //{
        //    FileInfo existingFile = new FileInfo(@"C:\Users\DELL\Desktop\test\test.xlsx");
        //    FileInfo fNewFile = new FileInfo(@"C:\Users\DELL\Desktop\test\test1.xlsx");
        //    using (ExcelPackage MyExcel = new ExcelPackage(existingFile))
        //    {
        //        ExcelWorksheet MyWorksheet = MyExcel.Workbook.Worksheets["baogiamuaho"];
        //        MyWorksheet.Cells["C6"].Value = "Hello";
                
        //        for (int i = 1; i <= 30; i++)
        //        {
                    
        //            MyWorksheet.Cells["B"+(12+i)].Value = i.ToString();
        //        }
        //        //Add additional info here
        //        MyExcel.SaveAs(fNewFile);
        //    }
        //}
    }
}
