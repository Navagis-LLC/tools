using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisterProject_Spice.Pages.Models;
using RegisterProject_Spice.Utilities;


namespace RegisterProject_Spice.Pages.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db;
        private string _isLogedIn;

        public ActionResult index()
        {
            return View();
        }

        public AdminController(ApplicationDbContext context)
        {
            db = context;
            _isLogedIn = "Yes";
        }
        
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.SetString("Username", "");
            HttpContext.Session.SetString("isLogedIn", "");
            HttpContext.Session.SetString("isSuperAdmin", "");
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        public ActionResult Login(AdminUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var _userInDb = db.AdminUsers.SingleOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (_userInDb != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("isLogedIn", _isLogedIn);
                HttpContext.Session.SetString("isSuperAdmin", "no");
                if (_userInDb.IsAdmin)
                {
                    HttpContext.Session.SetString("isSuperAdmin", "yes");
                }
                
                return RedirectToAction("Index", "Clients");
            }

            ViewBag.Message = "Invalid username or password.";

            return View(user);
        }

        [AdminRequiresAuthentication]
        public ActionResult profile()
        {
            var Username = HttpContext.Session.GetString("Username");
            var adminUser = db.AdminUsers.SingleOrDefault(s => s.Username == Username);
            return View(adminUser);
        }

        [AdminRequiresAuthentication]
        public ActionResult Edit(int? id)
        {
            var Username = HttpContext.Session.GetString("Username");
            var adminUser = db.AdminUsers.SingleOrDefault(s => s.Username == Username);
            return View(adminUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken]
        [AdminRequiresAuthentication]
        public ActionResult Edit(AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                var adminUserDb = db.AdminUsers.SingleOrDefault(s => s.Id == adminUser.Id);
                adminUserDb.FirstName = adminUser.FirstName;
                adminUserDb.LastName = adminUser.LastName;
                adminUserDb.Password = adminUser.Password;
                //adminUserDb.IsAdmin = adminUser.IsAdmin;
                db.SaveChanges();
                return RedirectToAction("profile");
            }
            return View(adminUser);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }
    }
}