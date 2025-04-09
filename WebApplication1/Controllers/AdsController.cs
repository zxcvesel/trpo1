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
    public class AdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ads
        public ActionResult Index(string searchString, string sortOrder)
        {
            ViewBag.TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.PriceSort = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.CompanySort = sortOrder == "company" ? "company_desc" : "company";
            ViewBag.CategorySort = sortOrder == "category" ? "category_desc" : "category";

            var ads = db.Ads.Include(a => a.Company).Include(a => a.Category);

            if (!String.IsNullOrEmpty(searchString))
            {
                ads = ads.Where(a => a.Title.Contains(searchString)
                                 || a.Description.Contains(searchString)
                                 || a.Company.Name.Contains(searchString)
                                 || a.Category.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    ads = ads.OrderByDescending(a => a.Title);
                    break;
                case "price":
                    ads = ads.OrderBy(a => a.Price);
                    break;
                case "price_desc":
                    ads = ads.OrderByDescending(a => a.Price);
                    break;
                case "company":
                    ads = ads.OrderBy(a => a.Company.Name);
                    break;
                case "company_desc":
                    ads = ads.OrderByDescending(a => a.Company.Name);
                    break;
                case "category":
                    ads = ads.OrderBy(a => a.Category.Name);
                    break;
                case "category_desc":
                    ads = ads.OrderByDescending(a => a.Category.Name);
                    break;
                default:
                    ads = ads.OrderBy(a => a.Title);
                    break;
            }

            return View(ads.ToList());
        }

        // GET: Ads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: Ads/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name");
            return View();
        }

        // POST: Ads/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyId,Title,Description,CategoryId,Price")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Ads.Add(ad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", ad.CategoryId);
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", ad.CompanyId);
            return View(ad);
        }

        // GET: Ads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", ad.CategoryId);
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", ad.CompanyId);
            return View(ad);
        }

        // POST: Ads/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyId,Title,Description,CategoryId,Price")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", ad.CategoryId);
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", ad.CompanyId);
            return View(ad);
        }

        // GET: Ads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ads.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ad ad = db.Ads.Find(id);
            db.Ads.Remove(ad);
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
