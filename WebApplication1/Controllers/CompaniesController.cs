using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoAdsWebApp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace AutoAdsWebApp.Controllers
{
    public class CompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Companies
        public ActionResult Index(string searchString, string sortOrder)
        {
            ViewBag.NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.RatingSort = sortOrder == "rating" ? "rating_desc" : "rating";

            var companies = from c in db.Companies select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(c => c.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    companies = companies.OrderByDescending(c => c.Name);
                    break;
                case "rating":
                    companies = companies.OrderBy(c => c.Rating);
                    break;
                case "rating_desc":
                    companies = companies.OrderByDescending(c => c.Rating);
                    break;
                default:
                    companies = companies.OrderBy(c => c.Name);
                    break;
            }

            return View(companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Rating,Reviews")] Company company, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    imageFile.SaveAs(path);
                    company.Image = fileName;
                }

                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Rating,Reviews,Image")] Company company, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Удаляем старое изображение
                    if (!string.IsNullOrEmpty(company.Image))
                    {
                        var oldPath = Path.Combine(Server.MapPath("~/images"), company.Image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // Сохраняем новое изображение
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    imageFile.SaveAs(path);
                    company.Image = fileName;
                }

                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);

            // Удаляем изображение
            if (!string.IsNullOrEmpty(company.Image))
            {
                var path = Path.Combine(Server.MapPath("~/images"), company.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            db.Companies.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GeneratePdf()
        {
            var companies = db.Companies.OrderBy(c => c.Name).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font font = new Font(baseFont, 12);
                Font headerFont = new Font(baseFont, 14, Font.BOLD);

                Paragraph header = new Paragraph("Список компаний", headerFont);
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                document.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3, 1, 1, 2 });

                table.AddCell(new Phrase("Название", headerFont));
                table.AddCell(new Phrase("Рейтинг", headerFont));
                table.AddCell(new Phrase("Отзывы", headerFont));
                table.AddCell(new Phrase("Изображение", headerFont));

                foreach (var company in companies)
                {
                    table.AddCell(new Phrase(company.Name, font));
                    table.AddCell(new Phrase(company.Rating.ToString("0.0"), font));
                    table.AddCell(new Phrase(company.Reviews ?? "0", font));
                    table.AddCell(new Phrase(company.Image ?? "Нет изображения", font));
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Список_компаний.pdf");
            }
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