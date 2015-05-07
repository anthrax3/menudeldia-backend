using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Resources;

namespace MenuDelDia.Entities
{
    [Table("Locations")]
    public class Location : EntityBase
    {
        public Location()
        {
            OpenDays = new Collection<OpenDay>();
            Tags = new Collection<Tag>();
            Menus = new Collection<Menu>();
        }

        [Key]
        public override Guid Id { get; set; }


        [DisplayName("Identificador")]
        public string Identifier { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Dirección")]
        public string Streets { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Teléfono")]
        public string Phone { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Delivery")]
        public bool Delivery { get; set; }

        [DisplayName("Latitud")]
        public double Latitude { get; set; }

        [DisplayName("Longitud")]
        public double Longitude { get; set; }

        public DbGeography SpatialLocation { get; set; }

        public Guid RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public virtual Restaurant Restaurant { get; set; }

        [DisplayName("Días abiertos")]
        public virtual ICollection<OpenDay> OpenDays { get; set; }

        [DisplayName("Tags")]
        public virtual ICollection<Tag> Tags { get; set; }

        [DisplayName("Menús")]
        public virtual ICollection<Menu> Menus { get; set; }

        public string Zone { get; set; }
    }
}
