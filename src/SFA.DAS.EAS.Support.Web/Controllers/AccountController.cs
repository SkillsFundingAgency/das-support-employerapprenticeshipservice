using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountHandler _handler;

        public AccountController(IAccountHandler handler)
        {
            _handler = handler;
        }

        public async Task<ActionResult> Index(string id)
        {
            var response = await _handler.FindOrganisations(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account
                };

                return View(vm);
            }

            return HttpNotFound();
        }

        public async Task<ActionResult> PayeSchemes(string id)
        {
            var response = await _handler.FindPayeSchemes(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new AccountDetailViewModel
                {
                    Account = response.Account
                };

                return View(vm);
            }

            return new HttpNotFoundResult();
        }

        public async Task<ActionResult> Header(string id)
        {
            var response = await _handler.Find(id);

            if (response.StatusCode != SearchResponseCodes.Success)
                return HttpNotFound();

            return View("SubHeader", response.Account);
        }

        public async Task<ActionResult> Finance(string id)
        {
            var response = await _handler.FindFinance(id);

            if (response.StatusCode == SearchResponseCodes.Success)
            {
                var vm = new FinanceViewModel
                {
                    Account = response.Account,
                    Balance = response.Balance
                };

                return View(vm);
            }

            return HttpNotFound();
        }
    }
}