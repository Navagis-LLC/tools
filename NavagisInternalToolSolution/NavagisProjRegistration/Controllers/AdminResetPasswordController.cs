using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NavagisProjRegistration.Models;

namespace NavagisProjRegistration.Controllers
{
    public class AdminResetPasswordController : Controller
    {
        private ApplicationDBContext db;
        private readonly Random _random = new Random();

        public AdminResetPasswordController()
        {
            db = new ApplicationDBContext(); 
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public ActionResult SendEmailToResetEmail()
        {
            ViewBag.gotoForm = 1;
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailToResetEmail(AdminUser user)
        {
            ViewBag.gotoForm = 1;
            if (!ModelState.IsValid)
                return View(user);

            var _userInDb = db.AdminUsers.SingleOrDefault(u => u.Username == user.Username);
            if (_userInDb == null)
            {
                ViewBag.Message = "Email is not found.";
            }
            else if (_userInDb != null)
            {
                var SecurityCode = GenerateSecurityCode();
                var AdminResetPass = new AdminResetPassword();

                AdminResetPass.ResetCode = SecurityCode;
                AdminResetPass.AdminUserId = Convert.ToInt32(_userInDb.Id);
                db.AdminResetPasswords.Add(AdminResetPass);
                db.SaveChanges();

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                var resetLink = "http://localhost:52629/AdminResetPassword/RetrivedPassword?SecurityCode="+ SecurityCode;

                // I need a valid SMTP account.
                var smtp_username = "acomique285@gmail.com";
                var smtp_password = "password";

                mail.From = new MailAddress(smtp_username);
                mail.To.Add(_userInDb.Username);
                mail.Subject = "Navagis Internal Tool - Reset Password";
                mail.Body = "<p>Dear "+ _userInDb.FirstName + ", <br />" +
                            "Please use this link to reset your password:<br>" +
                            " <a href='"+resetLink+"'>" +resetLink+ "</a><br /><br />" +
                            "Thank you.</p>";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(smtp_username, smtp_password);
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;

                //SmtpServer.Send(mail);
                ViewBag.gotoForm = 0;

            }

            return View(user);
        }

        public ActionResult RetrivedPassword(String SecurityCode)
        {
            var resetPass = db.AdminResetPasswords.SingleOrDefault(u => u.ResetCode == SecurityCode);
            var AdminUserId = 0;
            var AdminU = new AdminUser();

            ViewBag.gotoForm = 0;
            ViewBag.MessageTitle = "Error!";
            ViewBag.Message = "The security code is not valid.";
            ViewBag.SecurityCode = SecurityCode;

            
            if (resetPass != null)
            {
                AdminUserId = Convert.ToInt32(resetPass.AdminUserId);
                AdminU = db.AdminUsers.Find(AdminUserId);
                if (AdminU != null)
                {
                    ViewBag.gotoForm = 1;
                    ViewBag.Message = "";
                    return View(AdminU);
                }
                else
                {
                    return View(AdminU);
                }
            }
            else
            {
                return View(AdminU);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetrivedPassword(AdminUser user, String SecurityCode)
        {
            var resetPass = db.AdminResetPasswords.SingleOrDefault(u => u.ResetCode == SecurityCode);
            var AdminUserId = 0;
            var AdminU = new AdminUser();

            ViewBag.gotoForm = 0;
            ViewBag.MessageTitle = "Error!";
            ViewBag.Message = "The security code is not valid.";
            ViewBag.SecurityCode = SecurityCode;

            if (resetPass != null)
            {
                AdminUserId = Convert.ToInt32(resetPass.AdminUserId);
                AdminU = db.AdminUsers.Find(AdminUserId);
                if (AdminU != null)
                {
                    AdminU.Password = user.Password;
                    db.AdminResetPasswords.Remove(resetPass);
                    db.SaveChanges();

                    ViewBag.MessageTitle = "Success!";
                    ViewBag.Message = "You have a new password.";
                    return View(AdminU);
                }
                else
                {
                    return View(AdminU);
                }

            }
            return View(AdminU);
        }

        private string GenerateSecurityCode()
        {
            var passwordBuilder = new StringBuilder();

            // 4-Letters lower case   
            passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999  
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case  
            passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }

        private string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        // Generates a random number within a range.      
        private int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}