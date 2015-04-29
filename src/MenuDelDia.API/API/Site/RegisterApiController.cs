using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MenuDelDia.API.Models.Site;
using MenuDelDia.Entities;
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

        private async Task<bool> CreateRestaurantUser(string emailUserName, string password, Guid restaurantId)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = emailUserName, Email = emailUserName, RestaurantId = restaurantId };

                var result = await UserManager.CreateAsync(user, password);
                UserManager.AddToRole(user.Id, "User");

                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return true;
                }
            }
            return false;
        }

        private async Task<bool> ValidateUserName(string emailUserName)
        {
            return (await UserManager.FindByEmailAsync(emailUserName)) == null;
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



        [HttpGet]
        [Route("api/site/companyInfo/{id:guid}")]
        public Task<HttpResponseMessage> CompanyInfo([FromUri] Guid id)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants.FirstOrDefault(r => r.Id == id);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest);

                        return Request.CreateResponse(HttpStatusCode.OK, new { name = restaurant.Name });
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [HttpPost]
        [Route("api/site/register")]
        public async Task<HttpResponseMessage> Register([FromBody] RegisterApiModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await ValidateUserName(model.EmailUserName) == false)
                    {
                        ModelState.AddModelError("EmailUserName", "El nombre de usuario ingresado ya se encuentra registrado en el sistema.");
                    }

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

                    CurrentAppContext.Restaurants.Add(entityRestaurant);
                    CurrentAppContext.SaveChanges();

                    await CreateRestaurantUser(model.EmailUserName, model.Password, entityRestaurant.Id);

                    return Request.CreateResponse(HttpStatusCode.OK, new { RestaurantId = entityRestaurant.Id });
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/site/updateregister")]
        public Task<HttpResponseMessage> UpdateRegister([FromBody] UpdateRegisterApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants
                            .Include(r => r.Locations)
                            .Include(r => r.Tags)
                            .Include(r => r.Cards)
                            .FirstOrDefault(r => r.Id == model.Id);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");




                        var entityCards = CurrentAppContext.Cards.Where(c => model.Cards.Contains(c.Id)).ToList();
                        var entityTags = CurrentAppContext.Tags.Where(t => model.Tags.Contains(t.Id)).ToList();


                        restaurant.Tags.ToList().ForEach(tag => restaurant.Tags.Remove(tag));
                        restaurant.Cards.ToList().ForEach(card => restaurant.Cards.Remove(card));

                        entityCards.ForEach(c => restaurant.Cards.Add(c));
                        entityTags.ForEach(t => restaurant.Tags.Add(t));

                        CurrentAppContext.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }






        [HttpGet]
        [Route("api/site/stores/{id:guid}")]
        public Task<HttpResponseMessage> Stores([FromUri] Guid id)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants
                                                          .Include(r => r.Locations)
                                                          .AsNoTracking()
                                                          .FirstOrDefault(r => r.Id == id);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");


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
                                RestaurantId = l.RestaurantId,
                                Zone = l.Zone,
                            };

                            #region Days

                            var monday = new DaysApiModel { DayOfWeek = DayOfWeek.Monday };
                            var odMonday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == monday.DayOfWeek);
                            if (odMonday != null)
                            {
                                monday.From = string.Format("{0}:{1}", odMonday.OpenHour, odMonday.OpenMinutes);
                                monday.To = string.Format("{0}:{1}", odMonday.CloseHour, odMonday.CloseMinutes);
                                monday.Open = true;
                            }

                            var tuesday = new DaysApiModel { DayOfWeek = DayOfWeek.Tuesday };
                            var odTuesday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == tuesday.DayOfWeek);
                            if (odTuesday != null)
                            {
                                tuesday.From = string.Format("{0}:{1}", odTuesday.OpenHour, odTuesday.OpenMinutes);
                                tuesday.To = string.Format("{0}:{1}", odTuesday.CloseHour, odTuesday.CloseMinutes);
                                tuesday.Open = true;
                            }

                            var wednesday = new DaysApiModel { DayOfWeek = DayOfWeek.Wednesday };
                            var odWednesday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == wednesday.DayOfWeek);
                            if (odWednesday != null)
                            {
                                wednesday.From = string.Format("{0}:{1}", odWednesday.OpenHour, odWednesday.OpenMinutes);
                                wednesday.To = string.Format("{0}:{1}", odWednesday.CloseHour, odWednesday.CloseMinutes);
                                wednesday.Open = true;
                            }

                            var thursday = new DaysApiModel { DayOfWeek = DayOfWeek.Thursday };
                            var odThursday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == thursday.DayOfWeek);
                            if (odThursday != null)
                            {
                                thursday.From = string.Format("{0}:{1}", odThursday.OpenHour, odThursday.OpenMinutes);
                                thursday.To = string.Format("{0}:{1}", odThursday.CloseHour, odThursday.CloseMinutes);
                                thursday.Open = true;
                            }

                            var friday = new DaysApiModel { DayOfWeek = DayOfWeek.Friday };
                            var odFriday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == friday.DayOfWeek);
                            if (odFriday != null)
                            {
                                friday.From = string.Format("{0}:{1}", odFriday.OpenHour, odFriday.OpenMinutes);
                                friday.To = string.Format("{0}:{1}", odFriday.CloseHour, odFriday.CloseMinutes);
                                friday.Open = true;
                            }

                            var sunday = new DaysApiModel { DayOfWeek = DayOfWeek.Sunday };
                            var odSunday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == sunday.DayOfWeek);
                            if (odSunday != null)
                            {
                                sunday.From = string.Format("{0}:{1}", odSunday.OpenHour, odSunday.OpenMinutes);
                                sunday.To = string.Format("{0}:{1}", odSunday.CloseHour, odSunday.CloseMinutes);
                                sunday.Open = true;
                            }

                            var saturday = new DaysApiModel { DayOfWeek = DayOfWeek.Saturday };
                            var odSaturday = l.OpenDays.FirstOrDefault(od => od.DayOfWeek == saturday.DayOfWeek);
                            if (odSaturday != null)
                            {
                                saturday.From = string.Format("{0}:{1}", odSaturday.OpenHour, odSaturday.OpenMinutes);
                                saturday.To = string.Format("{0}:{1}", odSaturday.CloseHour, odSaturday.CloseMinutes);
                                saturday.Open = true;
                            }

                            #endregion

                            return model;
                        }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, models);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [HttpPost]
        [Route("api/site/store")]
        public Task<HttpResponseMessage> Store([FromBody] LocationApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants
                                                          .AsNoTracking()
                                                          .FirstOrDefault(r => r.Id == model.RestaurantId);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");

                        var location = new Location
                        {
                            Id = Guid.NewGuid(),
                            RestaurantId = model.RestaurantId,
                            Identifier = model.Identifier,
                            Description = model.Features,
                            Phone = model.Phone,
                            Streets = model.Address,
                            Delivery = model.Delivery,
                            Zone = model.Zone,

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
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }


        [HttpPost]
        [Route("api/site/updatestore")]
        public Task<HttpResponseMessage> UpdateStore([FromBody] LocationApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants
                                                          .AsNoTracking()
                                                          .FirstOrDefault(r => r.Id == model.RestaurantId);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");

                        var entityLocation = CurrentAppContext.Locations.FirstOrDefault(l => l.Id == model.Id);

                        if (entityLocation == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Location does not exist.");

                        entityLocation.RestaurantId = model.RestaurantId;
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
                        CurrentAppContext.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }





        [HttpGet]
        [Route("api/site/menus/{id:guid}")]
        public Task<HttpResponseMessage> Menus([FromUri] Guid id)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants.FirstOrDefault(r => r.Id == id);
                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");

                        var locations = CurrentAppContext.Locations
                                                          .Include(r => r.Menus)
                                                          .Where(l => l.RestaurantId == id)
                                                          .AsNoTracking()
                                                          .ToList();
                        if (locations.Any() == false || locations.All(l => l.Menus.Any()) == false)
                            return Request.CreateResponse(HttpStatusCode.OK);

                        var menus = locations.SelectMany(l => l.Menus).Distinct(new MenuComparer()) .ToList();
                        var monday = new MenuApiModel { DayOfWeek = DayOfWeek.Monday };
                        menus.Where(m => m.MenuDays.Monday).ForEach(m => monday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));
                        var tuesday = new MenuApiModel { DayOfWeek = DayOfWeek.Tuesday };
                        menus.Where(m => m.MenuDays.Tuesday).ForEach(m => tuesday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));
                        var wednesday = new MenuApiModel { DayOfWeek = DayOfWeek.Wednesday };
                        menus.Where(m => m.MenuDays.Wednesday).ForEach(m => wednesday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));
                        var thursday = new MenuApiModel { DayOfWeek = DayOfWeek.Thursday };
                        menus.Where(m => m.MenuDays.Thursday).ForEach(m => thursday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));
                        var friday = new MenuApiModel { DayOfWeek = DayOfWeek.Friday };
                        menus.Where(m => m.MenuDays.Friday).ForEach(m => friday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));
                        var saturday = new MenuApiModel { DayOfWeek = DayOfWeek.Saturday };
                        menus.Where(m => m.MenuDays.Saturday).ForEach(m => saturday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));
                        var sunday = new MenuApiModel { DayOfWeek = DayOfWeek.Sunday };
                        menus.Where(m => m.MenuDays.Sunday).ForEach(m => sunday.Menus.Add(new DailyMenuApiModel
                        {
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Cost,
                        }));

                        var model = new RestaurantMenuApiModel
                        {
                            RestaurantId = id,
                            Menus = new List<MenuApiModel>
                            {
                                monday,
                                tuesday,
                                wednesday,
                                thursday,
                                friday,
                                saturday,
                                sunday
                            }
                        };

                        return Request.CreateResponse(HttpStatusCode.OK, model);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }

        [HttpPost]
        [Route("api/site/menu")]
        public Task<HttpResponseMessage> Menu([FromBody] RestaurantMenuApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants
                                                          .Include(r => r.Locations.Select(l => l.Menus))
                                                          .FirstOrDefault(r => r.Id == model.RestaurantId);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");

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

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }


        [HttpPost]
        [Route("api/site/updatemenu")]
        public Task<HttpResponseMessage> UpdateMenu([FromBody] RestaurantMenuApiModel model)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var restaurant = CurrentAppContext.Restaurants
                                                          .Include(r => r.Locations.Select(l => l.Menus))
                                                          .FirstOrDefault(r => r.Id == model.RestaurantId);

                        if (restaurant == null)
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Restaurant does not exist.");


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

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            });
        }
    }
}
