using Ohayoo.Api;
using OhayooApi.Message.Response.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;
using System.Web.Configuration;
using Ohayoo.DB;
using Ohayoo.Lib;


namespace OhayooApi.Controllers
{
    public class ExchangeRateController : ApiController
    {
        public string url = WebConfigurationManager.AppSettings["exchangerates"];
        [Route("api/ExchangeRateVietComBank")]
        [ResponseType(typeof(double))]
        [HttpGet]
        public HttpResponseMessage ExchangeRateVietComBank()
        {
            double rate = 0;
            try
            {
                OhayooDB db = new OhayooDB();
                rate = ExchangeRateVietComBankUtils.VietComBank(url);
                //update exchage rate
                if (db.ExchageRates.Count() > 0)
                {
                    if (db.ExchageRates.OrderByDescending(n=>n.id).FirstOrDefault().exRate != rate)
                    {
                        ExchageRate model = new ExchageRate()
                        {
                            createDate = DateTime.Now,
                            exRate = rate
                        };
                        db.ExchageRates.Add(model);
                        db.SaveChangesAsync();
                    }
                }
                else
                {
                    ExchageRate model = new ExchageRate()
                    {
                        createDate = DateTime.Now,
                        exRate = rate
                    };
                    db.ExchageRates.Add(model);
                    db.SaveChangesAsync();
                }
            }
            catch
            {
                rate = 0;
            }
            return Request.CreateResponse(HttpStatusCode.OK, rate);
        }

        [Route("api/ExchangeRate")]
        [ResponseType(typeof(double))]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            double rate = 0; OhayooDB db = new OhayooDB();
            try
            {
                
                rate = ExchangeRateVietComBankUtils.VietComBank(url);
                //update exchage rate
                if (db.ExchageRates.Count() > 0)
                {
                    if (db.ExchageRates.OrderByDescending(n=>n.id).FirstOrDefault().exRate != rate)
                    {
                        ExchageRate model = new ExchageRate()
                        {
                            createDate = DateTime.Now,
                            exRate = rate
                        };
                        db.ExchageRates.Add(model);
                        db.SaveChangesAsync();
                    }
                }
                else
                {
                    ExchageRate model = new ExchageRate()
                    {
                        createDate = DateTime.Now,
                        exRate = rate
                    };
                    db.ExchageRates.Add(model);
                    db.SaveChangesAsync();
                }
                
                if (db.ExchageRateCharts.Count() == 0)
                {
                    rate = rate * 1.02;
                }
                else
                {
                    rate = rate * db.ExchageRateCharts.OrderByDescending(n => n.id).FirstOrDefault().exRate.Value;
                }
            }
            catch
            {
                if (db.ExchageRates.Count() > 0)
                {
                    rate = db.ExchageRates.OrderByDescending(n => n.id).FirstOrDefault().exRate.Value;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, rate);
        }

    }
}
