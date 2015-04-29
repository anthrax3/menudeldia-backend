using System;
using System.Collections.Generic;

namespace MenuDelDia.API.Models.Mobile
{
    public class MenusApiModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public double Price { get; set; }
        public MenuDaysApiModel MenuDays { get; set; }
        public SpecialDayApiModel SpecialDay { get; set; }
        //public IList<LocationApiModel> Locations { get; set; }
        public IList<TagApiModel> Tags { get; set; }
        public LocationApiModel NearestLocation { get; set; }
        public LogoApiModel Logo { get; set; }
    }
}