using System;
using System.ComponentModel;

namespace MenuDelDia.API.Models
{
    public class TagModel
    {
        public Guid Id { get; set; }
        
        [DisplayName("Nombre")]
        public string Name { get; set; }

        public bool Selected { get; set; }
    }
}
