using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WareHouseJP.Website.Helpers;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Controllers
{
    public class SecurityController : AsyncController
    {
        public WareHouseJPDB db = new WareHouseJPDB();

        [HttpGet]
        public ActionResult Index()
        {

            ViewBag.role = new SelectList(SelectListUtils.RoleLogin(), "Value", "Text", 1);
            return View();
        }
        public ActionResult Choose()
        {
            ViewBag.Title = "Chọn đại lý";
            return View(db.Agencies.OrderBy(n => n.CreatedAt));
        }
        public ActionResult SetAgency(string id)
        {
            #region create cookies
            HttpCookie CkAgencyBetterLife = new HttpCookie("CkAgencyBetterLife");
            var agency = db.Agencies.Find(id);
            CkAgencyBetterLife["AgencyId"] = agency.Id;
            CkAgencyBetterLife["AgencyName"] = agency.Name;
            CkAgencyBetterLife.Expires = DateTime.Now.AddHours(24);
            Response.Cookies.Add(CkAgencyBetterLife);
            #endregion
            return Redirect("/Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel loginModel)
        {
            ViewBag.role = new SelectList(SelectListUtils.RoleLogin(), "Value", "Text", loginModel.role);
            try
            {
                //login step hight agency
                var userAgency = db.AgencyAccounts.Single(n=>n.UserName==loginModel.userName&&n.IsActive==true);
                if (userAgency.Password == SimpleEncrypt.Encrypt(loginModel.password, true))
                {
                    Session["AccBetterLife"] = userAgency;
                    if (loginModel.reMember == true)
                    {
                        #region create cookies
                        HttpCookie ckiUser = new HttpCookie("CkBetterLife");
                        ckiUser["Id"] = userAgency.UserName;
                        ckiUser["Pw"] = SimpleEncrypt.Encrypt(loginModel.password, true);
                        ckiUser.Expires = DateTime.Now.AddDays(24);
                        Response.Cookies.Add(ckiUser);
                        #endregion
                    }
                    //redirect app
                    #region create cookies role
                    HttpCookie ckiRoleLogin = new HttpCookie("ckiRoleLogin");
                    ckiRoleLogin["ckiRoleLogin"] = "1";
                    ckiRoleLogin.Expires = DateTime.Now.AddDays(24);
                    Response.Cookies.Add(ckiRoleLogin);
                    #endregion
                    return Redirect("/Home");
                }
                else
                {
                    ViewBag.Message = " Sai thông tin đăng nhập";
                    return View("Index");
                }

            }
            //login blf account
            catch
            {
                try
                {
                    var user = db.Staffs.Single(n => n.UserName == loginModel.userName&&n.IsActive==true);
                    if (user.Password == SimpleEncrypt.Encrypt(loginModel.password, true))
                    {
                        Session["AccBetterLife"] = user;
                        if (loginModel.reMember == true)
                        {
                            #region create cookies
                            HttpCookie ckiUser = new HttpCookie("CkBetterLife");
                            ckiUser["Id"] = user.UserName;
                            ckiUser["Pw"] = SimpleEncrypt.Encrypt(loginModel.password, true);
                            ckiUser.Expires = DateTime.Now.AddDays(24);
                            Response.Cookies.Add(ckiUser);
                            #endregion
                        }
                        //redirect app
                        #region create cookies role
                        HttpCookie ckiRoleLogin = new HttpCookie("ckiRoleLogin");
                        ckiRoleLogin["ckiRoleLogin"] = "2";
                        ckiRoleLogin.Expires = DateTime.Now.AddDays(24);
                        Response.Cookies.Add(ckiRoleLogin);
                        #endregion
                        return Redirect("/Security/Choose");
                    }
                    else
                    {
                        ViewBag.Message = " Sai thông tin đăng nhập";
                        return View("Index");
                    }
                }
                catch(Exception ex)
                {
                    ViewBag.Message = " Sai thông tin đăng nhập "+ex.Message;
                    ViewBag.role = new SelectList(SelectListUtils.RoleLogin(), "Value", "Text", loginModel.role);
                    return View("Index");
                }
            }
        }
    }
}