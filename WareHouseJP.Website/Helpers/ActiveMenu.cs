using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Helpers
{
    public class ActiveMenu
    {
        public static string Active(string controlllers, string controllerCurrent)
        {
            string cssClass = "";
            try
            {
                string[] arrController = controlllers.Split(',');
                foreach (var item in arrController)
                {
                    if (item.ToLower() == controllerCurrent.ToLower())
                    {
                        cssClass = "active open";
                        break;
                    }
                }
                if (arrController.Length == 1&&controlllers.ToLower()== controllerCurrent.ToLower()) { cssClass = "active"; }
            }
            catch { }
            return cssClass;
        }
    }
}