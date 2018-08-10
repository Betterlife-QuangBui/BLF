using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WareHouseJP.Website.Helpers
{
    public class StatusUtils
    {
        public static List<SelectListItem> GetSettingStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Không sử dụng", Value = "false" });
            list.Add(new SelectListItem() { Text = "Đang sử dụng", Value = "true" });
            return list;
        }
        public static List<SelectListItem> GetGender()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Nữ", Value = "false" });
            list.Add(new SelectListItem() { Text = "Nam", Value = "true" });
            return list;
        }
        public static List<SelectListItem> GetStatus(int cate = 0)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            switch (cate)
            {
                //lô hàng
                case 0:
                    {
                        list.Add(new SelectListItem() { Text = "Đang thực hiện", Value = "-1" });
                        list.Add(new SelectListItem() { Text = "Đã hoàn tất", Value = "-2" });
                        break;
                    }
                //tren duong
                case 1:
                    {
                        //list.Add(new SelectListItem() { Text = "Chưa khai báo", Value = "1" });
                        //list.Add(new SelectListItem() { Text = "Đã khai báo", Value = "2" });
                        list.Add(new SelectListItem() { Text = "Trên đường", Value = "2" });
                        list.Add(new SelectListItem() { Text = "Đã đến kho", Value = "3" });
                        //list.Add(new SelectListItem() { Text = "Đã nhận", Value = "4" });
                        break;
                    }
                //luu kho
                case 2:
                    {
                        //list.Add(new SelectListItem() { Text = "Trên đường", Value = "5" });
                        list.Add(new SelectListItem() { Text = "Đã nhận", Value = "6" });
                        list.Add(new SelectListItem() { Text = "Đang kiểm", Value = "7" });
                        list.Add(new SelectListItem() { Text = "Đã kiểm", Value = "8" });
                        break;
                    }
                //xuat kho
                case 3:
                    {
                        list.Add(new SelectListItem() { Text = "Đang đóng gói", Value = "9" });
                        list.Add(new SelectListItem() { Text = "Đã đóng gói", Value = "10" });
                        break;
                    }
                //Booking
                case 4:
                    {
                        list.Add(new SelectListItem() { Text = "Đang booking", Value = "11" });
                        list.Add(new SelectListItem() { Text = "Đã booking", Value = "12" });
                        break;
                    }
                //Chuyến Bay 
                case 5:
                    {
                        list.Add(new SelectListItem() { Text = "Chưa thông quan", Value = "13" });
                        list.Add(new SelectListItem() { Text = "Đã thông quan", Value = "14" });
                        break;
                    }
                //Trả hàng 
                case 6:
                    {
                        list.Add(new SelectListItem() { Text = "Đang trả hàng", Value = "15" });
                        list.Add(new SelectListItem() { Text = "Đã trả hàng", Value = "16" });
                        break;
                    }
                default:
                    break;
            }

            return list;
        }
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CateId { get; set; }
    }
}