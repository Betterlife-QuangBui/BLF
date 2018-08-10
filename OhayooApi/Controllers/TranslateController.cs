using Ohayoo.Api;
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
using System.Web.Mvc;

namespace OhayooApi.Controllers
{
    public class TranslateController : ApiController
    {
        [Route("api/Transalte")]
        [ResponseType(typeof(TranslateResponseBody))]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]TranslateRequestBody jsonbody)
        {
            TranslateResponseBody model = new TranslateResponseBody();
            try
            {
                String title=TranslateUtils.Translate(jsonbody.translateRequest.text);
                model.message = "Translate successfull !!!";
                model.code = HttpStatusCodeDefine.SUCCESS.GetHashCode();
                model.data = new TranslateResponse() { title = title };
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
