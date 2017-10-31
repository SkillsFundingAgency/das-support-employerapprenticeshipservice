using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sfa.Das.Console.ApplicationServices.Responses;
using Sfa.Das.Console.Core.Domain.Model;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using ESFA.DAS.Support.Shared;

namespace Sfa.Das.Console.ApplicationServices.Services
{
    public class AccountHandler : IAccountHandler
    {
        private readonly IAccountRepository _accountRepository;

        public AccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
                response.Balance = account.Transactions.Any() ? account.Transactions.First().Balance : await _accountRepository.GetAccountBalance(id);
            }

            return response;
        }

        public IEnumerable<SearchItem> FindSearchItems()
        {
            var models = _accountRepository.FindAllDetails();
            foreach (var model in models)
            {
                yield return MapToSearch(model);
            }
        }

        private SearchItem MapToSearch(AccountDetailViewModel arg)
        {
            var keywords = new List<string>
            {
                arg.HashedAccountId,
                arg.DasAccountName,
                arg.OwnerEmail
            };

            keywords.AddRange(arg.PayeSchemes.Select(x => x.Id));

            return new SearchItem
            {
                SearchId = $"ACC-{arg.DasAccountId}",
                Html = $"<div><a href=\"/resource/?key=account&id={arg.DasAccountId}\">{arg.DasAccountName}</a></div>",
                Keywords = keywords.Where(x => x != null).ToArray()
            };
        }

    }
}