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
using MenuDelDia.Repository;

namespace MenuDelDia.API.API.Mobile
{
    public class MenusController : ApiController
    {

        #region Private Methods

        public static DbGeography CreatePoint(double latitude, double longitude)
        {
            var text = string.Format(CultureInfo.InvariantCulture.NumberFormat,
                                     "POINT({0} {1})", longitude, latitude);
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(text, 4326);
        }

        private IList<MenusApiModel> QueryMenus(Guid? id = null, double? latitude = null, double? longitude = null, double? radius = null)
        {
            DbGeography currentPosition = null;
            if (latitude != null && longitude != null)
            {
                currentPosition = CreatePoint(latitude.Value, longitude.Value);
            }

            using (var db = new AppContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;


                var dayOfWeek = DateTime.Now.DayOfWeek;
                var today = DateTime.Now.Date;

                var menus = db.Menus
                    .Include(m => m.Locations.Select(l => l.OpenDays))
                    .Include(m => m.Locations.Select(l => l.Restaurant))
                    .Include(m => m.Tags)
                    .Where(m => m.Active && (id.HasValue == false || id.Value == m.Id)
                        && (
                            (m.Locations.Any(l => l.Restaurant.Active)) &&
                            (dayOfWeek == DayOfWeek.Monday && m.MenuDays.Monday) ||
                            (dayOfWeek == DayOfWeek.Tuesday && m.MenuDays.Tuesday) ||
                            (dayOfWeek == DayOfWeek.Wednesday && m.MenuDays.Wednesday) ||
                            (dayOfWeek == DayOfWeek.Thursday && m.MenuDays.Thursday) ||
                            (dayOfWeek == DayOfWeek.Friday && m.MenuDays.Friday) ||
                            (dayOfWeek == DayOfWeek.Saturday && m.MenuDays.Saturday) ||
                            (dayOfWeek == DayOfWeek.Sunday && m.MenuDays.Sunday) ||
                            (
                                (m.SpecialDay.Date.HasValue && m.SpecialDay.Date == today) ||
                                (m.SpecialDay.Date.HasValue && m.SpecialDay.Recurrent && m.SpecialDay.Date.Value.Day == today.Day)
                            )
                           )
                    && m.Locations.Any(l =>
                                            l.OpenDays.Any(od =>
                                            (od.DayOfWeek == DayOfWeek.Monday) ||
                                            (od.DayOfWeek == DayOfWeek.Tuesday) ||
                                            (od.DayOfWeek == DayOfWeek.Wednesday) ||
                                            (od.DayOfWeek == DayOfWeek.Thursday) ||
                                            (od.DayOfWeek == DayOfWeek.Friday) ||
                                            (od.DayOfWeek == DayOfWeek.Saturday) ||
                                            (od.DayOfWeek == DayOfWeek.Sunday))
                                        ) &&
                    (radius.HasValue == false || m.Locations.Any(l => l.SpatialLocation.Distance(currentPosition) <= radius))
                        )
                .Select(m =>
                    new MenusApiModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        Ingredients = m.Ingredients,
                        Price = m.Cost,
                        MenuDays = new MenuDaysApiModel
                        {
                            Monday = m.MenuDays.Monday,
                            Tuesday = m.MenuDays.Tuesday,
                            Wednesday = m.MenuDays.Wednesday,
                            Thursday = m.MenuDays.Thursday,
                            Friday = m.MenuDays.Friday,
                            Sunday = m.MenuDays.Sunday,
                            Saturday = m.MenuDays.Saturday,
                        },
                        SpecialDay = new SpecialDayApiModel
                        {
                            Date = m.SpecialDay.Date,
                            Recurrent = m.SpecialDay.Recurrent
                        },
                        NearestLocation = m.Locations
                                           .OrderBy(l => l.SpatialLocation.Distance(currentPosition))
                                           .Select(l => new LocationApiModel
                        {
                            Id = l.Id,
                            Identifier = l.Identifier,
                            Description = l.Description,
                            Delivery = l.Delivery,
                            Phone = l.Phone,
                            Streets = l.Streets,
                            RestaurantId = l.RestaurantId,
                            RestaurantName = l.Restaurant.Name,
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
                            Distance = l.SpatialLocation.Distance(currentPosition),
                        }).FirstOrDefault(),

                        Tags = m.Tags.Select(t => new TagApiModel { Id = t.Id, Name = t.Name }).ToList(),
                    })
               .ToList();

                var restaurantsIds = menus.Select(m => m.NearestLocation.RestaurantId).ToList().Distinct();

                var logos = db.Restaurants
                    .Where(r => restaurantsIds.Contains(r.Id))
                    .Select(r => new
                    {
                        r.Id,
                        logo = new LogoApiModel
                        {
                            LogoExtension = r.LogoExtension,
                            LogoName = r.LogoName,
                            LogoPath = r.LogoPath,
                        }
                    }).ToList();


                foreach (var menu in menus)
                {
                    var location = menu.NearestLocation;
                    if (location != null)
                    {
                        var logoInfo = logos.FirstOrDefault(l => l.Id == location.RestaurantId);
                        if (logoInfo != null && string.IsNullOrEmpty(logoInfo.logo.LogoPath) == false)
                        {
                            var path = Path.Combine(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FolderLogos"]), string.Format("{0}", logoInfo.logo.LogoName));
                            var file = new FileInfo(path);
                            if (file.Exists)
                            {
                                logoInfo.logo.LogoBase64 = StringHelper.EncodeToBase64(file.FullName);
                                logoInfo.logo.LogoExtension = logoInfo.logo.LogoExtension.Replace(".", "");
                            }
                        }
                        if (logoInfo != null)
                        {
                            menu.Logo = logoInfo.logo;
                        }
                    }
                }

                return menus;
            }
        }

