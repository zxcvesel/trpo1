﻿using System.ComponentModel.DataAnnotations;

namespace AutoAdsWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, StringLength(50)]
        public string Role { get; set; } = "Buyer";
    }
}
