using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using NPOI.XSSF.UserModel;

namespace WareHouseJP.Website.Helpers
{
    public class ExcelUtils
    {
        public static void CreateCellLink(ICellStyle style, IRow dataRow, int cellIndex, string value,string link="")
        {
            var cell = dataRow.CreateCell(cellIndex);
            if (link != null)
            {
                if (link.Length < 250)
                {
                    string hyperlink = "HYPERLINK(\"" + link + "\", \"" + value + "\")";
                    cell.SetCellFormula(hyperlink);
                }
                else { cell.SetCellValue(value); }
            }
            else
            {
                cell.SetCellValue(value);
            }
            cell.CellStyle = style;
        }
        public static string ReturnUrlFromBase64(string Id)
        {
            string url = ConfigurationManager.AppSettings["url_report_storejp"] + Id;
            return url;
        }
        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        public static void CreateCell(ICellStyle style, IRow dataRow, int cellIndex, string value)
        {
            var cell = dataRow.CreateCell(cellIndex);
            cell.SetCellValue(value);
            cell.CellStyle = style;
        }
        public static void CreateCell(ICellStyle style, IRow dataRow, int cellIndex, int value)
        {
            var cell = dataRow.CreateCell(cellIndex);
            cell.SetCellValue(value);
            cell.CellStyle = style;
        }
        public static void CreateCell(ICellStyle style, IRow dataRow, int cellIndex, double value)
        {
            var cell = dataRow.CreateCell(cellIndex);
            cell.SetCellValue(value);
            cell.CellStyle = style;
        }
        public static ICellStyle BorderMain(HSSFWorkbook templateWorkbook)
        {
            ICellStyle style = templateWorkbook.CreateCellStyle();
            style.FillForegroundColor = IndexedColors.Aqua.Index;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            return style;
        }
        public static ICellStyle BorderMain(XSSFWorkbook templateWorkbook)
        {
            ICellStyle style = templateWorkbook.CreateCellStyle();
            style.FillForegroundColor = IndexedColors.Aqua.Index;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            return style;
        }
        public static ICellStyle ColorMain(HSSFWorkbook templateWorkbook)
        {
            ICellStyle styleColor = templateWorkbook.CreateCellStyle();
            styleColor.BorderBottom = BorderStyle.Thin;
            styleColor.BorderLeft = BorderStyle.Thin;
            styleColor.BorderRight = BorderStyle.Thin;
            styleColor.BorderTop = BorderStyle.Thin;
            return styleColor;
        }
        public static ICellStyle SubColor(HSSFWorkbook templateWorkbook)
        {
            ICellStyle styleSubColor = templateWorkbook.CreateCellStyle();
            styleSubColor.FillForegroundColor = IndexedColors.PaleBlue.Index;
            styleSubColor.FillBackgroundColor = IndexedColors.PaleBlue.Index;
            styleSubColor.FillPattern = FillPattern.SolidForeground;
            styleSubColor.BorderBottom = BorderStyle.Thin;
            styleSubColor.BorderLeft = BorderStyle.Thin;
            styleSubColor.BorderRight = BorderStyle.Thin;
            styleSubColor.BorderTop = BorderStyle.Thin;
            return styleSubColor;
        }
        public static void InsertRows(ref ISheet sheet1, int fromRowIndex, int rowCount)
        {
            sheet1.ShiftRows(fromRowIndex, sheet1.LastRowNum, rowCount);

            for (int rowIndex = fromRowIndex; rowIndex < fromRowIndex + rowCount; rowIndex++)
            {
                IRow rowSource = sheet1.GetRow(rowIndex + rowCount);
                IRow rowInsert = sheet1.CreateRow(rowIndex);
                rowInsert.Height = rowSource.Height;
                for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
                {
                    ICell cellSource = rowSource.GetCell(colIndex);
                    ICell cellInsert = rowInsert.CreateCell(colIndex);
                    if (cellSource != null)
                    {
                        cellInsert.CellStyle = cellSource.CellStyle;
                    }
                }
            }
        }
        public static void CreateCellImage(HSSFWorkbook templateWorkbook, ISheet sheet, string url, int row1, int row2)
        {
            var patriarch = sheet.CreateDrawingPatriarch();
            HSSFClientAnchor anchor;
            anchor = new HSSFClientAnchor(0, 0, 0, 0, 0, row1, 0, row2);
            var picture = patriarch.CreatePicture(anchor, LoadImage(url, templateWorkbook));
            picture.Resize();
        }
        public static byte[] GetBytesFromUrl(string url)
        {

            byte[] imageBytes;
            HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(url);
            WebResponse imageResponse = imageRequest.GetResponse();

            Stream responseStream = imageResponse.GetResponseStream();

            using (BinaryReader br = new BinaryReader(responseStream))
            {
                imageBytes = br.ReadBytes(500000);
                br.Close();
            }
            responseStream.Close();
            imageResponse.Close();
            return imageBytes;
        }

