using API.Message.Response.Category;
using Ohayoo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    
    public class CategoryController : ApiController
    {
        [Route("api/Category")]
        [ResponseType(typeof(List<CategoryResponse>))]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            List<CategoryResponse> list = new List<CategoryResponse>();
            try
            {
                OhayooDB db = new OhayooDB();
                list.AddRange(db.Categories.Select(n => new CategoryResponse() { 
                            description=n.description,id=n.id,
                            is_leaf=n.is_leaf.Value,level=n.levelCate.Value,
                            name=n.name,parent_id=n.parent_id.Value,
                            status=n.status.Value
                }));
            }
            catch { }
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }
    }
}
