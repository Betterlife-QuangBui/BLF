using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Models
{
    public class UserPage
    {
        public Staff Staff
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Session["AccBetterLife"] == null) return new Staff();
                    else
                    {
                        HttpCookie ckiRoleLogin = HttpContext.Current.Request.Cookies["ckiRoleLogin"];
                        if (ckiRoleLogin != null)
                        {
                            int role = Convert.ToInt32(ckiRoleLogin["ckiRoleLogin"]);
                            if (role == 2)
                            {
                                return HttpContext.Current.Session["AccBetterLife"] as Staff;
                            }
                            else if (role == 1)
                            {
                                AgencyAccount staff = HttpContext.Current.Session["AccBetterLife"] as AgencyAccount;
                                var result = new Staff()
                                {
                                    Avatar = staff.Avatar,
                                    CreatedAt = staff.CreatedAt,
                                    Email = staff.Email,
                                    Gender = staff.Gender,
                                    IsActive = staff.IsActive,
                                    IsDeleted = staff.IsDeleted,
                                    Name = staff.Name,
                                    Password = staff.Password,
                                    Phone = staff.Phone,
                                    RoleId = staff.RoleId,
                                    UpdatedAt = staff.UpdatedAt,
                                    UserName = staff.UserName
                                };
                                return result;
                            }
                        }

                    }
                }
                catch { }
                return new Staff();
            }
        }

        public Agency Agency
        {
            get
            {
                HttpCookie ckiRoleLogin = HttpContext.Current.Request.Cookies["ckiRoleLogin"];
                HttpCookie ckiUser = HttpContext.Current.Request.Cookies["CkBetterLife"];
                WareHouseJPDB db = new WareHouseJPDB();
                if (ckiRoleLogin != null && ckiUser != null)
                {
                    try
                    {
                        int role = Convert.ToInt32(ckiRoleLogin["ckiRoleLogin"]);
                        if (role == 1)
                        {
                            return db.AgencyAccounts.Find(Staff.UserName).Agency;
                        }
                        else if (role == 2)
                        {
                            var cki = HttpContext.Current.Request.Cookies["CkAgencyBetterLife"];
                            string id = cki["AgencyId"];
                            return db.Agencies.Find(id);
                        }
                    }
                    catch { }
                }
                return new Agency();
            }
        }
    }
}