using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;

namespace Statistics
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static int ItemsPerPage { get; protected set; }

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
