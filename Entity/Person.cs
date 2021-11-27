using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingFormsWeb.Entity
{
    public class Person 
    {
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Biography { get; set; }
        public string PlaceOfBirth { get; set; }

    }

    public class ApplicationUser : IdentityUser
    {
        public string ImageAdress { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class Author : Person
    {
        public int AuthorId { get; set; }
        [Required]
        public List<Product> Products { get; set; }

    }
}
