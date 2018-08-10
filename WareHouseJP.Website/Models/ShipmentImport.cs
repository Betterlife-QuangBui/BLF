using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Models
{
    public class ShipmentImport
    {
        public string TrackingNumber { get; set; }
        public string DeliveryName { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime RecivedDate { get; set; }
        public string RecivedHour { get; set; }
        public double Weigh { get; set; }
        public String Status { get; set; }
        public String Notes { get; set; }
        public String ItemName { get; set; }
        public string ItemCategoryName { get; set; }
        public int ItemQuantity { get; set; }
        public String ItemNotes { get; set; }
    }
}