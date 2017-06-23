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
        private IAccountManager _accountManager;

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
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
            if (model != null)
            {
                string currentUserName = User.Identity.Name;

                if (!ModelState.IsValid
                    ||
                    (!currentUserName.Equals(model.UserName) && !AccountManager.IsAdmin(HttpContext.GetOwinContext(), currentUserName)))
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
            }                        

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "administrators")]
        public ActionResult EditUser(string id)
        {
            if (TempData["modelErrors"] != null)
            {
                List<string> errors = (List<string>)TempData["modelErrors"];
                foreach (var error in errors)
                    ModelState.AddModelError("", error);
            }

            return View("EditUser", model: id);
        }

        [Authorize(Roles = "administrators")]
        public ActionResult EditUserData(string id)
        {
            var user = _accountManager.GetUsers(HttpContext.GetOwinContext(), u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
                return null;
            user.EditMode = UserEditMode.Edit;
            return PartialView("EditUserData", model: user);
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
            //return View("EditUser", model: model.Id);
            List<string> errors = new List<string>();
            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors.Add(error.ErrorMessage);
            TempData["modelErrors"] = errors;
            Response.Redirect(Url.Action("EditUser", new { id = model.Id}), true);

            return null;
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
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(UserViewModel model)
        {
            if (model == null)
                return RedirectToAction("Users");

            if (!ModelState.IsValid)
            {
                model.Password = model.PasswordConfirm = "";
                return View(model);
            }

            var result = _accountManager.CreateUser(HttpContext.GetOwinContext(), model);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);
                model.Password = model.PasswordConfirm = "";
                return View(model);
            }

            return RedirectToAction("UserRoles", new { id = model.Id });
        }

        [Authorize(Roles = "administrators")]
        public ActionResult DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Users");

            var context = HttpContext.GetOwinContext();
            var user = _accountManager.GetUsers(context, u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
                return RedirectToAction("Users");            

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]
        public ActionResult DeleteUser(UserViewModel model)
        {
            var result = _accountManager.DeleteUser(HttpContext.GetOwinContext(), model.Id);
            if (result.Succeeded)
                return RedirectToAction("Users");

            ErrorViewModel errorModel = new ErrorViewModel()
            {
                ReturnUrl = Url.Action("Users"),
                Errors = new List<string>()
            };
            foreach (var error in result.Errors)
                errorModel.Errors.Add(error);

            return View("ErrorView");
        }

        protected RolesViewModel CreateRolesModel(UserViewModel userModel)
        {
            if (userModel == null)
                return null;

            RolesViewModel rolesModel = new RolesViewModel()
            {
                UserId = userModel.Id,
                UserName = userModel.UserName,
                Roles = new SortedDictionary<string, bool>()
            };

            if (userModel.Roles != null)
            {
                foreach (var role in userModel.Roles)
                    rolesModel.Roles.Add(role, true);
            }            

            var roles = _accountManager.GetRoles(HttpContext.GetOwinContext()).Except(rolesModel.Roles.Keys);

            foreach (var role in roles)
                rolesModel.Roles.Add(role, false);

            return rolesModel;
        }

        [Authorize(Roles = "administrators")]
        public ActionResult UserRoles(string id)
        {
            var user = _accountManager.GetUsers(HttpContext.GetOwinContext(), u => u.Id.Equals(id)).FirstOrDefault();
            if (user == null)
                return null;
            RolesViewModel model = CreateRolesModel(user);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]
        public ActionResult UserRoles(RolesViewModel model)
        {
            if (model != null && model.submit == "Save")
            {
                var res = _accountManager.UpdateUserRoles(HttpContext.GetOwinContext(), model);

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

            return RedirectToAction("Users");
        }
    }
}