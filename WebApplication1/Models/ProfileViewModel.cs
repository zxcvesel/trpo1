using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoAdsWebApp.Models
{
    public class ProfileViewModel
    {
        public List<UserReview> Reviews { get; set; }
        public List<Company> Companies { get; set; }
    }

    public class UserReview
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}