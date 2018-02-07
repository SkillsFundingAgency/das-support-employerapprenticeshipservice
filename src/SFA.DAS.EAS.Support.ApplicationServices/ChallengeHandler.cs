﻿using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services;

namespace SFA.DAS.EAS.Support.ApplicationServices
{
    public class ChallengeHandler : IChallengeHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeService _challengeService;

        public ChallengeHandler(IAccountRepository accountRepository, IChallengeService challengeService,
            IChallengeRepository challengeRepository)
        {
            _accountRepository = accountRepository;
            _challengeService = challengeService;
            _challengeRepository = challengeRepository;
        }

        public async Task<ChallengeResponse> Get(string id)
        {
            var response = new ChallengeResponse
            {
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var record = await _accountRepository.Get(id, AccountFieldsSelection.PayeSchemes);


            if (record != null)
            {
                response.StatusCode = SearchResponseCodes.Success;
                response.Account = record;
                response.Characters = _challengeService.GetPayeSchemesCharacters(record.PayeSchemes);
            }

            return response;
        }

        public async Task<ChallengePermissionResponse> Handle(ChallengePermissionQuery message)
        {
            var response = new ChallengePermissionResponse
            {
                Id = message.Id,
                Url = message.Url,
                IsValid = false
            };

            int balance;

            if (string.IsNullOrEmpty(message.Balance)
                || string.IsNullOrEmpty(message.ChallengeElement1)
                || string.IsNullOrEmpty(message.ChallengeElement2)
                || !int.TryParse(message.Balance.Split('.')[0].Replace("£", string.Empty), out balance)
                || message.ChallengeElement1.Length != 1
                || message.ChallengeElement2.Length != 1)
                return response;

            var record = await _accountRepository.Get(message.Id, AccountFieldsSelection.ChallengePayeSchemes);

            if (record != null)
            {
                var isValid = await _challengeRepository.CheckData(record, message);

                response.IsValid = isValid;
            }

            return response;
        }
    }
}