using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Support.Shared;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public class AccountHandler : IAccountHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapAccountSearch _mapAccountSearch;

        public AccountHandler(IAccountRepository accountRepository, IMapAccountSearch mapAccountSearch)
        {
            _accountRepository = accountRepository;
            _mapAccountSearch = mapAccountSearch;
        }

        public async Task<AccountDetailOrganisationsResponse> FindOrganisations(string id)
        {
            var response = new AccountDetailOrganisationsResponse
            {
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var record = await _accountRepository.Get(id, AccountFieldsSelection.Organisations);

            if (record != null)
            {
                response.StatusCode = SearchResponseCodes.Success;
                response.Account = record;
            }

            return response;
        }

        public async Task<AccountPayeSchemesResponse> FindPayeSchemes(string id)
        {
            var response = new AccountPayeSchemesResponse
            {
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var record = await _accountRepository.Get(id, AccountFieldsSelection.PayeSchemes);

            if (record != null)
            {
                response.StatusCode = SearchResponseCodes.Success;
                response.Account = record;
            }

            return response;
        }

        public async Task<AccountFinanceResponse> FindFinance(string id)
        {
            var response = new AccountFinanceResponse
            {
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var account = await _accountRepository.Get(id, AccountFieldsSelection.Finance);

            if (account != null)
            {
                response.StatusCode = SearchResponseCodes.Success;
                response.Account = account;
                response.Balance = account.Transactions.Any()
                    ? account.Transactions.First().Balance
                    : await _accountRepository.GetAccountBalance(id);
            }

            return response;
        }

        public async Task<IEnumerable<SearchItem>> FindSearchItems()
        {
            var models = await _accountRepository.FindAllDetails();
            return models.Select(x => _mapAccountSearch.Map(x)).ToList();
        }

        public async Task<AccountReponse> Find(string id)
        {
            var response = new AccountReponse
            {
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var account = await _accountRepository.Get(id, AccountFieldsSelection.None);

            if (account != null)
            {
                response.StatusCode = SearchResponseCodes.Success;
                response.Account = account;
            }

            return response;
        }
    }
}