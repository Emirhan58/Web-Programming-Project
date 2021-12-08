
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebProgrammingProject.Data;
using WebProgrammingProject.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebProgrammingProject.Controllers
{
    public class BookApiController : ApiController
    {
        private readonly ApplicationIdentityDbContext context;
        public BookApiController(ApplicationIdentityDbContext _context)
        {
            context = _context;
        }


        
        public IEnumerable<Product> Get()
        {
            return context.Products.ToList();
        }

        
        public IHttpActionResult Get(int id)
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
        public IHttpActionResult Post([FromBody] Product newBook)
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
