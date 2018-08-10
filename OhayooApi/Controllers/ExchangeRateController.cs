using Ohayoo.Api;
using OhayooApi.Message.Response.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using OhayooApi.net.webservicex.www;

namespace OhayooApi.Controllers
{
    public class ExchangeRateController : ApiController
    {
        [Route("api/ExchangeRate")]
        [ResponseType(typeof(ExchangeRateResponseBody))]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            ExchangeRateResponseBody model = new ExchangeRateResponseBody();
            try
            {
                CurrencyConvertor exchangerate = new CurrencyConvertor();
                
                double exchangevalue = exchangerate.ConversionRate(Currency.JPY, Currency.VND);
                model.message = "ExchangeRate successfull !!!";
                model.code = HttpStatusCodeDefine.SUCCESS.GetHashCode();
                model.data = new ExchangeRateResponse() { rate = exchangevalue.ToString() };
            }
            catch (Exception ex)
            {
                #region error system
                model.code = HttpStatusCodeDefine.SERVER_ERROR.GetHashCode();
                model.message = ex.Message;
                model.data = null;
                #endregion
            }
            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

    }
}
