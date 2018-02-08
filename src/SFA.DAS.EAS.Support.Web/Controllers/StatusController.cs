using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        // GET: Status
        [System.Web.Mvc.AllowAnonymous]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(new
            {
                ServiceName = AddServiceName(),
                ServiceTime = AddServerTime(),
                Request = AddRequestContext()
            });
        }

        private string AddRequestContext()
        {
            try
            {
                return $" {HttpContext.Current.Request.HttpMethod}: {HttpContext.Current.Request.RawUrl}";
            }
            catch
            {
                return "Unknown";
            }
        }

        private DateTimeOffset AddServerTime()
        {
            return DateTimeOffset.UtcNow;
        }

        private string AddServiceName()
        {
            try
            {
                return "SFA DAS Employer Apprenticeship Service Support Site";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}