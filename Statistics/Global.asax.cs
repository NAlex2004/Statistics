using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using Statistics.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Statistics
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static int ItemsPerPage { get; protected set; }

        public static bool IsAdmin(IOwinContext owinContext, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;
            AppUserManager userManager = owinContext.GetUserManager<AppUserManager>();
            var user = userManager.FindByName(userName);
            return userManager.IsInRole(user.Id, "administrators");
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            DependencyResolver.SetResolver(new DResolver());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            ItemsPerPage = int.Parse(ConfigurationManager.AppSettings["itemsPerPage"]);
        }
    }
}
