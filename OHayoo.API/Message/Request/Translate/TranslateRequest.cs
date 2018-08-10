using Ohayoo.Api.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ohayoo.Api.Message.Translate
{
    public class TranslateRequest:Request
    {
        public string text { get; set; }
    }
    public class TranslateRequestBody
    {
        public TranslateRequest translateRequest { get; set; }
    }
}