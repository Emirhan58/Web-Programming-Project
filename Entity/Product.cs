using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingFormsWeb.Entity
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageAdress { get; set; }
        [Required]
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public List<Category> Categories { get; set; }
    }
}
