using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MenuDelDia.API.Models.Site
{
    public class UserRegisterApiModel
    {
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string ValidationPassword { get; set; }


        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Email)) return false;
                if (string.IsNullOrEmpty(Password)) return false;
                if (string.IsNullOrEmpty(ValidationPassword)) return false;
                if (Password.Equals(ValidationPassword, StringComparison.InvariantCulture) == false) return false;

                return true;
            }
        }
    }
}
