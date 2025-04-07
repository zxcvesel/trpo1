using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAdsWebApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Ad")]
        public int AdId { get; set; }
        public Ad Ad { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "В обработке";
    }
}
