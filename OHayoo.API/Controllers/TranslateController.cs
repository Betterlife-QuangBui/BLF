﻿using Ohayoo.Api;
using Ohayoo.Api.Message.Response.Translate;
using Ohayoo.Api.Message.Translate;
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
    public class TranslateController : ApiController
    {
        [Route("api/Transalte")]
        [ResponseType(typeof(TranslateResponse))]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]TranslateRequest jsonbody)
        {
            String title = "";
            TranslateResponse trans = new TranslateResponse();
            try
            {
                title = TranslateUtils.TranslateText(jsonbody.text);
            }
            catch { }
            trans.title = title;
            return Request.CreateResponse(HttpStatusCode.OK, trans);
        }
    }
}