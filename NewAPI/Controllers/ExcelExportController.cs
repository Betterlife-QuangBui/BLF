using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Controllers
{
    public class ExcelExportController : ApiController
    {
        //[HttpPost]
        //public HttpResponseMessage ExportToExcel(string fromTimeStamp, string toTimeStamp)
        //{
        //    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        //    //If you have Physical file Read the fileStream and use it.
        //    response.Content = new StreamContent(fileStream);
        //    //OR
        //    //Create a file on the fly and get file data as a byte array and send back to client
        //    response.Content = new ByteArrayContent(fileBytes);//Use your byte array
        //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //    response.Content.Headers.ContentDisposition.FileName = "text.xls";//your file Name- text.xls
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
        //    //response.Content.Headers.ContentType  = new MediaTypeHeaderValue("application/octet-stream");
        //    response.Content.Headers.ContentLength = fileStream.Length;
        //    response.StatusCode = System.Net.HttpStatusCode.OK;
        //}
    }
}
