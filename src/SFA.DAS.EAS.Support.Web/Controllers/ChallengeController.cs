using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers
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
                return HttpNotFound($"There was a problem finding the account {id}");

            return View(new ChallengeViewModel
            {
                Characters = response.Characters,
                Id = id,
                Url = url,
                HasError = hasError
            });
        }

        [HttpPost]
        public async Task<ActionResult> Index(ChallengeEntry challengeEntry)
        {
            var response = await _handler.Handle(Map(challengeEntry));

            if (response.IsValid)
                return Content(string.Empty);

            Response.StatusCode = 403;
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