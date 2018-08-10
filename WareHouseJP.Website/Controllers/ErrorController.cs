using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Controllers
{
    public class ErrorController : ManagementSystemController
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
    }
}