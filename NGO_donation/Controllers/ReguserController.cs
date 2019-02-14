using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using NGO_donation.Models;

namespace NGO_donation.Controllers
{
    public class ReguserController : Controller
    {
        private DonationEntities db = new DonationEntities();

        // GET: Reguser
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
    //    [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "firstname,lastname,cma_,phone,emailid,address1,address2,city,state,zipcode,country,urbanization")] tbl_registeruserdetail tbl_registeruserdetail)
        {
            if (ModelState.IsValid)
            {
                db.tbl_registeruserdetail.Add(tbl_registeruserdetail);
                db.SaveChanges();
                int regid = tbl_registeruserdetail.regid;
                Session["RegId"] = regid;
                ViewData["Success"] = "Registration place successfully.";
                return RedirectToAction("DonationAmount", "DonationType");
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
