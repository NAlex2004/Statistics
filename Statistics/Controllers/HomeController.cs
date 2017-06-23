using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Statistics.BL;
using Statistics.Models;
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
            SaleFilterModel model = new SaleFilterModel()
            {                
                //StartDate = DateTime.Now.AddDays(-1),
                //EndDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SaleFilterModel model)
        {
            var result = _salesManager.GetSales(model);
            if (Request.IsAjaxRequest())
                return Json(result, JsonRequestBehavior.AllowGet);
            //return PartialView("Statistics", result);
            return View("Statistics", result);
        }

        [Authorize(Roles = "administrators")]
        public ActionResult CreateSale()
        {
            SaleViewModel model = new SaleViewModel()
            {
                SaleDate = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]
        public ActionResult CreateSale(SaleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool res = _salesManager.CreateSale(model);
            if (!res)
            {
                ErrorViewModel err = new ErrorViewModel()
                {
                    Errors = new List<string>() { "Sale cannot be created..." },
                    ReturnUrl = Url.Action("CreateSale")
                };

                return View("ErrorView", err);
            }
            
            return RedirectToAction("CreateSale");
        }

        [Authorize(Roles = "administrators")]
        public ActionResult EditSale(int id)
        {
            var sale = _salesManager.GetSale(id);
            if (sale == null)
                return RedirectToAction("Index");

            if (Request.IsAjaxRequest())
                return PartialView("EditSaleData", sale);
            return View(sale);
        }

        private ActionResult ReturnFromEditSale(SaleViewModel model, bool success)
        {
            if (success)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("StatisticsItem", model);
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
                return PartialView("EditSaleData", model);
            return View("EditSale", model);
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]
        public ActionResult EditSale(SaleViewModel model)
        {
            if (!ModelState.IsValid)
                return ReturnFromEditSale(model, false);
            if (model.submit == "Cancel")
                return ReturnFromEditSale(model, true);

            bool res = _salesManager.UpdateSale(model);
            if (!res)
                ModelState.AddModelError("", "Update faled..");

            return ReturnFromEditSale(model, res);
        }


        [Authorize(Roles = "administrators")]
        public ActionResult DeleteSale(int id)
        {
            bool res = _salesManager.DeleteSale(id);
            if (Request.IsAjaxRequest())
                return Json(res.ToString().ToLower(), JsonRequestBehavior.AllowGet);
            if (!res)
            {
                ErrorViewModel err = new ErrorViewModel()
                {
                    Errors = new List<string>() { "Delete failed..." }
                };
                return View("ErrorView", err);
            }
            return RedirectToAction("Index");
        }

        public PartialViewResult OneSaleById(int id)
        {
            var sale = _salesManager.GetSale(id);
            return PartialView("StatisticsItem", sale);
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