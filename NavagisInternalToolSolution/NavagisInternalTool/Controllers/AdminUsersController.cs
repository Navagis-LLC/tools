using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NavagisInternalTool.Models;
using NavagisInternalTool.Credentials;

namespace NavagisInternalTool.Controllers
{
    [AdminRequiresAuthentication]
    public class AdminUsersController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: AdminUsers
        public ActionResult Index()
        {
            return View(db.AdminUsers.ToList());
        }

        // GET: AdminUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = db.AdminUsers.Find(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // GET: AdminUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Password,FirstName,LastName,IsAdmin")] AdminUser adminUser)
        {
            var _userInDb = db.AdminUsers.SingleOrDefault(u => u.Username == adminUser.Username);

            if (_userInDb != null)
            {
                ViewBag.UniqueUsernameErrorMessage = "Cannot insert username exist in the database.";
            }
            else if (ModelState.IsValid)
            {
                db.AdminUsers.Add(adminUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adminUser);
        }

        // GET: AdminUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = db.AdminUsers.Find(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // POST: AdminUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,FirstName,LastName,IsAdmin")] AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adminUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adminUser);
        }

        // GET: AdminUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = db.AdminUsers.Find(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdminUser adminUser = db.AdminUsers.Find(id);
            db.AdminUsers.Remove(adminUser);
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
