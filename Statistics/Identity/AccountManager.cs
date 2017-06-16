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

namespace Statistics.Identity
{
    public class AccountManager
    {
        public IQueryable<UserViewModel> GetUsers(IOwinContext owinContext)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppRoleManager roleManager = owinContext.GetUserManager<AppRoleManager>();

            var users = userManager.Users.Select(u => new UserViewModel()
            {
                UserName = u.UserName,
                LastName = u.LastName,
                Email = u.Email,
                Roles = u.Roles.SelectMany(ur => roleManager.Roles.Where(r => r.Id.Equals(ur.RoleId))//.DefaultIfEmpty(),
                    .Select(r => new KeyValuePair<string, bool>(r.Name, true)).AsEnumerable())
                   // (ur, r) => new KeyValuePair<string, bool>(r.Name, true))
            });

            return users;
        }

        public IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, Func<UserViewModel, bool> condition)
        {
            return GetUsers(owinContext).Where(condition);
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
                foreach (var roleEntry in userModel.Roles.Where(r => r.Value))
                {
                    res = userManager.AddToRole(user.Id, roleEntry.Key);
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
                var newRoles = userModel.Roles.Where(r => r.Value).Select(r => r.Key);
                var rolesToRemove = existingRoles.Except(newRoles);
                var rolesToAdd = newRoles.Except(existingRoles);

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

        public bool IsAdmin(IOwinContext owinContext, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();                        
            var user = userManager.FindByName(userName);
            return userManager.IsInRole(user.Id, "administrators");
        }
    }
}