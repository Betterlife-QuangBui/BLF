using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class AgencyPackageItemController : ManagementSystemController
    {
        // GET: AgencyPackageItem
        public ActionResult Index(Guid id)
        {
            var agencyPackageItems = db.AgencyPackageItems.Where(n=>n.AgencyPackageId==id).OrderByDescending(n=>n.CreatedAt);
            ViewBag.Package = db.AgencyPackages.Find(id);
            ViewBag.page = 1;
            var categories = db.WareHouseCategories.OrderBy(n=>n.NameEN);
            ViewBag.CategoryId = new SelectList(categories, "Id", "NameEN");
            ViewBag.id = id;
            
            return View(Pager<AgencyPackageItem>.CreatePagging(agencyPackageItems, 1,10));
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                db.AgencyPackageItems.Remove(db.AgencyPackageItems.Find(id));
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }
            
        }
        [HttpPost]
        public ActionResult DeleteMultiple(string ids)
        {
            try
            {
                foreach (var id in ids.Split(','))
                {
                    db.AgencyPackageItems.Remove(db.AgencyPackageItems.Find(Guid.Parse(id)));
                }
                db.SaveChanges();
                return Json(new { message = "Xóa dữ liệu thành công", status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "Xóa dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult Add(Guid id,string name,int category,int quantity,string note,string weblink,string jancode,string productId,double price)
        {
            var IdItem= Guid.NewGuid();
            AgencyPackageItem agencyPackageItem = new AgencyPackageItem()
            {
                AgencyPackageId = id,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Staff.UserName,
                Id = IdItem,
                CategoryId = category,
                ItemName = name,
                ItemCategory = db.WareHouseCategories.Find(category).Name,
                ItemNotes = note,
                ItemQuantity = quantity,
                UpdatedAt = DateTime.Now,
                UpdatedBy = user.Staff.UserName,
                ItemUrl = weblink,
                ItemCode = jancode,
                Price = price,
                ProductCode = productId
            };
            if (ModelState.IsValid)
            {
                db.AgencyPackageItems.Add(agencyPackageItem);
                db.SaveChanges();
                return Json(new { message = IdItem, status = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Thêm dữ liệu thất bại !", status = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(Guid id, string name, int category, int quantity, string note, string weblink, string jancode, string productId, double price)
        {
            try
            {
                var agencyPackageItem = db.AgencyPackageItems.Find(id);
                
                agencyPackageItem.CategoryId = category;
                agencyPackageItem.ItemCategory = db.WareHouseCategories.Find(category).Name;
                agencyPackageItem.ItemNotes = note;
                agencyPackageItem.ItemName = name;
                agencyPackageItem.ItemUrl = weblink;
                agencyPackageItem.ItemCode = jancode;
                agencyPackageItem.ItemQuantity = quantity;
                agencyPackageItem.ProductCode = productId;
                agencyPackageItem.Price = price;
                agencyPackageItem.UpdatedAt = DateTime.Now;
                agencyPackageItem.UpdatedBy = user.Staff.UserName;
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
    }
}
