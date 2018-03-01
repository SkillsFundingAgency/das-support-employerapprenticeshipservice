﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
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

        [HttpGet]
        [Route("challenge/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var response = await _handler.Get(id);

            if (response.StatusCode != SearchResponseCodes.Success)
                return HttpNotFound($"There was a problem finding the account {id}");

            return View(new ChallengeViewModel
            {
                Characters = response.Characters,
                Id = id
            });
        }

        [HttpPost]
        [Route("challenge/{id}")]
        public async Task<ActionResult> Index(string id,  ChallengeEntry challengeEntry)
        {
            var response = await _handler.Handle(Map(challengeEntry));

            if (response.IsValid)
                return Content(string.Empty);

            var model = new ChallengeViewModel
            {
                Characters = new List<int> {challengeEntry.FirstCharacterPosition, challengeEntry.SecondCharacterPosition},
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
                ChallengeElement1 = challengeEntry.Challenge1,
                ChallengeElement2 = challengeEntry.Challenge2,
                Balance = challengeEntry.Balance,
                FirstCharacterPosition = challengeEntry.FirstCharacterPosition,
                SecondCharacterPosition = challengeEntry.SecondCharacterPosition
            };
        }
    }
}