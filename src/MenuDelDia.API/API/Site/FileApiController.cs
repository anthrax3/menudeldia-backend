using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Http;

namespace MenuDelDia.API.API.Site
{
    public class FileApiController : ApiBaseController
    {
        [HttpPost]
        [Route("api/site/file/upload")]
        public HttpResponseMessage Upload()
        {
            var request = HttpContext.Current.Request;

            if (request.Files.Count == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var restaurant = CurrentAppContext.Restaurants.FirstOrDefault(r => r.Id == CurrentRestaurantId);

            if (restaurant == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var path = request.MapPath("~/RestaurantImages");
            var file = request.Files[0];
            var extension = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            var name = string.Format("{0}{1}", Guid.NewGuid(), extension);

            var fullPath = Path.Combine(path, name);
            file.SaveAs(fullPath);

            restaurant.LogoExtension = extension;
            restaurant.LogoName = name;
            restaurant.LogoPath = name;
            
            CurrentAppContext.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}


