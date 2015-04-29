using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MenuDelDia.API.Models;

namespace MenuDelDia.API.API.Site
{
    public class TagsApiController : ApiBaseController
    {

        public IList<TagModel> LoadTags()
        {
            return CurrentAppContext.Tags
                .Where(t => t.ApplyToRestaurant)
                .Select(t => new TagModel
                {
                    Id = t.Id,
                    Name = t.Name,
                }).ToList();
        }

        [HttpGet]
        [Route("api/site/tags")]
        public HttpResponseMessage Get()
        {
            var result = LoadTags();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
