using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MenuDelDia.API.Helpers;
using MenuDelDia.API.Models.Mobile;
using MenuDelDia.API.Resources;
using MenuDelDia.Entities.Enums;
using MenuDelDia.Repository;

namespace MenuDelDia.API.API.Mobile
{
    public class RestaurantsController : ApiController
    {

        #region Private Methods

        public static DbGeography CreatePoint(double latitude, double longitude)
        {
            var text = string.Format(CultureInfo.InvariantCulture.NumberFormat,
                                     "POINT({0} {1})", longitude, latitude);
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(text, 4326);
        }

        private IList<RestaurantApiModel> QueryRestaurants(Guid? id = null)
        {
            using (var db = new AppContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;


                var restaurants = db.Restaurants
                    .Include(r => r.Locations)
                    .Include(r => r.Cards)
                    .Include(r => r.Tags)
                    .Where(r => r.Active && (id.HasValue == false || r.Id == id.Value))
                    .Select(r => new RestaurantApiModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Email = r.Email,
                        LogoPath = r.LogoPath,
                        LogoName = r.LogoName,
                        LogoExtension = r.LogoExtension,
                        Url = r.Url,
                        Tags = r.Tags.Select(t => new TagApiModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                        }).ToList(),
                        Cards = r.Cards.Select(c => new CardApiModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CardType = (c.CardType == CardType.Credit) ? ViewResources.CardTypeCredit : ViewResources.CardTypeDebit,
                        }).ToList(),
                        Locations = r.Locations.Select(l => new LocationApiModel
                        {
                            Id = l.Id,
                            Identifier = l.Identifier,
                            Description = l.Description,
                            Delivery = l.Delivery,
                            Phone = l.Phone,
                            Streets = l.Streets,
                            OpenDays = l.OpenDays.Select(od => new OpenDayApiModel
                            {
                                DayOfWeek = od.DayOfWeek,
                                OpenHour = od.OpenHour,
                                OpenMinutes = od.OpenMinutes,
                                CloseHour = od.CloseHour,
                                CloseMinutes = od.CloseMinutes,
                            }).ToList(),
                            Latitude = l.Latitude,
                            Longitude = l.Longitude,
                            Distance = -1.0,
                        }).ToList()
                    }).ToList();


                restaurants.ForEach(r =>
                {
                    if (string.IsNullOrEmpty(r.LogoPath) == false)
                    {
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FolderLogos"]), string.Format("{0}", r.LogoName));
                        var file = new FileInfo(path);
                        if (file.Exists)
                        {
                            r.LogoBase64 = StringHelper.EncodeToBase64(file.FullName);
                            r.LogoExtension = r.LogoExtension.Replace(".", "");
                        }
                    }
                });
                return restaurants;
            }
        }

        private IList<RestaurantApiModel> FilterRestaurant(IList<RestaurantApiModel> data, RestaurantFilter filter)
        {
            if (filter.Start.HasValue && filter.Size.HasValue)
            {
                return data.Skip(filter.Start.Value * filter.Size.Value)
                           .Take(filter.Size.Value)
                           .OrderBy(d=>d.Name)
                           .ToList();
            }

            return data.OrderBy(d => d.Name)
                       .ToList();
        }

        private IList<LocationApiModel> FilterStores(IList<LocationApiModel> data, StoreFilter filter)
        {
            if (filter.Start.HasValue && filter.Size.HasValue)
            {

                return data.Skip(filter.Start.Value * filter.Size.Value)
                           .Take(filter.Size.Value)
                           .ToList();
            }

            return data;
        }

        private IList<LocationApiModel> QueryLocations(double? latitude, double? longitude, Guid? id = null)
        {
            DbGeography currentPosition = null;

            if (latitude.HasValue && longitude.HasValue)
            {
                currentPosition = CreatePoint(latitude.Value, longitude.Value);    
            }
            
            
            using (var db = new AppContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                var stores = db.Locations
                    .Include(l => l.Restaurant)
                    .Where(l => id.HasValue == false || l.Id == id.Value);

                if(currentPosition != null)
                    stores = stores.OrderBy(l => l.SpatialLocation.Distance(currentPosition));

                var storeList = stores.Select(l => new LocationApiModel
                    {
                        Id = l.Id,
                        Delivery = l.Delivery,
                        Description = l.Description,
                        Distance = currentPosition != null ? l.SpatialLocation.Distance(currentPosition).Value : -1,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        Phone = l.Phone,
                        RestaurantName = l.Restaurant.Name,
                        Streets = l.Streets,
                        LogoPath = l.Restaurant.LogoPath,
                        LogoName = l.Restaurant.LogoName,
                        LogoExtension = l.Restaurant.LogoExtension,
                        Tags = l.Restaurant.Tags.Select(t => new TagApiModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                        }).ToList(),
                        RestaurantDescription = l.Restaurant.Description,
                        RestaurantEmail = l.Restaurant.Email,
                        RestaurantUrl = l.Restaurant.Url

                    })
                    .ToList();

                storeList.ForEach(r =>
                {
                    if (string.IsNullOrEmpty(r.LogoPath) == false)
                    {
                        var path = Path.Combine(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FolderLogos"]), string.Format("{0}", r.LogoName));
                        var file = new FileInfo(path);
                        if (file.Exists)
                        {
                            r.LogoBase64 = StringHelper.EncodeToBase64(file.FullName);
                            r.LogoExtension = r.LogoExtension.Replace(".", "");
                        }
                    }
                    r.Tags = r.Tags.Count > 1 ? r.Tags.Take(2).ToList() : r.Tags;
                });
                return storeList;
            }
        }
 
        #endregion

        [HttpGet]
        [Route("api/restaurants/{id:guid}")]
        public HttpResponseMessage Get(Guid id)
        {
            var restaurants = FilterRestaurant(QueryRestaurants(id), new RestaurantFilter());
            var restaurant = restaurants.FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, restaurant);
        }


        [HttpGet]
        [Route("api/restaurants/{start:int}/{size:int}")]
        public HttpResponseMessage Get(int start, int size)
        {
            var restaurants = FilterRestaurant(QueryRestaurants(), new RestaurantFilter { Start = start, Size = size });
            return Request.CreateResponse(HttpStatusCode.OK, restaurants);
        }

        [HttpGet]
        [Route("api/restaurants/store/{id:guid}")]
        public HttpResponseMessage GetStore(Guid? id)
        {
            var stores = FilterStores(QueryLocations(null, null, id), new StoreFilter());
            var store = stores.FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, store);
        }

        [HttpGet]
        [Route("api/restaurants/stores/{latitude:double}/{longitude:double}/{start:int}/{size:int}")]
        public HttpResponseMessage GetStores(int start, int size, double? latitude, double? longitude)
        {
            var stores = FilterStores(QueryLocations(latitude, longitude), new StoreFilter { Start = start, Size = size});
            return Request.CreateResponse(HttpStatusCode.OK, stores);
        }
    }

    public class RestaurantFilter
    {
        public RestaurantFilter()
        {
            Start = null;
            Size = null;
        }

        public int? Start { get; set; }
        public int? Size { get; set; }

    }

    public class StoreFilter
    {
        public StoreFilter()
        {
            Start = null;
            Size = null;
            Latitude = null;
            Longitude = null;
        }

        public int? Start { get; set; }
        public int? Size { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}

