using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Statistics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Statistics.Identity
{    
    public class AccountManager
    {
        protected IQueryable<UserViewModel> GetUsers(IOwinContext owinContext)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppRoleManager roleManager = owinContext.GetUserManager<AppRoleManager>();

            var users = userManager.Users
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = u.Roles.Select(r => r.RoleId).Join(roleManager.Roles.Select(r => new { r.Id, r.Name })
                    , rId => rId, rr => rr.Id, (rId, rr) => rr.Name)
                });

            return users;
        }

        public IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, PagerData pager = null)
        {
            if (pager == null)
                return GetUsers(owinContext);

            return GetUsersPage(GetUsers(owinContext), pager);
        }

        protected IEnumerable<UserViewModel> GetUsersPage(IQueryable<UserViewModel> query, PagerData pager)
        {
            int skip = pager.ItemsPerPage * (pager.CurrentPage - 1);
            var page = query.OrderBy(x => x.UserName).Skip(skip).Take(pager.ItemsPerPage);

            int total = page.Count();
            int rest = total % pager.ItemsPerPage;
            pager.TotalPages = total / pager.ItemsPerPage + (rest > 0 ? 1 : 0);

            return page;
        }

        public IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, Expression<Func<UserViewModel, bool>> condition, PagerData pager = null)
        {
            var query = GetUsers(owinContext).Where(condition);
            if (pager == null)
                return query;
            return GetUsersPage(query, pager);            
        }

        public bool SignIn(IOwinContext owinContext, LoginViewModel loginModel)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            IAuthenticationManager authentication = owinContext.Authentication;
            
            AppUser user = userManager.Find(loginModel.UserName, loginModel.Password);
            if (user != null)
            {
                authentication.SignOut();
                AuthenticationProperties props = new AuthenticationProperties()
                {
                    IsPersistent = true
                };
                ClaimsIdentity claim = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authentication.SignIn(props, claim);
                
                return true;
            }
            return false;
        }

        public void SignOut(IOwinContext owinContext)
        {
            owinContext.Authentication.SignOut();
        }

        //public async Task<AppUser> GetCurrentUserAsync(IOwinContext owinContext)
        //{
        //    var id = owinContext.Authentication.User.Identity.GetUserId();
        //    AppUser user = await owinContext.GetUserManager<AppUserManager>().FindByIdAsync(id);
        //    return user;
        //}   

        public IdentityResult ChangePassword(IOwinContext owinContext, ChangePasswordViewModel model)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.Find(model.UserName, model.OldPassword);
            if (user == null)
                return new IdentityResult("User not found");
            
            IdentityResult res = userManager.ChangePassword(user.Id, model.OldPassword, model.Password);
            return res;
        }

        public IdentityResult CreateUser(IOwinContext owinContext, UserViewModel userModel)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = new AppUser()
            {
                UserName = userModel.UserName,
                LastName = userModel.LastName,
                Email = userModel.Email
            };

            IdentityResult res = userManager.Create(user, userModel.Password);

            if (res.Succeeded)
            {                
                foreach (var roleEntry in userModel.Roles)
                {
                    res = userManager.AddToRole(user.Id, roleEntry);
                    if (!res.Succeeded)
                        break;
                }
            }

            return res;
        }

        public IdentityResult UpdateUser(IOwinContext owinContext, UserViewModel userModel)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(userModel.UserName);
            var context = owinContext.Get<AppDbContext>();
            
            if (user != null)
            {
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;

                var existingRoles = userManager.GetRoles(user.Id);                
                var rolesToRemove = existingRoles.Except(userModel.Roles);
                var rolesToAdd = userModel.Roles.Except(existingRoles);

                var tran = context.Database.BeginTransaction();
                
                IdentityResult result = userManager.Update(user);

                // !!! ??? 

                if (result.Succeeded)
                {
                    result = userManager.ResetPassword(user.Id, userManager.GeneratePasswordResetToken(user.Id), userModel.Password);

                    if (result.Succeeded)
                    {
                        foreach (var role in rolesToRemove)
                        {
                            result = userManager.RemoveFromRole(user.Id, role);
                            if (!result.Succeeded)
                                break;
                        }

                        if (result.Succeeded)
                        {
                            foreach (var role in rolesToAdd)
                            {
                                result = userManager.AddToRole(user.Id, role);
                                if (!result.Succeeded)
                                    break;
                            }
                        }

                        if (result.Succeeded)
                            tran.Commit();
                        else
                            tran.Rollback();
                        
                    }
                }

                return result;
            }

            return new IdentityResult("User not found");
        }

        public IdentityResult DeleteUser(IOwinContext owinContext, UserViewModel userModel)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(userModel.UserName);
            return userManager.Delete(user);
        }

        public static bool IsAdmin(IOwinContext owinContext, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();                        
            var user = userManager.FindByName(userName);
            return userManager.IsInRole(user.Id, "administrators");
        }
    }
}