using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenuDelDia.Entities
{
    [Table("OpenDays")]
    public class OpenDay:EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        
        [DisplayName("Día de la semana")]
        public DayOfWeek DayOfWeek { get; set; }

        [DisplayName("Hora de apertura")]
        public int OpenHour { get; set; }
        public int OpenMinutes { get; set; }

        [DisplayName("Hora de cierre")]
        public int CloseHour { get; set; }
        public int CloseMinutes { get; set; }

    }
}
