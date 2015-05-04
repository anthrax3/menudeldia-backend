using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MenuDelDia.API.Models;

namespace MenuDelDia.API.API.Site
{
    public class RestaurantApiController : ApiBaseController
    {


        [HttpGet]
        [Route("api/site/restaurant")]
        public HttpResponseMessage Get()
        {
            var restaurant = CurrentAppContext.Restaurants.Where(r => r.Id == CurrentRestaurantId).AsNoTracking().FirstOrDefault();
            if (restaurant != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, restaurant);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
