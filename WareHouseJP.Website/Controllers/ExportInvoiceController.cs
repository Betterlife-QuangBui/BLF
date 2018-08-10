using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Controllers
{
    public class ExportInvoiceController : ManagementSystemController
    {
        UserPage user = new UserPage();
        // GET: ExportInvoice
        public ActionResult Index(Guid id)
        {
            var exportInvoices = db.ExportInvoices.Where(n => n.ExportId == id && n.AgencyId == user.Agency.Id).Include(e => e.Agency).Include(e => e.ExportGood).OrderByDescending(n => n.CreatedAt);
            ExportInvoice model = exportInvoices.FirstOrDefault() == null ? new ExportInvoice() { AgencyId = user.Agency.Id, ExportId = id, InvoiceDate = DateTime.Now, CreatedBy = user.Staff.UserName, CreatedAt = DateTime.Now, Id = Guid.NewGuid() } : exportInvoices.FirstOrDefault();
            
            ViewBag.MAWB = new SelectList(db.MAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Name");
            
            ViewBag.HAWB = new SelectList(db.HAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Name");

            ViewBag.DeliveryComId = new SelectList(db.DeliveryComs, "Id", "Name", model.DeliveryComId);
            return View(model);
        }

        // POST: ExportInvoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ExportInvoice exportInvoice, string[] MAWB,string [] HAWB)
        {
            if (MAWB == null) { MAWB = new string[] { }; }
            if (HAWB == null) { HAWB = new string[] { }; }
            exportInvoice.InvoiceNo = Request["InvoiceNo"];
            if (db.ExportInvoices.Where(n=>n.Id==exportInvoice.Id).Count()>0)
            {
                #region Update
                var item = exportInvoice;
                item.AgencyId = user.Agency.Id;
                item.ExportId = exportInvoice.ExportId;
                item.InvoiceDate = exportInvoice.InvoiceDate;
                item.InvoiceHour = exportInvoice.InvoiceHour;
                item.InvoiceNo = exportInvoice.InvoiceNo;
                item.Notes = exportInvoice.Notes;
                item.DeliveryComId = exportInvoice.DeliveryComId;
                item.StaffId = user.Staff.UserName;
                item.UpdatedAt = DateTime.Now;
                item.UpdatedBy = user.Staff.UserName;
                foreach (var ha in db.HAWBDetails.Where(n=>n.ExportInvoiceId==item.Id))
                {
                    db.HAWBDetails.Remove(ha);
                }
                foreach (var ma in db.MAWBDetails.Where(n => n.ExportInvoiceId == item.Id))
                {
                    db.MAWBDetails.Remove(ma);
                }

                foreach (var ha in HAWB)
                {
                    db.HAWBDetails.Add(new HAWBDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportInvoiceId = item.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        HAWBId = ha
                    });
                }
                foreach (var ma in MAWB)
                {
                    db.MAWBDetails.Add(new MAWBDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportInvoiceId = item.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        MAWBId = ma
                    });
                }
                db.SaveChanges();
                #endregion
            }
            else 
            {
                #region add
                var item = exportInvoice;
                item.AgencyId = user.Agency.Id;
                item.ExportId = exportInvoice.ExportId;
                item.InvoiceDate = exportInvoice.InvoiceDate;
                item.InvoiceHour = exportInvoice.InvoiceHour;
                item.InvoiceNo = exportInvoice.InvoiceNo;
                item.Notes = exportInvoice.Notes;
                item.StaffId = user.Staff.UserName;
                item.UpdatedAt = DateTime.Now;
                item.UpdatedBy = user.Staff.UserName;
                db.ExportInvoices.Add(item);
                foreach (var ha in item.HAWBDetails)
                {
                    db.HAWBDetails.Remove(ha);
                }
                foreach (var ma in item.MAWBDetails)
                {
                    db.MAWBDetails.Remove(ma);
                }

                foreach (var ha in HAWB)
                {
                    db.HAWBDetails.Add(new HAWBDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportInvoiceId = item.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        HAWBId = ha
                    });
                }
                foreach (var ma in MAWB)
                {
                    db.MAWBDetails.Add(new MAWBDetail()
                    {
                        CreatedAt = DateTime.Now,
                        CreatedBy = user.Staff.UserName,
                        ExportInvoiceId = item.Id,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = user.Staff.UserName,
                        Id = Guid.NewGuid(),
                        MAWBId = ma
                    });
                }
                db.SaveChanges();
                #endregion
            }

            var exportInvoices = db.ExportInvoices.Where(n => n.ExportId == exportInvoice.Id && n.AgencyId == user.Agency.Id).Include(e => e.Agency).Include(e => e.ExportGood).OrderByDescending(n => n.CreatedAt);
            
            ViewBag.MAWB = new SelectList(db.MAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Name");
            
            ViewBag.HAWB = new SelectList(db.HAWBs.Where(n => n.AgencyId == user.Agency.Id), "Id", "Name");
            ViewBag.DeliveryComId = new SelectList(db.DeliveryComs, "Id", "Name", exportInvoice.DeliveryComId);
            return Redirect("/ExportInvoice/Index/" + exportInvoice.ExportId);
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
