using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MenuDelDia.API.Helpers;
using MenuDelDia.API.Models.Mobile;
using MenuDelDia.Repository;

namespace MenuDelDia.API.API.Mobile
{
    public class LogosController : ApiController
    {

        #region Private Methods

        private LogoApiModel QueryLogo(Guid restaurantId)
        {
            using (var db = new AppContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var logoApiModel = db.Restaurants
                    .Where(r => r.Active && r.Id == restaurantId)
                    .Select(r => new LogoApiModel
                    {
                        LogoPath = r.LogoPath,
                        LogoName = r.LogoName,
                        LogoExtension = r.LogoExtension,
                    }).FirstOrDefault();

                if (logoApiModel != null && string.IsNullOrEmpty(logoApiModel.LogoPath) == false)
                {
                    var path = Path.Combine(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FolderLogos"]), string.Format("{0}", logoApiModel.LogoName));
                    var file = new FileInfo(path);
                    if (file.Exists)
                    {
                        logoApiModel.LogoBase64 = StringHelper.EncodeToBase64(file.FullName);
                    }
                }

                return logoApiModel;
            }
        }
        #endregion


        [HttpGet]
        [Route("api/logo/{restaurantId:guid}")]
        public HttpResponseMessage Get(Guid restaurantId)
        {
            var logo = QueryLogo(restaurantId);
            return Request.CreateResponse(HttpStatusCode.OK, logo);
        }
    }
}
