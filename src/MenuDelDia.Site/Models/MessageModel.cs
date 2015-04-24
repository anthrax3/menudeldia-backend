

using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace MenuDelDia.Site.Models
{
    public class MessageModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar una dirección de correo.")]
        [EmailAddress(ErrorMessage = "Debe ingresar una dirección de correo válida.")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un asunto.")]
        [MaxLength(255, ErrorMessage = "El asunto no puede superar los 255 caracteres")]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un mensaje.")]
        [MaxLength(1000, ErrorMessage = "El mensaje no puede superar los 1000 caracteres")]
        public string Message { get; set; }

    }
}