﻿using Microsoft.AspNet.Identity;
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
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Statistics.Identity
{    
    public class AccountManager : IAccountManager
    {
        public AccountManager()
        {
            var expr = Mapper.CreateMap<AppUser, UserViewModel>();
            expr.ForAllMembers(m => m.Ignore());
            expr.ForMember(dst => dst.Id, opt => opt.MapFrom(usr => usr.Id))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(usr => usr.UserName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(usr => usr.LastName))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(usr => usr.Email));

            var expr2 = Mapper.CreateMap<UserViewModel, AppUser>();
            expr2.ForAllMembers(m => m.Ignore());
            expr2.ForMember(dst => dst.Id, opt => opt.MapFrom(usr => usr.Id))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(usr => usr.UserName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(usr => usr.LastName))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(usr => usr.Email));
        }

        protected IQueryable<UserViewModel> GetUsers(IOwinContext owinContext, Expression<Func<UserViewModel, bool>> condition)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppRoleManager roleManager = owinContext.GetUserManager<AppRoleManager>();
            Expression<Func<AppUser, bool>> expr = Mapper.Map<Expression<Func<AppUser, bool>>>(condition);

            var users = userManager.Users.Where(expr)
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

        public IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, 
            PagerData pager = null, Expression<Func<IQueryable<UserViewModel>, IOrderedQueryable<UserViewModel>>> orderBy = null)
        {
            var query = orderBy == null ? GetUsers(owinContext, u => true).OrderBy(u => u.UserName) : orderBy.Compile()(GetUsers(owinContext, u => true));

            if (pager == null)
                return query.ToArray();

            return GetUsersPage(query, pager);
        }

        protected IEnumerable<UserViewModel> GetUsersPage(IQueryable<UserViewModel> query, PagerData pager)
        {
            int total = query.Count();
            int rest = total % pager.ItemsPerPage;
            pager.TotalPages = total / pager.ItemsPerPage + (rest > 0 ? 1 : 0);
            int skip = pager.ItemsPerPage * (pager.CurrentPage - 1);
            var page = query.Skip(skip).Take(pager.ItemsPerPage);            
            
            return page.ToArray();
        }

        public IEnumerable<UserViewModel> GetUsers(IOwinContext owinContext, Expression<Func<UserViewModel, bool>> condition,
            PagerData pager = null, Expression < Func<IQueryable<UserViewModel>, IOrderedQueryable<UserViewModel>>> orderBy = null)
        {
            var query = GetUsers(owinContext, condition);
            var orderedQuery = orderBy == null ? query.OrderBy(u => u.UserName)
                : orderBy.Compile()(query);
            if (pager == null)
                return orderedQuery.ToArray();
            return GetUsersPage(orderedQuery, pager);            
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

        public IdentityResult ChangePassword(IOwinContext owinContext, ChangePasswordViewModel model)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.Find(model.UserName, model.OldPassword);
            if (user == null)
                return new IdentityResult("User name or password incorrect.");
            
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
                userModel.Id = user.Id;

                foreach (var roleEntry in userModel.Roles)
                {
                    res = userManager.AddToRole(user.Id, roleEntry);
                    if (!res.Succeeded)
                        break;
                }
            }

            return res;
        }

        public IdentityResult UpdateUserRoles(IOwinContext owinContext, RolesViewModel model)
        {
            if (model == null)
                return new IdentityResult("Roles not found.");

            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.FindById(model.UserId);

            var checkedRoles = model.Roles.Where(r => r.Value).Select(r => r.Key);
            var existingRoles = userManager.GetRoles(user.Id);
            var rolesToRemove = existingRoles.Except(checkedRoles);
            var rolesToAdd = checkedRoles.Except(existingRoles);

            IdentityResult result = IdentityResult.Success;

            var context = owinContext.Get<AppDbContext>();
            using (var tran = context.Database.BeginTransaction())
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

            return result;
        }

        public IdentityResult UpdateUser(IOwinContext owinContext, UserViewModel userModel)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.FindByName(userModel.UserName);            
            
            if (user != null)
            {
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;
               
                IdentityResult result = userManager.Update(user);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(userModel.Password))
                        result = userManager.ResetPassword(user.Id, userManager.GeneratePasswordResetToken(user.Id), userModel.Password);
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

        public IdentityResult DeleteUser(IOwinContext owinContext, string userId)
        {
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            AppUser user = userManager.FindById(userId);
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

        public IEnumerable<string> GetRoles(IOwinContext owinContext)
        {
            AppRoleManager roleManager = owinContext.GetUserManager<AppRoleManager>();
            return roleManager.Roles.Select(r => r.Name).ToArray();
        }
    }
}