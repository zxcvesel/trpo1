﻿using AutoAdsWebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

public class AccountController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // GET: Единая форма
    public ActionResult LoginOrRegister()
    {
        ViewBag.IsLoginActive = true;
        return PartialView("LoginOrRegister");
    }

    [HttpPost]
    public ActionResult Login(string login, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        if (user != null)
        {
            FormsAuthentication.SetAuthCookie(user.Login, false);
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

    [Authorize]
    public ActionResult UserProfile()
    {
        var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
        if (currentUser == null) return HttpNotFound();

        var model = new ProfileViewModel
        {
            Reviews = db.Reviews
                .Where(r => r.UserId == currentUser.Id && r.IsApproved) // Только approved отзывы
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new UserReview
                {
                    Id = r.Id,
                    CompanyName = r.Company.Name,
                    Rating = (int)r.Rating,
                    Comment = r.Comment,
                    Date = r.CreatedAt
                }).ToList(),

            Companies = db.Companies.ToList()
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult AddReview(int CompanyId, decimal Rating, string Comment)
    {
        var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
        if (currentUser == null)
            return Json(new { success = false, error = "Пользователь не найден" });

        var review = new Review
        {
            UserId = currentUser.Id,
            CompanyId = CompanyId,
            Rating = Rating,
            Comment = Comment,
            CreatedAt = DateTime.Now,
            IsApproved = false // Модерация перед показом
        };

        db.Reviews.Add(review);
        db.SaveChanges();

        return Json(new
        {
            success = true,
            message = "Отзыв отправлен на модерацию"
        });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteReview(int id)
    {
        var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
        var review = db.Reviews.Find(id);

        if (review == null || review.UserId != currentUser?.Id)
            return Json(new { success = false, error = "Отзыв не найден или нет прав" });

        db.Reviews.Remove(review);
        db.SaveChanges();

        return Json(new { success = true });
    }

    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Index", "Home");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}