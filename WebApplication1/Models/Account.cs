using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoAdsWebApp.Models
{
    public class AccountViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "������ ���� �����������")]
        public string Login { get; set; }

        [Required(ErrorMessage = "������ ���� �����������")]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "������ ���� �����������")]
        public string Login { get; set; }

        [Required(ErrorMessage = "������ ���� �����������")]
        public string Password { get; set; }

        [Required(ErrorMessage = "������ ���� �����������")]
        [Compare("Password", ErrorMessage = "������ �� ���������")]
        public string RepeatPassword { get; set; }
        
    }
}