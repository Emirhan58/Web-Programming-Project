using WebProgrammingProject.Models;
using WebProgrammingProject.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using WebProgrammingProject.Data;
using System.Text;
using System.Net.Mail;

namespace WebProgrammingProject.Controllers
{
    [RequireHttps]
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationIdentityDbContext dbcontext;
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signinManager;
        private readonly ILogger<AccountController> logger;
        private IWebHostEnvironment Environment;

        public AccountController(
            ApplicationIdentityDbContext _dbcontext,
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signinManager,
            ILogger<AccountController> _logger,
            IWebHostEnvironment _Environment)
        {
            dbcontext = _dbcontext;
            userManager = _userManager;
            signinManager = _signinManager;
            logger = _logger;
            Environment = _Environment;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Content("You are already logged");
            }
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                
                if (user.EmailConfirmed == false)
                {
                    ModelState.AddModelError("EmailComfirmed", "Email has not comfirmed yet.");
                    return View(model);
                }
                if (user != null)
                {
                    await signinManager.SignOutAsync();
                    var result = await signinManager.PasswordSignInAsync(user, model.Password,false,false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("Email", "Invalid Email or Password");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Content("You are already logged");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = false;

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    BuildEmailTemplate(user.Id);
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Confirm(string regId)
        {
            ViewBag.regID = regId;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult RegisterConfirm(string regId)
        {
            ApplicationUser Data = dbcontext.Users.Where(u => u.Id == regId).FirstOrDefault();
            Data.EmailConfirmed = true;
            dbcontext.Users.Update(Data);
            dbcontext.SaveChanges();
            var msg = "Your Email Is Verified!";
            return Json(msg, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }

        public async void BuildEmailTemplate(string regID)
        {
            string body = System.IO.File.ReadAllText(this.Environment.ContentRootPath + "/EmailTemplate/" + "Text" + ".cshtml");
            var regInfo = await userManager.FindByIdAsync(regID);
            var url = "https://localhost:44368/" + "Account/Confirm?regId=" + regID;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account Is Successfully Created", body, regInfo.Email);
        }

        public void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "superboookss@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if(!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("superboookss@gmail.com", "nemutluturkumdiyene58");
            try
            {
                client.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
