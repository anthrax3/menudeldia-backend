using System;
using System.Collections.Generic;
namespace MenuDelDia.API.Models.Mobile
{
    public class RestaurantApiModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoPath { get; set; }
        public string LogoName { get; set; }
        public string LogoExtension { get; set; }
        public string LogoBase64 { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
        public IList<LocationApiModel> Locations { get; set; }
        public IList<TagApiModel> Tags { get; set; }
        public IList<CardApiModel> Cards { get; set; }
    }
}