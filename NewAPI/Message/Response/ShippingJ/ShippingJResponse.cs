using Ohayoo.Api.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Message.Response.ShippingJ
{
    public class ShippingJResponse
    {
        public double price { get; set; }
    }
    public class TranslateResponseBody : ApiResponse<ShippingJResponse>
    { }
}