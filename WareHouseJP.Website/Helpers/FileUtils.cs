using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Helpers
{
    public static class FileUtils
    {
        public static string urlImageError = "/images/noimage.png";
        public static string DisplayImage(this string s, string path)
        {

            if (File.Exists(HttpContext.Current.Server.MapPath(path + s)))
            {
                return path + s;
            }
            else { return urlImageError; }
        }
    }
}