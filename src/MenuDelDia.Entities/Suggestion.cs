using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Resources;

namespace MenuDelDia.Entities
{
    [Table("Suggestions")]
    public class Suggestion : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(MessagesResource))]
        [DisplayName("Mensaje")]
        public string Message { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
        public string Ip { get; set; }
        public string Uuid { get; set; }
    }
}
