using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng email đăng nhập")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng địn dạng email")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string password { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn quyền đăng nhập")]
        [RegularExpression("^\\d+$", ErrorMessage = "Vui lòng chọn quyền đăng nhập")]
        public int role { get; set; }
        public bool reMember { get; set; }
    }
}