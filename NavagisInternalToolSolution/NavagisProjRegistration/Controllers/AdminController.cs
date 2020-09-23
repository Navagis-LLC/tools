using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using NavagisProjRegistration.Models;
using NavagisProjRegistration.Credentials;
using System.Data.Entity;

namespace NavagisProjRegistration.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDBContext db;
        private int _settingRecordID;
        private string _isLogedIn;

        public AdminController()
        {
            db = new ApplicationDBContext();
            _settingRecordID = Convert.ToInt32(WebConfigurationManager.AppSettings["DBDefaultSettingID"]);
            _isLogedIn = "Yes";
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index()
        {
            if(Session["isLogedIn"] == _isLogedIn)
                return RedirectToAction("Index", "Clients");
            else
                return RedirectToAction("Login", "Admin");
        }

        public ActionResult Logout()
        {
            Session["isLogedIn"] = "No";
            return RedirectToAction("Login", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var _userInDb = db.AdminUsers.SingleOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (_userInDb != null)
            {
                Session["Username"] = user.Username;
                Session["isLogedIn"] = _isLogedIn;
                return RedirectToAction("Index", "Clients");
            }

            ViewBag.Message = "Invalid username or password.";

            return View(user);
        }

        [AdminRequiresAuthentication]
        public ActionResult profile()
        {
            var Username = Session["Username"];
            var adminUser = db.AdminUsers.SingleOrDefault(s => s.Username == Username);
            return View(adminUser);
        }

        [AdminRequiresAuthentication]
        public ActionResult Edit(int? id)
        {
            var Username = Session["Username"];
            var adminUser = db.AdminUsers.SingleOrDefault(s => s.Username == Username);
            return View(adminUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminRequiresAuthentication]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,FirstName,LastName,IsAdmin")] AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                Session["Username"] = adminUser.Username;
                db.Entry(adminUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("profile");
            }
            return View(adminUser);
        }
    }
}