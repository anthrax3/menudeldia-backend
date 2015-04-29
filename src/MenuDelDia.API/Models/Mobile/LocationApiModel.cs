using System;
using System.Collections.Generic;

namespace MenuDelDia.API.Models.Mobile
{
    public class LocationApiModel
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public bool Delivery { get; set; }
        public string Phone { get; set; }
        public string Streets { get; set; }
        public IList<OpenDayApiModel> OpenDays { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Distance { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string LogoName { get; set; }
        public string LogoExtension { get; set; }
        public string LogoBase64 { get; set; }
        public string LogoPath { get; set; }
        public IList<TagApiModel> Tags { get; set; }
        public string RestaurantUrl { get; set; }
        public string RestaurantEmail { get; set; }
        public string RestaurantDescription { get; set; }
    }
}