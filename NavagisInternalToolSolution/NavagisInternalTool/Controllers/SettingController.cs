using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NavagisInternalTool.Models;
using NavagisInternalTool.Credentials;
using System.Web.Configuration;
using System.Web.Mvc;


namespace NavagisInternalTool.Controllers
{
    [AdminRequiresAuthentication]
    public class SettingController : Controller
    {
        private int _settingRecordID;
        private ApplicationDBContext db;

        public SettingController()
        {
            db = new ApplicationDBContext();
            _settingRecordID = Convert.ToInt32(WebConfigurationManager.AppSettings["DBDefaultSettingID"]);
    
        }

        public ActionResult Index()
        {  
            var settingInDB = db.Settings.SingleOrDefault(s => s.Id == _settingRecordID);
            return View(settingInDB);
        }

        public ActionResult Edit()
        {
            var settingInDB = db.Settings.SingleOrDefault(s => s.Id == _settingRecordID);
            return View(settingInDB);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid)
                return View("Edit", setting);

            var settingInDB = db.Settings.SingleOrDefault(s => s.Id == _settingRecordID);

            settingInDB.ClientId = setting.ClientId;
            settingInDB.ClientSecret = setting.ClientSecret;
            settingInDB.ServiceAccountEmail = setting.ServiceAccountEmail;

            db.SaveChanges();

            Session["Message"] = "Setting is successfully updated.";

            return RedirectToAction("Index", "Setting");
        }
    }
}