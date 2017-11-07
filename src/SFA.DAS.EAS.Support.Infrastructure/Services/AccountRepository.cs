using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sfa.Das.Console.ApplicationServices;
using Sfa.Das.Console.ApplicationServices.Services;
using Sfa.Das.Console.Core.Domain.Model;
using Sfa.Das.Console.Core.Helpers;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;

namespace Sfa.Das.Console.Infrastructure
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;
        private readonly IDatetimeService _datetimeService;

        public AccountRepository(IAccountApiClient accountApiClient, IPayeSchemeObfuscator payeSchemeObfuscator, IDatetimeService datetimeService)
        {
            _accountApiClient = accountApiClient;
            _payeSchemeObfuscator = payeSchemeObfuscator;
            _datetimeService = datetimeService;
        }

        public async Task<Account> Get(string id, AccountFieldsSelection selection)
        {
            try
            {
                //_logger.Debug($"IAccountApiClient.GetResource<AccountDetailViewModel>(\"/api/accounts/{id}\");");
                var response = await _accountApiClient.GetResource<AccountDetailViewModel>($"/api/accounts/{id}");

                return await GetAdditionalFields(response, selection);
            }
            catch (Exception e)
            {
                //_logger.Error(e, $"Account with id {id} not found");
                return null;
            }
        }

        private IEnumerable<AccountWithBalanceViewModel> FindAll()
        {
            var size = 10;

            var accounts = _accountApiClient.GetPageOfAccounts(1, size);
            Task.WaitAll(accounts);
            foreach (var model in accounts.Result.Data)
            {
                yield return model;
            }

            for (int i = 2; i <= accounts.Result.TotalPages; i++)
            {
                var page = _accountApiClient.GetPageOfAccounts(i, size);
                foreach (var model in page.Result.Data)
                {
                    yield return model;
                }
            }
        }

        public IEnumerable<AccountDetailViewModel> FindAllDetails()
        {
            foreach (var account in FindAll())
            {
                var task = _accountApiClient.GetAccount(account.AccountHashId);
                Task.WaitAll(task);
                var result = task.Result;

                if (result != null)
                {
                    yield return result;
                }
            }
        }

        public async Task<decimal> GetAccountBalance(string id)
        {
            try
            {
                var response = await _accountApiClient.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}");

                //_logger.Debug($"IAccountApiClient.GetResource<AccountWithBalanceViewModel>(\"/api/accounts/{id}\"); {response.Balance}");

                return response.Balance;
            }
            catch (Exception e)
            {
                //_logger.Error(e, $"Account Balance with id {id} not found");
                return 0;
            }
        }

        private async Task<Account> GetAdditionalFields(AccountDetailViewModel response, AccountFieldsSelection selection)
        {
            var result = MapToAccount(response);

            switch (selection)
            {
                case AccountFieldsSelection.Organisations:
                    var legalEntities = await GetLegalEntities(response.LegalEntities);
                    result.LegalEntities = legalEntities;
                    break;
                case AccountFieldsSelection.TeamMembers:
                    var teamMembers = await GetAccountTeamMembers(result.HashedAccountId);
                    result.TeamMembers = teamMembers;
                    break;
                case AccountFieldsSelection.PayeSchemes:
                    var payeSchemes = await GetObscuredPayeSchemes(response);
                    result.PayeSchemes = payeSchemes;
                    return result;
                case AccountFieldsSelection.ChallengePayeSchemes:
                    var challengePayeSchemes = await GetPayeSchemes(response);
                    result.PayeSchemes = challengePayeSchemes;
                    return result;
                case AccountFieldsSelection.Finance:
                    var payeSchemeData = await GetObscuredPayeSchemes(response);
                    result.PayeSchemes = payeSchemeData;
                    var transactions = await GetAccountTransactions(response.HashedAccountId);
                    result.Transactions = transactions;
                    return result;
            }

            return result;
        }

        private async Task<List<TransactionViewModel>> GetAccountTransactions(string accountId)
        {
            var endDate = DateTime.Now;
            var financialYearIterator = _datetimeService.GetBeginningFinancialYear(endDate);
            var response = new List<TransactionViewModel>();

            while (financialYearIterator <= endDate)
            {
                var transactions = await _accountApiClient.GetTransactions(accountId, financialYearIterator.Year, financialYearIterator.Month);

                response.AddRange(transactions);
                financialYearIterator = financialYearIterator.AddMonths(1);
            }

            return GetFilteredTransactions(response);
        }

        private List<TransactionViewModel> GetFilteredTransactions(IEnumerable<TransactionViewModel> response)
        {
            return response.Where(x => x.Description != null && x.Amount != 0).OrderByDescending(x => x.DateCreated).ToList();
        }

        private async Task<IEnumerable<PayeSchemeViewModel>> GetPayeSchemes(AccountDetailViewModel response)
        {
            var result = new List<PayeSchemeViewModel>();
            foreach (var payeScheme in response.PayeSchemes)
            {
                var obscured = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Id).Replace("/", "%252f");
                var paye = payeScheme.Id.Replace("/", "%252f");
                //_logger.Debug($"IAccountApiClient.GetResource<PayeSchemeViewModel>(\"{payeScheme.Href.Replace(paye, obscured)}\");");
                var payeSchemeViewModel = await _accountApiClient.GetResource<PayeSchemeViewModel>(payeScheme.Href);

                if (IsValidPayeScheme(payeSchemeViewModel))
                {
                    var item = new PayeSchemeViewModel
                    {
                        Ref = payeSchemeViewModel.Ref,
                        DasAccountId = payeSchemeViewModel.DasAccountId,
                        AddedDate = payeSchemeViewModel.AddedDate,
                        RemovedDate = payeSchemeViewModel.RemovedDate,
                        Name = payeSchemeViewModel.Name
                    };
                    result.Add(item);
                }
            }

            return result.OrderBy(x => x.Ref);
        }

        private async Task<IEnumerable<PayeSchemeViewModel>> GetObscuredPayeSchemes(AccountDetailViewModel response)
        {
            var payeSchemes = await GetPayeSchemes(response);

            return payeSchemes.Select(payeScheme => new PayeSchemeViewModel
            {
                Ref = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Ref),
                DasAccountId = payeScheme.DasAccountId,
                AddedDate = payeScheme.AddedDate,
                RemovedDate = payeScheme.RemovedDate,
                Name = payeScheme.Name
            })
                .ToList();
        }

        private bool IsValidPayeScheme(PayeSchemeViewModel result)
        {
            return result.AddedDate <= DateTime.UtcNow && (result.RemovedDate == null || result.RemovedDate > DateTime.UtcNow);
        }

        private async Task<ICollection<TeamMemberViewModel>> GetAccountTeamMembers(string resultHashedAccountId)
        {
            try
            {
                //_logger.Debug($"IAccountApiClient.GetAccountUsers(\"{resultHashedAccountId}\");");
                var teamMembers = await _accountApiClient.GetAccountUsers(resultHashedAccountId);

                return teamMembers;
            }
            catch (Exception e)
            {
                //_logger.Error(e, $"Account Team Member with id {resultHashedAccountId} not found");
                return new List<TeamMemberViewModel>();
            }
        }

        private async Task<List<LegalEntityViewModel>> GetLegalEntities(ResourceList responseLegalEntities)
        {
            var legalEntitiesList = new List<LegalEntityViewModel>();

            foreach (var legalEntity in responseLegalEntities)
            {
                //_logger.Debug($"IAccountApiClient.GetResource<LegalEntityViewModel>(\"{legalEntity.Href}\");");
                var legalResponse = await _accountApiClient.GetResource<LegalEntityViewModel>(legalEntity.Href);

                if (legalResponse.AgreementStatus == EmployerAgreementStatus.Signed ||
                    legalResponse.AgreementStatus == EmployerAgreementStatus.Pending ||
                    legalResponse.AgreementStatus == EmployerAgreementStatus.Superseded)
                {
                    legalEntitiesList.Add(legalResponse);
                }
            }
            return legalEntitiesList;
        }

        private Account MapToAccount(AccountDetailViewModel accountDetailViewModel)
        {
            return new Account
            {
                AccountId = accountDetailViewModel.AccountId,
                DasAccountName = accountDetailViewModel.DasAccountName,
                HashedAccountId = accountDetailViewModel.HashedAccountId,
                DateRegistered = accountDetailViewModel.DateRegistered,
                OwnerEmail = accountDetailViewModel.OwnerEmail,
            };
        }
    }
}
