using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Resources;

namespace MenuDelDia.Entities
{
    [Table("Restaurants")]
    public class Restaurant : EntityBase
    {
        public Restaurant()
        {
            Locations = new Collection<Location>();
            Tags = new Collection<Tag>();
            Cards = new Collection<Card>();
        }

        [Key]
        public override Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Logo")]
        public string LogoPath { get; set; }

        public string LogoExtension { get; set; }
        public string LogoName { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Url")]
        public string Url { get; set; }

        [EmailAddress(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Sucursales")]
        public virtual ICollection<Location> Locations { get; set; }

        [DisplayName("Tags")]
        public virtual ICollection<Tag> Tags { get; set; }

        [DisplayName("Tarjetas")]
        public virtual ICollection<Card> Cards { get; set; }

        public bool Active { get; set; }
    }
}
