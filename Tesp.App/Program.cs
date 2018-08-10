using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Helpers.Database;
using Web.Helpers.Images;
using System.IO;
using Tesp.App.db;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using WareHouseJP.Website.Models;

namespace Tesp.App
{
    class Program
    {
        public static WareHouseJPDB db = new WareHouseJPDB();
        public static WareHouseCategory getCategory(string name)
        {
            try
            {
                name = name.Trim();
                return db.WareHouseCategories.ToList().Single(n => n.NameEN.Trim() == name);
            }
            catch
            {
                return db.WareHouseCategories.Find(1);
            }
        }
        public static Country getMadeIn(string name)
        {
            try
            {
                name = name.Trim();
                return db.Countries.ToList().Single(n => n.Name.Trim() == name);
            }
            catch
            {
                return db.Countries.Find(108);
            }
        }
        static void Main1(string[] args)
        {
            string[] fileArray = Directory.GetFiles(@"C:\FILES\");
            foreach (var itemFile in fileArray)
            {
                try
                {
                    // Opening the Excel template... 
                    FileStream fs = new FileStream(itemFile, FileMode.Open, FileAccess.Read);
                    // Getting the complete workbook...
                    ISheet sheet = null;
                    string flighCode = "";
                    if (itemFile.Contains(".xlsx"))
                    {
                        XSSFWorkbook templateWorkbook = new XSSFWorkbook(itemFile);
                        sheet = templateWorkbook.GetSheet("INVOICE");
                        flighCode = templateWorkbook.GetSheet("INFO").GetRow(4).Cells[8].StringCellValue.Trim();
                    }
                    else if (itemFile.Contains(".xls"))
                    {
                        HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                        sheet = templateWorkbook.GetSheet("INVOICE");
                        flighCode = templateWorkbook.GetSheet("INFO").GetRow(4).Cells[8].StringCellValue.Trim();
                    }
                    WebsiteHelpers websiteHelper = new WebsiteHelpers();
                    string namejp = "";
                    List<ProductItem> lst = new List<ProductItem>();
                    for (int row = 11; row <= sheet.LastRowNum; row++)
                    { // Ignoring first row as headers.
                        namejp = sheet.GetRow(row).GetCell(1).StringCellValue;
                        if (namejp != "" && namejp != null && namejp.Trim().Length > 0)
                        {
                            ProductItem p = new ProductItem();
                            p.NameJP = namejp;
                            p.NameEN = sheet.GetRow(row).GetCell(2).StringCellValue;
                            p.CategoryName = sheet.GetRow(row).GetCell(3).StringCellValue;
                            p.Link = sheet.GetRow(row).GetCell(4).StringCellValue;
                            p.Price = sheet.GetRow(row).GetCell(5).NumericCellValue;
                            try
                            {
                                string ImageUrl = websiteHelper.GetImage(p.Link);
                                p.ImageUrl = ImageUrl;
                                p.ImageBase64 = ImageUtils.Images(ImageUrl);
                            }
                            catch { }
                            p.ShippingMark = sheet.GetRow(row).GetCell(6).StringCellValue;
                            try
                            {
                                p.JanCode = sheet.GetRow(row).GetCell(7).NumericCellValue + "";
                            }
                            catch
                            {
                                p.JanCode = sheet.GetRow(row).GetCell(7) == null ? "" : sheet.GetRow(row).GetCell(7).StringCellValue;
                            }
                            p.Quantity = (int)sheet.GetRow(row).GetCell(8).NumericCellValue;
                            p.MadeIn = sheet.GetRow(row).GetCell(9).StringCellValue;
                            try
                            {
                                p.Note1 = sheet.GetRow(row).GetCell(10).NumericCellValue + "";
                            }
                            catch
                            {
                                p.Note1 = sheet.GetRow(row).GetCell(10).StringCellValue;
                            }
                            try
                            {
                                p.Note2 = sheet.GetRow(row).GetCell(11).NumericCellValue + "";
                            }
                            catch
                            {
                                p.Note2 = sheet.GetRow(row).GetCell(11).StringCellValue;
                            }
                            p.Amount = sheet.GetRow(row).GetCell(12).NumericCellValue;
                            lst.Add(p);
                        }
                    }
                    foreach (var item in lst)
                    {
                        WareHouseItem warehouse = new WareHouseItem()
                        {
                            Amount = item.Amount,
                            CategoryId = getCategory(item.CategoryName).Id,
                            CategoryName = item.CategoryName,
                            CreatedAt = DateTime.Now,
                            CreatedBy = "blf",
                            Id = Guid.NewGuid(),
                            Image = item.ImageUrl,
                            JanCode = item.JanCode,
                            LinkWeb = item.Link,
                            ImageBase64 = item.ImageBase64,
                            Component = "",
                            ComponentImage = "",
                            MadeIn = getMadeIn(item.MadeIn).Id + "",
                            Material = item.Note1,
                            NameEN = item.NameEN,
                            NameJP = item.NameJP,
                            Notes = item.Note1,
                            PriceTax = item.Price,
                            ProductCode = item.JanCode,
                            Quantity = item.Quantity,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = "blf",
                            ProductTypeId = 1,
                            IsDeny = false,
                            FlightCode = flighCode
                        };
                        db.WareHouseItems.Add(warehouse);
                    }

                    db.SaveChanges();
                }
                catch { }
            }
            Console.WriteLine("finished");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            try
            {
                string itemFile = @"D:\ADMIN_LUUKHO.xls";
                // Opening the Excel template... 
                FileStream fs = new FileStream(itemFile, FileMode.Open, FileAccess.Read);
                // Getting the complete workbook...
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                var sheet = templateWorkbook.GetSheet("例");
                var lst = templateWorkbook.GetAllPictures();
                int i = 0;
                foreach (var item in lst)
                {
                    var pic = (HSSFPictureData)item;
                    byte[] data = pic.Data;
                    BinaryWriter writer = new BinaryWriter(File.OpenWrite(String.Format("{0}.jpeg", i)));
                    writer.Write(data);
                    writer.Flush();
                    writer.Close();
                    i++;
                }
            }
            catch { }
        }
    }
}
