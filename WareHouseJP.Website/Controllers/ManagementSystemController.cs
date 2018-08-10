using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Controllers
{
    public class ManagementSystemController : Controller
    {
        public WareHouseJPDB db = new WareHouseJPDB();
        public UserPage user = new UserPage();
        public PageUtils PageUtils = new PageUtils();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //UpdateTrackingStatusHelpers helpers = new UpdateTrackingStatusHelpers();
            //helpers.UpdateStatus(db);
            //db = new WareHouseJPDB();

            String ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (filterContext.HttpContext.Session["AccBetterLife"] == null)
            {
                HttpCookie ckiUser = filterContext.HttpContext.Request.Cookies["CkBetterLife"];
                HttpCookie ckiRoleLogin = filterContext.HttpContext.Request.Cookies["ckiRoleLogin"];
                HttpCookie CkAgencyBetterLife = filterContext.HttpContext.Request.Cookies["CkAgencyBetterLife"];
                if (ckiUser != null)
                {
                    string UserName = ckiUser["Id"];
                    string PassWord = ckiUser["Pw"];
                    int role = Convert.ToInt32(ckiRoleLogin["ckiRoleLogin"]);
                    try
                    {
                        if (role == 2)
                        {
                            var user = db.Staffs.Single(n => n.UserName == UserName&&n.IsActive==true);
                            if (user.Password == PassWord)
                            {
                                if (filterContext.HttpContext.Request.Cookies["CkAgencyBetterLife"] != null)
                                {
                                    Session["AccBetterLife"] = user;
                                    
                                }
                                else
                                {
                                    filterContext.HttpContext.Response.Redirect("/Security");
                                }
                            }
                            else
                            {
                                ckiUser.Expires = DateTime.Now.AddDays(-20);
                                Response.Cookies.Add(ckiUser);
                                ckiRoleLogin.Expires = DateTime.Now.AddDays(-20);
                                Response.Cookies.Add(ckiRoleLogin);
                                CkAgencyBetterLife.Expires = DateTime.Now.AddDays(-20);
                                Response.Cookies.Add(CkAgencyBetterLife);
                                filterContext.HttpContext.Response.Redirect("/Security");
                            }
                        }
                        else if (role == 1)
                        {
                            var user = db.AgencyAccounts.Single(n => n.UserName == UserName&&n.IsActive==true);
                            if (user.Password == PassWord)
                            {
                                Session["AccBetterLife"] = user;
                                
                            }
                            else
                            {
                                ckiUser.Expires = DateTime.Now.AddDays(-20);
                                Response.Cookies.Add(ckiUser);
                                ckiRoleLogin.Expires = DateTime.Now.AddDays(-20);
                                Response.Cookies.Add(ckiRoleLogin);
                                CkAgencyBetterLife.Expires = DateTime.Now.AddDays(-20);
                                Response.Cookies.Add(CkAgencyBetterLife);
                                filterContext.HttpContext.Response.Redirect("/Security");
                            }
                        }
                        else
                        {
                            ckiUser.Expires = DateTime.Now.AddDays(-20);
                            Response.Cookies.Add(ckiUser);
                            ckiRoleLogin.Expires = DateTime.Now.AddDays(-20);
                            Response.Cookies.Add(ckiRoleLogin);
                            CkAgencyBetterLife.Expires = DateTime.Now.AddDays(-20);
                            Response.Cookies.Add(CkAgencyBetterLife);
                            filterContext.HttpContext.Response.Redirect("/Security");
                        }
                    }
                    catch
                    {
                        ckiUser.Expires = DateTime.Now.AddDays(-20);
                        Response.Cookies.Add(ckiUser);
                        ckiRoleLogin.Expires = DateTime.Now.AddDays(-20);
                        Response.Cookies.Add(ckiRoleLogin);
                        try
                        {
                            CkAgencyBetterLife.Expires = DateTime.Now.AddDays(-20);
                            Response.Cookies.Add(CkAgencyBetterLife);
                        }
                        catch { }
                        filterContext.HttpContext.Response.Redirect("/Security");
                    }
                }
                else
                {
                    filterContext.HttpContext.Response.Redirect("/Security");
                }
            }
        }
       public string javasctipt_add(string url = "", string message = "")
        {
            TempData["Message"] = message;
            string java = "<script language='javascript' type='text/javascript'></script>";
            try
            {
                java = System.IO.File.ReadAllText(Server.MapPath("~/notify/add.html"));
                java = String.Format(java, url, message);
            }
            catch { }

            return java;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}