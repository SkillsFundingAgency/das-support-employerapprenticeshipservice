using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public class PayeLevySubmissionsHandler : IPayeLevySubmissionsHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILevySubmissionsRepository _levySubmissionsRepository;
        private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;


        public PayeLevySubmissionsHandler(IAccountRepository accountRepository,
            ILevySubmissionsRepository levySubmissionsRepository,
            IPayeSchemeObfuscator payeSchemeObfuscator)
        {
            _accountRepository = accountRepository;
            _levySubmissionsRepository = levySubmissionsRepository;
            _payeSchemeObfuscator = payeSchemeObfuscator;
        }

        public async Task<PayeLevySubmissionsResponse> Handle(string accountId, string payeId)
        {
            var account = await _accountRepository.Get(accountId, AccountFieldsSelection.RawSearchPayeSchemes);

            if (account == null)
            {
                return new PayeLevySubmissionsResponse
                {
                    StatusCode = PayeLevySubmissionsResponseCodes.AccountNotFound
                };
            }

            var selectedPayeScheme = account.PayeSchemes.First(o => o.Ref.Equals(payeId, StringComparison.OrdinalIgnoreCase));
            selectedPayeScheme.Ref = _payeSchemeObfuscator.ObscurePayeScheme(selectedPayeScheme.Ref);

            try
            {
                var levySubmissions = await _levySubmissionsRepository.Get(payeId);

                return new PayeLevySubmissionsResponse
                {
                    StatusCode = PayeLevySubmissionsResponseCodes.Success,
                    LevySubmissions = levySubmissions,
                    PayeScheme = selectedPayeScheme
                };
            }
            catch (Exception ex)
            {
                return new PayeLevySubmissionsResponse
                {
                    StatusCode = PayeLevySubmissionsResponseCodes.DeclarationsNotFound,
                    PayeScheme = selectedPayeScheme,
                    ResponseException = ex
                };
            }
        }

      
    }
}