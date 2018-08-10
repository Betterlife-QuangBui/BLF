using Ohayoo.Api.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhayooApi.Message.Response.ExchangeRate
{
    public class ExchangeRateResponse
    {
        public String rate { get; set; }
    }
    public class ExchangeRateResponseBody : ApiResponse<ExchangeRateResponse>
    { }
}