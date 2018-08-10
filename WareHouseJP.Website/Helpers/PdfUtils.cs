using PdfSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace WareHouseJP.Website.Helpers
{
    public class PdfUtils
    {
        public static Byte[] PdfSharpConvert(String html)
        {
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                PdfGenerateConfig config = new PdfGenerateConfig();
                config.PageSize = PageSize.A4;
                config.SetMargins(20);
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, config,null, PdfUtils.OnStylesheetLoad);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
        public static void OnStylesheetLoad(object sender, HtmlStylesheetLoadEventArgs e)
        {
            var stylesheet = GetStylesheet();
            if (stylesheet != null)
                e.SetStyleSheet = stylesheet;
        }
        public static string GetStylesheet()
        {
            return @"
                    .wapper{width:900px;margin:0 auto;}
                    .header{width:inherit;text-align:center;margin-bottom:10px;}
                    .title{background:#86c791;width:200px;margin:0 auto;line-height:25px;}
                    h1, h2, h3 { color: navy; font-weight:normal; }
                    h1 { margin-bottom: .47em }
                    h2 { margin-bottom: .3em }
                    h3 { margin-bottom: .4em }
                    ul { margin-top: .5em }
                    ul li {margin: .25em}
                    body { font:10pt Tahoma }
		            pre  { border:solid 1px gray; background-color:#eee; padding:1em }
                    a:link { text-decoration: none; }
                    a:hover { text-decoration: underline; }
                    .gray    { color:gray; }
                    .example { background-color:#efefef; corner-radius:5px; padding:0.5em; }
                    .whitehole { background-color:white; corner-radius:10px; padding:15px; }
                    .caption { font-size: 1.1em }
                    .comment { color: green; margin-bottom: 5px; margin-left: 3px; }
                    .comment2 { color: green; }";
        }
        public static void SaveImageFromUrlAdv(string imageUrl, string saveLocation)
        {
            byte[] imageBytes;
            HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
            WebResponse imageResponse = imageRequest.GetResponse();

            Stream responseStream = imageResponse.GetResponseStream();

            using (BinaryReader br = new BinaryReader(responseStream))
            {
                imageBytes = br.ReadBytes(500000);
                br.Close();
            }
            responseStream.Close();
            imageResponse.Close();

            FileStream fs = new FileStream(saveLocation, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(imageBytes);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
        }
        public static void SaveImageFromUrlBasc(string imageUrl, string saveLocation)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(imageUrl, saveLocation);
            webClient.Dispose();
        }
    }
}