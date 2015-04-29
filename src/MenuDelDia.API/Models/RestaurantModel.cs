using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MenuDelDia.API.Models
{
    public class RestaurantModel
    {
        public RestaurantModel()
        {
            Cards = new List<CardModel>();
            Tags = new List<TagModel>();
            Active = true;
        }

        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [DisplayName("Logo")]
        public string LogoPath { get; set; }

        public bool ClearLogoPath { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Url")]
        public string Url { get; set; }

        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Tarjetas")]
        public IList<CardModel> Cards { get; set; }

        [EmailAddress]
        [DisplayName("Nombre de usuario (email)")]
        public string EmailUserName { get; set; }

        [DisplayName("Activo")]
        public bool Active { get; set; }

        [DisplayName("Tags")]
        public IList<TagModel> Tags { get; set; }
    }

    public class CardModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Selected { get; set; }

    }
}
