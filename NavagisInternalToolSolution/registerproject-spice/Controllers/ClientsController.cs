using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegisterProject_Spice.Pages.Models;
using RegisterProject_Spice.Utilities;


namespace RegisterProject_Spice.Controllers
{
    [AdminRequiresAuthentication]
    public class ClientsController : Controller
    {
        private ApplicationDbContext db;

        public ClientsController(ApplicationDbContext context)
        {
            db = context;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public IActionResult Index()
        {
            var clients = db.Clients.Include(c => c.BillingAccount);
            return View(clients.ToList());
        }
        // GET: Clients/Create
        public ActionResult Create()
        {
            ViewBag.BillingAccountId = new SelectList(db.BillingAccounts, "Id", "BillingAccountName");
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BillingAccountId = new SelectList(db.BillingAccounts, "Id", "BillingAccountName", client.BillingAccountId);
            return View(client);
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return new StatusCodeResult(400);
            }
            var billingAccountDb = db.BillingAccounts.SingleOrDefault(s => s.Id == client.BillingAccountId);
            ViewBag.BillingAccountId = billingAccountDb.BillingAccountName;
            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return new StatusCodeResult(400);
            }
            ViewBag.BillingAccountId = new SelectList(db.BillingAccounts, "Id", "BillingAccountName", client.BillingAccountId);
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                var clientDb = db.Clients.SingleOrDefault(s => s.Id == client.Id);
                clientDb.Email = client.Email;
                clientDb.FirstName = client.FirstName;
                clientDb.LastName = client.LastName;
                clientDb.BillingAccountId = client.BillingAccountId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BillingAccountId = new SelectList(db.BillingAccounts, "Id", "BillingAccountName", client.BillingAccountId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return new StatusCodeResult(400);
            }
            var billingAccountDb = db.BillingAccounts.SingleOrDefault(s => s.Id == client.BillingAccountId);
            ViewBag.BillingAccountId = billingAccountDb.BillingAccountName;
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}