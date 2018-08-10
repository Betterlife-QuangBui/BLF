using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using static WareHouseJP.Website.Helpers.PaggerUtils;

namespace WareHouseJP.Website.Controllers
{
    public class AjaxController : ManagementSystemController
    {
        public ActionResult StoreJP(int page = 1, string key = "", int sort = 0)
        {
            ViewBag.key = key;
            ViewBag.page = page;
            ViewBag.sort = sort;
            var item = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            if (key != "")
            {
                item = item.Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
            }
            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }
            ViewBag.Count = item.Count();
            return PartialView("~/Views/StorageJP/_ItemOfPage.cshtml", Pager<StorageJP>.CreatePagging(item, page, 10));
        }
        public ActionResult AutocompleteStoreJP(string term = "")
        {
            try
            {
                var listjp = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(term)).OrderBy(n => n.TrackingCode);
                return Json(listjp.Select(n => n.TrackingCode), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Không có data", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StoreJPNew(int page = 1,string address="",string delivery="", string trackingcode = "", int spilt = -1, double weigh = -1, double size = -1, string date_recive = "", string hour = "", int status = 0, string notes = "", int error = -1, string data_sort = "")
        {
            ViewBag.key = trackingcode;
            ViewBag.page = page;
            ViewBag.sort = data_sort;

            var listjp = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            var exports = db.ExportGoodDetails.Where(n => n.ExportGood.AgencyId == user.Agency.Id);
            var returns = db.ReturnDetails.Where(n => n.PackageReturn.AgencyId == user.Agency.Id);
            listjp = listjp.Where(m => m.TrackingDetails.Count() ==0|| m.TrackingDetails.Count() != (exports.Where(n => n.TrackingDetail.StorageJP.Id == m.Id).Count() + returns.Where(n => n.TrackingDetail.StorageJP.Id == m.Id).Count()))
                .OrderByDescending(n => n.CreatedAt);

            var lstDetail = listjp;
            //lstDetail.AddRange(listjp.AsEnumerable());
            if (address != "")
            {
                lstDetail = lstDetail.Where(n => n.DeliveryAddress.Contains(address)).OrderByDescending(n => n.CreatedAt);
            }
            if (delivery != "")
            {
                var deli = int.Parse(delivery);
                lstDetail = lstDetail.Where(n => n.DeliveryId== deli).OrderByDescending(n => n.CreatedAt);
            }
            if (trackingcode != "")
            {
                lstDetail = lstDetail.Where(n => n.TrackingCode.Contains(trackingcode)).OrderByDescending(n => n.CreatedAt);
            }
            if (spilt != -1)
            {
                lstDetail = lstDetail.Where(n => n.TrackingDetails.Count() == spilt).OrderByDescending(n => n.CreatedAt);
            }
            if (weigh != -1)
            {
                lstDetail = lstDetail.Where(n => n.Weigh == weigh).OrderByDescending(n => n.CreatedAt);
            }
            if (size != -1)
            {
                lstDetail = lstDetail.Where(n => PageUtils.DisplaySizeIndex(n.Weigh, n.SizeInput, n.SizeTableId, n.Size) == size).OrderByDescending(n => n.CreatedAt);
            }
            if (date_recive != "")
            {
                DateTime date = DateTime.ParseExact(date_recive, "yyyy-MM-dd", null);
                lstDetail = lstDetail.Where(n => n.ReceivedDate == date).OrderByDescending(n => n.CreatedAt);
            }
            if (hour != "")
            {
                lstDetail = lstDetail.Where(n => n.ReceivedHour.Contains(hour)).OrderByDescending(n => n.CreatedAt);
            }
            if (status != 0)
            {
                lstDetail = lstDetail.Where(n => n.StatusId == status).OrderByDescending(n => n.CreatedAt);
            }
            if (notes != "")
            {
                lstDetail = lstDetail.Where(n => n.Notes.Contains(notes)).OrderByDescending(n => n.CreatedAt);
            }
            if (error != -1)
            {
                lstDetail = lstDetail.ToList().Where(n => PageUtils.CheckItemStoreJPCount(n) == error).AsQueryable().OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "delivery":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.DeliveryCom.Name);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.DeliveryCom.Name);
                            }
                            break;
                        }
                    //case "address":
                    //    {
                    //        lstDetail = lstDetail.OrderBy(n => n.TrackingCode);
                    //        if (sort[1] == "desc")
                    //        {
                    //            lstDetail = lstDetail.OrderByDescending(n => n.TrackingCode);
                    //        }
                    //        break;
                    //    }
                    case "tracking":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.TrackingCode);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.TrackingCode);
                            }
                            break;
                        }
                    case "trackingsub":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.TrackingDetails.Count());
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.TrackingDetails.Count());
                            }
                            break;
                        }
                    case "weigh":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Weigh);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Weigh);
                            }
                            break;
                        }
                    case "size":
                        {
                            lstDetail = lstDetail.OrderBy(n => PageUtils.DisplaySizeIndex(n.Weigh, n.SizeInput, n.SizeTableId, n.Size));
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => PageUtils.DisplaySizeIndex(n.Weigh, n.SizeInput, n.SizeTableId, n.Size));
                            }
                            break;
                        }
                    case "date":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ReceivedDate);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ReceivedDate);
                            }
                            break;
                        }
                    case "hour":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ReceivedHour);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ReceivedHour);
                            }
                            break;
                        }
                    case "status":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.StatusId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.StatusId);
                            }
                            break;
                        }
                    case "notes":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Notes);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Notes);
                            }
                            break;
                        }
                    case "errorcontent":
                        {
                            var item1s = lstDetail.ToList().OrderBy(n => PageUtils.CheckItemStoreJPCount(n));
                            if (sort[1] == "desc")
                            {
                                item1s = lstDetail.ToList().OrderByDescending(n => PageUtils.CheckItemStoreJPCount(n));
                            }
                            var lstReturns = Pager<StorageJP>.CreatePagging(item1s.AsQueryable(), page, 10);
                            return PartialView("~/Views/StorageJP/_ItemOfPage.cshtml", lstReturns);
                        }
                    default:
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<StorageJP>.CreatePagging(lstDetail, page, 10);
            return PartialView("~/Views/StorageJP/_ItemOfPage.cshtml", lstReturn);
        }

        public ActionResult ControlCrud(int page = 1, string key = "")
        {
            ViewBag.key = key;
            ViewBag.page = page;
            if (key != "")
            {
                var item = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/StorageJP/_ControlItemOfPage.cshtml", Pager<StorageJP>.CreatePagging(item, page));
            }
            else
            {
                var item = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/StorageJP/_ControlItemOfPage.cshtml", Pager<StorageJP>.CreatePagging(item, page));
            }
        }

        public ActionResult ExportGood(int page = 1, string key = "", int sort = 9)
        {
            ViewBag.key = key;
            ViewBag.page = page;
            var item = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.ShippingMarkVN.Contains(key)).OrderByDescending(n => n.CreatedAt);
            if (sort != 0)
            {
                item = item.Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt);
            }
            if (key != "")
            {
                item = item.Where(n => n.ShippingMarkVN.Contains(key)).OrderByDescending(n => n.CreatedAt);
            }
            return PartialView("~/Views/ExportGoods/_ItemOfPage.cshtml", Pager<ExportGood>.CreatePagging(item, page, 5));
        }
        public ActionResult ExportGoodsJP(int page = 1, string trackingcode = "", string spilt = "", int status = 0, string data_sort = "")
        {
            var listjp = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.StatusId == 8 && n.TrackingCode.Contains(trackingcode)).OrderByDescending(n => n.CreatedAt);
            var lstDetail = new List<TrackingDetail>();
            foreach (var item in listjp)
            {
                lstDetail.AddRange(item.TrackingDetails.Where(n => n.TrackingSubCode != "21").Where(n => n.TrackingSubCode.Contains(spilt)));
            }
            lstDetail = lstDetail.Where(n => PageUtils.IsExistExport(n.Id, user.Agency.Id) == false).ToList();
            if (status != 0)
            {
                lstDetail = lstDetail.Where(n => PageUtils.Status(n.StorageJP.Id).Value == status + "").ToList();
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.StorageJP.TrackingCode).ToList();
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.StorageJP.TrackingCode).ToList();
                            }
                            break;
                        }
                    case "split":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.TrackingSubCode).ToList();
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.TrackingSubCode).ToList();
                            }
                            break;
                        }
                    case "status":
                        {
                            lstDetail = lstDetail.OrderBy(n => PageUtils.Status(n.StorageJP.Id).Value).ToList();
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => PageUtils.Status(n.StorageJP.Id).Value).ToList();
                            }
                            break;
                        }
                    case "createdat":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt).ToList();
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt).ToList();
                            }
                            break;
                        }
                }
            }


            var lstReturn = Pager<TrackingDetail>.CreatePagging(lstDetail.AsQueryable(), page, 10);
            return PartialView("~/Views/ExportGoods/_DetailOfJP.cshtml", lstReturn);
        }
        public ActionResult ExportGoodsVN(Guid Id, int page = 1, string trackingcode = "", string spilt = "", string data_sort = "")
        {
            ExportGood exportGood = db.ExportGoods.Find(Id);
            var listvn = exportGood.ExportGoodDetails.OrderByDescending(n => n.CreatedAt);
            if (trackingcode != "")
            {
                listvn = listvn.Where(n => n.TrackingDetail.StorageJP.TrackingCode.Contains(trackingcode)).OrderByDescending(n => n.CreatedAt);
            }
            if (spilt != "")
            {
                listvn = listvn.Where(n => n.TrackingDetail.TrackingSubCode.Contains(spilt)).OrderByDescending(n => n.CreatedAt);
            }

            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            listvn = listvn.OrderBy(n => n.TrackingDetail.StorageJP.TrackingCode);
                            if (sort[1] == "desc")
                            {
                                listvn = listvn.OrderByDescending(n => n.TrackingDetail.StorageJP.TrackingCode);
                            }
                            break;
                        }
                    case "split":
                        {
                            listvn = listvn.OrderBy(n => n.TrackingDetail.TrackingSubCode);
                            if (sort[1] == "desc")
                            {
                                listvn = listvn.OrderByDescending(n => n.TrackingDetail.TrackingSubCode);
                            }
                            break;
                        }
                    case "createdat":
                        {
                            listvn = listvn.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                listvn = listvn.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<ExportGoodDetail>.CreatePagging(listvn.AsQueryable(), page, 10);
            return PartialView("~/Views/ExportGoods/_DetailOfVN.cshtml", lstReturn);
        }
        public ActionResult ExportGoodControl(int page = 1, string key = "")
        {
            ViewBag.key = key;
            ViewBag.page = page;
            if (key != "")
            {
                var item = db.ExportGoods.Where(n => n.ShippingMarkVN.Contains(key)).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/ExportGoods/_ControlItemOfPage.cshtml", Pager<ExportGood>.CreatePagging(item, page));
            }
            else
            {
                var item = db.ExportGoods.OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/ExportGoods/_ControlItemOfPage.cshtml", Pager<ExportGood>.CreatePagging(item, page));
            }
        }


        public ActionResult AgencyPackage(int page = 1, string key = "")
        {
            ViewBag.key = key;
            ViewBag.page = page;
            if (key != "")
            {
                var item = db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/AgencyPackage/_ItemOfPage.cshtml", Pager<AgencyPackage>.CreatePagging(item, page));
            }
            else
            {
                var item = db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/AgencyPackage/_ItemOfPage.cshtml", Pager<AgencyPackage>.CreatePagging(item, page));
            }
        }

        public ActionResult AgencyPackageControl(int page = 1, string key = "")
        {
            ViewBag.key = key;
            ViewBag.page = page;
            if (key != "")
            {
                var item = db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.TrackingCode.Contains(key)).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/AgencyPackage/_ControlItemOfPage.cshtml", Pager<AgencyPackage>.CreatePagging(item, page));
            }
            else
            {
                var item = db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/AgencyPackage/_ControlItemOfPage.cshtml", Pager<AgencyPackage>.CreatePagging(item, page));
            }
        }

        public ActionResult Shipment(int page = 1, string trackingcode = "", int spilt = -1, double weigh = -1, string date_recive = "", int status = 0, string notes = "", string data_sort = "")
        {
            ViewBag.key = trackingcode;
            ViewBag.page = page;
            ViewBag.sort = data_sort;

            var listjp = db.Shipments.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            var lstDetail = listjp;
            if (trackingcode != "")
            {
                lstDetail = lstDetail.Where(n => n.ShipmentName.Contains(trackingcode)).OrderByDescending(n => n.CreatedAt);
            }
            if (weigh != -1)
            {
                if (weigh == 0)
                {
                    lstDetail = lstDetail.Where(n => n.AgencyPackages.Where(m => m.Weigh != null).Sum(m => m.Weigh) == weigh|| n.AgencyPackages.Count()== weigh).OrderByDescending(n => n.CreatedAt);
                }
                else
                {
                    lstDetail = lstDetail.Where(n => n.AgencyPackages.Where(m => m.Weigh != null).Sum(m => m.Weigh) == weigh).OrderByDescending(n => n.CreatedAt);
                }
            }
            if (spilt != -1)
            {
                lstDetail = lstDetail.Where(n => n.AgencyPackages.Count() == spilt).OrderByDescending(n => n.CreatedAt);
            }
            if (date_recive != "")
            {
                DateTime date = DateTime.ParseExact(date_recive, "yyyy-MM-dd", null);
                lstDetail = lstDetail.Where(n => n.FlightDate == date).OrderByDescending(n => n.CreatedAt);
            }
            if (status != 0)
            {
                lstDetail = lstDetail.Where(n => n.StatusId == status).OrderByDescending(n => n.CreatedAt);
            }
            if (notes != "")
            {
                lstDetail = lstDetail.Where(n => n.Notes.Contains(notes)).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ShipmentName);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ShipmentName);
                            }
                            break;
                        }
                    case "trackingsub":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.AgencyPackages.Count());
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.AgencyPackages.Count());
                            }
                            break;
                        }
                    case "weigh":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.AgencyPackages.Where(m => m.Weigh != null).Sum(m => m.Weigh));
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.AgencyPackages.Where(m => m.Weigh != null).Sum(m => m.Weigh));
                            }
                            break;
                        }
                    case "date":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.FlightDate);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.FlightDate);
                            }
                            break;
                        }
                    case "status":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.StatusId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.StatusId);
                            }
                            break;
                        }
                    case "notes":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Notes);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Notes);
                            }
                            break;
                        }
                    default:
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<Shipment>.CreatePagging(lstDetail, page, 10);
            return PartialView("~/Views/Shipment/_ItemOfPage.cshtml", lstReturn);
        }
        public ActionResult Package(Guid id, int page = 1, string trackingcode = "", int delivery = 0, double weigh = -1, string date_send = "", string hour_send = "", string date_recive = "", string hour_recive = "", int status = 0, string notes = "", string data_sort = "")
        {
            ViewBag.key = trackingcode;
            ViewBag.page = page;
            ViewBag.sort = data_sort;
            ViewBag.ShipmentId = id;
            var listjp = db.AgencyPackages.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.ShipmentId == id).OrderByDescending(n => n.CreatedAt);
            var lstDetail = listjp;
            if (trackingcode != "")
            {
                lstDetail = lstDetail.Where(n => n.TrackingCode.Contains(trackingcode)).OrderByDescending(n => n.CreatedAt);
            }
            if (delivery != 0)
            {
                lstDetail = lstDetail.Where(n => n.DeliveryId == delivery).OrderByDescending(n => n.CreatedAt);
            }
            if (weigh != -1)
            {
                lstDetail = lstDetail.Where(n => n.Weigh == weigh).OrderByDescending(n => n.CreatedAt);
            }
            if (date_send != "")
            {
                DateTime date = DateTime.ParseExact(date_send, "yyyy-MM-dd", null);
                lstDetail = lstDetail.Where(n => n.SentDate == date).OrderByDescending(n => n.CreatedAt);
            }
            if (hour_send != "")
            {
                lstDetail = lstDetail.Where(n => n.SendHour.Contains(hour_send)).OrderByDescending(n => n.CreatedAt);
            }
            if (date_recive != "")
            {
                DateTime date = DateTime.ParseExact(date_recive, "yyyy-MM-dd", null);
                lstDetail = lstDetail.Where(n => n.ReceivedDate == date).OrderByDescending(n => n.CreatedAt);
            }
            if (hour_recive != "")
            {
                lstDetail = lstDetail.Where(n => n.ReceivedHour.Contains(hour_recive)).OrderByDescending(n => n.CreatedAt);
            }
            if (status != 0)
            {
                lstDetail = lstDetail.Where(n => n.StatusId == status).OrderByDescending(n => n.CreatedAt);
            }
            if (notes != "")
            {
                lstDetail = lstDetail.Where(n => n.Notes.Contains(notes)).OrderByDescending(n => n.CreatedAt);
            }
            ViewBag.TotalCount = lstDetail.Count();
            ViewBag.TotalWeigh = lstDetail.Where(n => n.Weigh != null).Sum(n => n.Weigh);
           
            try
            {
                ViewBag.TotalItems = lstDetail.Sum(m => m.AgencyPackageItems.Count());
            }
            catch { ViewBag.TotalItems = 0; }
            try
            {
                ViewBag.TotalItemQuantitys = lstDetail.Sum(m => m.AgencyPackageItems.Sum(n => n.ItemQuantity));
            }
            catch { ViewBag.TotalItemQuantitys = 0; }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.TrackingCode);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.TrackingCode);
                            }
                            break;
                        }
                    case "delivery":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.DeliveryId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.DeliveryId);
                            }
                            break;
                        }
                    case "weigh":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Weigh);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Weigh);
                            }
                            break;
                        }
                    case "senddate":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.SentDate);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.SentDate);
                            }
                            break;
                        }
                    case "sendhour":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.SendHour);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.SendHour);
                            }
                            break;
                        }
                    case "recivedate":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ReceivedDate);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ReceivedDate);
                            }
                            break;
                        }
                    case "recivehour":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ReceivedHour);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ReceivedHour);
                            }
                            break;
                        }
                    case "status":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.StatusId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.StatusId);
                            }
                            break;
                        }
                    case "notes":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Notes);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Notes);
                            }
                            break;
                        }
                    default:
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<AgencyPackage>.CreatePagging(lstDetail, page, 10);
            return PartialView("~/Views/Shipment/Package-Ajax.cshtml", lstReturn);
        }

        public ActionResult ShipmentControl(int page = 1, int sort = -1)
        {
            ViewBag.sort = sort;
            ViewBag.page = page;
            var item = Pager<Shipment>.CreatePagging(db.Shipments.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.StatusId == sort).OrderByDescending(n => n.CreatedAt), page, 5);
            ViewBag.Count = item.Count();
            return PartialView("~/Views/Shipment/_ItemOfPage.cshtml", item);
        }

        public ActionResult FlightBooking(int page = 1, string key = "")
        {
            ViewBag.key = key;
            ViewBag.page = page;
            if (key != "")
            {
                var item = db.FlightBookings.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.Code.Contains(key)).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/FlightBooking/_ItemOfPage.cshtml", Pager<FlightBooking>.CreatePagging(item, page));
            }
            else
            {
                var item = db.FlightBookings.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/FlightBooking/_ItemOfPage.cshtml", Pager<FlightBooking>.CreatePagging(item, page));
            }
        }

        public ActionResult FlightBookingControl(int page = 1, string key = "")
        {
            ViewBag.key = key;
            ViewBag.page = page;
            if (key != "")
            {
                var item = db.FlightBookings.Where(n => n.AgencyId == user.Agency.Id).Where(n => n.Code.Contains(key)).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/FlightBooking/_ControlItemOfPage.cshtml", Pager<FlightBooking>.CreatePagging(item, page));
            }
            else
            {
                var item = db.FlightBookings.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
                ViewBag.Count = item.Count();
                return PartialView("~/Views/FlightBooking/_ControlItemOfPage.cshtml", Pager<FlightBooking>.CreatePagging(item, page));
            }
        }

        public ActionResult DataBaseStorage(int page = 1, string namejp = "", string nameen = "", int category = 0, string categorywebname = "",
            string jancode = "", string productcode = "", double quantity = 0, double price = 0, double amount = 0, string origin = "",
            string material = "", string component = "", string linkweb = "", string flightcode = "", string tracking = "", int producttype = 0,
            string data_sort = "")
        {
            ViewBag.key = namejp;
            ViewBag.page = page;
            ViewBag.sort = data_sort;

            var listjp = db.WareHouseItems.OrderBy(n => n.CreatedAt); // Error out of memory
            var lstDetail = listjp;
            #region filter
            if (namejp != "")
            {
                lstDetail = lstDetail.Where(n => n.NameJP.Contains(namejp)).OrderBy(n => n.CreatedAt);
            }
            if (nameen != "")
            {
                lstDetail = lstDetail.Where(n => n.NameEN.Contains(nameen)).OrderBy(n => n.CreatedAt);
            }
            if (category != 0)
            {
                lstDetail = lstDetail.Where(n => n.CategoryId == category).OrderBy(n => n.CreatedAt);
            }
            if (categorywebname != "")
            {
                lstDetail = lstDetail.Where(n => n.CategoryWebName.Contains(categorywebname)).OrderBy(n => n.CreatedAt);
            }
            if (jancode != "")
            {
                lstDetail = lstDetail.Where(n => n.JanCode.Contains(jancode)).OrderBy(n => n.CreatedAt);
            }
            if (productcode != "")
            {
                lstDetail = lstDetail.Where(n => n.ProductCode.Contains(productcode)).OrderBy(n => n.CreatedAt);
            }
            if (quantity != 0)
            {
                lstDetail = lstDetail.Where(n => n.Quantity == quantity).OrderBy(n => n.CreatedAt);
            }
            if (price != 0)
            {
                lstDetail = lstDetail.Where(n => n.PriceTax == price).OrderBy(n => n.CreatedAt);
            }
            if (amount != 0)
            {
                lstDetail = lstDetail.Where(n => n.Amount == amount).OrderBy(n => n.CreatedAt);
            }
            if (origin != "")
            {
                lstDetail = lstDetail.Where(n => n.MadeIn.Contains(origin)).OrderBy(n => n.CreatedAt);
            }
            if (material != "")
            {
                lstDetail = lstDetail.Where(n => n.Material.Contains(material)).OrderBy(n => n.CreatedAt);
            }
            if (component != "")
            {
                lstDetail = lstDetail.Where(n => n.Component.Contains(component)).OrderBy(n => n.CreatedAt);
            }
            if (flightcode != "")
            {
                lstDetail = lstDetail.Where(n => n.FlightCode.Contains(flightcode)).OrderBy(n => n.CreatedAt);
            }
            if (tracking != "")
            {
                lstDetail = lstDetail.Where(n => n.TrackingCode.Contains(tracking)).OrderBy(n => n.CreatedAt);
            }
            if (producttype != 0)
            {
                lstDetail = lstDetail.Where(n => n.ProductTypeId == producttype).OrderBy(n => n.CreatedAt);
            }
            #endregion
            if (data_sort != "")
            {
                #region sort
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "namejp":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.NameJP);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.NameJP);
                            }
                            break;
                        }
                    case "nameen":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.NameEN);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.NameEN);
                            }
                            break;
                        }
                    case "category":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CategoryId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CategoryId);
                            }
                            break;
                        }
                    case "categoryweb":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CategoryWebName);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CategoryWebName);
                            }
                            break;
                        }
                    case "jancode":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.JanCode);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.JanCode);
                            }
                            break;
                        }
                    case "productcode":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ProductCode);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ProductCode);
                            }
                            break;
                        }
                    case "quantity":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Quantity);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Quantity);
                            }
                            break;
                        }
                    case "price":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.PriceTax);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.PriceTax);
                            }
                            break;
                        }
                    case "amount":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Amount);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Amount);
                            }
                            break;
                        }
                    case "origin":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.MadeIn);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.MadeIn);
                            }
                            break;
                        }
                    case "material":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Material);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Material);
                            }
                            break;
                        }
                    case "component":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Component);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Component);
                            }
                            break;
                        }
                    case "linkweb":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.LinkWeb);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.LinkWeb);
                            }
                            break;
                        }
                    case "flightcode":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.FlightCode);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.FlightCode);
                            }
                            break;
                        }
                    case "tracking":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.TrackingCode);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.TrackingCode);
                            }
                            break;
                        }
                    case "producttype":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ProductTypeId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ProductTypeId);
                            }
                            break;
                        }
                    default:
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
                #endregion
            }

            var lstReturn = Pager<WareHouseItem>.CreatePagging(lstDetail.AsQueryable(), page, 20);
            return PartialView("~/Views/StorageJP/_SearchDBOfJP.cshtml", lstReturn);
        }

        public ActionResult DBStorage(int pageno = 1, int pageSize = 15, int totalCount = 0, string data_sort = "", string namejp = null, string nameen = null,
            int category = 0, string categorywebname = null, string jancode = null, string productcode = null, double quantity = 0, double price = 0,
            double amount = 0, string origin = null, string flightcode = null, string tracking = null, int producttype = 0)
        {
            var lstCateSearch = db.WareHouseCategories.Select(n => new SelectListItem() { Text = n.NameEN, Value = n.Id + "" }).ToList();
            lstCateSearch.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            var lstMadeInSearch = db.Countries.Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            lstMadeInSearch.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            var lsCategoryId = db.WareHouseCategories.Select(n => new SelectListItem() { Text = n.NameEN, Value = n.Id + "" }).ToList();
            //lsCategoryId.Insert(0, new SelectListItem() { Value = "0", Text = "-- Chọn --" });
            var lsMadeIn = db.Countries.Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            //lsMadeIn.Insert(0, new SelectListItem() { Value = "0", Text = "-- Chọn --" });

            var lstTypeSearch = db.ProductTypes.Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            lstTypeSearch.Insert(0, new SelectListItem() { Value = "0", Text = "Tất cả" });
            var lsProductType = db.ProductTypes.Select(n => new SelectListItem() { Text = n.Name, Value = n.Id + "" }).ToList();
            //lsProductType.Insert(0, new SelectListItem() { Value = "0", Text = "-- Chọn --" });
            ViewBag.TypeSearch = new SelectList(lstTypeSearch, "Value", "Text");
            ViewBag.ProductTypeId = new SelectList(lsProductType, "Value", "Text");

            ViewBag.CategoryId = new SelectList(lsCategoryId, "Value", "Text");
            ViewBag.SearchCategoryId = new SelectList(lstCateSearch, "Value", "Text");
            ViewBag.SearchMadeIn = new SelectList(lstMadeInSearch, "Value", "Text");
            ViewBag.MadeIn = new SelectList(lsMadeIn, "Value", "Text", 108);

            //pageno = page;
            string sortcolumn = "", orderby = "";
            ViewBag.key = namejp;
            ViewBag.page = pageno;
            ViewBag.sort = data_sort;
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                sortcolumn = sort[0].ToString(); orderby = sort[1].ToString();
            }

            var spOutput = new SqlParameter
            {
                ParameterName = "TotalRows",
                SqlDbType = System.Data.SqlDbType.BigInt,
                Direction = System.Data.ParameterDirection.Output
            };
            var spPageNo = new SqlParameter
            {
                ParameterName = "PageNo",
                Value = pageno,
                SqlDbType = System.Data.SqlDbType.BigInt,
                Direction = System.Data.ParameterDirection.Input
            };
            var spPageSize = new SqlParameter
            {
                ParameterName = "PageSize",
                Value = pageSize,
                SqlDbType = System.Data.SqlDbType.BigInt,
                Direction = System.Data.ParameterDirection.Input
            };
            var spSort = new SqlParameter
            {
                ParameterName = "SortColumn",
                Value = sortcolumn,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spOrder = new SqlParameter
            {
                ParameterName = "SortOrder",
                Value = orderby,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spNameJP = new SqlParameter
            {
                ParameterName = "NameEN",
                Value = namejp,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spNameEN = new SqlParameter
            {
                ParameterName = "NameJP",
                Value = nameen,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spCategoryId = new SqlParameter
            {
                ParameterName = "CategoryId",
                Value = category,
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input
            };
            var spCategoryWeb = new SqlParameter
            {
                ParameterName = "CategoryWeb",
                Value = categorywebname,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spJanCode = new SqlParameter
            {
                ParameterName = "JanCode",
                Value = jancode,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spProductCode = new SqlParameter
            {
                ParameterName = "ProductCode",
                Value = productcode,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spQuantity = new SqlParameter
            {
                ParameterName = "Quantity",
                Value = quantity,
                SqlDbType = System.Data.SqlDbType.Float,
                Direction = System.Data.ParameterDirection.Input
            };
            var spPrice = new SqlParameter
            {
                ParameterName = "Price",
                Value = price,
                SqlDbType = System.Data.SqlDbType.Float,
                Direction = System.Data.ParameterDirection.Input
            };
            var spAmount = new SqlParameter
            {
                ParameterName = "Amount",
                Value = amount,
                SqlDbType = System.Data.SqlDbType.Float,
                Direction = System.Data.ParameterDirection.Input
            };
            var spOrigin = new SqlParameter
            {
                ParameterName = "Origin",
                Value = origin,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spFlightCode = new SqlParameter
            {
                ParameterName = "FlightCode",
                Value = flightcode,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spTracking = new SqlParameter
            {
                ParameterName = "Tracking",
                Value = tracking,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Input
            };
            var spProductTypeId = new SqlParameter
            {
                ParameterName = "ProductTypeId",
                Value = producttype,
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Input
            };

            var data = db.Database.SqlQuery<WareHouseItem>(@"exec [dbo].[USP_SEL_Contacts] @TotalRows Out, @PageNo, @PageSize, @SortColumn,
                @SortOrder, @NameJP, @NameEN, @CategoryId, @CategoryWeb, @JanCode, @ProductCode, @Quantity, @Price, @Amount, @Origin, @FlightCode,
                @Tracking, @ProductTypeId",
                spOutput, spPageNo, spPageSize, spSort, spOrder, spNameJP, spNameEN, spCategoryId, spCategoryWeb, spJanCode, spProductCode,
                spQuantity, spPrice, spAmount, spOrigin, spFlightCode, spTracking, spProductTypeId).ToList();
            if (spOutput.Value != null)
            {
                totalCount = int.Parse(spOutput.Value.ToString());
            }
            //PagerCus<WareHouseItem> pager = new PagerCus<WareHouseItem>(data.AsQueryable(), pageno, pageSize, totalCount);
            ViewBag.Pagging = Pager<int>.CreatePagging(totalCount, pageno, 15);
            ViewBag.TotalCount = totalCount;
            return PartialView("~/Views/StorageJP/_SearchDBOfJP.cshtml", Pager<WareHouseItem>.CreatePagging(data.AsQueryable(), 1, 15));
        }

        public ActionResult Shipping(int page = 1, string shippingCode = "", int shipper = 0, string mawb = "", string dateFlight = "", string from = "", string to = "", int status = 0, string notes = "", string data_sort = "")
        {
            ViewBag.page = page;
            ViewBag.sort = data_sort;
            var item = db.Shippings.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            if (shippingCode != "")
            {
                item = item.Where(n => n.ShippingCode.Contains(shippingCode)).OrderByDescending(n => n.CreatedAt);
            }
            if (shipper != 0)
            {
                item = item.Where(n => n.WareHouseInfoId == shipper).OrderByDescending(n => n.CreatedAt);
            }
            if (mawb != "")
            {
                item = item.Where(n => n.MaWBId == mawb).OrderByDescending(n => n.CreatedAt);
            }
            if (dateFlight != "")
            {
                DateTime date = DateTime.ParseExact(dateFlight, "yyyy-MM-dd", null);
                item = item.Where(n => n.DateAir == date).OrderByDescending(n => n.CreatedAt);
            }
            if (from != "")
            {
                Guid fromId = Guid.Parse(from);
                item = item.Where(n => n.FromAirId == fromId).OrderByDescending(n => n.CreatedAt);
            }
            if (to != "")
            {
                Guid toId = Guid.Parse(to);
                item = item.Where(n => n.ToAirId == toId).OrderByDescending(n => n.CreatedAt);
            }
            if (status != 0)
            {
                item = item.Where(n => n.StatusId == status).OrderByDescending(n => n.CreatedAt);
            }
            if (notes != "")
            {
                item = item.Where(n => n.Notes.Contains(notes)).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "shipping":
                        {
                            item = item.OrderBy(n => n.ShippingCode);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ShippingCode);
                            }
                            break;
                        }
                    case "shipper":
                        {
                            item = item.OrderBy(n => n.WareHouseInfo.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.WareHouseInfo.Name);
                            }
                            break;
                        }
                    case "mawb":
                        {
                            item = item.OrderBy(n => n.MAWB.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.MAWB.Name);
                            }
                            break;
                        }
                    case "flightdate":
                        {
                            item = item.OrderBy(n => n.DateAir);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.DateAir);
                            }
                            break;
                        }
                    case "from":
                        {
                            item = item.OrderBy(n => n.FromAir.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.FromAir.Name);
                            }
                            break;
                        }
                    case "to":
                        {
                            item = item.OrderBy(n => n.ToAir.Name);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ToAir.Name);
                            }
                            break;
                        }
                    case "notes":
                        {
                            item = item.OrderBy(n => n.Notes);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Notes);
                            }
                            break;
                        }
                    case "status":
                        {
                            item = item.OrderBy(n => n.StatusId);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.StatusId);
                            }
                            break;
                        }
                    case "pread":
                        {
                            item = item.OrderBy(n => n.PreAD_Note);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.PreAD_Note);
                            }
                            break;
                        }
                    default:
                        {
                            item = item.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            return PartialView("~/Views/Shipping/_ItemOfPage.cshtml", Pager<Shipping>.CreatePagging(item, page, 10));
        }
        public ActionResult BookingHistory(Guid Id)
        {
            Shipping model = db.Shippings.Find(Id);
            //lay danh sach lich su booking
            var lstBookingHistory = model.ShippingBookings.OrderByDescending(n => n.CreatedAt);
            ViewBag.BookingHistory = lstBookingHistory;
            return PartialView("~/Views/Shipping/_ItemOfBooking.cshtml");
        }


        public ActionResult ShippingExportGoodsVN(int page = 1, string shippingmask = "", double weigh = 0, string data_sort = "")
        {
            var listjp = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id && n.StatusId == 10).OrderByDescending(n => n.CreatedAt);
            listjp = listjp.Where(n => db.ShippingHAWBDetails.Where(m => m.AgencyId == user.Agency.Id).Where(m => m.ExportGoodId == n.Id).Count() == 0).OrderByDescending(n => n.CreatedAt);
            var lstDetail = listjp;
            if (shippingmask != "")
            {
                lstDetail = lstDetail.Where(n => n.ShippingMarkVN.Contains(shippingmask)).OrderByDescending(n => n.CreatedAt);
            }
            if (weigh != 0)
            {
                lstDetail = lstDetail.Where(n => n.Weigh == weigh).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "shipping":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ShippingMarkVN);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ShippingMarkVN);
                            }
                            break;
                        }
                    case "weigh":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.Weigh);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.Weigh);
                            }
                            break;
                        }
                    case "createdat":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<ExportGood>.CreatePagging(lstDetail, page, 10);
            return PartialView("~/Views/Shipping/_DetailOfVN.cshtml", lstReturn);
        }
        public ActionResult ShippingHAWB(Guid Id, int page = 1, string shippingmask = "", string hawb = "", string data_sort = "")
        {
            Shipping model = db.Shippings.Find(Id);
            var listHAWB = db.HAWBs.Where(n => n.AgencyId == user.Agency.Id && n.IsActive == true).OrderBy(n => n.CreatedAt);
            ViewBag.HAWB = new SelectList(listHAWB, "Id", "Name", "");
            var listjp = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id && n.ShippingHAWB.ShippingId == model.Id).OrderByDescending(n => n.CreatedAt);
            var lstDetail = listjp;
            if (shippingmask != "")
            {
                lstDetail = lstDetail.Where(n => n.ExportGood.ShippingMarkVN.Contains(shippingmask)).OrderByDescending(n => n.CreatedAt);
            }
            if (hawb != "")
            {
                lstDetail = lstDetail.Where(n => n.ShippingHAWB.HAWBId.Contains(hawb)).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "shipping":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ExportGood.ShippingMarkVN);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ExportGood.ShippingMarkVN);
                            }
                            break;
                        }
                    case "hawb":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.ShippingHAWB.HAWBId);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.ShippingHAWB.HAWBId);
                            }
                            break;
                        }
                    case "createdat":
                        {
                            lstDetail = lstDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                lstDetail = lstDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<ShippingHAWBDetail>.CreatePagging(lstDetail, page, 10);
            return PartialView("~/Views/Shipping/_DetailOfShipping.cshtml", lstReturn);
        }

        public ActionResult ExportGoodIndexNews(int page = 1, string shippingMask = "", double weigh = 0, double size = 0, string exportDate = "", string exportHour = "", int status = 0, string notes = "", string data_sort = "")
        {
            ViewBag.page = page;
            var item = db.ExportGoods.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            var exportShipping = db.ShippingHAWBDetails.Where(n => n.AgencyId == user.Agency.Id);
            item = item.Where(m => exportShipping.Where(n => n.ExportGoodId == m.Id).Count() == 0)
                .OrderByDescending(n => n.CreatedAt);
            if (shippingMask != "")
            {
                item = item.Where(n => n.ShippingMarkVN.Contains(shippingMask)).OrderByDescending(n => n.CreatedAt);
            }
            if (weigh != 0)
            {
                item = item.Where(n => n.Weigh == weigh).OrderByDescending(n => n.CreatedAt);
            }
            if (size != 0)
            {
                item = item.Where(n => PageUtils.DisplaySizeIndex(n.Weigh, n.SizeInput, n.SizeTableId, n.Size) == size).OrderByDescending(n => n.CreatedAt);
            }
            if (exportDate != "")
            {
                DateTime date = DateTime.ParseExact(exportDate, "yyyy-MM-dd", null);
                item = item.Where(n => n.ExportDate == date).OrderByDescending(n => n.CreatedAt);
            }
            if (exportHour != "")
            {
                item = item.Where(n => n.ExportHour == exportHour).OrderByDescending(n => n.CreatedAt);
            }
            if (status != 0)
            {
                item = item.Where(n => n.StatusId == status).OrderByDescending(n => n.CreatedAt);
            }
            if (notes != "")
            {
                item = item.Where(n => n.Notes.Contains(notes)).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            item = item.OrderBy(n => n.ShippingMarkVN);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ShippingMarkVN);
                            }
                            break;
                        }
                    case "weigh":
                        {
                            item = item.OrderBy(n => n.Weigh);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Weigh);
                            }
                            break;
                        }
                    case "size":
                        {
                            item = item.OrderBy(n => PageUtils.DisplaySizeIndex(n.Weigh, n.SizeInput, n.SizeTableId, n.Size));
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => PageUtils.DisplaySizeIndex(n.Weigh, n.SizeInput, n.SizeTableId, n.Size));
                            }
                            break;
                        }
                    case "date":
                        {
                            item = item.OrderBy(n => n.ExportDate);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ExportDate);
                            }
                            break;
                        }
                    case "hour":
                        {
                            item = item.OrderBy(n => n.ExportHour);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ExportHour);
                            }
                            break;
                        }
                    
                    case "notes":
                        {
                            item = item.OrderBy(n => n.Notes);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Notes);
                            }
                            break;
                        }
                    case "status":
                        {
                            item = item.OrderBy(n => n.StatusId);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.StatusId);
                            }
                            break;
                        }
                    default:
                        {
                            item = item.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            return PartialView("~/Views/ExportGoods/_ItemOfPage.cshtml", Pager<ExportGood>.CreatePagging(item, page, 10));
        }

        public ActionResult AgencyPackageItem(Guid id,int page = 1)
        {
            var agencyPackageItems = db.AgencyPackageItems.Where(n => n.AgencyPackageId == id).OrderByDescending(n => n.CreatedAt);
            ViewBag.Package = db.AgencyPackages.Find(id);
            ViewBag.page = page;
            ViewBag.id = id;
            ViewBag.CategoryId = new SelectList(db.WareHouseCategories.OrderBy(n=>n.NameEN), "Id", "NameEN");
            return PartialView("~/Views/AgencyPackageItem/_ItemOfPage.cshtml", Pager<AgencyPackageItem>.CreatePagging(agencyPackageItems, page, 10));
        }

        public ActionResult PackageReturn(int page = 1, string trackingcode = "",string reciver="", string date = "", string hour = "", int status = 0, string notes = "", string data_sort = "")
        {
            ViewBag.page = page;
            ViewBag.sort = data_sort;
            var item = db.PackageReturns.Where(n => n.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            if (status != 0)
            {
                item = item.Where(n => n.StatusId == status).OrderByDescending(n => n.CreatedAt);
            }
            if (trackingcode != "")
            {
                item = item.Where(n => n.ReturnCode.Contains(trackingcode)).OrderByDescending(n => n.CreatedAt);
            }
            if (reciver != "")
            {
                item = item.Where(n => n.ReciveName.Contains(reciver)).OrderByDescending(n => n.CreatedAt);
            }
            if (date != "")
            {
                DateTime date1 = DateTime.ParseExact(date, "yyyy-MM-dd", null);
                item = item.Where(n => n.ReturnDate == date1).OrderByDescending(n => n.CreatedAt);
            }
            if (hour != "")
            {
                item = item.Where(n => n.ReturnHour.Contains(hour)).OrderByDescending(n => n.CreatedAt);
            }
            if (notes != "")
            {
                item = item.Where(n => n.Notes.Contains(notes)).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            item = item.OrderBy(n => n.ReturnCode);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ReturnCode);
                            }
                            break;
                        }
                    case "reciver":
                        {
                            item = item.OrderBy(n => n.ReciveName);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ReciveName);
                            }
                            break;
                        }
                    case "date":
                        {
                            item = item.OrderBy(n => n.ReturnDate);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ReturnDate);
                            }
                            break;
                        }
                    case "hour":
                        {
                            item = item.OrderBy(n => n.ReturnHour);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.ReturnHour);
                            }
                            break;
                        }
                    case "status":
                        {
                            item = item.OrderBy(n => n.StatusId);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.StatusId);
                            }
                            break;
                        }
                    case "notes":
                        {
                            item = item.OrderBy(n => n.Notes);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.Notes);
                            }
                            break;
                        }
                    default:
                        {
                            item = item.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                item = item.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<PackageReturn>.CreatePagging(item, page, 10);
            return PartialView("~/Views/PackageReturn/_ItemOfPage.cshtml", lstReturn);
        }


        public ActionResult PackageReturnJP(int page = 1, string tracking = "", string split="", string data_sort = "")
        {
            var ListJP = db.TrackingDetails.Where(n => n.StorageJP.AgencyId == user.Agency.Id && n.TrackingSubCode == "21").OrderByDescending(n => n.CreatedAt);
            ListJP = ListJP.Where(n => db.ReturnDetails.Where(m => m.PackageReturn.AgencyId == user.Agency.Id).Where(m => m.TrackingDetailId == n.Id).Count() == 0).OrderByDescending(n => n.CreatedAt);
            

            if (tracking != "")
            {
                tracking = tracking.Trim();
                ListJP = ListJP.Where(n => n.StorageJP.TrackingCode.Contains(tracking)).OrderByDescending(n => n.CreatedAt);
            }
            if (split != "")
            {
                split = split.Trim();
                if (split == "TH" || split == "th") { split = "21"; }
                ListJP = ListJP.Where(n => n.TrackingSubCode==split).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            ListJP = ListJP.OrderBy(n => n.StorageJP.TrackingCode);
                            if (sort[1] == "desc")
                            {
                                ListJP = ListJP.OrderByDescending(n => n.StorageJP.TrackingCode);
                            }
                            break;
                        }
                    case "split":
                        {
                            ListJP = ListJP.OrderBy(n => n.TrackingSubCode);
                            if (sort[1] == "desc")
                            {
                                ListJP = ListJP.OrderByDescending(n => n.TrackingSubCode);
                            }
                            break;
                        }
                    case "createdat":
                        {
                            ListJP = ListJP.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                ListJP = ListJP.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<TrackingDetail>.CreatePagging(ListJP, page, 10);
            return PartialView("~/Views/PackageReturn/_DetailOfJP.cshtml", lstReturn);
        }

        public ActionResult PackageReturnDetail(Guid Id,int page = 1, string tracking = "", string split = "", string data_sort = "")
        {
            PackageReturn model = db.PackageReturns.Find(Id); ViewBag.Model = model;
            var ReturnDetail = db.ReturnDetails.Where(n => n.PackageReturnId == Id).Where(n => n.PackageReturn.AgencyId == user.Agency.Id).OrderByDescending(n => n.CreatedAt);
            ViewBag.Id = Id;
            if (tracking != "")
            {
                tracking = tracking.Trim();
                ReturnDetail = ReturnDetail.Where(n => n.TrackingDetail.StorageJP.TrackingCode.Contains(tracking)).OrderByDescending(n => n.CreatedAt);
            }
            if (split != "")
            {
                split = split.Trim();
                if (split == "TH" || split == "th") { split = "21"; }
                ReturnDetail = ReturnDetail.Where(n => n.TrackingSubCode == split).OrderByDescending(n => n.CreatedAt);
            }
            if (data_sort != "")
            {
                string[] sort = data_sort.Split('-');
                switch (sort[0])
                {
                    case "tracking":
                        {
                            ReturnDetail = ReturnDetail.OrderBy(n => n.TrackingDetail.StorageJP.TrackingCode);
                            if (sort[1] == "desc")
                            {
                                ReturnDetail = ReturnDetail.OrderByDescending(n => n.TrackingDetail.StorageJP.TrackingCode);
                            }
                            break;
                        }
                    case "split":
                        {
                            ReturnDetail = ReturnDetail.OrderBy(n => n.TrackingSubCode);
                            if (sort[1] == "desc")
                            {
                                ReturnDetail = ReturnDetail.OrderByDescending(n => n.TrackingSubCode);
                            }
                            break;
                        }
                    case "createdat":
                        {
                            ReturnDetail = ReturnDetail.OrderBy(n => n.CreatedAt);
                            if (sort[1] == "desc")
                            {
                                ReturnDetail = ReturnDetail.OrderByDescending(n => n.CreatedAt);
                            }
                            break;
                        }
                }
            }
            var lstReturn = Pager<ReturnDetail>.CreatePagging(ReturnDetail, page, 10);
            return PartialView("~/Views/PackageReturn/_DetailOfReturnDetail.cshtml", lstReturn);
        }
    }
}