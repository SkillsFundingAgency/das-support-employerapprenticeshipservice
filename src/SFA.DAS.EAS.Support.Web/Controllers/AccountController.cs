using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sfa.Das.Console.ApplicationServices.Queries;
using Sfa.Das.Console.ApplicationServices.Responses;
using Sfa.Das.Console.ApplicationServices.Services;
using Sfa.Das.Console.Web.ViewModels;

namespace Sfa.Das.Console.Web.Controllers
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

            return new HttpNotFoundResult();
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

            return new HttpNotFoundResult();
        }
    }
}
