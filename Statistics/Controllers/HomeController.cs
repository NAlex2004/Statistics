using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Statistics.BL;
using NAlex.Selling.DAL.Units;

namespace Statistics.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private SalesManager _salesManager;

        public HomeController()
        {
            _salesManager = new SalesManager(new SalesUoW());
        }

        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _salesManager.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}