﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProgrammingProject.Entity;
using WebProgrammingProject.Models;

namespace WebProgrammingProject.Controllers
{
    [RequireHttps]
    [Authorize(Roles ="Owner")]
    public class AdminRoleController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;

        public AdminRoleController(RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            roleManager = _roleManager;
            userManager = _userManager;
        }

        // ROLE MANAGER PANEL (IN ADMIN PANEL)
        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }

        [HttpGet] // CREATE ROLE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]  // CREATE ROLE
        public async Task<IActionResult> Create(string name)
        {
            if(ModelState.IsValid)
            {

                var user = await roleManager.FindByNameAsync(name);
                if (user == null)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(name));
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "This role already exists");
                }
            }
            return View(name);
        }

        [HttpGet] // EDIT ROLE
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            var members = new List<ApplicationUser>();
            var nonmembers = new List<ApplicationUser>();

            foreach (var user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name)
                    ? members : nonmembers;

                list.Add(user);
            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };

            return View(model);
        }

        [HttpPost] // EDIT ROLE
        public async Task<IActionResult> Edit(RoleEditModel model)
        {
            IdentityResult result;

            if(ModelState.IsValid)
            {
                foreach(var userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if(user!=null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);

                        if(!result.Succeeded)
                        {
                            foreach(var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                foreach (var userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit", model.RoleId);
            }
        }

        [HttpPost]  // DELETE ROLE
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if(role != null)
            {
                var result = await roleManager.DeleteAsync(role);

                if(result.Succeeded)
                {
                    TempData["message"] = $"{role.Name} has been deleted.";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}
