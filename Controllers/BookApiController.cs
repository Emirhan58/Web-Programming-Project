
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebProgrammingProject.Data;
using WebProgrammingProject.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebProgrammingProject.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class BookApiController : ControllerBase
    {
        private readonly ApplicationIdentityDbContext context;
        public BookApiController(ApplicationIdentityDbContext _context)
        {
            context = _context;
        }


        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return context.Products.Include(b => b.Categories).ToList();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = (context.Products.Where(p => p.Id == id).Include(b => b.Categories)).FirstOrDefault();
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(book);
            }
        }

        // POST api/<BookApiController>
        [HttpPost]
        public IActionResult Post([FromBody] Product newBook)
        {
            var bookName = newBook != null ? newBook.Name : "";
            var author = newBook != null ? newBook.Author : "";
            var categories = newBook != null ? newBook.Categories : null;
            var shortDescription = newBook != null ? newBook.ShortDescription : "";
            var description = newBook != null ? newBook.Description : "";
            if (newBook != null)
            {
                context.Products.Add(newBook);
                context.SaveChanges();
            }
            return Ok(bookName);
        }

        // PUT api/<BookApiController>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookApiController>/5
        public void Delete(int id)
        {
        }
    }
}
