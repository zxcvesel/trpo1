using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoAdsWebApp.Models;

namespace AutoAdsWebApp.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public ActionResult Index(string searchString, string sortOrder)
        {
            ViewBag.DateSort = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.RatingSort = sortOrder == "rating" ? "rating_desc" : "rating";
            ViewBag.CompanySort = sortOrder == "company" ? "company_desc" : "company";

            var reviews = db.Reviews.Include(r => r.Company).Include(r => r.User);

            if (!string.IsNullOrEmpty(searchString))
            {
                reviews = reviews.Where(r => r.Comment.Contains(searchString)
                                        || r.Company.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date":
                    reviews = reviews.OrderBy(r => r.CreatedAt);
                    break;
                case "date_desc":
                    reviews = reviews.OrderByDescending(r => r.CreatedAt);
                    break;
                case "rating":
                    reviews = reviews.OrderBy(r => r.Rating);
                    break;
                case "rating_desc":
                    reviews = reviews.OrderByDescending(r => r.Rating);
                    break;
                case "company":
                    reviews = reviews.OrderBy(r => r.Company.Name);
                    break;
                case "company_desc":
                    reviews = reviews.OrderByDescending(r => r.Company.Name);
                    break;
                default:
                    reviews = reviews.OrderByDescending(r => r.CreatedAt);
                    break;
            }

            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Login");
            return View();
        }

        // POST: Reviews/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyId,UserId,Rating,Comment,CreatedAt,IsApproved")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", review.CompanyId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Login", review.UserId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", review.CompanyId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Login", review.UserId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyId,UserId,Rating,Comment,CreatedAt,IsApproved")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", review.CompanyId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Login", review.UserId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
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
}
