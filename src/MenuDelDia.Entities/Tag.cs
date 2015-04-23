using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MenuDelDia.Entities.Enums;
using Resources;

namespace MenuDelDia.Entities
{
    [Table("Tags")]
    public class Tag : EntityBase
    {
        public Tag()
        {
            Locations = new Collection<Location>();
            Restaurants = new Collection<Restaurant>();
            Menus = new Collection<Menu>();
        }

        [Key]
        public override Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Aplica a restaurantes")]
        public bool ApplyToRestaurant { get; set; }
        [DisplayName("Aplica a sucursales")]
        public bool ApplyToLocation { get; set; }
        [DisplayName("Aplica a los menus")]
        public bool ApplyToMenu { get; set; }
        
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
