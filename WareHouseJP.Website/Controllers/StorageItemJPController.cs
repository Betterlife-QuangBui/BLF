using CsQuery;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using Web.Helpers.Adidas;
using Web.Helpers.Amazon;
using Web.Helpers.HM;
using Web.Helpers.library;
using Web.Helpers.Rakuten;
using Web.Helpers.Uniqlo;
using Web.Helpers.YahooAuction;
using Web.Helpers.YahooShopping;

namespace WareHouseJP.Website.Controllers
{
    public class StorageItemJPController : ManagementSystemController
    {
        // GET: StorageItemJP
        public ActionResult Index(Guid id)
        {
            var storageItemJPs = db.StorageItemJPs.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Include(s => s.TrackingDetail).Where(n => n.StoregeJPId == id).OrderByDescending(c => c.CreatedAt);
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name", 108);
            ViewBag.StatusId = new SelectList(db.StatusWareHouses, "Id", "Name");
            ViewBag.StorageJP = db.StorageJPs.Find(id);
            ViewBag.Website = new SelectList(SearchWebsiteUtils.GetWebsite(), "Value", "Text");
            ViewBag.listResultSearch = new List<SearchProductInfo>();
            ViewBag.searchCate = new string[] { "database" };
            ViewBag.TrackingStatusId = new SelectList(StatusUtils.GetStatus(2), "Value", "Text", "");
            if (db.StorageJPs.Find(id).TrackingDetails.Count > 0)
            {
                ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(id).TrackingCode), "Value", "Text");
            }
            else
            {
                ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(id).TrackingCode).Where(n => n.Value.Contains(" - 01")), "Value", "Text");
            }
            ViewBag.searchCate = new string[] { "Database", "Rakuten", "Amazon" };
            return View(storageItemJPs.ToList());
        }
        string getJanCode(string LinkWeb)
        {
            string result = "";
            try
            {
                result = CQ.CreateFromUrl(LinkWeb)[".item_number"].Select(x => x.Cq().Text()).FirstOrDefault().ToString().Trim();
            }
            catch
            {


            }
            return result;
        }
        public String getRakutenCategories(int CategoryId)
        {
            string appId = "1095695100824025173";
            List<String> lst = new List<String>();
            try
            {
                string url = "https://app.rakuten.co.jp/services/api/IchibaGenre/Search/20140222?format=xml&genreId=" + CategoryId + "&applicationId=" + appId;
                XDocument xdoc = OhayooLib.getXDocument(url);
                return WebUtility.HtmlEncode(xdoc.Element("root").Element("current").Element("genreName").Value);
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
        public ActionResult SearchRakuten(string SearchCode, Guid StoregeJPId)
        {
            List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
            RakutenUtils rakuten = new RakutenUtils();

            try
            {
                listSearch.AddRange(rakuten.getProductsSearchWareHouse(SearchCode).Items.Select(n => new SearchProductInfo()
                {
                    Id = Guid.NewGuid(),
                    Image = n.SmallImageUrls[0].ImageUrl,
                    JanCode = getJanCode(n.ItemUrl),
                    ProductCode = n.ItemCode,
                    Amount = n.ItemPrice * 1,
                    LinkWeb = n.ItemUrl,
                    NameJP = n.ItemName,
                    NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                    PriceTax = n.ItemPrice,
                    Quantity = 1,
                    Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                    CategoryId = n.CategoryId,
                    CategoryName = getRakutenCategories(n.CategoryId)
                }));
            }
            catch { }
            return PartialView(listSearch);
        }
        [HttpPost]
        public ActionResult UpdateSearchProduct(Guid id, string jancode, string namejp, double price)
        {
            var listSearch = Session["searchResult"] as List<SearchProductInfo>;
            var item = listSearch.Single(n => n.Id == id);
            item.JanCode = jancode;
            item.NameJP = namejp;
            item.PriceTax = price;
            return Content("");
        }
        [HttpPost]
        public ActionResult UpdateParent(Guid id)
        {
            try
            {
                var item = db.StorageJPs.Find(id);
                if (item.IsCheck == false || item.IsCheck == null)
                {
                    if (item.StorageItemJPs.Count > 0 && item.TrackingDetails.Count > 1) { item.StatusId = 7; }
                    else if (item.StorageItemJPs.Count > 0 && item.TrackingDetails.Count <= 1) { item.StatusId = 6; }
                    else { item.StatusId = 5; }
                    db.SaveChanges();
                }
                var PageUtils = new PageUtils();
                return Json(new { message = new { status = PageUtils.PackageStatus(item.StatusId.Value, 2), weight = item.TrackingDetails.Sum(m => m.Weigh), count = item.TrackingDetails.Count }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SearchInternet(string SearchCode, string[] Website, Guid StoregeJPId)
        {
            var storageItemJPs = db.StorageItemJPs.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Include(s => s.TrackingDetail).Where(n => n.StoregeJPId == StoregeJPId).OrderByDescending(c => c.CreatedAt);
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name", 108);
            ViewBag.StatusId = new SelectList(db.StatusWareHouses, "Id", "Name");
            ViewBag.TrackingStatusId = new SelectList(StatusUtils.GetStatus(1), "Value", "Text", "");
            ViewBag.StorageJP = db.StorageJPs.Find(StoregeJPId);
            if (db.StorageJPs.Find(StoregeJPId).TrackingDetails.Count > 0)
            {
                ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(StoregeJPId).TrackingCode), "Value", "Text");
            }
            else
            {
                ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(StoregeJPId).TrackingCode).Where(n => n.Value.Contains(" - 01")), "Value", "Text");
            }
            var searchCate = Website;
            List<SearchProductInfo> listSearch = new List<SearchProductInfo>();
            if (searchCate == null) { searchCate = new string[] { "database" }; }
            foreach (var item in searchCate)
            {
                if (item.ToLower().Contains("Rakuten".ToLower()))
                {
                    #region Rakuten
                    RakutenUtils rakuten = new RakutenUtils();
                    try
                    {
                        listSearch.AddRange(rakuten.getProductsSearchWareHouse(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.SmallImageUrls[0].ImageUrl,
                            JanCode = getJanCode(n.ItemUrl),
                            ProductCode = n.ItemCode,
                            Amount = n.ItemPrice * 1,
                            LinkWeb = n.ItemUrl,
                            NameJP = n.ItemName,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.ItemPrice,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                            CategoryId = n.CategoryId,
                            CategoryName = getRakutenCategories(n.CategoryId)
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("YahooShopping".ToLower()))
                {
                    #region Yahoo Shopping
                    YahooShoppingUtils yahooShop = new YahooShoppingUtils();
                    try
                    {
                        listSearch.AddRange(yahooShop.getProductsBySearch(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.Code,
                            ProductCode = n.ProductId,
                            Amount = n.Price * 1,
                            LinkWeb = n.Url,
                            NameJP = n.Name,
                            //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Name),
                            PriceTax = n.Price,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags(),
                            CategoryId = n.CategoryId,
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("YahooAuction".ToLower()))
                {
                    #region YahooAuction
                    YahooAuctionUtils yahooAuction = new YahooAuctionUtils();
                    try
                    {
                        listSearch.AddRange(yahooAuction.getProductsBySarch(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.AuctionID,
                            ProductCode = n.AuctionID,
                            Amount = n.CurrentPrice * 1,
                            LinkWeb = n.AuctionItemUrl,
                            NameJP = n.Title,
                            //NameEN = TranslateUtils.TranslateGoogleTextEN(n.Title),
                            PriceTax = n.CurrentPrice,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Material).RemoveHtmlTags(),
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Amazon".ToLower()))
                {
                    #region Amazon
                    AmazonUtils amazon = new AmazonUtils();
                    try
                    {
                        listSearch.AddRange(amazon.SearchBy(SearchCode).Items.Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.ImageUrl,
                            JanCode = n.ItemCode,
                            ProductCode = n.ItemCode,
                            Amount = n.ItemPrice * 1,
                            LinkWeb = n.ItemUrl,
                            NameJP = n.ItemName,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.ItemPrice,
                            Quantity = 1,
                            CategoryName = n.CategoryName,
                            Material = WebUtility.HtmlDecode(n.ItemCaption).RemoveHtmlTags().SplitMaritalRakuten(),
                        }));
                    }
                    catch (Exception ex) { }
                    #endregion
                }
                else if (item.ToLower().Contains("Uniqlo".ToLower()))
                {
                    #region Uniqlo
                    UniqloUtils uniqlo = new UniqloUtils();
                    try
                    {
                        listSearch.AddRange(uniqlo.getSearch(SearchCode).Select(n => new Models.SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductCode,
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            //NameEN = TranslateUtils.TranslateGoogleTextEN(n.NameJP),
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = n.Material,
                            CategoryName = ""
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Adidas".ToLower()))
                {
                    #region Adidas
                    AdidasUtils adidas = new AdidasUtils();
                    try
                    {
                        listSearch.AddRange(adidas.Search(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductId,
                            ProductCode = n.ProductId,
                            Amount = n.Price * 1,
                            LinkWeb = n.Url,
                            NameJP = n.Name,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.Price,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("Hm".ToLower()))
                {
                    #region Hm
                    HMUtils hm = new HMUtils();
                    try
                    {
                        listSearch.AddRange(hm.Search(SearchCode).Select(n => new SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductId,
                            ProductCode = n.ProductId,
                            Amount = n.Price * 1,
                            LinkWeb = n.Url,
                            NameJP = n.Name,
                            NameEN = "",//TranslateUtils.TranslateGoogleTextEN(n.ItemName),
                            PriceTax = n.Price,
                            Quantity = 1,
                            Material = WebUtility.HtmlDecode(n.Description).RemoveHtmlTags().SplitMaritalRakuten(),
                            CategoryName = n.CategoryName
                        }));
                    }
                    catch { }
                    #endregion
                }
                else if (item.ToLower().Contains("database".ToLower()))
                {
                    #region Database
                    try
                    {
                        listSearch.AddRange(db.WareHouseItems.Where(n => n.ProductCode.Contains(SearchCode) || n.JanCode.Contains(SearchCode) || n.NameJP.Contains(SearchCode) || n.NameEN.Contains(SearchCode)).Take(20).Select(n => new Models.SearchProductInfo()
                        {
                            Id = Guid.NewGuid(),
                            Image = n.Image,
                            JanCode = n.ProductCode,
                            ProductCode = n.ProductCode,
                            Amount = n.PriceTax * 1,
                            LinkWeb = n.LinkWeb,
                            NameJP = n.NameJP,
                            NameEN = n.NameEN,
                            PriceTax = n.PriceTax,
                            Quantity = 1,
                            Material = n.Material,
                            CategoryName = n.CategoryName,
                            MadeIn = n.MadeIn.ToString(),
                        }));
                        
                    }
                    catch { }
                    #endregion
                }
            }
            ViewBag.listResultSearch = listSearch.OrderBy(n => n.PriceTax);
            Session["searchResult"] = listSearch.OrderBy(n => n.PriceTax).ToList();
            ViewBag.Website = new SelectList(SearchWebsiteUtils.GetWebsite(), "Value", "Text");
            ViewBag.SearchCode = SearchCode;
            ViewBag.searchCate = searchCate;
            return View("Index", storageItemJPs);
        }

        [HttpPost]
        public ActionResult Research(string SearchCode, int SearchQuantity, Guid StoregeJPId)
        {
            try
            {
                ResearchProductInfo research = new ResearchProductInfo();
                research.Reseach(SearchCode, SearchQuantity, StoregeJPId);
            }
            catch { }
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.StatusId = new SelectList(db.StatusWareHouses, "Id", "Name");
            ViewBag.MadeIn = new SelectList(db.Countries, "Name", "Name");
            ViewBag.StorageJP = db.StorageJPs.Find(StoregeJPId);
            var storageItemJPs = db.StorageItemJPs.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Include(s => s.TrackingDetail).Where(n => n.StoregeJPId == StoregeJPId).OrderByDescending(c => c.CreatedAt);
            return View("Index", storageItemJPs);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var item = db.StorageItemJPs.Find(id);
                double quanity = item.Quantity.Value;
                double count = item.TrackingDetail.StorageItemJPs.Sum(n => n.Quantity).Value;

                double amount = item.TrackingDetail.StorageItemJPs.Sum(n => n.Amount).Value;
                double _amount = item.Amount.Value;
                db.StorageItemJPs.Remove(db.StorageItemJPs.Find(id));
                db.SaveChanges();
                return Json(new { message = new { count = count - quanity, amount = amount - _amount }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult UpdateTrackingDetail(Guid id, string weigh, string size, string note, int status)
        {
            try
            {
                var item = db.TrackingDetails.Find(id);
                item.Weigh = double.Parse(weigh);
                item.Size = size;
                item.Notes = note;
                item.StatusId = status;
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult DeleteTracking(Guid id, Guid dataParent)
        {
            try
            {
                var lst = db.StorageItemJPs.Where(n => n.TrackingDetailId == id);
                var parent = db.TrackingDetails.Find(dataParent);
                foreach (var item in lst)
                {
                    var model = db.StorageItemJPs.Where(n => n.ProductCode == item.ProductCode && n.TrackingDetailId == parent.Id).FirstOrDefault();
                    try
                    {
                        model.Quantity = model.Quantity + item.Quantity;
                    }
                    catch
                    {
                        var itemNew = item;
                        itemNew.TrackingDetailId = parent.Id;
                        db.StorageItemJPs.Add(itemNew);
                    }
                }
                db.StorageItemJPs.RemoveRange(lst);
                db.TrackingDetails.Remove(db.TrackingDetails.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        void deleteTracking(Guid trackingDetailId)
        {
            if (db.StorageItemJPs.Where(n => n.TrackingDetailId == trackingDetailId).Count() == 0)
            {
                db.TrackingDetails.Remove(db.TrackingDetails.Find(trackingDetailId));
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult DeleteItemChild(Guid id, Guid parentJ)
        {
            try
            {
                var lst = db.StorageItemJPs.Find(id);
                var trackingDetaildId = lst.TrackingDetailId.Value;
                var parent = db.TrackingDetails.Find(parentJ);
                var model = db.StorageItemJPs.Where(n => n.ProductCode == lst.ProductCode && n.TrackingDetailId == parent.Id).FirstOrDefault();
                try
                {
                    model.Quantity = model.Quantity + lst.Quantity;
                }
                catch
                {
                    var itemNew = new StorageItemJP()
                    {
                        Id = Guid.NewGuid(),
                        Amount = lst.Amount,
                        CategoryId = lst.CategoryId,
                        CategoryName = lst.CategoryName,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Image = lst.Image,
                        JanCode = lst.JanCode,
                        LinkWeb = lst.LinkWeb,
                        MadeIn = lst.MadeIn,
                        Material = lst.Material,
                        NameEN = lst.NameEN,
                        NameJP = lst.NameJP,
                        Notes = lst.Notes,
                        PriceTax = lst.PriceTax,
                        ProductCode = lst.ProductCode,
                        Quantity = lst.Quantity,
                        StatusId = lst.StatusId,
                        StoregeJPId = lst.StoregeJPId,
                        TrackingDetailId = parent.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.StorageItemJPs.Add(itemNew);
                }
                db.StorageItemJPs.Remove(lst);
                db.SaveChanges();
                deleteTracking(trackingDetaildId);
                return Json(new { message = "Xóa dữ liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UpdateItem(Guid id, int categoryId, string nameJP, string nameEN, double price, int quantity, string marital, int madeIn, int status)
        {
            try
            {
                var item = db.StorageItemJPs.Find(id);
                item.CategoryId = categoryId;
                item.NameEN = nameEN; item.NameJP = nameJP;
                item.PriceTax = price; item.Quantity = quantity;
                item.Amount = price * quantity;
                item.MadeIn = madeIn + ""; item.Material = marital;
                item.StatusId = status;
                db.SaveChanges();
                return Json(new { message = new { count = item.TrackingDetail.StorageItemJPs.Sum(n => n.Quantity), amount = item.TrackingDetail.StorageItemJPs.Sum(n => n.Amount) }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        TrackingDetail CreateDetail(Guid idDetail, Guid StoregeJPId, string trackingSub)
        {
            TrackingDetail detail = new TrackingDetail()
            {
                CreatedAt = DateTime.Now,
                CreatedBy = user.Staff.UserName,
                Id = idDetail,
                Image = "",
                Notes = "",
                SaveDate = DateTime.Now,
                SaveHour = DateTime.Now.ToString("HH:mm tt"),
                Size = "",
                StatusId = 1,
                StoregeJPId = StoregeJPId,
                TrackingSubCode = trackingSub,
                UpdatedAt = DateTime.Now,
                UpdatedBy = user.Staff.UserName,
                Weigh = 0
            };
            return detail;
        }
        StorageItemJP CreateStorageItemJP(Guid Id, Guid idDetail, StorageItemJP item, int categoryId, double price, int quantity, string madeIn, string marital, string nameEN, string nameJP, int status)
        {
            StorageItemJP storeItem = new StorageItemJP()
            {
                Amount = price * quantity,
                CategoryId = categoryId,
                CategoryName = item.CategoryName,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Staff.UserName,
                Id = Guid.NewGuid(),
                Image = item.Image,
                IsExport = item.IsExport,
                JanCode = item.JanCode,
                LinkWeb = item.LinkWeb,
                MadeIn = madeIn + "",
                Material = marital,
                NameEN = nameEN,
                NameJP = nameJP,
                Notes = item.Notes,
                PriceTax = price,
                ProductCode = item.ProductCode,
                Quantity = quantity,
                StatusId = status,
                StoregeJPId = item.StoregeJPId,
                TrackingDetailId = idDetail,
                UpdatedAt = DateTime.Now,
                UpdatedBy = user.Staff.UserName,
                IsDeny = item.IsDeny
            };
            return storeItem;
        }
        [HttpPost]
        public ActionResult UpdateItemV2(Guid id, string trackingSub, int categoryId, string nameJP, string nameEN, double price, int quantity, string marital, int madeIn, int status, string trackingSub_Old = "", bool clone = false, bool isDeny = false, string jancode = "")
        {
            trackingSub = trackingSub.Trim();
            trackingSub_Old = trackingSub_Old.Trim();
            var item = db.StorageItemJPs.Find(id); item.IsDeny = isDeny;
            var store = item.StorageJP;
            int check = 0;
            try
            {
                //edit on item not copy row
                if (item.TrackingDetail.TrackingSubCode == trackingSub)
                {
                    item.CategoryId = categoryId;
                    //item.JanCode = jancode;
                    item.NameEN = nameEN; item.NameJP = nameJP;
                    item.PriceTax = price; item.Quantity = quantity;
                    item.Amount = price * quantity;
                    item.MadeIn = madeIn + ""; item.Material = marital;
                    item.StatusId = status;
                }
                else
                {
                    var detail = store.TrackingDetails.SingleOrDefault(n => n.TrackingSubCode == trackingSub);
                    if (detail == null)
                    {
                        detail = new TrackingDetail();
                        Guid idDetail = Guid.NewGuid();
                        detail = CreateDetail(idDetail, store.Id, trackingSub);
                        db.TrackingDetails.Add(detail);
                        StorageItemJP itemJp = CreateStorageItemJP(Guid.NewGuid(), idDetail, item, categoryId, price, quantity, madeIn + "", marital, nameEN, nameJP, status);
                        db.StorageItemJPs.Add(itemJp);
                        check = 1;
                    }
                    else
                    {
                        var item__ = detail.StorageItemJPs.SingleOrDefault(n => n.JanCode == item.JanCode);
                        if (item__ == null)
                        {
                            var item_ = new StorageItemJP();

                            StorageItemJP itemJp = CreateStorageItemJP(Guid.NewGuid(), detail.Id, item, categoryId, price, quantity, madeIn + "", marital, nameEN, nameJP, status);
                            db.StorageItemJPs.Add(itemJp);
                            check = 2;
                        }
                        else
                        {
                            item__.CategoryId = categoryId;
                            item__.NameEN = nameEN; item.NameJP = nameJP;
                            item__.PriceTax = price; item__.Quantity = item__.Quantity + quantity;
                            item__.Amount = price * item__.Quantity;
                            item__.MadeIn = madeIn + ""; item__.Material = marital;
                            item__.StatusId = status;
                            check = 3;
                        }
                    }
                }

                db.SaveChanges();
                return Json(new { message = new { count = item.TrackingDetail.StorageJP.TrackingDetails.Sum(n => n.StorageItemJPs.Sum(m => m.Quantity)), amount = item.TrackingDetail.StorageJP.TrackingDetails.Sum(n => n.StorageItemJPs.Sum(m => m.Amount)), check = check }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult SaveTrackingSub(Guid id, string subtracking, int quantity, Guid detailId, Guid modalStoregeJPId)
        {
            try
            {
                subtracking = subtracking.Trim();
                StorageItemJP search = db.StorageItemJPs.Find(id);
                TrackingDetail trackingDetail = db.TrackingDetails.Find(detailId);
                //add tracking detail
                Guid idTracking = Guid.NewGuid();
                if (db.TrackingDetails.Where(n => n.StoregeJPId == modalStoregeJPId && n.TrackingSubCode == subtracking).Count() == 0)
                {
                    //add new
                    TrackingDetail tracking = new TrackingDetail()
                    {
                        Id = idTracking,
                        StoregeJPId = modalStoregeJPId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        SaveDate = DateTime.Now,
                        SaveHour = "",
                        Size = "",
                        TrackingSubCode = subtracking,
                        Weigh = 0,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.TrackingDetails.Add(tracking);
                }
                else
                {
                    idTracking = db.TrackingDetails.Where(n => n.StoregeJPId == modalStoregeJPId && n.TrackingSubCode == subtracking).FirstOrDefault().Id;
                }

                if (db.StorageItemJPs.Where(n => n.StoregeJPId == modalStoregeJPId && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).Count() == 0)
                {
                    //add new StorageItem
                    StorageItemJP model = new StorageItemJP()
                    {
                        Amount = search.PriceTax * quantity,
                        CategoryId = search.CategoryId,
                        CategoryName = search.CategoryName,
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        Image = search.Image,
                        JanCode = search.JanCode,
                        LinkWeb = search.LinkWeb,
                        MadeIn = search.MadeIn,
                        Material = search.Material,
                        NameEN = search.NameEN,
                        NameJP = search.NameJP,
                        Notes = "",
                        PriceTax = search.PriceTax,
                        ProductCode = search.ProductCode,
                        Quantity = quantity,
                        StatusId = search.StatusId,
                        StoregeJPId = modalStoregeJPId,
                        UpdatedAt = DateTime.Now,
                        TrackingDetailId = idTracking,
                        UpdatedBy = user.Staff.UserName
                    };
                    db.StorageItemJPs.Add(model);
                }

                else
                {
                    var storageItem = db.StorageItemJPs.Where(n => n.StoregeJPId == modalStoregeJPId && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).FirstOrDefault();
                    storageItem.Quantity = quantity + storageItem.Quantity;
                    storageItem.Amount = storageItem.PriceTax * (quantity + storageItem.Quantity);
                }

                if (search.Quantity == quantity)
                {
                    db.StorageItemJPs.Remove(search);
                }
                else
                {
                    search.Quantity = search.Quantity - quantity;
                }

                db.SaveChanges();
                return Json(new { message = "Cập nhật liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult AddNewItem(Guid StoregeJPId, string IdItem, string TrackingCode, int Quantity, int CategoryId, int MadeIn, int StatusId)
        {
            //add tracking detail
            TrackingCode = TrackingCode.Trim();
            Guid idTracking = Guid.NewGuid();
            if (db.TrackingDetails.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingSubCode == TrackingCode).Count() == 0)
            {
                //add new
                TrackingDetail tracking = new TrackingDetail()
                {
                    Id = idTracking,
                    StoregeJPId = StoregeJPId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = user.Staff.UserName,
                    SaveDate = DateTime.Now,
                    SaveHour = "",
                    Size = "",
                    TrackingSubCode = TrackingCode,
                    Weigh = 0,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = user.Staff.UserName
                };
                db.TrackingDetails.Add(tracking);
            }
            else
            {
                idTracking = db.TrackingDetails.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingSubCode == TrackingCode).FirstOrDefault().Id;
            }
            //add product into tracking details
            var lst = Session["searchResult"] as List<SearchProductInfo>;
            foreach (var id in IdItem.Split(','))
            {
                if (id.Trim().Length > 0)
                {
                    SearchProductInfo search = lst.FirstOrDefault(n => n.Id == Guid.Parse(id));
                    if (db.StorageItemJPs.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).Count() == 0)
                    {
                        //add new StorageItem
                        StorageItemJP model = new StorageItemJP()
                        {
                            Amount = search.PriceTax * Quantity,
                            CategoryId = CategoryId,
                            CategoryName = search.CategoryName,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            Image = search.Image,
                            JanCode = search.JanCode,
                            LinkWeb = search.LinkWeb,
                            MadeIn = MadeIn + "",
                            Material = search.Material,
                            NameEN = search.NameEN,
                            NameJP = search.NameJP,
                            Notes = "",
                            PriceTax = search.PriceTax,
                            ProductCode = search.ProductCode,
                            Quantity = Quantity,
                            StatusId = StatusId,
                            StoregeJPId = StoregeJPId,
                            UpdatedAt = DateTime.Now,
                            TrackingDetailId = idTracking,
                            UpdatedBy = user.Staff.UserName
                        };
                        if (model.ProductCode == null || model.ProductCode.Trim().Length == 0)
                        {
                            model.ProductCode = model.JanCode;
                        }
                        if (model.JanCode == null || model.JanCode.Trim().Length == 0)
                        {
                            model.JanCode = model.ProductCode;
                        }
                        db.StorageItemJPs.Add(model);
                        //add warehouse
                        WareHouseItem warehouse = new WareHouseItem();
                        if (db.WareHouseItems.Where(n => n.JanCode == model.JanCode && n.ProductCode == model.ProductCode).Count() == 0)
                        {
                            warehouse = new WareHouseItem()
                            {
                                Amount = model.Amount,
                                CategoryId = model.CategoryId,
                                CategoryName = model.CategoryName,
                                CreatedAt = model.CreatedAt,
                                CreatedBy = model.CreatedBy,
                                Id = Guid.NewGuid(),
                                Image = model.Image,
                                JanCode = model.JanCode,
                                LinkWeb = model.LinkWeb,
                                MadeIn = model.MadeIn,
                                Material = model.Material,
                                NameEN = TranslateUtils.TranslateGoogleTextEN(model.NameJP),
                                NameJP = model.NameJP,
                                Notes = model.Notes,
                                PriceTax = model.PriceTax,
                                ProductCode = model.ProductCode,
                                Quantity = model.Quantity,
                                UpdatedAt = model.UpdatedAt,
                                UpdatedBy = model.UpdatedBy
                            };
                            db.WareHouseItems.Add(warehouse);
                        }
                    }

                    else
                    {
                        var storageItem = db.StorageItemJPs.Where(n => n.StoregeJPId == StoregeJPId && n.TrackingDetailId == idTracking && n.ProductCode == search.ProductCode && n.JanCode == search.JanCode).FirstOrDefault();
                        storageItem.Quantity = Quantity + storageItem.Quantity;
                        storageItem.Amount = storageItem.PriceTax * (Quantity + storageItem.Quantity);
                    }
                }
            }
            db.SaveChanges();

            Session["searchResult"] = null;
            var storageItemJPs = db.StorageItemJPs.Where(n => n.StorageJP.AgencyId == user.Agency.Id).Include(s => s.TrackingDetail).Where(n => n.StoregeJPId == StoregeJPId).OrderByDescending(c => c.CreatedAt);
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories, "Id", "NameEN");
            ViewBag.MadeIn = new SelectList(db.Countries, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.StatusWareHouses, "Id", "Name");
            ViewBag.StorageJP = db.StorageJPs.Find(StoregeJPId);
            ViewBag.listResultSearch = new List<SearchProductInfo>();
            ViewBag.TrackingCode = new SelectList(SelectListUtils.TrackingList(db.StorageJPs.Find(StoregeJPId).TrackingCode), "Value", "Text");
            ViewBag.searchCate = new string[] { "Rakuten" };
            return Redirect("/StorageItemJP/Index/" + StoregeJPId);
        }

        [HttpPost]
        public ActionResult DeleteItemRow(Guid id)
        {
            try
            {
                var item = db.StorageItemJPs.Find(id);
                Guid idTracking = item.TrackingDetailId.Value;
                var count = item.TrackingDetail.StorageJP.TrackingDetails.Sum(n => n.StorageItemJPs.Sum(m => m.Quantity));
                var amount = item.TrackingDetail.StorageJP.TrackingDetails.Sum(n => n.StorageItemJPs.Sum(m => m.Amount));
                db.StorageItemJPs.Remove(item);
                db.SaveChanges();
                int delete = 0;
                //delete tracking if item <=0
                if (db.StorageItemJPs.Where(n => n.TrackingDetailId == idTracking).Count() == 0)
                {
                    db.TrackingDetails.Remove(db.TrackingDetails.Find(idTracking));
                    db.SaveChanges();
                    delete = 1;
                }
                return Json(new { message = new { count = count, amount = amount, delete = delete, parent = idTracking.ToString() }, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        //21-10-2016
        [HttpPost]
        public ActionResult DeleteItems(string ids = "")
        {
            try
            {
                foreach (var item in ids.Split(','))
                {
                    var store = db.StorageItemJPs.Find(Guid.Parse(item));
                    //var trackingDetail = store.TrackingDetail;
                    db.StorageItemJPs.Remove(store);
                    //if (trackingDetail.StorageItemJPs.Count() == 0)
                    //{
                    //    db.TrackingDetails.Remove(trackingDetail);
                    //}
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult UpdateMadeIn(string ids = "", string MadeIn = "108")
        {
            try
            {
                foreach (var item in ids.Split(','))
                {
                    var store = db.StorageItemJPs.Find(Guid.Parse(item));
                    store.MadeIn = MadeIn;
                }
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult SplitPackage(string ids = "", string trackingTranfer = "", int quantity = 1)
        {
            try
            {
                foreach (var item in ids.Split(','))
                {
                    if (trackingTranfer == "21")
                    {
                        var store = db.StorageItemJPs.Find(Guid.Parse(item));
                        var trackingParent = db.StorageJPs.Find(store.TrackingDetail.StoregeJPId);
                        
                        var trackingId = store.TrackingDetailId;
                        //check tracking tranfer is exits
                        TrackingDetail destTrackingDetail = null;
                        if (trackingParent.TrackingDetails.Where(n => n.TrackingSubCode == trackingTranfer).Count() > 0)
                        {
                            destTrackingDetail = trackingParent.TrackingDetails.SingleOrDefault(n => n.TrackingSubCode == trackingTranfer);
                        }
                        else
                        {
                            destTrackingDetail = new TrackingDetail()
                            {
                                CreatedAt = DateTime.Now,
                                CreatedBy = user.Staff.UserName,
                                Id = Guid.NewGuid(),
                                StoregeJPId = trackingParent.Id,
                                TrackingSubCode = trackingTranfer,
                                Weigh = 0,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = user.Staff.UserName,
                            };
                            db.TrackingDetails.Add(destTrackingDetail);
                        }
                        //check item exits
                        var storeItem = new StorageItemJP()
                        {
                            Amount = quantity * store.PriceTax,
                            PriceTax = store.PriceTax,
                            Quantity = store.Quantity,
                            CategoryName = store.CategoryName,
                            Component = store.Component,
                            ComponentImage = store.ComponentImage,
                            CreatedAt = DateTime.Now,
                            CreatedBy = user.Staff.UserName,
                            Id = Guid.NewGuid(),
                            Image = store.Image,
                            ImageBase64 = store.ImageBase64,
                            ImageBinary = store.ImageBinary,
                            ImageLinkWeb = store.ImageLinkWeb,
                            IsDeny = store.IsDeny,
                            IsExport = store.IsExport,
                            JanCode = store.JanCode,
                            LinkWeb = store.LinkWeb,
                            MadeIn = store.MadeIn,
                            Material = store.Material,
                            MaterialImage = store.MaterialImage,
                            NameEN = store.NameEN,
                            NameJP = store.NameJP,
                            Notes = store.Notes,
                            ProductCode = store.ProductCode,
                            StatusId = store.StatusId,
                            StoregeJPId = trackingParent.Id,
                            TrackingDetailId = destTrackingDetail.Id,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = user.Staff.UserName,
                            CategoryId = store.CategoryId,
                        };
                        db.StorageItemJPs.Add(storeItem);
                        //check quantity
                        
                        if (db.StorageItemJPs.Where(n => n.TrackingDetailId == store.TrackingDetailId).Count() == 0)
                        {
                            db.TrackingDetails.Remove(db.TrackingDetails.Find(trackingId));
                        }
                        db.StorageItemJPs.Remove(store);
                    }
                    else
                    {
                        var store = db.StorageItemJPs.Find(Guid.Parse(item));
                        var trackingParent = db.StorageJPs.Find(store.TrackingDetail.StoregeJPId);
                        var trackingId = store.TrackingDetailId;
                        //check tracking tranfer is exits
                        TrackingDetail destTrackingDetail = null;
                        if (trackingParent.TrackingDetails.Where(n => n.TrackingSubCode == trackingTranfer).Count() > 0)
                        {
                            destTrackingDetail = trackingParent.TrackingDetails.SingleOrDefault(n => n.TrackingSubCode == trackingTranfer);
                        }
                        else
                        {
                            destTrackingDetail = new TrackingDetail()
                            {
                                CreatedAt = DateTime.Now,
                                CreatedBy = user.Staff.UserName,
                                Id = Guid.NewGuid(),
                                StoregeJPId = trackingParent.Id,
                                TrackingSubCode = trackingTranfer,
                                Weigh = 0,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = user.Staff.UserName,
                            };
                            db.TrackingDetails.Add(destTrackingDetail);
                        }
                        //check item exits
                        if (destTrackingDetail.StorageItemJPs.Where(n => n.JanCode == store.JanCode).Count() > 0)
                        {
                            var storeDest = destTrackingDetail.StorageItemJPs.SingleOrDefault(n => n.JanCode == store.JanCode);
                            storeDest.Quantity = storeDest.Quantity + quantity;
                        }
                        else
                        {
                            var storeItem = new StorageItemJP()
                            {
                                Amount = quantity * store.PriceTax,
                                PriceTax = store.PriceTax,
                                Quantity = quantity,
                                CategoryName = store.CategoryName,
                                Component = store.Component,
                                ComponentImage = store.ComponentImage,
                                CreatedAt = DateTime.Now,
                                CreatedBy = user.Staff.UserName,
                                Id = Guid.NewGuid(),
                                Image = store.Image,
                                ImageBase64 = store.ImageBase64,
                                ImageBinary = store.ImageBinary,
                                ImageLinkWeb = store.ImageLinkWeb,
                                IsDeny = store.IsDeny,
                                IsExport = store.IsExport,
                                JanCode = store.JanCode,
                                LinkWeb = store.LinkWeb,
                                MadeIn = store.MadeIn,
                                Material = store.Material,
                                MaterialImage = store.MaterialImage,
                                NameEN = store.NameEN,
                                NameJP = store.NameJP,
                                Notes = store.Notes,
                                ProductCode = store.ProductCode,
                                StatusId = store.StatusId,
                                StoregeJPId = trackingParent.Id,
                                TrackingDetailId = destTrackingDetail.Id,
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = user.Staff.UserName,
                                CategoryId = store.CategoryId,
                            };
                            db.StorageItemJPs.Add(storeItem);
                        }
                        //check quantity
                        if (quantity >= store.Quantity)
                        {
                            var id = store.TrackingDetailId;db.StorageItemJPs.Remove(store);
                            if (db.StorageItemJPs.Where(n => n.TrackingDetailId == id).Count() == 0)
                            {
                                db.TrackingDetails.Remove(db.TrackingDetails.Find(trackingId));
                            }                        }
                        else
                        {
                            store.Quantity = store.Quantity - quantity;
                        }
                    }
                }
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult UpdateComponentMaterialJancode(string ids = "", string data = "108", string option = "3.1")
        {
            try
            {
                foreach (var item in ids.Split(','))
                {
                    var store = db.StorageItemJPs.Find(Guid.Parse(item));
                    switch (option)
                    {
                        case "2": store.Quantity = int.Parse(data); break;
                        case "3": store.PriceTax = double.Parse(data); break;
                        case "3.1": store.CategoryId = int.Parse(data); break;
                        case "5": store.Component = data; break;
                        case "6": store.Material = data; break;
                        case "7": store.JanCode = data; break;
                        case "8": store.NameJP = data; break;
                    }
                    store.Amount = store.PriceTax * store.Quantity;
                }
                db.SaveChanges();
                return Json(new { message = "Cập nhật dữ liệu thành công !", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Cập nhật dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult UpdateItemProduct(StorageItemJP model)
        {
            try
            {
                var item = db.StorageItemJPs.Find(model.Id);
                item.NameJP = model.NameJP;
                item.NameEN = model.NameEN;
                item.CategoryId = model.CategoryId;
                item.ImageBase64 = model.ImageBase64;
                item.ProductCode = model.ProductCode;
                item.JanCode = model.JanCode;
                item.Quantity = model.Quantity;
                item.PriceTax = model.PriceTax;
                item.Amount = model.Quantity * model.PriceTax;
                item.MadeIn = model.MadeIn;
                item.LinkWeb = model.LinkWeb;
                item.Material = model.Material;
                item.Component = model.Component;
                item.ComponentImage = model.ComponentImage;
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
    }
}
