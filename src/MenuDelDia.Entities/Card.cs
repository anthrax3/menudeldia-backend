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
    [Table("Cards")]
    public class Card : EntityBase
    {
        public Card()
        {
            Restaurants = new Collection<Restaurant>();
        }

        [Key]
        public override Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Nombre")]
        public string Name { get; set; }
        [DisplayName("Tipo Tarjeta")]
        public CardType CardType { get; set; }

        public ICollection<Restaurant> Restaurants { get; set; }
    }
}