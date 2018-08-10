using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Controllers
{
    public class GeneralController : ManagementSystemController
    {
        // GET: General
        public ActionResult Index(Guid? id)
        {
            var storeJP = db.StorageJPs.Find(id);
            return View(storeJP);
        }
    }
}