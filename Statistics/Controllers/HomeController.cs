using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Statistics.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Identity.AppUserManager manager = HttpContext.GetOwinContext().GetUserManager<Identity.AppUserManager>();
            //Identity.AppRoleManager roleManager = HttpContext.GetOwinContext().GetUserManager<Identity.AppRoleManager>();

            //Identity.AppRole admRole = new Identity.AppRole()
            //{
            //    Name = "administrators"
            //};

            //Identity.AppRole usrRole = new Identity.AppRole()
            //{
            //    Name = "users"
            //};

            //roleManager.Create(admRole);
            //roleManager.Create(usrRole);

            //Identity.AppUser admUser = new Identity.AppUser()
            //{
            //    UserName = "admin",
            //    LastName = "Administrator"
            //};

            //Identity.AppUser user = new Identity.AppUser()
            //{
            //    UserName = "user",
            //    LastName = "User"
            //};

            //var res = manager.Create(admUser, "admin");
            //var res2 = manager.Create(user, "user");

            //manager.AddToRole(admUser.Id, "administrators");
            //manager.AddToRole(user.Id, "users");

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