using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Resources;

namespace MenuDelDia.Entities
{
    [Table("Comments")]
    public class Comment : EntityBase
    {
        public Comment()
        {

        }

        [Key]
        public override Guid Id { get; set; }

        [EmailAddress(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(MessagesResource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Fecha")]
        public DateTimeOffset DateTime { get; set; }

        [Range(0.0, 5.0, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Valor")]
        public int Value { get; set; }

        [DisplayName("Mensaje")]
        public string Message { get; set; }


        public Guid? MenuId { get; set; }
        public Menu Menu { get; set; }
    }
}
