using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoAdsWebApp.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;

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
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Rating,Reviews")] Company company)
        {
            if (ModelState.IsValid)
            {
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
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Rating,Reviews")] Company company)
        {
            if (ModelState.IsValid)
            {
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
            db.Companies.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GeneratePdf()
        {
            var companies = db.Companies.OrderBy(c => c.Name).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                // Размер документа и поля
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Шрифт с поддержкой кириллицы
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font font = new Font(baseFont, 12);
                Font headerFont = new Font(baseFont, 14, Font.BOLD);

                // Заголовок
                Paragraph header = new Paragraph("Список компаний", headerFont);
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                document.Add(new Paragraph(" "));

                // Таблица
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3, 1, 1 });

                // Заголовки таблицы
                table.AddCell(new Phrase("Название", headerFont));
                table.AddCell(new Phrase("Рейтинг", headerFont));
                table.AddCell(new Phrase("Отзывы", headerFont));

                // Данные
                foreach (var company in companies)
                {
                    table.AddCell(new Phrase(company.Name, font));
                    table.AddCell(new Phrase(company.Rating.ToString("0.0"), font));
                    table.AddCell(new Phrase(company.Reviews ?? "0", font));
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
