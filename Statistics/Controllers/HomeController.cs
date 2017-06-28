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
        private ISalesManager _salesManager;

        public HomeController(ISalesManager saleManager)
        {
            _salesManager = saleManager; 
        }

        [ActionName("Index")]
        public ActionResult IndexGet(SaleFilterModel model)
        {
            if (model == null || model.page == 0)
            {
                model = new SaleFilterModel()
                {
                    page = 1
                };
                if (Session["ItemsPerPage"] != null)
                    model.ItemsPerPage = (int)Session["ItemsPerPage"];
                return View(model);
            }

            return Statistics(model);
        }        

        [ActionName("Index")]
        [HttpPost]
        public ActionResult IndexPost(SaleFilterModel model)
        {
            Session["ItemsPerPage"] = model.ItemsPerPage;
            return Statistics(model);
        }

        public ActionResult Statistics(SaleFilterModel model)
        {
            if (Session["ItemsPerPage"] != null)
                model.ItemsPerPage = (int)Session["ItemsPerPage"];
            PagerData pager = new PagerData() { ItemsPerPage = model.ItemsPerPage, CurrentPage = model.page };
            var result = _salesManager.GetSales(model, null, pager);
            if (Request.IsAjaxRequest())
                return Json(new { CurrentPage = pager.CurrentPage, TotalPages = pager.TotalPages, Result = result }, JsonRequestBehavior.AllowGet);
            SalesListModel listModel = new SalesListModel()
            {
                Sales = result,
                Pager = pager,
                Filter = model
            };
            return View("Statistics", listModel);
        }


        [Authorize(Roles = "administrators")]
        public ActionResult CreateSale(int? createdID = null)
        {
            SaleViewModel model = new SaleViewModel()
            {
                SaleDate = DateTime.Now
            };

            if (createdID.HasValue)
                ViewBag.CreatedId = createdID;

            if (Request.IsAjaxRequest())
                return PartialView(model);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "administrators")]
        public ActionResult CreateSale(SaleViewModel model)
        {
            if (model.submit == "Cancel")
            {
                if (Request.IsAjaxRequest())
                    return Json("");
                return RedirectToAction("Index");
            }

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

                if (Request.IsAjaxRequest())
                    return PartialView("ErrorView", err);
                return View("ErrorView", err);
            }


            //Response.Redirect(Url.Action("CreateSale", new { createdId = model.Id }), true);
            //return null;

            return RedirectToAction("CreateSale", new { createdId = model.Id });
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