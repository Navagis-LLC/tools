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
    [SuperAdminRoleAuthentications]
    public class AdminUsersController : Controller
    {
        private ApplicationDbContext db;

        public AdminUsersController(ApplicationDbContext context)
        {
            db = context;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public IActionResult Index()
        {
            return View(db.AdminUsers.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            AdminUser adminUser = db.AdminUsers.Find(id);
            if (adminUser == null)
            {
                return new StatusCodeResult(400);
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
        //[IgnoreAntiforgeryToken]
        public ActionResult Create(AdminUser adminUser)
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
                return new StatusCodeResult(400);
            }
            AdminUser adminUser = db.AdminUsers.Find(id);
            if (adminUser == null)
            {
                return new StatusCodeResult(400);
            }
            return View(adminUser);
        }

        // POST: AdminUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult Edit(AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                var adminUserDb = db.AdminUsers.SingleOrDefault(s => s.Id == adminUser.Id);
                adminUserDb.FirstName = adminUser.FirstName;
                adminUserDb.LastName = adminUser.LastName;
                adminUserDb.Password = adminUser.Password;
                adminUserDb.IsAdmin = adminUser.IsAdmin;
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
                return new StatusCodeResult(400);
            }
            AdminUser adminUser = db.AdminUsers.Find(id);

            if (adminUser.Username == "david@navagis.com")
            {
                return RedirectToAction("CannotDelete");
            }

            if (adminUser == null)
            {
                return new StatusCodeResult(400);
            }
            return View(adminUser);
        }
        
        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdminUser adminUser = db.AdminUsers.Find(id);

            if (adminUser.Username == "david@navagis.com")
            {
                return RedirectToAction("CannotDelete");
            }

            db.AdminUsers.Remove(adminUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CannotDelete()
        {
            return View();
        }

    }
}