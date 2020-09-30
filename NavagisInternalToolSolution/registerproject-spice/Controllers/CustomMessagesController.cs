using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RegisterProject_Spice.Controllers
{
    public class CustomMessagesController : Controller
    {
        public IActionResult insufficientPrivileges()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult SystemError()
        {
            return View();
        }
    }
}