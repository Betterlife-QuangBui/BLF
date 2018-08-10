using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;
using Web.Helpers.Database;
using Web.Helpers.Images;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class DatabaseController : ManagementSystemController
    {
        // GET: Database
        public ActionResult Index(int page = 1, string key = "")
        {
            ViewBag.Title = "Danh sách sản phẩm";
            ViewBag.key = key;
            return View(Pager<WareHouseItem>.CreatePagging(db.WareHouseItems.AsEnumerable().Where(n => String.Format("{0}", n.JanCode.ChangeUnsigned()).Contains(String.Format("{0}", key.ChangeUnsigned()))).AsQueryable().OrderByDescending(n => n.CreatedAt), page));
        }
        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Import(string fileName = "")
        {
            //upload file
            HttpPostedFileBase file = Request.Files[0];
            fileName = file.FileName.DoiTenFile();
            file.SaveAs(Server.MapPath("~/Uploads/Database/" + fileName));
            // Opening the Excel template... 
            FileStream fs = new FileStream(Server.MapPath("~/Uploads/Database/" + fileName), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook...
            ISheet sheet = null;
            if (fileName.Contains(".xlsx"))
            {
                XSSFWorkbook templateWorkbook = new XSSFWorkbook(Server.MapPath("~/Uploads/Database/" + fileName));
                sheet = templateWorkbook.GetSheet("例");
            }
            else if (fileName.Contains(".xls"))
            {
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                sheet = templateWorkbook.GetSheet("例");
            }
            WebsiteHelpers websiteHelper = new WebsiteHelpers();
            //string namejp = "";
            string jancode = "";
            double orgprice;
            List<ProductItem> lst = new List<ProductItem>();
            for (int row = 8; row <= sheet.LastRowNum; row++)
            {   // Ignoring first row as headers.
                //namejp = sheet.GetRow(row).GetCell(1).StringCellValue;
                //if (namejp != "" && namejp != null && namejp.Trim().Length > 0)
                try
                {
                    jancode = (string)sheet.GetRow(row).GetCell(12).NumericCellValue.ToString();
                }
                catch (Exception)
                {
                    jancode = (string)sheet.GetRow(row).GetCell(12).StringCellValue.ToString();
                }
                if (jancode != "" && jancode != null && jancode.Trim().Length > 3)
                {
                    ProductItem p = new ProductItem();
                    p.JanCode = jancode;
                    p.Link = sheet.GetRow(row).GetCell(1).StringCellValue;
                    p.HSCode = sheet.GetRow(row).GetCell(2).StringCellValue;
                    p.NameJP = sheet.GetRow(row).GetCell(3).StringCellValue;
                    p.NameEN = sheet.GetRow(row).GetCell(4).StringCellValue;
                    p.DescriptionOfGoods = sheet.GetRow(row).GetCell(5).StringCellValue;
                    p.Material = sheet.GetRow(row).GetCell(6).StringCellValue;
                    p.MadeIn = sheet.GetRow(row).GetCell(14).StringCellValue;
                    try
                    {
                        orgprice = sheet.GetRow(row).GetCell(7).NumericCellValue;
                    }
                    catch (Exception)
                    {
                        orgprice = Convert.ToDouble(sheet.GetRow(row).GetCell(7).StringCellValue.ToString().Replace(@"\d", ""));
                    }
                    
                    p.Price = orgprice;

                    //p.NameJP = namejp;
                    //p.NameEN = sheet.GetRow(row).GetCell(2).StringCellValue;
                    //p.CategoryName = sheet.GetRow(row).GetCell(3).StringCellValue;
                    //p.Link = sheet.GetRow(row).GetCell(4).StringCellValue;
                    //p.Price = sheet.GetRow(row).GetCell(5).NumericCellValue;
                    //try
                    //{
                    //    string ImageUrl = websiteHelper.GetImage(p.Link);
                    //    p.ImageUrl = ImageUrl;
                    //    p.ImageBase64 = ImageUtils.Images(ImageUrl);
                    //}
                    //catch { }
                    //p.ShippingMark = sheet.GetRow(row).GetCell(6).StringCellValue;
                    //try
                    //{
                    //    p.JanCode = sheet.GetRow(row).GetCell(7).NumericCellValue + "";
                    //}
                    //catch
                    //{
                    //    p.JanCode = sheet.GetRow(row).GetCell(7) == null ? "" : sheet.GetRow(row).GetCell(7).StringCellValue;
                    //}
                    //p.Quantity = (int)sheet.GetRow(row).GetCell(8).NumericCellValue;
                    //p.MadeIn = sheet.GetRow(row).GetCell(9).StringCellValue;
                    //try
                    //{
                    //    p.Note1 = sheet.GetRow(row).GetCell(10).NumericCellValue + "";
                    //}
                    //catch
                    //{
                    //    p.Note1 = sheet.GetRow(row).GetCell(10).StringCellValue;
                    //}
                    //try
                    //{
                    //    p.Note2 = sheet.GetRow(row).GetCell(11).NumericCellValue + "";
                    //}
                    //catch
                    //{
                    //    p.Note2 = sheet.GetRow(row).GetCell(11).StringCellValue;
                    //}
                    //p.Amount = sheet.GetRow(row).GetCell(12).NumericCellValue;
                    if (lst.Where(n => n.JanCode == p.JanCode).Count() == 0)
                        lst.Add(p);
                    else
                    {
                        if (p.Price > 0)
                        {
                            lst.Single(n => n.JanCode == p.JanCode).Price = p.Price > lst.Single(n => n.JanCode == p.JanCode).Price ? (lst.Single(n => n.JanCode == p.JanCode).Price>0? lst.Single(n => n.JanCode == p.JanCode).Price:p.Price) : p.Price;
                        }
                    }
                }
            }
            foreach (var item in lst)
            {
                try
                {
                    var itemOld = db.WareHouseItems.Single(n => n.JanCode == item.JanCode);
                    if (item.Price > 0)
                    {
                        itemOld.PriceTax = item.Price > itemOld.PriceTax ? (itemOld.PriceTax>0? itemOld.PriceTax:item.Price) : item.Price;
                    }
                    if (itemOld.MadeIn == "" || itemOld.MadeIn == null)
                    {
                        itemOld.MadeIn = getMadeIn(item.MadeIn).Id + "";
                    }
                }
                catch 
                {
                    WareHouseItem warehouse = new WareHouseItem()
                    {
                        Id = Guid.NewGuid(),
                        JanCode = item.JanCode,
                        LinkWeb = item.Link,
                        HSCode = item.HSCode,
                        NameJP = item.NameJP,
                        NameEN = item.NameEN,
                        DescriptionOfGoods = item.DescriptionOfGoods,
                        Material = item.Material,
                        PriceTax = item.Price,
                        ProductCode = item.JanCode,
                        MadeIn = getMadeIn(item.MadeIn).Id + "",
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        ProductTypeId = 1,
                        IsDeny = false

                        //Amount = item.Amount,
                        //CategoryId = getCategory(item.CategoryName).Id,
                        //CategoryName = item.CategoryName,
                        //CreatedAt = DateTime.Now,
                        //CreatedBy = user.Staff.UserName,
                        //Id = Guid.NewGuid(),
                        //Image = item.ImageUrl,
                        //JanCode = item.JanCode,
                        //LinkWeb = item.Link,
                        //ImageBase64 = item.ImageBase64,
                        //Component = "",
                        //ComponentImage = "",
                        //MadeIn = getMadeIn(item.MadeIn).Id + "",
                        //Material = item.Note1,
                        //NameEN = item.NameEN,
                        //NameJP = item.NameJP,
                        //Notes = item.Note1,
                        //PriceTax = item.Price,
                        //ProductCode = item.JanCode,
                        //Quantity = item.Quantity,
                        //UpdatedAt = DateTime.Now,
                        //UpdatedBy = user.Staff.UserName,
                        //ProductTypeId = 1,
                        //IsDeny = false
                    };
                    db.WareHouseItems.Add(warehouse);
                }
                
                
            }

            db.SaveChanges();
            Session["Database"] = "Bar";
            return View(lst);
            //return Redirect("/Database");
        }
        public WareHouseCategory getCategory(string name)
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
        public Country getMadeIn(string name)
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

        [HttpPost]
        public ActionResult Add(WareHouseItem warehouseItem)
        {
            var lstCateSearch = db.WareHouseCategories.Select(n => new SelectListItem() { Text = n.NameEN, Value = n.Id + "" }).ToList();
            
            warehouseItem.CreatedAt = warehouseItem.UpdatedAt = DateTime.Now;
            warehouseItem.CreatedBy = warehouseItem.UpdatedBy = user.Staff.UserName;
            warehouseItem.CreatedBy = warehouseItem.UpdatedBy = user.Staff.UserName;
            warehouseItem.IsDeny = false;

            if(warehouseItem.CategoryId != 0)
            {
                warehouseItem.CategoryName = lstCateSearch.Where(n => n.Value.Equals(warehouseItem.CategoryId.ToString())).First().Text;
            }
            else
            {
                warehouseItem.CategoryId = null;
                warehouseItem.CategoryName = null;
            }
            
            if (warehouseItem.MadeIn.Equals("0"))
            {
                warehouseItem.MadeIn = null;
            }

            if(warehouseItem.Quantity < 0)
            {
                warehouseItem.Quantity = 0;
            }

            if(warehouseItem.PriceTax < 0)
            {
                warehouseItem.PriceTax = 0;
            }
            
            if (warehouseItem.Quantity != null && warehouseItem.PriceTax != null)
            {
                warehouseItem.Amount = warehouseItem.Quantity * warehouseItem.PriceTax;
            }
            else
            {
                warehouseItem.Amount = 0;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    warehouseItem.Id = Guid.NewGuid();
                    db.WareHouseItems.Add(warehouseItem);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            return Json(warehouseItem, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(WareHouseItem warehouseItem)
        {
            try
            {
                var item = db.WareHouseItems.Find(warehouseItem.Id);
                item.NameJP = warehouseItem.NameJP;
                item.NameEN = warehouseItem.NameEN;
                item.CategoryId = warehouseItem.CategoryId;
                item.CategoryWebName = warehouseItem.CategoryWebName;
                item.ImageBase64 = warehouseItem.ImageBase64;
                item.ProductCode = warehouseItem.ProductCode;
                item.JanCode = warehouseItem.JanCode;
                item.Quantity = warehouseItem.Quantity;
                item.PriceTax = warehouseItem.PriceTax;
                item.Amount = warehouseItem.Quantity * warehouseItem.PriceTax;
                item.MadeIn = warehouseItem.MadeIn;
                item.LinkWeb = warehouseItem.LinkWeb;
                item.Material = warehouseItem.Material;
                item.Component = warehouseItem.Component;
                item.ComponentImage = warehouseItem.ComponentImage;
                item.ProductTypeId = warehouseItem.ProductTypeId;
                item.FlightCode = warehouseItem.FlightCode;
                item.TrackingCode = warehouseItem.TrackingCode;
                item.UpdatedAt = DateTime.Now;
                item.UpdatedBy = user.Staff.UserName;
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.WareHouseItems.Remove(db.WareHouseItems.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
        //delete multiple
        [HttpPost]
        public ActionResult DeleteMultiple(string id)
        {
            try
            {
                foreach (var item in id.Split(','))
                {
                    db.WareHouseItems.Remove(db.WareHouseItems.Find(Guid.Parse(item.Trim())));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { message = "Đã xảy ra lỗi trong quá trình xóa dữ liệu", status = false }, JsonRequestBehavior.AllowGet); }
        }
    }
}