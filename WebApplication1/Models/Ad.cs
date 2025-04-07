using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAdsWebApp.Models
{
    public class Ad
    {
        public int Id { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public decimal Price { get; set; }
    }
}
