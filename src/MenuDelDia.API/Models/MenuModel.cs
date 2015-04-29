using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MenuDelDia.Entities;

namespace MenuDelDia.API.Models
{

    public class MenuModel
    {
        public MenuModel()
        {
            Comments = new List<Comment>();
            Tags = new List<TagModel>();
            Locations = new List<MenuLocationModel>();
            MenuDays = new MenuDaysModel();
            SpecialDay = new SpecialDayModel();
        }

        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Ingredientes")]
        public string Ingredients { get; set; }

        [DisplayName("Días Menú")]
        public MenuDaysModel MenuDays { get; set; }

        [DisplayName("Día especial")]
        public SpecialDayModel SpecialDay { get; set; }

        //[DisplayName("Fecha")]
        //public DateTime? SpecialDayDate { get; set; }

        //[DisplayName("Recurrente")]
        //public bool SpecialDayRecurrent { get; set; }

        [DisplayName("Precio")]
        [Required(AllowEmptyStrings = false)]
        public double Cost { get; set; }


        [DisplayName("Activo")]
        public bool Active { get; set; }

        [DisplayName("Comentarios")]
        public virtual IList<Comment> Comments { get; set; }

        [DisplayName("Tags")]
        public virtual IList<TagModel> Tags { get; set; }

        [DisplayName("Sucursales")]
        public virtual IList<MenuLocationModel> Locations { get; set; }
    }

    public class MenuDaysModel
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


    public class SpecialDayModel
    {
        [DisplayName("Fecha")]
        public DateTime? Date { get; set; }

        [DisplayName("Recurrente")]
        public bool Recurrent { get; set; }
    }


    public class MenuLocationModel
    {
        public Guid Id { get; set; }
        public bool Selected { get; set; }
        public string Name { get; set; }
    }
}
