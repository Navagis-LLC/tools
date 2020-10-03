using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using RegisterProject_Spice.Pages.Models;
using RegisterProject_Spice.Utilities;

namespace RegisterProject_Spice.Controllers
{
    [AdminRequiresAuthentication]
    public class BillingAccountsController : Controller
    {
        private ApplicationDbContext db;

        public BillingAccountsController(ApplicationDbContext context)
        {
            db = context;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public IActionResult Index()
        {
            return View(db.BillingAccounts.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: BillingAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult Create(BillingAccount billingAccount)
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
                return new StatusCodeResult(400);
            }
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            if (billingAccount == null)
            {
                return new StatusCodeResult(400);
            }
            return View(billingAccount);
        }

        // POST: BillingAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult Edit(BillingAccount billingAccount)
        {
            if (ModelState.IsValid)
            {
                var billingAccountDb = db.BillingAccounts.SingleOrDefault(s => s.Id == billingAccount.Id);
                billingAccountDb.BillingAccountName = billingAccount.BillingAccountName;
                billingAccountDb.Description = billingAccount.Description;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(billingAccount);
        }

        // GET: BillingAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            if (billingAccount == null)
            {
                return new StatusCodeResult(400);
            }
            return View(billingAccount);
        }

        // GET: BillingAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            if (billingAccount == null)
            {
                return new StatusCodeResult(400);
            }
            return View(billingAccount);
        }

        // POST: BillingAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BillingAccount billingAccount = db.BillingAccounts.Find(id);
            db.BillingAccounts.Remove(billingAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}