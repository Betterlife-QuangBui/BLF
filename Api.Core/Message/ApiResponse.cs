using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ohayoo.Api.Message
{
    public partial class ApiResponse<T>
    {
        public int code { get; set; }
        public String message { get; set; }
        public virtual T data { get; set; }
    }
    public partial class ApiUploadResponse<T>
    {
        public int code { get; set; }
        public String urlFile { get; set; }
        public String message { get; set; }
        public virtual T data { get; set; }
    }

    public partial class ApiResponsePager<T>
    {
        public int code { get; set; }
        public String message { get; set; }
        public bool hashMore { get; set; }
        public int rowCounts { get; set; }
        public virtual T data { get; set; }
    }
}