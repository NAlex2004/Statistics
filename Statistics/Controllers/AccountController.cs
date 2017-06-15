using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Statistics.Models;
using Microsoft.AspNet.Identity;
using Statistics.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;

namespace Statistics.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {        
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();                
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }
            
            AppUser user = UserManager.Find(model.UserName, model.Password);
            if (user != null)
            {
                AuthenticationManager.SignOut();
                AuthenticationProperties props = new AuthenticationProperties()
                {
                    IsPersistent = true
                };
                ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(props, claim);

                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Index", "Home");

                return Redirect(returnUrl);
            }

            ModelState.AddModelError("", "Login or password incorrect.");
            model.Password = "";
            return View(model);
        }

        
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> ChangePassword()
        {
            AppUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Password = "";
                model.OldPassword = "";
                model.PasswordConfirm = "";
                return View(model);
            }

            if (model.PasswordConfirm.Equals(model.Password))
        }
    }
}