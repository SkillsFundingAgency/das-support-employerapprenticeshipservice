using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [RoutePrefix("api/echo")]
    [AllowAnonymous]
    public class EchoController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return Json(new {Echo = "Working..."});
        }
    }
}