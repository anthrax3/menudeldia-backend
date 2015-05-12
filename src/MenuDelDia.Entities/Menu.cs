using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Resources;

namespace MenuDelDia.Entities
{
    [Table("Menus")]
    public class Menu : EntityBase
    {
        public Menu()
        {
            Comments = new Collection<Comment>();
            Tags = new Collection<Tag>();
            Locations = new Collection<Location>();
        }

        [Key]
        public override Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Ingredientes")]
        public string Ingredients { get; set; }

        [DisplayName("Días Menú")]
        public MenuDays MenuDays { get; set; }

        [DisplayName("Día especial")]
        public SpecialDay SpecialDay { get; set; }

        [DisplayName("Precio")]
        public double Cost { get; set; }


        [DisplayName("Activo")]
        public bool Active { get; set; }

        [DisplayName("Comentarios")]
        public virtual ICollection<Comment> Comments { get; set; }

        [DisplayName("Tags")]
        public virtual ICollection<Tag> Tags { get; set; }

        [DisplayName("Sucursales")]
        public virtual ICollection<Location> Locations { get; set; }
    }

    [ComplexType]
    public class MenuDays
    {
        [DisplayName("Lunes")]
        public bool Monday { get; set; }

        [DisplayName("Martes")]
        public bool Tuesday { get; set; }

        [DisplayName("Miércoles")]
        public bool Wednesday { get; set; }

        [DisplayName("Juéves")]
        public bool Thursday { get; set; }

        [DisplayName("Viernes")]
        public bool Friday { get; set; }

        [DisplayName("Sábado")]
        public bool Saturday { get; set; }

        [DisplayName("Domingo")]
        public bool Sunday { get; set; }
    }

    [ComplexType]
    public class SpecialDay
    {
        [DisplayName("Fecha")]
        public DateTime? Date { get; set; }

        [DisplayName("Recurrente")]
        public bool Recurrent { get; set; }
    }

    public class MenuComparer : IEqualityComparer<Menu>
    {
        public bool Equals(Menu x, Menu y)
        {
            if (x == null && y == null) { return true; }
            if (x == null) { return false; }
            if (y == null) { return false; }

            return (x.Id == y.Id);
        }

        public int GetHashCode(Menu obj)
        {
            return obj.Id.GetHashCode();
        }
    }

}
