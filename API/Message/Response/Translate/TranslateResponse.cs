using Ohayoo.Api.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ohayoo.Api.Message.Response.Translate
{
    public class TranslateResponse
    {
        public String title { get; set; }
    }
    public class TranslateResponseBody : ApiResponse<TranslateResponse>
    {}
}