﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly IAccountRepository _accountRepository;

        public ChallengeRepository(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> CheckData(Core.Models.Account record, ChallengePermissionQuery message)
        {
            var balance = await _accountRepository.GetAccountBalance(message.Id);

            var validPayeSchemesData = CheckPayeSchemesData(record.PayeSchemes, message);

            decimal messageBalance;

            if (!decimal.TryParse(message.Balance.Replace("£", string.Empty), out messageBalance))
                return false;

            return Math.Truncate(balance) == Math.Truncate(Convert.ToDecimal(messageBalance)) && validPayeSchemesData;
        }

        private bool CheckPayeSchemesData(IEnumerable<PayeSchemeViewModel> recordPayeSchemes,
            ChallengePermissionQuery message)
        {
            var challengeInput = new List<string>
            {
                message.ChallengeElement1.ToLower(),
                message.ChallengeElement2.ToLower()
            };

            var list = recordPayeSchemes.Select(x => x.Ref.Replace("/", string.Empty));
            var index1 = message.FirstCharacterPosition;
            var index2 = message.SecondCharacterPosition;

            return list.Any(x => x[index1].ToString().ToLower() == challengeInput[0] &&
                                 x[index2].ToString().ToLower() == challengeInput[1]);
        }
    }
}