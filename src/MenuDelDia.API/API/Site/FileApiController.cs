using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Http;

namespace MenuDelDia.API.API.Site
{
    public class FileApiController : ApiBaseController
    {
        [HttpPost]
        [Authorize]
        [Route("api/site/file/upload")]
        public HttpResponseMessage Upload()
        {
            var request = HttpContext.Current.Request;

            if (request.Files.Count == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var restaurant = CurrentAppContext.Restaurants.FirstOrDefault(r => r.Id == CurrentRestaurantId);

            if (restaurant == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var oldImagePath = restaurant.LogoPath;
            var oldImageName = restaurant.LogoName;

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

            if (string.IsNullOrEmpty(oldImagePath) == false)
            {
                var oldImage = new FileInfo(Path.Combine(path, oldImageName));
                if (oldImage.Exists)
                    oldImage.Delete();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet]
        [Route("api/site/file/view/{imageName}")]
        public HttpResponseMessage View(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return null;

            var path = HttpContext.Current.Request.MapPath("~/RestaurantImages");

            var directory = new DirectoryInfo(path);
            var file = directory.EnumerateFiles().FirstOrDefault(f => f.Name.StartsWith(imageName));

            if (file == null)
                return null;

            var extension = file.Name.Substring(file.Name.LastIndexOf('.'));

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            Stream stream = new MemoryStream(File.ReadAllBytes(file.FullName));
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(file.Name);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(string.Format("image/{0}", extension.Replace(".", "").ToLower()));
            result.Content.Headers.ContentLength = stream.Length;
            return result;

        }
    }
}