        public static int LoadImage(string path, HSSFWorkbook wb)
        {
            //FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

            byte[] buffer = GetBytesFromUrl(path);



            return wb.AddPicture(buffer, PictureType.JPEG);

        }
        #region LUUKHO
        public static ICellStyle SubColorStoreJP(HSSFWorkbook templateWorkbook)
        {
            ICellStyle styleSubColor = templateWorkbook.CreateCellStyle();
            styleSubColor.FillForegroundColor = IndexedColors.Yellow.Index;
            styleSubColor.FillBackgroundColor = IndexedColors.Grey80Percent.Index;
            styleSubColor.FillPattern = FillPattern.SolidForeground;
            styleSubColor.BorderBottom = BorderStyle.Thin;
            styleSubColor.BorderLeft = BorderStyle.Thin;
            styleSubColor.BorderRight = BorderStyle.Thin;
            styleSubColor.BorderTop = BorderStyle.Thin;
            return styleSubColor;
        }
        public static ICellStyle SubColorStoreJP(XSSFWorkbook templateWorkbook)
        {
            ICellStyle styleSubColor = templateWorkbook.CreateCellStyle();
            styleSubColor.FillForegroundColor = IndexedColors.Yellow.Index;
            styleSubColor.FillBackgroundColor = IndexedColors.Grey80Percent.Index;
            styleSubColor.FillPattern = FillPattern.SolidForeground;
            styleSubColor.BorderBottom = BorderStyle.Thin;
            styleSubColor.BorderLeft = BorderStyle.Thin;
            styleSubColor.BorderRight = BorderStyle.Thin;
            styleSubColor.BorderTop = BorderStyle.Thin;
            return styleSubColor;
        }

        public static ICellStyle ColorMainStoreJP(HSSFWorkbook templateWorkbook)
        {
            ICellStyle styleColor = templateWorkbook.CreateCellStyle();
            styleColor.FillForegroundColor = IndexedColors.LightCornflowerBlue.Index;
            styleColor.FillBackgroundColor = IndexedColors.Grey80Percent.Index;
            styleColor.FillPattern = FillPattern.SolidForeground;
            styleColor.BorderBottom = BorderStyle.Thin;
            styleColor.BorderLeft = BorderStyle.Thin;
            styleColor.BorderRight = BorderStyle.Thin;
            styleColor.BorderTop = BorderStyle.Thin;
            return styleColor;
        }
        public static ICellStyle ColorMainStoreJP(XSSFWorkbook templateWorkbook)
        {
            ICellStyle styleColor = templateWorkbook.CreateCellStyle();
            styleColor.FillForegroundColor = IndexedColors.LightCornflowerBlue.Index;
            styleColor.FillBackgroundColor = IndexedColors.Grey80Percent.Index;
            styleColor.FillPattern = FillPattern.SolidForeground;
            styleColor.BorderBottom = BorderStyle.Thin;
            styleColor.BorderLeft = BorderStyle.Thin;
            styleColor.BorderRight = BorderStyle.Thin;
            styleColor.BorderTop = BorderStyle.Thin;
            return styleColor;
        }
        #endregion
    }
}