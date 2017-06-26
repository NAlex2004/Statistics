using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Statistics.Models;

namespace Statistics.Identity
{
    public interface IAccountManager
    {
        IdentityResult ChangePassword(IOwinContext owinContext, ChangePasswordViewModel model);
        IdentityResult CreateUser(IOwinContext owinContext, UserViewModel userModel);
        IdentityResult DeleteUser(IOwinContext owinContext, UserViewModel userModel);
        IdentityResult DeleteUser(IOwinContext owinContext, string userId);
        IEnumerable<string> GetRoles(IOwinContext owinContext);
        IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, PagerData pager = null, Expression<Func<IQueryable<UserViewModel>, IOrderedQueryable<UserViewModel>>> orderBy = null);
        IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, Expression<Func<UserViewModel, bool>> condition, PagerData pager = null, Expression<Func<IQueryable<UserViewModel>, IOrderedQueryable<UserViewModel>>> orderBy = null);
        bool SignIn(IOwinContext owinContext, LoginViewModel loginModel);
        void SignOut(IOwinContext owinContext);
        IdentityResult UpdateUser(IOwinContext owinContext, UserViewModel userModel);
        IdentityResult UpdateUserRoles(IOwinContext owinContext, RolesViewModel model);
        bool IsAdmin(IOwinContext owinContext, string userName);
    }
}