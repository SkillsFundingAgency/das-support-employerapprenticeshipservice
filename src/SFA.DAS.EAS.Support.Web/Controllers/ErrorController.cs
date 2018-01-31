using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Error()
        {
            return View();
        }


        public ActionResult NotFound()
        {
            return View("Error");
        }

        public ActionResult BadRequest()
        {
            return View("Error");
        }


    }
}