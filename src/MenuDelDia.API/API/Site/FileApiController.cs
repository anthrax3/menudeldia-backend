using System.Net;
using System.Net.Http;
using System.Web;

namespace MenuDelDia.API.API.Site
{
    public class FileApiController : ApiBaseController
    {
        public HttpResponseMessage Upload(HttpPostedFile file)
        {
            var x = file;

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
