using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;
using Sfa.Das.Console.ApplicationServices.Queries;
using Sfa.Das.Console.ApplicationServices.Responses;
using Sfa.Das.Console.Core.Domain.Model;
using Sfa.Das.Console.Web.Models;
using SFA.DAS.EAS.Support.ApplicationServices;

namespace Sfa.Das.Console.Web.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly IChallengeHandler _handler;

        public ChallengeController(IChallengeHandler handler)
        {
            _handler = handler;
        }

        public async Task<ActionResult> Index(string id, string url, bool hasError = false)
        {
            var response = await _handler.Get(id);

            if (response.StatusCode != SearchResponseCodes.Success)
            {
                return HttpNotFound($"There was a problem finding the account {id}");
            }

            return View(new ChallengeViewModel { Characters = response.Characters, Id = id, Url = url, HasError = hasError });
        }

        [HttpPost]
        public async Task<ActionResult> Index(ChallengeEntry challengeEntry)
        {
            var response = await _handler.Handle(Map(challengeEntry));

            if (response.IsValid)
            {
                return Content(string.Empty);
            }

            var model = new ChallengeViewModel
            {
                Characters = new List<int> {challengeEntry.Challenge1, challengeEntry.Challenge2},
                Id = challengeEntry.Id,
                Url = challengeEntry.Url,
                HasError = true
            };

            return View(model);
        }

        private ChallengePermissionQuery Map(ChallengeEntry challengeEntry)
        {
            return new ChallengePermissionQuery
            {
                Id = challengeEntry.Id,
                Url = challengeEntry.Url,
                ChallengeElement1 = challengeEntry.Challenge1.ToString(),
                ChallengeElement2 = challengeEntry.Challenge2.ToString(),
                Balance = challengeEntry.Balance,
                FirstCharacterPosition = challengeEntry.FirstCharacterPosition,
                SecondCharacterPosition = challengeEntry.SecondCharacterPosition
            };
        }
    }
}