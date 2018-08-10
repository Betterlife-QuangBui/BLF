using NewAPI.Message.Response.ShippingJ;
using NewAPI.Message.ShippingJ;
using Ohayoo.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace OhayooApi.Controllers
{
    public class ShippingJController : ApiController
    {
        [Route("api/ShipJPrice")]
        [ResponseType(typeof(ShippingJResponse))]
        [HttpPost]
        public HttpResponseMessage ShippingJPrice([FromBody]ShippingJRequest jsonbody)
        {
            double price = 864;
            ShippingJResponse trans = new ShippingJResponse();
            try
            {
                price = ShippingJapanUtils.ShippingPrice(jsonbody.shopCode);
            }
            catch { }
            trans.price = price;
            return Request.CreateResponse(HttpStatusCode.OK, trans);
        }
    }
}
