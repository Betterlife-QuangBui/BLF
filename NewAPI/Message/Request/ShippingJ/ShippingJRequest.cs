using Ohayoo.Api.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Message.ShippingJ
{
    public class ShippingJRequest : Request
    {
        public string shopCode { get; set; }
    }
}