﻿using NGO_donation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NGO_donation.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(tbl_user model)
        {
            if (ModelState.IsValid)
            {
                using (DonationEntities db = new DonationEntities())
                {
                    var v = db.tbl_user.Where(a => a.email.Equals(model.email) && a.password.Equals(model.password)).FirstOrDefault();
                    if (v != null)
                    {
                        if (v.roleid == 1)//If its an admin User,provide access to User Management & Doantion Management
                            return RedirectToAction("Index", "UserManage");
                        else if (v.roleid == 2)//If its a regular User,gets redirected to User View Controller
                            return RedirectToAction("Index", "Reguser");
                    }
                    else
                    {
                        ViewBag.NotValidUser = "Please check your login credentials!";
                    }
                }
            }
            return View("Index");
        }
    }
}