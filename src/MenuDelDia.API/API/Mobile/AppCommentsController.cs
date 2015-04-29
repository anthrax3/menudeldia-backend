using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MenuDelDia.API.Models.Mobile;
using MenuDelDia.Entities;
using MenuDelDia.Repository;
using MenuDelDia.Site.Helpers;

namespace MenuDelDia.API.API.Mobile
{
    public class AppCommentsController : ApiController
    {
        [HttpPost]
        [Route("api/appcomment")]
        public HttpResponseMessage Post([FromBody]AppComentApiModel comment)
        {
            if (ModelState.IsValid)
            {
                using (var db = new AppContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;

                    db.AppComments.Add(new AppComment
                    {
                        Id = Guid.NewGuid(),
                        CreationDateTime = DateTimeOffset.Now,
                        Message = comment.Message,
                        Ip = HttpContext.Current.Request.GetIPAddress(),
                        Uuid = comment.Uuid
                    });
                    db.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
