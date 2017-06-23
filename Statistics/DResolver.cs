using NAlex.Selling.DAL.Units;
using Statistics.BL;
using Statistics.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Statistics
{
    public class DResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(Statistics.Controllers.AccountController))
                return new Statistics.Controllers.AccountController(new AccountManager());

            if (serviceType == typeof(Statistics.Controllers.HomeController))
                return new Statistics.Controllers.HomeController(new SalesManager(new SalesUoW()));

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

    }
}