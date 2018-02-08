using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Support.ApplicationServices.Services;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private readonly IAccountHandler _handler;

        public SearchController(IAccountHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Organisations()
        {
            var accounts = await _handler.FindSearchItems();
            return Json(accounts);
        }
    }
}