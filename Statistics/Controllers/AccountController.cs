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
                ( !currentUserName.Equals(model.UserName) && !AccountManager.IsAdmin(HttpContext.GetOwinContext(), currentUserName) ))
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

        [Authorize(Roles = "administrators")]
        public ActionResult EditUser(string id)
        {
            return View(id);
        }

        [Authorize(Roles = "administrators")]
        public ActionResult EditUserData(string id)
        {
            var user = _accountManager.GetUsers(HttpContext.GetOwinContext(), u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
                return null;
            return PartialView(user);
        }

        protected ActionResult ReturnFromEditUser(UserViewModel model, bool success)
        {
            model.Password = model.PasswordConfirm = "";

            if (success)
            {                
                if (Request.IsAjaxRequest())
                    return PartialView("OneUser", model);
                else
                    return View("Users");
            }
            
            if (Request.IsAjaxRequest())
                return PartialView("EditUserData", model);
            return View("EditUser", model.Id);            
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]        
        public ActionResult EditUserData(UserViewModel model)
        {
            if (model.submit == "Cancel")
                return ReturnFromEditUser(model, true);

            if (ModelState.IsValid)
            {
                var updResult = _accountManager.UpdateUser(HttpContext.GetOwinContext(), model);

                if (!updResult.Succeeded)
                {
                    foreach (var err in updResult.Errors)
                        ModelState.AddModelError("", err);

                    return ReturnFromEditUser(model, false);
                }

                return ReturnFromEditUser(model, true);
            }

            return ReturnFromEditUser(model, false);
        }

        [Authorize(Roles = "administrators")]
        public ActionResult Users(int? page = null)
        {            
            return View(page);
        }

        [Authorize(Roles = "administrators")]
        public PartialViewResult UsersData(int? page = null)
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

            return PartialView(users);
        }

        [Authorize(Roles = "administrators")]
        public PartialViewResult OneUser(UserViewModel model)
        {
            return PartialView(model);
        }

        [Authorize(Roles = "administrators")]        
        public PartialViewResult OneUserById(string id)
        {
            var user = _accountManager.GetUsers(HttpContext.GetOwinContext(), u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
                return null;
            return PartialView("OneUser", user);
        }

        [Authorize(Roles = "administrators")]
        public ActionResult CreateUser()
        {
            return View();
        }

        [Authorize(Roles = "administrator")]
        public ActionResult DeleteUser(string id)
        {
            return View();
        }

        [Authorize(Roles = "administrator")]
        public ActionResult UserRoles(string id)
        {
            var user = _accountManager.GetUsers(HttpContext.GetOwinContext(), u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
                return null;
            RolesViewModel model = new RolesViewModel()
            {
                UserId = id,
                UserName = user.UserName,
                Roles = new SortedDictionary<string, bool>()
            };
            
            foreach (var role in user.Roles)
                model.Roles.Add(role, true);

            var roles = _accountManager.GetRoles(HttpContext.GetOwinContext());
            foreach (var role in roles)
                model.Roles.Add(role, false);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "administrator")]
        public ActionResult UserRoles(RolesViewModel model)
        {
            if (model != null)
            {
                var context = HttpContext.GetOwinContext();
                var user = _accountManager.GetUsers(context, u => u.Id.Equals(model.UserId)).FirstOrDefault();
                if (user != null)
                {
                    user.Roles = model.Roles.Where(r => r.Value).Select(r => r.Key).ToArray();
                    var res = _accountManager.UpdateUser(context, user);

                    if (!res.Succeeded)
                    {
                        ErrorViewModel errModel = new ErrorViewModel()
                        {
                            Errors = new List<string>(),
                            ReturnUrl = Url.Action("Users")
                        };
                        foreach (var error in res.Errors)
                            errModel.Errors.Add(error);
                        return View("ErrorView", errModel);
                    }
                }
            }

            return RedirectToAction("Users");
        }
    }
}