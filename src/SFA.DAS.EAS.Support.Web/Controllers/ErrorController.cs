using System.Web.Mvc;

namespace SFA.DAS.EAS.Support.Web.Controllers
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