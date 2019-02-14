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
    public class UserManageController : Controller
    {
        private DonationEntities db = new DonationEntities();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_user = db.tbl_user.Include(t => t.tbl_role).Include(t => t.tbl_registeruserdetail);

            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_user = tbl_user.Where(s => s.firstname.Contains(searchString)
                                       || s.lastname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    tbl_user = tbl_user.OrderByDescending(s => s.lastname);
                    break;
                default:  // Name ascending 
                    tbl_user = tbl_user.OrderBy(s => s.lastname);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_user.ToPagedList(pageNumber, pageSize));
        }

        // GET: UserManage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_user tbl_user = db.tbl_user.Find(id);
            if (tbl_user == null)
            {
                return HttpNotFound();
            }
            return View(tbl_user);
        }

        // GET: UserManage/Create
        public ActionResult Create()
        {
            ViewBag.roleid = new SelectList(db.tbl_role, "roleid", "rolename");
            ViewBag.regid = new SelectList(db.tbl_registeruserdetail, "regid", "firstname");
            return View();
        }

        // POST: UserManage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid,firstname,lastname,email,password,roleid,regid")] tbl_user tbl_user)
        {
            if (ModelState.IsValid)
            {
                db.tbl_user.Add(tbl_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.roleid = new SelectList(db.tbl_role, "roleid", "rolename", tbl_user.roleid);
            ViewBag.regid = new SelectList(db.tbl_registeruserdetail, "regid", "firstname", tbl_user.regid);
            return View(tbl_user);
        }

        // GET: UserManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_user tbl_user = db.tbl_user.Find(id);
            if (tbl_user == null)
            {
                return HttpNotFound();
            }
            ViewBag.roleid = new SelectList(db.tbl_role, "roleid", "rolename", tbl_user.roleid);
            ViewBag.regid = new SelectList(db.tbl_registeruserdetail, "regid", "firstname", tbl_user.regid);
            return View(tbl_user);
        }

        // POST: UserManage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid,firstname,lastname,email,password,roleid,regid")] tbl_user tbl_user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.roleid = new SelectList(db.tbl_role, "roleid", "rolename", tbl_user.roleid);
            ViewBag.regid = new SelectList(db.tbl_registeruserdetail, "regid", "firstname", tbl_user.regid);
            return View(tbl_user);
        }

        // GET: UserManage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_user tbl_user = db.tbl_user.Find(id);
            if (tbl_user == null)
            {
                return HttpNotFound();
            }
            return View(tbl_user);
        }

        // POST: UserManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_user tbl_user = db.tbl_user.Find(id);
            db.tbl_user.Remove(tbl_user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
