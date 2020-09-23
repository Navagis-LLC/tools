using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NavagisProjRegistration.Models;
using NavagisProjRegistration.Credentials;

namespace NavagisProjRegistration.Controllers
{
    [AdminRequiresAuthentication]
    public class BillingAccountsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: BillingAccounts
        public ActionResult Index()
        {
            return View(db.BillingAccounts.ToList());
        }

        // GET: BillingAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            if (billingAccount == null)
            {
                return HttpNotFound();
            }
            return View(billingAccount);
        }

        // GET: BillingAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BillingAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BillingAccountName,Description")] BillingAccount billingAccount)
        {
            if (ModelState.IsValid)
            {
                db.BillingAccounts.Add(billingAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(billingAccount);
        }

        // GET: BillingAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            if (billingAccount == null)
            {
                return HttpNotFound();
            }
            return View(billingAccount);
        }

        // POST: BillingAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BillingAccountName,Description")] BillingAccount billingAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(billingAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(billingAccount);
        }

        // GET: BillingAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            if (billingAccount == null)
            {
                return HttpNotFound();
            }
            return View(billingAccount);
        }

        // POST: BillingAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            db.BillingAccounts.Remove(billingAccount);
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
