using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Models
{
    public class GeneralRelation
    {
        public GeneralExportGood GeneralExportGood { get; set; }
        public GeneralShipping GeneralShipping { get; set; }
        public GeneralReturn GeneralReturn { get; set; }
    }
    public class GeneralExportGood
    {
        public string ShippingMask { get; set; }
        public string Status { get; set; }
        public double Weigh { get; set; }
        public string Size { get; set; }
    }
    public class GeneralShipping
    {
        public string ShippingCode { get; set; }
        public string Status { get; set; }
    }
    public class GeneralReturn
    {
        public string ReturnCode { get; set; }
        public string Status { get; set; }
    }
}