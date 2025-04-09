using AutoAdsWebApp.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

public class AccountController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: Единая форма
    public ActionResult LoginOrRegister()
    {
        ViewBag.IsLoginActive = true; // По умолчанию активна вкладка входа
        return PartialView("LoginOrRegister");
    }

    [HttpPost]
    public ActionResult Login(string login, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        if (user != null)
        {
            FormsAuthentication.SetAuthCookie(user.Login, false);
            // Возвращаем URL для перенаправления
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("UserProfile", "Account")
            });
        }
        return Json(new
        {
            success = false,
            error = "Неверный логин или пароль"
        });
    }

    [HttpPost]
    public ActionResult Register(string login, string password, string repeatPassword)
    {
        if (password != repeatPassword)
            return Json(new { error = "Пароли не совпадают" });

        if (db.Users.Any(u => u.Login == login))
            return Json(new { error = "Логин уже занят" });

        db.Users.Add(new User { Login = login, Password = password, Role = "User" });
        db.SaveChanges();

        FormsAuthentication.SetAuthCookie(login, false);
        return Json(new { success = true });
    }
    //[Authorize]
    public ActionResult UserProfile()
    {
        // Можно добавить логику загрузки данных пользователя
        return View();
    }
    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Index", "Home");
    }
}