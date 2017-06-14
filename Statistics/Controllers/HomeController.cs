using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Statistics.Controllers
{
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            Identity.AppUserManager manager = HttpContext.GetOwinContext().GetUserManager<Identity.AppUserManager>();
            Identity.AppUser user = new Identity.AppUser()
            {
                UserName = "Test"
            };
            var result = await manager.CreateAsync(user);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}