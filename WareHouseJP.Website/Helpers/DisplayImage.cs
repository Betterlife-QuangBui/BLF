using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;

namespace WareHouseJP.Website.Helpers
{
    public class DisplayImage
    {
        public string Avatar(string avatar)
        {
            string imgReturn = "/images/staff/noimage.png";
            if (avatar != null && avatar.Length > 0)
            {
                //kiem tra hinh anh co ton tai hay ko
                string url = HttpContext.Current.Server.MapPath("~/images/staff/"+avatar);
                if (System.IO.File.Exists(url))
                {
                    return "/images/staff/" + avatar;
                }
            }
            return imgReturn;
        }
    }
}