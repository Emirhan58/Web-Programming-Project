using WebProgrammingProject.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Providers.Entities;
using WebProgrammingProject.Models;
using Microsoft.AspNetCore.Authorization;
using WebProgrammingProject.Data;

namespace WebProgrammingProject.Controllers
{
    [RequireHttps]
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IPasswordValidator<ApplicationUser> passwordValidator;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        private readonly ApplicationIdentityDbContext context;
        public AdminController(UserManager<ApplicationUser> _userManager, 
            IPasswordValidator<ApplicationUser> passValidator, 
            IPasswordHasher<ApplicationUser> passHasher,
            ApplicationIdentityDbContext _context)
        {
            userManager = _userManager;
            passwordValidator = passValidator;
            passwordHasher = passHasher;
            context = _context;
        }

        // ADMIN PANEL USERS
        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        // ADMIN PANEL CATEGORIES
        public IActionResult Categories()
        {
            return View(context.Categories.ToList());
        }


        [HttpGet] // CREATE CATEGORY
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost] // CREATE CATEGORY
        public IActionResult CreateCategory(string CategoryName)
        {
            if(ModelState.IsValid)
            {
                if(!context.Categories.Where(c => c.Name == CategoryName).Any())
                {
                    Category category = new Category() { Name = CategoryName };
                    context.Categories.Add(category);
                    context.SaveChanges();
                    return View("Categories", context.Categories.ToList());
                }
                else
                {
                    ModelState.AddModelError("", "This category already exists");
                    return View();
                }
            }

            return View();
        }

        [HttpGet] // UPDATE CATEGORY
        public IActionResult UpdateCategory(int Id)
        {
            Category category = context.Categories.Where(cat => cat.Id == Id).Single();
            return View(category);
        }

        [HttpPost] // UPDATE CATEGORY
        public IActionResult UpdateCategory(int Id, string Name)
        {
            Category category = context.Categories.Where(cat => cat.Id == Id).Single();
            
            if(!context.Categories.Where(cat => cat.Name == Name).Any())
            {
                category.Name = Name;
                context.Categories.Update(category);
                context.SaveChanges();
                return View("Categories", context.Categories.ToList());
            }
            else
            {
                ModelState.AddModelError("", "This category already exists");
                return View(category);
            }
        }

        [HttpPost] // DELETE CATEGORY
        public IActionResult DeleteCategory(int Id)
        {
            Category category = context.Categories.Where(cat => cat.Id == Id).Single();
            context.Categories.Remove(category);
            context.SaveChanges();
            return View("Categories", context.Categories.ToList());
        }

        [HttpGet] // CREATE USER
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // CREATE USER
        public async Task<IActionResult> Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var result2 = await userManager.AddToRoleAsync(user, "User");
                    
                    if (result2.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost] // DELETE USER
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
        
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "user not found");
            }
            return View("Index", userManager.Users);
        }

        [HttpGet] // EDIT USER
        public async Task<IActionResult> Update(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user!=null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]  // EDIT USER
        public async Task<IActionResult> Update(string Id,string Password, string Email)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user != null)
            {
                user.Email = Email;

                IdentityResult validPass = null;

                if (!string.IsNullOrEmpty(Password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, Password);
                
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, Password);
                    }
                    else
                    {
                        foreach (var item in validPass.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }

                if (validPass.Succeeded)
                {
                    var result = await userManager.UpdateAsync(user);
                
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }

            return View(user);
        }

    }
}