        private IList<MenusApiModel> FilterMenus(IList<MenusApiModel> menus, MenusFilter filter)
        {
            if (filter.FilteredByLatLong)
            {
                return menus.OrderBy(m => m.NearestLocation.Distance)
                .Skip(filter.Start * filter.Size ?? 0)
                .Take(filter.Size ?? menus.Count)
                .ToList();
            }

            return menus.OrderBy(m => m.Name)
                             .Skip(filter.Start * filter.Size ?? 0)
                             .Take(filter.Size ?? menus.Count)
                             .ToList();
        }


        #endregion

        [HttpGet]
        [Route("api/menus/{id:guid}")]
        public HttpResponseMessage Get(Guid id)
        {
            var menus = FilterMenus(QueryMenus(id), new MenusFilter());
            var menu = menus.FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, menu);
        }


        [HttpGet]
        [Route("api/menus/{start:int}/{size:int}")]
        public HttpResponseMessage Get(int start, int size)
        {
            var menus = FilterMenus(QueryMenus(), new MenusFilter { Start = start, Size = size });
            return Request.CreateResponse(HttpStatusCode.OK, menus);
        }

        //[HttpGet]
        //[Route("api/menus/{latitude:double}/{longitude:double}")]
        //public HttpResponseMessage Get(double latitude, double longitude)
        //{
        //    var menus = FilterMenus(QueryMenus(), new MenusFilter { Latitude = latitude, Longitude = longitude });
        //    return Request.CreateResponse(HttpStatusCode.OK, menus);
        //}

        [HttpGet]
        [Route("api/menus/{latitude:double}/{longitude:double}/{start:int}/{size:int}")]
        public HttpResponseMessage Get(double latitude, double longitude, int start, int size)
        {
            var menus = FilterMenus(QueryMenus(null, latitude, longitude), new MenusFilter
            {
                FilteredByLatLong = true,
                Start = start,
                Size = size
            });
            return Request.CreateResponse(HttpStatusCode.OK, menus);
        }

        [HttpGet]
        [Route("api/menus/{latitude:double}/{longitude:double}/{radius:int}")]
        public HttpResponseMessage Get(double latitude, double longitude, int radius)
        {
            var menus = FilterMenus(QueryMenus(null, latitude, longitude, radius), new MenusFilter
            {
                FilteredByLatLong = true,
            });
            return Request.CreateResponse(HttpStatusCode.OK, menus);
        }

        [HttpGet]
        [Route("api/menus/{latitude:double}/{longitude:double}/{radius:int}/{start:int}/{size:int}")]
        public HttpResponseMessage Get(double latitude, double longitude, int radius, int start, int size)
        {
            var menus = FilterMenus(QueryMenus(null, latitude, longitude, radius), new MenusFilter
            {
                FilteredByLatLong = true,
                Start = start,
                Size = size
            });
            return Request.CreateResponse(HttpStatusCode.OK, menus);
        }
    }

    public class MenusFilter
    {
        public MenusFilter()
        {
            Start = null;
            Size = null;
        }

        public bool FilteredByLatLong { get; set; }
        public int? Start { get; set; }
        public int? Size { get; set; }

    }
}
