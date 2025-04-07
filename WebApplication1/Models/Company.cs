using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoAdsWebApp.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        public string Reviews { get; set; }

        public string Image { get; set; } // путь к изображению

    }
}
