using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace WareHouseJP.Website.Helpers
{
    public static class DenyUtils
    {
        public static string keyJP = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Deny/KeywordJPs.txt"));
        public static string keyEN = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Deny/KeywordENs.txt"));
        public static bool CheckName(this string name,bool checkJP=true)
        {
            bool flag = false;
            if (name != null)
            {
                keyJP = (checkJP) ? keyJP : keyEN;
                foreach (var item in keyJP.ToUpper().Split(','))
                {
                    if (name.ToUpper().Contains(item)) { return true; }
                }
                return flag;
            }
            return flag;
        }
        public static bool CheckNameJPVN(this string name, bool checkJP = true)
        {
            bool flag = false;
            if (name != null)
            {
                keyJP = (checkJP) ? keyJP : keyEN;
                foreach (var item in keyJP.ToUpper().Split(','))
                {
                    if (name.ToUpper().Contains(item)) { return true; }
                }
                return flag;
            }
            return flag;
        }
    }
}