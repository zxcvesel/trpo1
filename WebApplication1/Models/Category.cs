using System.ComponentModel.DataAnnotations;

namespace AutoAdsWebApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
