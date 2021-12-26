using WebProgrammingProject.Data;
using WebProgrammingProject.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Web.Providers.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;

namespace WebProgrammingProject.Controllers
{

    [RequireHttps]
    public class HomeController : Controller
    {

        private readonly IHtmlLocalizer<HomeController> _localizer;
        private readonly ApplicationIdentityDbContext _context;

        public HomeController(ApplicationIdentityDbContext context, IHtmlLocalizer<HomeController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }
        
        [HttpGet]
        [Authorize] // MAIN PAGE
        public IActionResult Index(int? id)
        {
            var test = _localizer["CreateBook"];
            ViewData["CreateBook"] = test;
            
            var books = _context.Products.AsQueryable();
            ViewData["Categories"] = _context.Categories.ToList();
            if (id != null)
            {
                books = books
                    .Include(i => i.Categories)
                    .Where(i => i.Categories.Any(c => c.Id == id));
                return View(books);
            }
            return View(books.Include(i => i.Categories));
        }

        [HttpPost]
        public IActionResult CultureManagement(string culture,string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return LocalRedirect(returnUrl);
        }


        // BOOK DETAILS
        public IActionResult Details(int id)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            var myBookList = _context.Products.Where(book => book.Id == id).AsQueryable();
            Product myBook = myBookList.Include(i => i.Categories).Single();
            return View(myBook);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]  // CREATE BOOK
        public IActionResult Create()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost] // CREATE BOOK
        public IActionResult Create(Product product,List<int> CategoriesId, IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileName = System.IO.Path.GetFileName(postedFile.FileName);
                string path = "wwwroot/img/books/" + fileName;
                product.ImageAdress = fileName;

                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    postedFile.CopyTo(fileStream);
                }

            }

            product.Categories = _context.Categories.Where(c => CategoriesId.Contains(c.Id)).ToList();

            _context.Products.Add(product);
            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet] // EDIT BOOK
        public IActionResult Edit(int id)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            Product book = _context.Products.Where(book => book.Id == id).Single();
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost] // EDIT BOOK
        public IActionResult Edit(Product product, List<int> CategoriesId, IFormFile postedFile, string imAdress)
        {
            if (postedFile != null)
            {
                string fileName = System.IO.Path.GetFileName(postedFile.FileName);
                string path = "wwwroot/img/" + fileName;
                product.ImageAdress = "~/img/" + fileName;

                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    postedFile.CopyTo(fileStream);
                }

                if (imAdress != null)
                {
                    path = "wwwroot/" + imAdress.Substring(imAdress.IndexOf("~") + 1);
                    System.IO.File.Delete(path);
                }
            }
            else
            {
                product.ImageAdress = imAdress;
            }

            var myBookList = _context.Products.Where(book => book.Id == product.Id).AsQueryable();
            Product myBook = myBookList.Include(i => i.Categories).Single();

            myBook.Categories = _context.Categories.Where(cat => CategoriesId.Contains(cat.Id)).ToList();
            product.Categories = myBook.Categories;
            myBook = product;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        

        [Authorize(Roles = "Admin")]
        [HttpPost] // DELETE BOOK
        public IActionResult Delete(int id)
        {
            Product book = _context.Products.Where(book => book.Id == id).Single();
            _context.Products.Remove(book);
            _context.SaveChanges();
            if (book.ImageAdress != null)
            {
                string path = "wwwroot/" + book.ImageAdress.Substring(book.ImageAdress.IndexOf("~") + 1);
                System.IO.File.Delete(path);
            }
            return RedirectToAction("Index");
        }

        [HttpGet] // SEARCH BOOK
        public IActionResult Search(string q)
        {
            // gelen q değeri ile arama işlemi yapılır.

            if(string.IsNullOrWhiteSpace(q))
                return RedirectToAction("Index");
            ViewData["Categories"] = _context.Categories.ToList();
            return View("Index", _context.Products.Where(i => i.Name.ToLower().Contains(q.ToLower())).Include(b => b.Categories));
        }
    }
}
