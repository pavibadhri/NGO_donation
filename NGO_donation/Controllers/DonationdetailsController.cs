using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NGO_donation.Models;
using PagedList;

namespace NGO_donation.Controllers
{
    public class DonationdetailsController : Controller
    {
        private DonationEntities db = new DonationEntities();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var donations = db.tbl_donationdetails.Include(t => t.tbl_registeruserdetail)
                            .Include(t => t.tbl_donationtype);

            if (!String.IsNullOrEmpty(searchString))
            {
                donations = donations.Where(s => s.tbl_registeruserdetail.firstname.Contains(searchString)
                                       || s.tbl_registeruserdetail.lastname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    donations = donations.OrderByDescending(s => s.tbl_registeruserdetail.lastname);
                    break;
                case "Date":
                    donations = donations.OrderBy(s => s.date);
                    break;
                case "date_desc":
                    donations = donations.OrderByDescending(s => s.date);
                    break;
                default:  // Name ascending 
                    donations = donations.OrderBy(s => s.tbl_registeruserdetail.lastname);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(donations.ToPagedList(pageNumber, pageSize));
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
