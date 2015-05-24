using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Elmah;
using MenuDelDia.API.Models.Site;
using MenuDelDia.API.Resources;
using MenuDelDia.Entities;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;
using LocationApiModel = MenuDelDia.API.Models.Site.LocationApiModel;

namespace MenuDelDia.API.API.Site
{
    public class RegisterApiController : ApiBaseController
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CurrentAppContext.Dispose();
            }
            base.Dispose(disposing);
        }


        private async Task<bool> ValidateUserName(string emailUserName)
        {
            return (await UserManager.FindByEmailAsync(emailUserName)) == null;
        }

        private static string UnFormatUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            url = url.ToLower();
            url = url.Contains("https://") ? url.Replace("https://", "") : url.Replace("http://", "");

            return url;
        }
        private static string FormatUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            url = url.ToLower();
            if (url.Contains("https://"))
            {
                url = url.Replace("https://", "");
                url = url.Replace("www.", "");
                return string.Format("https://www.{0}", url);
            }
            else
            {
                url = url.Replace("http://", "");
                url = url.Replace("www.", "");
                return string.Format("http://www.{0}", url);
            }
        }


        [HttpPost]
        [Route("api/site/user/register")]
        public async Task<HttpResponseMessage> UserRegister([FromBody] UserRegisterApiModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.IsValid == false)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.RegisterUserInvalidData);

                    if (await ValidateUserName(model.Email) == false)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.RegisterUserEmailExist);

                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true,
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "User");
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.RegisterUserFail);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.GeneralError);
            }
            catch (Exception exception)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.GeneralError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("api/site/companyInfo")]
        public Task<HttpResponseMessage> CompanyInfo()
        {
            var baseUrl = string.Format("{0}://{1}/{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority,
                "api/site/file/view/");

            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    var restaurant = CurrentAppContext.Restaurants
                                                      .Include(r => r.Tags)
                                                      .Include(r => r.Cards)
                                                      .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.OK, new RegisterApiModel());

                    var registerApiModel = new RegisterApiModel
                    {
                        Name = restaurant.Name,
                        Description = restaurant.Description,
                        Email = restaurant.Email,
                        Url = UnFormatUrl(restaurant.Url),
                        Cards = restaurant.Cards.Select(c => c.Id).ToList(),
                        Tags = restaurant.Tags.Select(t => t.Id).ToList(),
                        LogoPath = !string.IsNullOrEmpty(restaurant.LogoPath) ? string.Format("{0}{1}", baseUrl, restaurant.LogoPath.Substring(0, restaurant.LogoPath.LastIndexOf('.'))) : "",
                        HasImage = (string.IsNullOrEmpty(restaurant.LogoPath) == false)
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, registerApiModel);
                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.GeneralError);
                }
            });

        }

        [HttpPost]
        [Authorize]
        [Route("api/site/company/save")]
        public HttpResponseMessage CompanySave([FromBody] RegisterApiModel model)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyInvalidData);

                var restaurant = CurrentAppContext.Restaurants
                    .Include(r => r.Locations)
                    .Include(r => r.Tags)
                    .Include(r => r.Cards)
                    .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                if (restaurant == null)
                {
                    var user = CurrentAppContext.Users.FirstOrDefault(u => u.Id == CurrentUserId);

                    if (user == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.LoggedUserNotValid);

                    //new
                    var entityCards = CurrentAppContext.Cards.Where(c => model.Cards.Contains(c.Id)).ToList();
                    var entityTags = CurrentAppContext.Tags.Where(t => model.Tags.Contains(t.Id)).ToList();

                    var entityRestaurant = new Restaurant
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        Email = model.Email,
                        Description = model.Description,
                        Url = FormatUrl(model.Url),
                        Active = true,
                    };

                    entityCards.ForEach(c => entityRestaurant.Cards.Add(c));
                    entityTags.ForEach(t => entityRestaurant.Tags.Add(t));

                    user.Restaurant = entityRestaurant;
                    CurrentAppContext.Restaurants.Add(entityRestaurant);
                    CurrentAppContext.SaveChanges();
                }
                else //edit
                {
                    restaurant.Name = model.Name;
                    restaurant.Email = model.Email;
                    restaurant.Description = model.Description;
                    restaurant.Url = FormatUrl(model.Url);
                    restaurant.Active = true;

                    var entityCards = CurrentAppContext.Cards.Where(c => model.Cards.Contains(c.Id)).ToList();
                    var entityTags = CurrentAppContext.Tags.Where(t => model.Tags.Contains(t.Id)).ToList();

                    restaurant.Tags.ToList().ForEach(tag => restaurant.Tags.Remove(tag));
                    restaurant.Cards.ToList().ForEach(card => restaurant.Cards.Remove(card));

                    entityCards.ForEach(c => restaurant.Cards.Add(c));
                    entityTags.ForEach(t => restaurant.Tags.Add(t));

                    CurrentAppContext.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, MessagesResources.GeneralSaveSucces);
            }
            catch (Exception exception)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.GeneralError);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("api/site/stores")]
        public Task<HttpResponseMessage> Stores()
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {

                    var restaurant = CurrentAppContext.Restaurants
                                                      .Include(r => r.Locations.Select(o => o.OpenDays))
                                                      .AsNoTracking()
                                                      .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);

                    var models = restaurant.Locations.Select(l =>
                    {
                        var model = new LocationApiModel
                        {
                            Id = l.Id,
                            Address = l.Streets,
                            Delivery = l.Delivery,
                            Features = l.Description,
                            Identifier = l.Identifier,
                            Location = new LatLong
                            {
                                Latitude = l.Latitude,
                                Longitude = l.Longitude,
                            },
                            Phone = l.Phone,
                            Zone = l.Zone,
                        };

                        #region Days

                        var monday = new DaysApiModel(DayOfWeek.Monday);
                        var odMonday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == monday.DayOfWeek);
                        if (odMonday != null)
                        {
                            monday.From = string.Format("{0}:{1}", odMonday.OpenHour.ToString("00"), odMonday.OpenMinutes.ToString("00"));
                            monday.To = string.Format("{0}:{1}", odMonday.CloseHour.ToString("00"), odMonday.CloseMinutes.ToString("00"));
                            monday.Open = true;
                        }
                        model.Days.Add(monday);

                        var tuesday = new DaysApiModel(DayOfWeek.Tuesday);
                        var odTuesday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == tuesday.DayOfWeek);
                        if (odTuesday != null)
                        {
                            tuesday.From = string.Format("{0}:{1}", odTuesday.OpenHour.ToString("00"), odTuesday.OpenMinutes.ToString("00"));
                            tuesday.To = string.Format("{0}:{1}", odTuesday.CloseHour.ToString("00"), odTuesday.CloseMinutes.ToString("00"));
                            tuesday.Open = true;
                        }
                        model.Days.Add(tuesday);

                        var wednesday = new DaysApiModel(DayOfWeek.Wednesday);
                        var odWednesday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == wednesday.DayOfWeek);
                        if (odWednesday != null)
                        {
                            wednesday.From = string.Format("{0}:{1}", odWednesday.OpenHour.ToString("00"), odWednesday.OpenMinutes.ToString("00"));
                            wednesday.To = string.Format("{0}:{1}", odWednesday.CloseHour.ToString("00"), odWednesday.CloseMinutes.ToString("00"));
                            wednesday.Open = true;
                        }
                        model.Days.Add(wednesday);

                        var thursday = new DaysApiModel(DayOfWeek.Thursday);
                        var odThursday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == thursday.DayOfWeek);
                        if (odThursday != null)
                        {
                            thursday.From = string.Format("{0}:{1}", odThursday.OpenHour.ToString("00"), odThursday.OpenMinutes.ToString("00"));
                            thursday.To = string.Format("{0}:{1}", odThursday.CloseHour.ToString("00"), odThursday.CloseMinutes.ToString("00"));
                            thursday.Open = true;
                        }
                        model.Days.Add(thursday);

                        var friday = new DaysApiModel(DayOfWeek.Friday);
                        var odFriday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == friday.DayOfWeek);
                        if (odFriday != null)
                        {
                            friday.From = string.Format("{0}:{1}", odFriday.OpenHour.ToString("00"), odFriday.OpenMinutes.ToString("00"));
                            friday.To = string.Format("{0}:{1}", odFriday.CloseHour.ToString("00"), odFriday.CloseMinutes.ToString("00"));
                            friday.Open = true;
                        }
                        model.Days.Add(friday);

                        var saturday = new DaysApiModel(DayOfWeek.Saturday);
                        var odSaturday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == saturday.DayOfWeek);
                        if (odSaturday != null)
                        {
                            saturday.From = string.Format("{0}:{1}", odSaturday.OpenHour.ToString("00"), odSaturday.OpenMinutes.ToString("00"));
                            saturday.To = string.Format("{0}:{1}", odSaturday.CloseHour.ToString("00"), odSaturday.CloseMinutes.ToString("00"));
                            saturday.Open = true;
                        }
                        model.Days.Add(saturday);

                        var sunday = new DaysApiModel(DayOfWeek.Sunday);
                        var odSunday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == sunday.DayOfWeek);
                        if (odSunday != null)
                        {
                            sunday.From = string.Format("{0}:{1}", odSunday.OpenHour.ToString("00"), odSunday.OpenMinutes.ToString("00"));
                            sunday.To = string.Format("{0}:{1}", odSunday.CloseHour.ToString("00"), odSunday.CloseMinutes.ToString("00"));
                            sunday.Open = true;
                        }
                        model.Days.Add(sunday);
                        #endregion

                        return model;
                    }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, models);
                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [HttpPost]
        [Authorize]
        [Route("api/site/store")]
        public Task<HttpResponseMessage> Store([FromBody] LocationApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid == false)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.LocationInvalidData);


                    var restaurant = CurrentAppContext.Restaurants
                                                      .AsNoTracking()
                                                      .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);

                    var location = new Location
                    {
                        Id = Guid.NewGuid(),
                        Identifier = model.Identifier,
                        Description = model.Features,
                        Phone = model.Phone,
                        Streets = model.Address,
                        Delivery = model.Delivery,
                        Zone = model.Zone,
                        RestaurantId = restaurant.Id,
                        Latitude = model.Location.Latitude,
                        Longitude = model.Location.Longitude,
                        SpatialLocation = CreatePoint(model.Location.Latitude, model.Location.Longitude),

                        OpenDays = model.Days
                                        .Where(d => d.Open &&
                                                    string.IsNullOrEmpty(d.From) == false &&
                                                    string.IsNullOrEmpty(d.To) == false)
                                        .Select(o => new OpenDay
                                        {
                                            Id = Guid.NewGuid(),
                                            DayOfWeek = o.DayOfWeek,
                                            OpenHour = Convert.ToInt32(o.From.Split(':')[0]),
                                            OpenMinutes = Convert.ToInt32(o.From.Split(':')[1]),
                                            CloseHour = Convert.ToInt32(o.To.Split(':')[0]),
                                            CloseMinutes = Convert.ToInt32(o.To.Split(':')[1]),
                                        }).ToList(),
                    };

                    CurrentAppContext.Locations.Add(location);
                    CurrentAppContext.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, new { location.Id });

                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }


        [HttpPost]
        [Authorize]
        [Route("api/site/updatestore")]
        public Task<HttpResponseMessage> UpdateStore([FromBody] LocationApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (CurrentRestaurantId == Guid.Empty)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");

                    if (ModelState.IsValid == false)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.LocationInvalidData);


                    var restaurant = CurrentAppContext.Restaurants
                        .AsNoTracking()
                        .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);

                    var entityLocation = CurrentAppContext.Locations
                                                          .Include(l => l.Menus)
                                                          .FirstOrDefault(l => l.Id == model.Id);

                    if (entityLocation == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.LocationNotExist);

                    entityLocation.RestaurantId = CurrentRestaurantId;
                    entityLocation.Identifier = model.Identifier;
                    entityLocation.Description = model.Features;
                    entityLocation.Phone = model.Phone;
                    entityLocation.Streets = model.Address;
                    entityLocation.Delivery = model.Delivery;
                    entityLocation.Latitude = model.Location.Latitude;
                    entityLocation.Longitude = model.Location.Longitude;
                    entityLocation.SpatialLocation = CreatePoint(model.Location.Latitude, model.Location.Longitude);
                    entityLocation.Zone = model.Zone;

                    entityLocation.OpenDays.ToList().ForEach(od => entityLocation.OpenDays.Remove(od));
                    entityLocation.OpenDays = model.Days
                        .Where(d => d.Open &&
                                    string.IsNullOrEmpty(d.From) == false &&
                                    string.IsNullOrEmpty(d.To) == false)
                        .Select(o => new OpenDay
                        {
                            Id = Guid.NewGuid(),
                            DayOfWeek = o.DayOfWeek,
                            OpenHour = Convert.ToInt32(o.From.Split(':')[0]),
                            OpenMinutes = Convert.ToInt32(o.From.Split(':')[1]),
                            CloseHour = Convert.ToInt32(o.To.Split(':')[0]),
                            CloseMinutes = Convert.ToInt32(o.To.Split(':')[1]),
                        }).ToList();



                    foreach (var openDay in model.Days.Where(d => d.Open == false))
                    {
                        DaysApiModel day = openDay;
                        var menus =
                            entityLocation.Menus.Where(m => day.DayOfWeek == DayOfWeek.Monday ? m.MenuDays.Monday
                                : day.DayOfWeek == DayOfWeek.Tuesday ? m.MenuDays.Tuesday
                                : day.DayOfWeek == DayOfWeek.Wednesday ? m.MenuDays.Wednesday
                                : day.DayOfWeek == DayOfWeek.Thursday ? m.MenuDays.Thursday
                                : day.DayOfWeek == DayOfWeek.Friday ? m.MenuDays.Friday
                                : day.DayOfWeek == DayOfWeek.Saturday ? m.MenuDays.Saturday
                                : day.DayOfWeek == DayOfWeek.Sunday && m.MenuDays.Sunday)
                                .ToList();

                        foreach (var menu in menus.ToList())
                        {
                            entityLocation.Menus.Remove(menu);
                        }
                    }

                    CurrentAppContext.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, MessagesResources.GeneralSaveSucces);
                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }



        private Func<IList<Menu>, DayOfWeek, MenuApiModel> LoadMenu = (menus, day) =>
        {
            var menuForDay = new MenuApiModel { DayOfWeek = day };

            switch (day)
            {
                case DayOfWeek.Monday: menus = menus.Where(m => m.MenuDays.Monday).ToList(); break;
                case DayOfWeek.Tuesday: menus = menus.Where(m => m.MenuDays.Tuesday).ToList(); break;
                case DayOfWeek.Wednesday: menus = menus.Where(m => m.MenuDays.Wednesday).ToList(); break;
                case DayOfWeek.Thursday: menus = menus.Where(m => m.MenuDays.Thursday).ToList(); break;
                case DayOfWeek.Friday: menus = menus.Where(m => m.MenuDays.Friday).ToList(); break;
                case DayOfWeek.Saturday: menus = menus.Where(m => m.MenuDays.Saturday).ToList(); break;
                case DayOfWeek.Sunday: menus = menus.Where(m => m.MenuDays.Sunday).ToList(); break;
            }

            menus.OrderBy(m => m.Name).ForEach(m => menuForDay.Menus.Add(new DailyMenuApiModel
            {
                Name = m.Name,
                Description = m.Description,
                Price = m.Cost,
            }));
            return menuForDay;
        };

        [HttpGet]
        [Authorize]
        [Route("api/site/menus/")]
        public Task<HttpResponseMessage> Menus()
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (CurrentRestaurantId == Guid.Empty)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);

                    var restaurant = CurrentAppContext.Restaurants.FirstOrDefault(r => r.Id == CurrentRestaurantId);
                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);

                    var locations = CurrentAppContext.Locations
                                                     .Include(r => r.Menus)
                                                     .Include(r => r.OpenDays)
                                                     .Where(l => l.RestaurantId == CurrentRestaurantId)
                                                     .AsNoTracking()
                                                     .ToList();

                    if (locations.Any() == false)
                       return Request.CreateResponse(HttpStatusCode.OK, new RestaurantMenuApiModel());

                    var menus = locations.SelectMany(l => l.Menus).Distinct(new MenuComparer()).ToList();
                    var daysOfWeek = locations.SelectMany(od => od.OpenDays.Select(o => o.DayOfWeek)).ToList();

                    var menusApiModel = new List<MenuApiModel>();
                    if (daysOfWeek.Contains(DayOfWeek.Monday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Monday)); }
                    if (daysOfWeek.Contains(DayOfWeek.Tuesday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Tuesday)); }
                    if (daysOfWeek.Contains(DayOfWeek.Wednesday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Wednesday)); }
                    if (daysOfWeek.Contains(DayOfWeek.Thursday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Thursday)); }
                    if (daysOfWeek.Contains(DayOfWeek.Friday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Friday)); }
                    if (daysOfWeek.Contains(DayOfWeek.Saturday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Saturday)); }
                    if (daysOfWeek.Contains(DayOfWeek.Sunday)) { menusApiModel.Add(LoadMenu(menus, DayOfWeek.Sunday)); }


                    var model = new RestaurantMenuApiModel
                    {
                        RestaurantId = CurrentRestaurantId,
                        Menus = menusApiModel
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, model);
                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [HttpPost]
        [Authorize]
        [Route("api/site/menu")]
        public Task<HttpResponseMessage> Menu([FromBody] RestaurantMenuApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid == false)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.MenuInvalidData);

                    var restaurant = CurrentAppContext.Restaurants
                        .Include(r => r.Locations.Select(l => l.Menus))
                        .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);

                    foreach (var location in restaurant.Locations)
                    {
                        location.Menus.ToList().ForEach(m => location.Menus.Remove(m));
                    }

                    foreach (var menuModel in model.Menus.Where(m => m.IsDayOpen))
                    {
                        foreach (var dailyMenu in menuModel.Menus.Where(dm => string.IsNullOrEmpty(dm.Name) == false &&
                                                                              string.IsNullOrEmpty(dm.Description) == false))
                        {
                            var menu = new Menu
                            {
                                Active = true,
                                Id = Guid.NewGuid(),
                                Name = dailyMenu.Name,
                                Description = dailyMenu.Description,
                                Cost = dailyMenu.Price,
                                MenuDays = new MenuDays
                                {
                                    Monday = (menuModel.DayOfWeek == DayOfWeek.Monday),
                                    Tuesday = (menuModel.DayOfWeek == DayOfWeek.Tuesday),
                                    Wednesday = (menuModel.DayOfWeek == DayOfWeek.Wednesday),
                                    Thursday = (menuModel.DayOfWeek == DayOfWeek.Thursday),
                                    Friday = (menuModel.DayOfWeek == DayOfWeek.Friday),
                                    Saturday = (menuModel.DayOfWeek == DayOfWeek.Saturday),
                                    Sunday = (menuModel.DayOfWeek == DayOfWeek.Sunday),
                                },
                                SpecialDay = new SpecialDay
                                {
                                    Date = null,
                                    Recurrent = false,
                                }
                            };
                            restaurant.Locations.ForEach(l => l.Menus.Add(menu));
                        }
                    }

                    CurrentAppContext.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, MessagesResources.GeneralSaveSucces);
                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }


        [HttpPost]
        [Authorize]
        [Route("api/site/updatemenu")]
        public Task<HttpResponseMessage> UpdateMenu([FromBody] RestaurantMenuApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid == false)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.MenuInvalidData);

                    var restaurant = CurrentAppContext.Restaurants
                                                      .Include(r => r.Locations.Select(l => l.Menus))
                                                      .FirstOrDefault(r => r.Id == CurrentRestaurantId);

                    if (restaurant == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, MessagesResources.CompanyNotExist);


                    foreach (var location in restaurant.Locations)
                    {
                        location.Menus.ToList().ForEach(m => location.Menus.Remove(m));
                    }

                    foreach (var menuModel in model.Menus)
                    {
                        foreach (var dailyMenu in menuModel.Menus)
                        {
                            var menu = new Menu
                            {
                                Active = true,
                                Id = Guid.NewGuid(),
                                Name = dailyMenu.Name,
                                Description = dailyMenu.Description,
                                Cost = dailyMenu.Price,
                                MenuDays = new MenuDays { Monday = true }
                            };

                            restaurant.Locations.ForEach(l => l.Menus.Add(menu));
                        }
                    }

                    CurrentAppContext.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, MessagesResources.GeneralSaveSucces);
                }
                catch (Exception exception)
                {
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception));
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }
    }
}
