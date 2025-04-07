using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoAdsWebApp.Models
{
    // Models/Review.cs
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.5, 5.0)]
        public decimal Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;

        // Навигационные свойства
        public virtual Company Company { get; set; }
        public virtual User User { get; set; }
    }
}