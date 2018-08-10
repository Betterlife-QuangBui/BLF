using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;
using System.Configuration;
using NPOI.SS.Util;

namespace WareHouseJP.Website.Controllers
{
    public class ExportController : ManagementSystemController
    {   
        //// GET: Export
        //public ActionResult StorageJP(Guid id)
        //{
        //    var item = db.StorageJPs.Find(id);
        //    Response.AddHeader("content-disposition", "attachment; filename=REPORT_"+item.AgencyId+"_LUUKHO_"+item.ReceivedDate.Value.ToString("MM.dd.yyyy")+".xls");
        //    Response.ContentType = "application/excel";
        //    return View(db.TrackingDetails.Where(n=>n.StoregeJPId==id));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        PageUtils pageUtils = new PageUtils();
        public ActionResult BarCode(Guid id)
        {
            try
            {
                var item = db.StorageJPs.Find(id);
                MemoryStream ms = new MemoryStream();
                string html = System.IO.File.ReadAllText(Server.MapPath(@"~/Export/template/HTML/PDFBarCode.html"));
                string template = @"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>";
                string body = "";
                string urlWebsite = ConfigurationManager.AppSettings["bar_code"];
                foreach (var itemDetail in item.TrackingDetails.OrderBy(n=>n.TrackingSubCode))
                {
                    string barcodeData = (item.TrackingCode + "-" + itemDetail.TrackingSubCode);
                    string imgUrl= urlWebsite+"Barcode.ashx?m=0&h=60&vCode=" + barcodeData;
                    string fileName = barcodeData + ".jpg";
                    string urlSaveImage = Server.MapPath("~/Images/BarCode/"+ fileName);
                    PdfUtils.SaveImageFromUrlAdv(imgUrl, urlSaveImage);
                    string srcImg = urlWebsite + "Images/BarCode/" + fileName;
                    string img = "<img id='img'  src='"+ srcImg + "'/>";

                    string temp = string.Format(template, item.TrackingCode, itemDetail.TrackingSubCode, img);
                    body += temp;
                }
                html = string.Format(html, item.TrackingCode, body);
                var bytedate = PdfUtils.PdfSharpConvert(html);
                return File(bytedate, "application/pdf", "REPORT_" + item.AgencyId + "_LUUKHO_" + item.ReceivedDate.Value.ToString("MM.dd.yyyy") + ".pdf");
            }
            catch (Exception ex)
            {
                return Redirect("/");
            }
            finally
            {
                var item = db.StorageJPs.Find(id);
                foreach (var itemDetail in item.TrackingDetails.OrderBy(n => n.TrackingSubCode))
                {
                    string barcodeData = (item.TrackingCode + "-" + itemDetail.TrackingSubCode);
                    string fileName = barcodeData + ".jpg";
                    string urlSaveImage = Server.MapPath("~/Images/BarCode/" + fileName);
                    System.IO.File.Delete(urlSaveImage);
                }
            }
        }
        /// <summary>
        /// Export Phần Nội Dung Trên Đường
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Shipment(Guid id)
        {

            try
            {
                var shipment = db.Shipments.Find(id);
                // Opening the Excel template... 
                FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/TRENDUONG/TRENDUONG.xls"), FileMode.Open, FileAccess.Read);
                // Getting the complete workbook... 
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                // Getting the worksheet by its name... 
                ISheet sheet = templateWorkbook.GetSheet("INVOICE");
                #region DEFINE COLOR & BORDER
                //Define border main
                ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
                //Define color main
                ICellStyle styleColor = Helpers.ExcelUtils.ColorMain(templateWorkbook);
                //Define sub color main
                ICellStyle styleSubColor = Helpers.ExcelUtils.SubColor(templateWorkbook);
                #endregion

                #region Code Export
                //Insert mã lô hàng
                IRow dataRow = sheet.GetRow(2);
                dataRow.Cells[3].SetCellValue(shipment.ShipmentName);
                IRow dataRowDate =  sheet.GetRow(3);
                dataRowDate.Cells[3].SetCellValue(shipment.FlightDate.Value.ToString("MM/dd/yy"));
                //insert data package theo lô hàng
                int packageIndex = 1; int row = 10; int cell = 1;

                int rowAdd = 0;
                foreach (var item in shipment.AgencyPackages)
                {
                    if (item.AgencyPackageItems.Count == 0)
                    {
                        rowAdd += 1;
                    }
                    else
                    {
                        rowAdd += item.AgencyPackageItems.Count;
                    }
                }
                Helpers.ExcelUtils.InsertRows(ref sheet, row, rowAdd);
                foreach (var pack in shipment.AgencyPackages.OrderByDescending(n => n.CreatedAt))
                {   
                    IRow iRow = sheet.CreateRow(row);
                    bool statusRow = false; cell = 1;
                    string status = pageUtils.PackageStatus(pack.StatusId.Value, 1);

                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, cell, packageIndex);
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, pack.TrackingCode);
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, pack.DeliveryCom.Name);
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, pack.SentDate.Value.ToString("yyyy-MM-dd"));
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, pack.ReceivedDate.Value.ToString("yyyy-MM-dd"));
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, pack.ReceivedHour);
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, pack.Weigh.Value);
                    Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, status);
                    if (pack.AgencyPackageItems.Count == 0)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                        Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                        Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                        Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                        Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                        row++;
                    }
                    foreach (var item in pack.AgencyPackageItems)
                    {
                        if (statusRow == false)
                        {
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.ItemName);

                            //int cellSelect = ++cell;
                            //CellRangeAddressList addressList = new CellRangeAddressList(row,row, cellSelect, cellSelect);
                            //DVConstraint dvConstraint = DVConstraint.CreateExplicitListConstraint(WareHouseExport());
                            
                            //IDataValidation dataValidation = new HSSFDataValidation(addressList, dvConstraint);
                            //dataValidation.SuppressDropDownArrow = true;
                            //sheet.AddValidationData(dataValidation);

                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.WareHouseCategory.NameEN);
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.ItemQuantity.Value);
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.ItemNotes);
                            statusRow = true;
                        }
                        else
                        {
                            iRow = sheet.CreateRow(row);
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.ItemName);
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.WareHouseCategory.NameEN);
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.ItemQuantity.Value);
                            Helpers.ExcelUtils.CreateCell(styleColor, iRow, ++cell, item.ItemNotes);
                        }
                        cell = 1; 
                        row++;
                    }
                    packageIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
                string quantity = "SUM(M11:M"+ row + ")";
                string trackingCount = "COUNTA(C11:C"+row+")";
                string shipmentWeigh = "SUM(H11:H" + row + ")";
                row = row+1;
                sheet.GetRow(row).Cells[2].SetCellFormula(trackingCount);
                sheet.GetRow(row).Cells[7].SetCellFormula(shipmentWeigh);
                sheet.GetRow(row).Cells[12].SetCellFormula(quantity);
                #endregion
                MemoryStream ms = new MemoryStream();
                // Writing the workbook content to the FileStream... 
                templateWorkbook.Write(ms);
                // Sending the server processed data back to the user computer... 
                return File(ms.ToArray(), "application/vnd.ms-excel", "TD_" + shipment.AgencyId + "_"+ shipment.Code+".xls");
            }
            catch(Exception ex)
            {
                return Redirect("/");
            }
        }
        string [] WareHouseExport()
        {
            return db.WareHouseCategories.Select(n => n.NameEN).ToArray();
        }
        public ActionResult StorageJP(Guid id)
        {
            try
            {
                var item = db.StorageJPs.Find(id);
                MemoryStream ms = new MemoryStream();
                if (user.Staff.RoleId == 6)
                {
                    ExportStoreJPUser(item).Write(ms);
                }
                else
                {
                    switch (user.Staff.RoleId)
                    {
                        case 1:
                            //ExportStoreJPAdmin(item).Write(ms);
                            ExportStoreJPAdminNew20161101(item).Write(ms);
                            break;
                        case 2:
                            ExportStoreJPKhoJP(item).Write(ms);
                            break;
                        case 3:
                            ExportStoreJPKhoVN(item).Write(ms);
                            break;
                        case 4:
                            ExportStoreJPLogistics(item).Write(ms);
                            break;
                        case 5:
                            ExportStoreJPSales(item).Write(ms);
                            break;
                        default:
                            ExportStoreJPUser(item).Write(ms);
                            break;
                    }
                }
                //fs.Close();
                // Sending the server processed data back to the user computer... 
                return File(ms.ToArray(), "application/vnd.ms-excel", "REPORT_" + item.AgencyId + "_LUUKHO_" + item.ReceivedDate.Value.ToString("MM.dd.yyyy") + ".xls");
            }
            catch(Exception ex)
            {
                return Redirect("/");
            }
        }
        public ActionResult StorageJPFull(string id="")
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                ExportStoreJPAdminNew20161101(id).Write(ms);
                //fs.Close();
                // Sending the server processed data back to the user computer... 
                return File(ms.ToArray(), "application/vnd.ms-excel", "REPORT_LUUKHO.xls");
            }
            catch (Exception ex)
            {
                return Redirect("/");
            }
        }
        public ActionResult Invoice(Guid id)
        {

            try
            {
                ExportGood  item = db.ExportGoods.Find(id);
                // Opening the Excel template... 
                FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/REPORT_INVOICE.xls"), FileMode.Open, FileAccess.Read);
                // Getting the complete workbook... 
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
                // Getting the worksheet by its name... 
                #region 例
                ISheet sheet = templateWorkbook.GetSheet("例");
                int row = 4;
                //Define border main
                ICellStyle style = templateWorkbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Aqua.Index;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;

                //Define color main
                ICellStyle styleColor = templateWorkbook.CreateCellStyle();
                styleColor.FillForegroundColor = IndexedColors.Aqua.Index;
                styleColor.FillBackgroundColor = IndexedColors.Aqua.Index;
                styleColor.FillPattern = FillPattern.SolidForeground;
                styleColor.BorderBottom = BorderStyle.Thin;
                styleColor.BorderLeft = BorderStyle.Thin;
                styleColor.BorderRight = BorderStyle.Thin;
                styleColor.BorderTop = BorderStyle.Thin;

                //Define sub color main
                ICellStyle styleSubColor = templateWorkbook.CreateCellStyle();
                styleSubColor.FillForegroundColor = IndexedColors.PaleBlue.Index;
                styleSubColor.FillBackgroundColor = IndexedColors.PaleBlue.Index;
                styleSubColor.FillPattern = FillPattern.SolidForeground;
                styleSubColor.BorderBottom = BorderStyle.Thin;
                styleSubColor.BorderLeft = BorderStyle.Thin;
                styleSubColor.BorderRight = BorderStyle.Thin;
                styleSubColor.BorderTop = BorderStyle.Thin;

                var dataList = item.ExportGoodDetails.Select(n => n.TrackingDetail).OrderBy(n => n.TrackingSubCode);
                foreach (var tr in dataList)
                {
                    IRow dataRow = sheet.GetRow(row);
                    String shippingMark = tr.TrackingSubCode;
                    int index_ = shippingMark.LastIndexOf('-');
                    shippingMark = shippingMark.Substring(index_ - 5);
                    for (int i = 1; i <= 8; i++)
                    {
                        if (i != 5)
                        {
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                        }
                        else
                        {
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, shippingMark);
                        }
                    }

                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 9, tr.TrackingSubCode);
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 10, tr.Weigh.Value);
                    row++;
                    foreach (var p in tr.StorageItemJPs)
                    {
                        dataRow = sheet.GetRow(row);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.NameJP);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, TranslateUtils.TranslateGoogleTextEN(p.NameJP));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, pageUtils.Category(p.CategoryId.Value));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.PriceTax.Value);
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 5, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 6, p.Quantity.Value);
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 7, "");
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.Amount.Value);
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 10, "");
                        row++;
                    }
                }
                IRow _dataRow = sheet.GetRow(row);
                Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 8, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
                Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 9, dataList.Count() + " kiện");
                Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 10, dataList.Sum(n => n.Weigh).Value);
                #endregion

                #region Info
                ISheet sheetInfo = templateWorkbook.GetSheet("INFO");
                IRow dataRowInfo = sheetInfo.GetRow(10);
                dataRowInfo.Cells[3].SetCellValue(user.Agency.Name);
                dataRowInfo = sheetInfo.GetRow(12);
                dataRowInfo.Cells[3].SetCellValue(user.Agency.Address);
                dataRowInfo = sheetInfo.GetRow(15);
                dataRowInfo.Cells[3].SetCellValue("Tel/ Fax:  " + user.Agency.Phone+"/"+user.Agency.Fax);

                //Invoice Info
                try
                {
                    var invoice=item.ExportInvoices.First();
                    dataRowInfo = sheetInfo.GetRow(4);
                    dataRowInfo.Cells[8].SetCellValue(invoice.InvoiceNo);
                    dataRowInfo = sheetInfo.GetRow(6);
                    dataRowInfo.Cells[8].SetCellValue(invoice.InvoiceDate.Value.ToString("MM/dd/yyyy"));
                }
                catch { }
                //Air Info
                try
                {
                    var airInfo = item.AirInfo;
                    dataRowInfo = sheetInfo.GetRow(9);
                    dataRowInfo.Cells[8].SetCellValue(airInfo.AirFrom);
                    dataRowInfo = sheetInfo.GetRow(12);
                    dataRowInfo.Cells[8].SetCellValue(airInfo.AirTo);

                    dataRowInfo = sheetInfo.GetRow(16);
                    dataRowInfo.Cells[8].SetCellValue(airInfo.Id);
                }
                catch { }
                #endregion

                #region MAWB
                ISheet sheetMAWB = templateWorkbook.GetSheet("MAWB_HAWB");
                int rowMAWB = 2;
                //Define border main
                ICellStyle styleMAWB = templateWorkbook.CreateCellStyle();
                styleMAWB.FillForegroundColor = IndexedColors.Aqua.Index;
                styleMAWB.BorderBottom = BorderStyle.Thin;
                styleMAWB.BorderLeft = BorderStyle.Thin;
                styleMAWB.BorderRight = BorderStyle.Thin;
                styleMAWB.BorderTop = BorderStyle.Thin;
                styleMAWB.WrapText = true;
                //Define color main
                ICellStyle styleColorMAWB = templateWorkbook.CreateCellStyle();
                styleColorMAWB.FillForegroundColor = IndexedColors.Aqua.Index;
                styleColorMAWB.FillBackgroundColor = IndexedColors.Lavender.Index;
                styleColorMAWB.FillPattern = FillPattern.SolidForeground;
                styleColorMAWB.BorderBottom = BorderStyle.Thin;
                styleColorMAWB.BorderLeft = BorderStyle.Thin;
                styleColorMAWB.BorderRight = BorderStyle.Thin;
                styleColorMAWB.BorderTop = BorderStyle.Thin; styleColorMAWB.WrapText = true;
                string mainTitle = "CONSIGNEE ( Consigned for　Mawb )";
                //Define sub color main
                try
                {
                    var lstMAWB = item.ExportInvoices.First().MAWBDetails;
                    foreach (var ma in lstMAWB)
                    {
                        var dataRowMAWB = sheetMAWB.GetRow(rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleColorMAWB, dataRowMAWB,2, mainTitle);
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, "Company Name : ");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, ma.MAWB.Name);
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, "Add : ");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, ma.MAWB.Address);
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, "");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, "");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 2, "Tel/ Fax:  "+ma.MAWB.Tel+"/"+ma.MAWB.Fax);
                        rowMAWB += 3;
                    }
                    
                }
                catch { }

                mainTitle = "CONSIGNEE ( Consigned for　Hawb )";
                try
                {
                    rowMAWB = 2;
                    styleColorMAWB = templateWorkbook.CreateCellStyle();
                    styleColorMAWB.FillForegroundColor = IndexedColors.Aqua.Index;
                    styleColorMAWB.FillBackgroundColor = IndexedColors.BrightGreen.Index;
                    styleColorMAWB.FillPattern = FillPattern.SolidForeground;
                    styleColorMAWB.BorderBottom = BorderStyle.Thin;
                    styleColorMAWB.BorderLeft = BorderStyle.Thin;
                    styleColorMAWB.BorderRight = BorderStyle.Thin;
                    styleColorMAWB.BorderTop = BorderStyle.Thin;
                    styleColorMAWB.WrapText = true;
                    var lstMAWB = item.ExportInvoices.First().HAWBDetails;
                    foreach (var ma in lstMAWB)
                    {
                        var dataRowMAWB = sheetMAWB.GetRow(rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleColorMAWB, dataRowMAWB, 4, mainTitle);
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, "Company Name : ");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, ma.HAWB.Name);
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, "Add : ");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, ma.HAWB.Address);
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, "");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, "");
                        dataRowMAWB = sheetMAWB.GetRow(++rowMAWB);
                        Helpers.ExcelUtils.CreateCell(styleMAWB, dataRowMAWB, 4, "Tel/ Fax:  " + ma.HAWB.Tel + "/" + ma.HAWB.Fax);
                        rowMAWB += 3;
                    }

                }
                catch { }


                #endregion
                MemoryStream ms = new MemoryStream();
                // Writing the workbook content to the FileStream... 
                templateWorkbook.Write(ms);
                //fs.Close();
                // Sending the server processed data back to the user computer... 
                return File(ms.ToArray(), "application/vnd.ms-excel", "REPORT_" + item.AgencyId + "_INVOICE_" + DateTime.Now.ToString("MM.dd.yyyy") + ".xls");
            }
            catch
            {
                return Redirect("/");
            }
        }
        #region Export LUU KHO
        public HSSFWorkbook ExportStoreJPAdminNewFull()
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/ADMIN_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 3;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion

            foreach (var item in db.StorageJPs.Where(n=>n.AgencyId==user.Agency.Id))
            {
                var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
                Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
                foreach (var tr in dataList)
                {
                    Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
                    IRow dataRow = sheet.GetRow(row);
                    String shippingMark = tr.TrackingSubCode;
                    //int index_ = shippingMark.LastIndexOf('-');
                    //shippingMark = shippingMark.Substring(index_ - 5);
                    for (int i = 1; i <= 21; i++)
                    {
                        if (i != 14)
                        {
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                        }
                        else
                        {
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, item.TrackingCode);
                        }
                    }
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, shippingMark);
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, "");
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 17, item.Notes == null ? "" : item.Notes);
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 18, item.Weigh == null ? 0 : item.Weigh.Value);
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 19, PageUtils.DisplaySize(item.Weigh,item.SizeInput, item.SizeTableId, item.Size));
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 20, (item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""));
                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 21, item.ReceivedHour);
                    row++;
                    foreach (var p in tr.StorageItemJPs)
                    {
                        try
                        {
                            dataRow = sheet.GetRow(row);

                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.LinkWeb);
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 2, "");
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, p.NameJP == null ? "" : p.NameJP);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.NameEN == null ? "" : p.NameEN);
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 5, "");
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Material == null ? "" : p.Material);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.PriceTax.Value);

                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.Quantity.Value);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 9, p.Amount.Value);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, pageUtils.Category(p.CategoryId.Value));
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 11, p.Image);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, PageUtils.MadeIn(int.Parse(p.MadeIn)));
                            
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 13, p.JanCode == null ? "" : p.JanCode);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 14, p.ProductCode == null ? "" : p.ProductCode);
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 15, p.Component == null ? "" : p.Component);
                            

                            Helpers.ExcelUtils.CreateCell(style, dataRow, 16, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 17, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 18, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 19, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 20, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 21, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 22, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 23, "");
                            row++;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            //IRow _dataRow = sheet.GetRow(row);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }

        public HSSFWorkbook ExportStoreJPAdminNew20161101(string id = "")
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/EXPORT_LUUKHO_2018_05_14.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 6;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion
            string urlhinh_van_don = ConfigurationManager.AppSettings["hinh_van_don"];
            var listStoreJPs = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id);
            var listDes = new List<StorageJP>();
            if (id != "")
            {
                foreach (var item in id.Split(','))
                {
                    Guid idGuid = Guid.Parse(item.Trim());
                    listDes.Add(listStoreJPs.Single(n => n.Id == idGuid));
                }
            }
            else
            {
                listDes.AddRange(listStoreJPs.AsEnumerable());
            }
            foreach (var item in listDes.OrderBy(n => n.CreatedAt))
            {
                var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
                Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
                foreach (var tr in dataList)
                {
                    Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
                    IRow dataRow = sheet.GetRow(row);
                    String shippingMark = tr.TrackingSubCode;
                    //int index_ = shippingMark.LastIndexOf('-');
                    //shippingMark = shippingMark.Substring(index_ - 5);
                    //for (int i = 1; i <= 24; i++)
                    //{
                    //    if (i != 14 || i != 6)
                    //    {
                    //        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    //    }
                    //    else
                    //    {
                    //        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, item.TrackingCode);
                    //    }
                    //}
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 6, item.TrackingCode);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, shippingMark);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, item.TrackingCode);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, item.Weigh == null ? 0 : item.Weigh.Value);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, PageUtils.DisplaySize(item.Weigh, item.SizeInput, item.SizeTableId, item.Size));
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 17, (item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""));
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 18, item.ReceivedHour);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 19, item.Image == null || item.Image == "" ? "" : urlhinh_van_don + item.Image);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 20, item.Notes == null ? "" : item.Notes);
                    //row++;
                    foreach (var p in tr.StorageItemJPs.OrderBy(n => n.CreatedAt))
                    {
                        try
                        {
                            dataRow = sheet.GetRow(row);

                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 1, p.LinkWeb);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 2, p.HSCode == null ? "" : p.HSCode);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 3, p.NameJP == null ? "" : p.NameJP);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 4, p.NameEN == null ? "" : p.NameEN);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 5, p.DescriptionOfGoods == null ? "" : p.DescriptionOfGoods);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 6, p.Material == null ? "" : p.Material);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 7, p.PriceTax.Value);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 8, "");

                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 9, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 10, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 11, "");
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.JanCode == null ? "" : p.JanCode);
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 13, p.Quantity.Value);
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 14, p.MadeIn == null ? "" : PageUtils.MadeIn(int.Parse(p.MadeIn)));
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 15, item.Notes == null ? "" : item.Notes);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, p.Amount.Value);
                            row++;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            //IRow _dataRow = sheet.GetRow(row);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }

        //public HSSFWorkbook ExportStoreJPAdminNew20161101(string id="")
        //{
        //    FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/ADMIN_LUUKHO_2016_11_01.xls"), FileMode.Open, FileAccess.Read);
        //    // Getting the complete workbook... 
        //    HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
        //    // Getting the worksheet by its name... 
        //    ISheet sheet = templateWorkbook.GetSheet("例");
        //    int row = 3;
        //    #region DEFINE COLOR & BORDER
        //    //Define border main
        //    ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
        //    //Define color main
        //    ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
        //    //Define sub color main
        //    ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
        //    #endregion
        //    string urlhinh_van_don = ConfigurationManager.AppSettings["hinh_van_don"];
        //    var listStoreJPs = db.StorageJPs.Where(n => n.AgencyId == user.Agency.Id);
        //    var listDes = new List<StorageJP>();
        //    if (id != "")
        //    {
        //        foreach (var item in id.Split(','))
        //        {
        //            Guid idGuid = Guid.Parse(item.Trim());
        //            listDes.Add(listStoreJPs.Single(n => n.Id == idGuid));
        //        }
        //    }
        //    else
        //    {
        //        listDes.AddRange(listStoreJPs.AsEnumerable());
        //    }
        //    foreach (var item in listDes.OrderBy(n=>n.CreatedAt))
        //    {
        //        var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
        //        Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
        //        foreach (var tr in dataList)
        //        {
        //            Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
        //            IRow dataRow = sheet.GetRow(row);
        //            String shippingMark = tr.TrackingSubCode;
        //            //int index_ = shippingMark.LastIndexOf('-');
        //            //shippingMark = shippingMark.Substring(index_ - 5);
        //            for (int i = 1; i <= 24; i++)
        //            {
        //                if (i != 14||i!=6)
        //                {
        //                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
        //                }
        //                else
        //                {
        //                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, item.TrackingCode);
        //                }
        //            }
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 6, item.TrackingCode);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, shippingMark);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, item.TrackingCode);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, item.Weigh == null ? 0 : item.Weigh.Value);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, PageUtils.DisplaySize(item.Weigh,item.SizeInput, item.SizeTableId, item.Size));
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 17, (item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""));
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 18, item.ReceivedHour);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 19, item.Image==null||item.Image==""?"": urlhinh_van_don+ item.Image);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 20, item.Notes == null ? "" : item.Notes);
        //            row++;
        //            foreach (var p in tr.StorageItemJPs.OrderBy(n=>n.CreatedAt))
        //            {
        //                try
        //                {
        //                    dataRow = sheet.GetRow(row);

        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.LinkWeb);
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 2, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, p.NameJP == null ? "" : p.NameJP);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.NameEN == null ? "" : p.NameEN);
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 5, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Material == null ? "" : p.Material);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.PriceTax.Value);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.CategoryId==null?"":pageUtils.Category(p.CategoryId.Value));
                            
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, p.JanCode == null ? "" : p.JanCode);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 11, p.Quantity.Value);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.MadeIn==null?"":PageUtils.MadeIn(int.Parse(p.MadeIn)));
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 13, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 15, p.Amount.Value);
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 16, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 17, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 18, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 19, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 20, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 21, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 22, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 23, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 24, p.ProductCode);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 25, p.Image);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 26, p.Component == null ? "" : p.Component);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 27, p.Material == null ? "" : p.Material);
        //                    row++;
        //                }
        //                catch (Exception ex)
        //                {
        //                }
        //            }
        //        }
        //    }
        //    //IRow _dataRow = sheet.GetRow(row);
        //    //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
        //    //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
        //    //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
        //    // Forcing formula recalculation... 
        //    sheet.ForceFormulaRecalculation = true;
        //    return templateWorkbook;
        //}

        public HSSFWorkbook ExportStoreJPAdminNew20161101(StorageJP items)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/EXPORT_LUUKHO_2018_05_14.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 6;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion
            string urlhinh_van_don = ConfigurationManager.AppSettings["hinh_van_don"];
            List<StorageJP> lstStore = new List<Models.StorageJP>();
            lstStore.Add(items);
            foreach (var item in lstStore)
            {
                var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
                Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
                foreach (var tr in dataList)
                {
                    Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
                    IRow dataRow = sheet.GetRow(row);
                    String shippingMark = tr.TrackingSubCode;
                    //int index_ = shippingMark.LastIndexOf('-');
                    //shippingMark = shippingMark.Substring(index_ - 5);
                    //for (int i = 1; i <= 24; i++)
                    //{
                    //    if (i != 14 || i != 6)
                    //    {
                    //        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    //    }
                    //    else
                    //    {
                    //        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, item.TrackingCode);
                    //    }
                    //}
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 6, item.TrackingCode);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, shippingMark);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, item.TrackingCode);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, item.Weigh == null ? 0 : item.Weigh.Value);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, PageUtils.DisplaySize(item.Weigh, item.SizeInput, item.SizeTableId, item.Size));
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 17, (item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""));
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 18, item.ReceivedHour);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 19, item.Image == null || item.Image == "" ? "" : urlhinh_van_don + item.Image);
                    //Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 20, item.Notes == null ? "" : item.Notes);
                    //row++;
                    foreach (var p in tr.StorageItemJPs.OrderBy(n => n.CreatedAt))
                    {
                        try
                        {
                            dataRow = sheet.GetRow(row);

                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 1, p.LinkWeb);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 2, p.HSCode == null ? "" : p.HSCode);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 3, p.NameJP == null ? "" : p.NameJP);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 4, p.NameEN == null ? "" : p.NameEN);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 5, p.DescriptionOfGoods == null ? "" : p.DescriptionOfGoods);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 6, p.Material == null ? "" : p.Material);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 7, p.PriceTax.Value);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 8, "");

                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 9, "");
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 10, "");
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 11, "");
                            Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.JanCode == null ? "" : p.JanCode);
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 13, p.Quantity.Value);
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 14, p.MadeIn == null ? "" : PageUtils.MadeIn(int.Parse(p.MadeIn)));
                            Helpers.ExcelUtils.CreateCell(style, dataRow, 15, item.Notes == null ? "" : item.Notes);
                            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, p.Amount.Value);
                            row++;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            //IRow _dataRow = sheet.GetRow(row);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }

        //public HSSFWorkbook ExportStoreJPAdminNew20161101(StorageJP items)
        //{
        //    FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/ADMIN_LUUKHO_2016_11_01.xls"), FileMode.Open, FileAccess.Read);
        //    // Getting the complete workbook... 
        //    HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
        //    // Getting the worksheet by its name... 
        //    ISheet sheet = templateWorkbook.GetSheet("例");
        //    int row = 3;
        //    #region DEFINE COLOR & BORDER
        //    //Define border main
        //    ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
        //    //Define color main
        //    ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
        //    //Define sub color main
        //    ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
        //    #endregion
        //    string urlhinh_van_don = ConfigurationManager.AppSettings["hinh_van_don"];
        //    List<StorageJP> lstStore = new List<Models.StorageJP>();
        //    lstStore.Add(items);
        //    foreach (var item in lstStore)
        //    {
        //        var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
        //        Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
        //        foreach (var tr in dataList)
        //        {
        //            Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
        //            IRow dataRow = sheet.GetRow(row);
        //            String shippingMark = tr.TrackingSubCode;
        //            //int index_ = shippingMark.LastIndexOf('-');
        //            //shippingMark = shippingMark.Substring(index_ - 5);
        //            for (int i = 1; i <= 24; i++)
        //            {
        //                if (i != 14 || i != 6)
        //                {
        //                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
        //                }
        //                else
        //                {
        //                    Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, item.TrackingCode);
        //                }
        //            }
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 6, item.TrackingCode);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, shippingMark);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, item.TrackingCode);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, item.Weigh == null ? 0 : item.Weigh.Value);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, PageUtils.DisplaySize(item.Weigh,item.SizeInput, item.SizeTableId, item.Size));
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 17, (item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""));
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 18, item.ReceivedHour);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 19, item.Image == null || item.Image == "" ? "" : urlhinh_van_don + item.Image);
        //            Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 20, item.Notes == null ? "" : item.Notes);
        //            row++;
        //            foreach (var p in tr.StorageItemJPs.OrderBy(n => n.CreatedAt))
        //            {
        //                try
        //                {
        //                    dataRow = sheet.GetRow(row);

        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.LinkWeb);
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 2, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, p.NameJP == null ? "" : p.NameJP);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.NameEN == null ? "" : p.NameEN);
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 5, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Material == null ? "" : p.Material);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.PriceTax.Value);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.CategoryId == null ? "" : pageUtils.Category(p.CategoryId.Value));

        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, p.JanCode == null ? "" : p.JanCode);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 11, p.Quantity.Value);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.MadeIn == null ? "" : PageUtils.MadeIn(int.Parse(p.MadeIn)));
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 13, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 15, p.Amount.Value);
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 16, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 17, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 18, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 19, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 20, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 21, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 22, "");
        //                    Helpers.ExcelUtils.CreateCell(style, dataRow, 23, "");
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 24, p.ProductCode);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 25, p.Image);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 26, p.Component == null ? "" : p.Component);
        //                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 27, p.Material == null ? "" : p.Material);
        //                    row++;
        //                }
        //                catch (Exception ex)
        //                {
        //                }
        //            }
        //        }
        //    }
        //    //IRow _dataRow = sheet.GetRow(row);
        //    //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
        //    //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
        //    //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
        //    // Forcing formula recalculation... 
        //    sheet.ForceFormulaRecalculation = true;
        //    return templateWorkbook;
        //}


        /// <summary>
        /// update by feedback of CEO [2016/10/31]
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportStoreJPAdminNew(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/ADMIN_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 3;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion

            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
            foreach (var tr in dataList)
            {
                Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 21; i++)
                {
                    if (i != 14)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, item.TrackingCode);
                    }
                }
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, shippingMark);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 16, "");
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 17, item.Notes == null ? "" : item.Notes);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 18, item.Weigh == null ? 0 : item.Weigh.Value);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 19, PageUtils.DisplaySize(item.Weigh,item.SizeInput, item.SizeTableId, item.Size));
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 20, (item.ReceivedDate != null ? item.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""));
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 21, item.ReceivedHour);
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    try
                    {
                        dataRow = sheet.GetRow(row);

                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.NameJP == null ? "" : p.NameJP);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, p.NameEN == null ? "" : p.NameEN);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, pageUtils.Category(p.CategoryId.Value));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.Image);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 5, p.LinkWeb);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, PageUtils.MadeIn(int.Parse(p.MadeIn)));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.Quantity.Value);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.PriceTax.Value);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 9, p.Amount.Value);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, p.JanCode == null ? "" : p.JanCode);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 11, p.ProductCode == null ? "" : p.ProductCode);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.Component == null ? "" : p.Component);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow,13, p.Material == null ? "" : p.Material);
                       
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 15, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 16, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 17, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 18, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 19, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 20, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 21, "");
                        row++;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            //IRow _dataRow = sheet.GetRow(row);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
            //Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        public HSSFWorkbook ExportStoreJPAdmin(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/ADMIN_REPORT_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 4;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion
            
            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            Helpers.ExcelUtils.InsertRows(ref sheet, row, dataList.Count());
            foreach (var tr in dataList)
            {
                Helpers.ExcelUtils.InsertRows(ref sheet, row, tr.StorageItemJPs.Count());
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 14; i++)
                {
                    if (i != 10)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, shippingMark+" ");
                    }
                }
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, tr.StorageJP.TrackingCode + "-" + tr.TrackingSubCode);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 15, tr.Weigh==null?0: tr.Weigh.Value);
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    try
                    {
                        dataRow = sheet.GetRow(row);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.JanCode == null ? "" : p.JanCode);

                        Helpers.ExcelUtils.CreateCellLink(styleSubColor, dataRow, 2, p.NameJP, p.LinkWeb);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, p.NameEN == null ? "" : p.NameEN);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, pageUtils.Category(p.CategoryId.Value));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 5, Helpers.ExcelUtils.ReturnUrlFromBase64(p.Id + ""));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Component == null ? "" : p.Component);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.Material == null ? "" : p.Material);
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, PageUtils.MadeIn(int.Parse(p.MadeIn)));
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 9, p.PriceTax.Value);
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 10, "");
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 11, p.Quantity.Value);
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 12, "");
                        Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 13, p.Amount.Value);
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
                        Helpers.ExcelUtils.CreateCell(style, dataRow, 15, "");
                        row++;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            IRow _dataRow = sheet.GetRow(row);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Count() + " kiện");
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 15, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        public HSSFWorkbook ExportStoreJPKhoJP(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/KHOJP_REPORT_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 4;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion


            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            foreach (var tr in dataList)
            {
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 14; i++)
                {
                    if (i != 9)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, shippingMark);
                    }
                }
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, tr.StorageJP.TrackingCode + "-" + tr.TrackingSubCode);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, tr.Weigh.Value);
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    dataRow = sheet.GetRow(row);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.JanCode);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, p.NameJP);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, TranslateUtils.TranslateGoogleTextEN(p.NameJP));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, pageUtils.Category(p.CategoryId.Value));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 5, Helpers.ExcelUtils.ReturnUrlFromBase64(p.Id + ""));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Component == null ? "" : p.Component);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.Material == null ? "" : p.Material);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.PriceTax.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, p.Quantity.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 11, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.Amount.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 13, "");
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
                    row++;
                }
            }
            IRow _dataRow = sheet.GetRow(row);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 12, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Count() + " kiện");
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        public HSSFWorkbook ExportStoreJPKhoVN(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/KHOVN_REPORT_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 4;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion


            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            foreach (var tr in dataList)
            {
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 14; i++)
                {
                    if (i != 9)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, shippingMark);
                    }
                }
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, tr.StorageJP.TrackingCode + "-" + tr.TrackingSubCode);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, tr.Weigh.Value);
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    dataRow = sheet.GetRow(row);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.JanCode);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, p.NameJP);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, TranslateUtils.TranslateGoogleTextEN(p.NameJP));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, pageUtils.Category(p.CategoryId.Value));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 5, Helpers.ExcelUtils.ReturnUrlFromBase64(p.Id + ""));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Component == null ? "" : p.Component);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.Material == null ? "" : p.Material);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.PriceTax.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, p.Quantity.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 11, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.Amount.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 13, "");
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
                    row++;
                }
            }
            IRow _dataRow = sheet.GetRow(row);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 12, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Count() + " kiện");
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        public HSSFWorkbook ExportStoreJPLogistics(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/LOGISTICS_REPORT_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 4;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion


            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            foreach (var tr in dataList)
            {
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 14; i++)
                {
                    if (i != 9)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, shippingMark);
                    }
                }
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 13, tr.StorageJP.TrackingCode + "-" + tr.TrackingSubCode);
                Helpers.ExcelUtils.CreateCell(styleColor, dataRow, 14, tr.Weigh.Value);
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    dataRow = sheet.GetRow(row);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.JanCode);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, p.NameJP);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, TranslateUtils.TranslateGoogleTextEN(p.NameJP));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, pageUtils.Category(p.CategoryId.Value));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 5, Helpers.ExcelUtils.ReturnUrlFromBase64(p.Id + ""));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Component == null ? "" : p.Component);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 7, p.Material == null ? "" : p.Material);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.PriceTax.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 10, p.Quantity.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 11, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 12, p.Amount.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 13, "");
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 14, "");
                    row++;
                }
            }
            IRow _dataRow = sheet.GetRow(row);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 12, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 13, dataList.Count() + " kiện");
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 14, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        public HSSFWorkbook ExportStoreJPSales(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/SALES_REPORT_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 4;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion


            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            foreach (var tr in dataList)
            {
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 9; i++)
                {
                    if (i != 5)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, tr.StorageJP.TrackingCode + "-" + tr.TrackingSubCode);
                    }
                }
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    dataRow = sheet.GetRow(row);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.NameJP);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, TranslateUtils.TranslateGoogleTextEN(p.NameJP));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, pageUtils.Category(p.CategoryId.Value));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.PriceTax.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 5, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Quantity.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 7, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.Amount.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
                    row++;
                }
            }
            IRow _dataRow = sheet.GetRow(row);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 8, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 5, dataList.Count() + " kiện");
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 9, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        public HSSFWorkbook ExportStoreJPUser(StorageJP item)
        {
            FileStream fs = new FileStream(Server.MapPath(@"~/Export/template/LUUKHO/USER_REPORT_LUUKHO.xls"), FileMode.Open, FileAccess.Read);
            // Getting the complete workbook... 
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            // Getting the worksheet by its name... 
            ISheet sheet = templateWorkbook.GetSheet("例");
            int row = 4;
            #region DEFINE COLOR & BORDER
            //Define border main
            ICellStyle style = Helpers.ExcelUtils.BorderMain(templateWorkbook);
            //Define color main
            ICellStyle styleColor = Helpers.ExcelUtils.ColorMainStoreJP(templateWorkbook);
            //Define sub color main
            ICellStyle styleSubColor = Helpers.ExcelUtils.SubColorStoreJP(templateWorkbook);
            #endregion


            var dataList = db.TrackingDetails.Where(n => n.StoregeJPId == item.Id).OrderBy(n => n.TrackingSubCode);
            foreach (var tr in dataList)
            {
                IRow dataRow = sheet.GetRow(row);
                String shippingMark = tr.TrackingSubCode;
                //int index_ = shippingMark.LastIndexOf('-');
                //shippingMark = shippingMark.Substring(index_ - 5);
                for (int i = 1; i <= 9; i++)
                {
                    if (i != 5)
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, "");
                    }
                    else
                    {
                        Helpers.ExcelUtils.CreateCell(styleColor, dataRow, i, tr.StorageJP.TrackingCode + "-" + tr.TrackingSubCode);
                    }
                }
                row++;
                foreach (var p in tr.StorageItemJPs)
                {
                    dataRow = sheet.GetRow(row);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 1, p.NameJP);
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 2, TranslateUtils.TranslateGoogleTextEN(p.NameJP));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 3, pageUtils.Category(p.CategoryId.Value));
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 4, p.PriceTax.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 5, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 6, p.Quantity.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 7, "");
                    Helpers.ExcelUtils.CreateCell(styleSubColor, dataRow, 8, p.Amount.Value);
                    Helpers.ExcelUtils.CreateCell(style, dataRow, 9, "");
                    row++;
                }
            }
            IRow _dataRow = sheet.GetRow(row);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 8, dataList.Sum(m => m.StorageItemJPs.Sum(n => n.Amount)).Value);
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 5, dataList.Count() + " kiện");
            Helpers.ExcelUtils.CreateCell(styleColor, _dataRow, 9, dataList.Sum(n => n.Weigh).Value);
            // Forcing formula recalculation... 
            sheet.ForceFormulaRecalculation = true;
            return templateWorkbook;
        }
        #endregion
    }
}