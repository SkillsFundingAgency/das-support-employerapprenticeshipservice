using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountHandler _accountHandler;
        private readonly IPayeLevySubmissionsHandler _payeLevySubmissionsHandler;
        private readonly ILog _log;
        private readonly IPayeLevyDeclarationMapper _payeLevyDeclarationMapper;

        public AccountController(IAccountHandler accountHandler,
            IPayeLevySubmissionsHandler payeLevySubmissionsHandler,
            ILog log,
            IPayeLevyDeclarationMapper payeLevyDeclarationMapper)
        {
            _accountHandler = accountHandler;
            _payeLevySubmissionsHandler = payeLevySubmissionsHandler;
            _log = log;
            _payeLevyDeclarationMapper = payeLevyDeclarationMapper;
        }

        [Route("account/{id}/{parent}")]
        public async Task<ActionResult> Index(string id, string parent)
        {
            var response = await _accountHandler.FindOrganisations(id);

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

        [Route("account/payeschemes/{id}")]
        public async Task<ActionResult> PayeSchemes(string id)
        {
            var response = await _accountHandler.FindPayeSchemes(id);

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

        [Route("account/header/{id}")]
        public async Task<ActionResult> Header(string id)
        {
            var response = await _accountHandler.Find(id);

            if (response.StatusCode != SearchResponseCodes.Success)
                return HttpNotFound();

            return View("SubHeader", response.Account);
        }

        [Route("account/finance/{id}")]
        public async Task<ActionResult> Finance(string id)
        {
            var response = await _accountHandler.FindFinance(id);

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

        [Route("account/levysubmissions/{id}/{parentId}")]
        public async Task<ActionResult> PayeSchemeLevySubmissions(string id, string parentId)
        {
            var response = await _payeLevySubmissionsHandler.Handle(id, parentId);
            var model = _payeLevyDeclarationMapper.MapPayeLevyDeclaration(response);
            
            return View(model);
        }
    }
}