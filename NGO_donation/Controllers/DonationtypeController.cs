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
    public class DonationtypeController : Controller
    {
        private DonationEntities db = new DonationEntities();

        public ActionResult Index()
        {
            return View(db.tbl_registeruserdetail.ToList());
        }

        public ActionResult DonationAmount(int? id)
        {
            var query = db.tbl_donationdetails.ToList();
            IList<tbl_donationtype> model = new List<tbl_donationtype>();

            foreach (var data in query)
            {
                if (query.Any(m => m.regid == id))
                {
                    var temp = from m in db.tbl_donationtype where m.donationtypeid == data.donationtypeid select m.donationname;
                    string don_name = temp.SingleOrDefault();
                    var type_donation = new tbl_donationtype()
                    {
                        donationdetailid = data.donationdetailid,
                        donationamount = Convert.ToDecimal(data.price),
                        recurringgift = Convert.ToBoolean(data.recurgift),
                        donationname = don_name
                    };
                    model.Add(type_donation);
                }
            }
            
            if (model.Count == 0)
                return View(db.tbl_donationtype.ToList());
            else
                return View(db.tbl_donationtype.AddRange(model));
        }

        [HttpPost]
        public ActionResult DonationAmount(tbl_donationtype[] model)
        {

            tbl_donationdetails details = new tbl_donationdetails();
            if (model != null)
            {
                int regid = Convert.ToInt32(Session["RegId"].ToString());
                var query = db.tbl_donationdetails.Where(m => m.regid == regid).ToList();

                foreach (var data in model)
                {
                    var querydata = db.tbl_donationdetails.Where(m => m.donationdetailid == data.donationdetailid).ToList().SingleOrDefault();
                    if (query.Count == 0)
                    {
                        details.regid = Convert.ToInt32(Session["RegId"]);
                        details.donationtypeid = data.donationtypeid;
                        details.recurgift = data.recurringgift;
                        if (details.recurgift == true)
                            details.quantity = 12;
                        else
                            details.quantity = 1;
                        details.price = Convert.ToDecimal(data.donationamount);
                        details.totalamount = Convert.ToDecimal(data.donationamount * details.quantity);
                        details.date = DateTime.Now;
                        db.tbl_donationdetails.Add(details);
                        db.SaveChanges();
                    }
                    else if (querydata.donationdetailid == data.donationdetailid)
                    {
                        querydata.recurgift = data.recurringgift;
                        if (data.recurringgift == true)
                            querydata.quantity = 12;
                        else
                            querydata.quantity = 1;
                        querydata.price = Convert.ToDecimal(data.donationamount);
                        querydata.totalamount = Convert.ToDecimal(data.donationamount * querydata.quantity);
                        querydata.date = DateTime.Now;
                        db.Entry(querydata).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("GetOrderSummary", "Donationtype");
            }
            return View(model);
        }

        public ActionResult GetOrderSummary()
        {
            int regid = Convert.ToInt32(Session["RegId"].ToString());
            var query = db.tbl_donationdetails.Where(m => m.regid == regid).ToList();
            decimal sum = 0;
            // decimal total=0;
            for (int i = 0; i < query.Count; i++)
            {
                sum = sum + Convert.ToDecimal(query[i].totalamount);
            }
            ViewData["Totalamount"] = sum;
            return View(query);
        }

        public ActionResult Removequantity(int? id)
        {
            tbl_donationdetails tbl_donationdetails = db.tbl_donationdetails.Find(id);
            if (tbl_donationdetails == null)
            {
                return HttpNotFound();
            }
            db.tbl_donationdetails.Remove(tbl_donationdetails);
            db.SaveChanges();
            return View("GetOrderSummary", db.tbl_donationdetails.ToList());
        }

        public ActionResult EmptyCart(int? id)
        {
            if (id != null)
            {
                var model = db.tbl_donationdetails.ToList();
                foreach (var data in model)
                    db.tbl_donationdetails.Remove(data);
                db.SaveChanges();
                ViewData["Empty"] = "No data items.";
            }
            else
            {
                int regid = Convert.ToInt32(Session["RegId"].ToString());
                var query = db.tbl_donationdetails.Where(m => m.regid == regid);
                return View(query.ToList());
            }
            return View("GetOrderSummary");
        }

        public async System.Threading.Tasks.Task<ActionResult> SendMail(int? id)
        {
            if (ModelState.IsValid)
            {
                var message = new MailMessage();
                var data = db.tbl_registeruserdetail.Where(m => m.regid == id).SingleOrDefault();
                message.To.Add(new MailAddress(data.emailid));
                message.From = new MailAddress("khyatish1989@gmail.com");
                message.Subject = "Order";
                string Body = "Your Order is placed";
                message.Body = Body;
                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("khyatish1989@gmail.com", "Shri@123"), // Enter seders User name and password   
                    EnableSsl = true
                };
                await smtp.SendMailAsync(message);
                ViewBag.Status = "Email Sent Successfully.";
                return View();
            }
            return View();
        }
    }
}
