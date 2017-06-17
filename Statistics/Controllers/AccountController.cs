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
using System.Threading.Tasks;

namespace Statistics.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AccountManager _accountManager = new AccountManager();

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
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }

            bool success = _accountManager.SignIn(HttpContext.GetOwinContext(), model);

            if (!success)
            {
                ModelState.AddModelError("", "Login or password incorrect.");
                model.Password = "";
                return View(model);
            }

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);                        
        }

        
        public ActionResult Logout()
        {
            _accountManager.SignOut(HttpContext.GetOwinContext());
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult ChangePassword(string returnUrl)
        {            
            //AppUser user = await _accountManager.GetCurrentUserAsync(HttpContext.GetOwinContext());
            
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                UserName = User.Identity.Name, //user.UserName,
                ReturnUrl = returnUrl
            };
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            string currentUserName = User.Identity.Name;

            if (!ModelState.IsValid || !ModelsValidator.IsValid(model)
                ||
                ( !currentUserName.Equals(model.UserName) && !_accountManager.IsAdmin(HttpContext.GetOwinContext(), currentUserName) ))
            {
                model.Password = model.OldPassword = model.PasswordConfirm = "";                
                return View(model);
            }
            
            IdentityResult res = _accountManager.ChangePassword(HttpContext.GetOwinContext(), model);

            if (!res.Succeeded)
            {
                foreach (var error in res.Errors)
                    ModelState.AddModelError("", error);
                model.Password = model.OldPassword = model.PasswordConfirm = "";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "administrator")]
        public ActionResult EditUser(string id)
        {

        }

        [Authorize(Roles = "administrator")]
        public ActionResult Users(int? page = null)
        {
            PagerData pager = new PagerData()
            {
                ItemsPerPage = MvcApplication.ItemsPerPage,
                CurrentPage = page ?? 1
            };

            UsersListViewModel users = new UsersListViewModel()
            {
                Pager = pager,
                Users = _accountManager.GetUsers(HttpContext.GetOwinContext(), pager)
            };
            
            return View(users);
        }

        [Authorize(Roles = "administrator")]
        public PartialViewResult OneUser(UserViewModel model)
        {
            return PartialView(model);
        }

        [Authorize(Roles = "administrator")]
        public ActionResult CreateUser()
        {

        }

        [Authorize(Roles = "administrator")]
        public ActionResult DeleteUser(string id)
        {

        }
    }
}