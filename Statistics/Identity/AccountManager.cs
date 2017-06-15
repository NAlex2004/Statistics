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

namespace Statistics.Identity
{
    public class AccountManager
    {
        public async Task<bool> SignIn(IOwinContext owinContext, LoginViewModel loginModel)
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
                ClaimsIdentity claim = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authentication.SignIn(props, claim);
                
                return true;
            }
            return false;
        }

        public void SignOut(IOwinContext owinContext)
        {
            owinContext.Authentication.SignOut();
        }

        public async Task<AppUser> GetCurrentUser(IOwinContext owinContext)
        {
            var id = owinContext.Authentication.User.Identity.GetUserId();
            AppUser user = await owinContext.GetUserManager<AppUserManager>().FindByIdAsync(id);
            return user;
        }

        public async Task<IdentityResult> ChangePassword(IOwinContext owinContext, ChangePasswordViewModel model)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.Find(model.UserName, model.OldPassword);
            if (user == null)
                return new IdentityResult("User is null");
            
            IdentityResult res = await userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.Password);
            return res;
        }

        public IdentityResult CreateUser(IOwinContext owinContext, UserViewModel userModel)
        {

        }
    }
}