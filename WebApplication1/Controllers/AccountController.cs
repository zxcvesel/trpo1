using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoAdsWebApp.Models;
using System.Web.Security;

namespace AutoAdsWebApp.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext(); // Замени на свой контекст данных

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["UserLogin"] = user.Login;
                    Session["UserRole"] = user.Role;

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Неверный логин или пароль");
            }
            return View(model);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Login == model.Login);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                    return View(model);
                }

                var newUser = new User
                {
                    Login = model.Login,
                    Password = model.Password,
                    Role = "Buyer"
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
