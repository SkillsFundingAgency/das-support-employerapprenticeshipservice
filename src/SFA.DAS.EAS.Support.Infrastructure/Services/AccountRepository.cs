using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly int _accountsPerPage = 10;
        private readonly IDatetimeService _datetimeService;
        private readonly ILog _logger;
        private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;

        public AccountRepository(IAccountApiClient accountApiClient, IPayeSchemeObfuscator payeSchemeObfuscator,
            IDatetimeService datetimeService, ILog logger)
        {
            _accountApiClient = accountApiClient;
            _payeSchemeObfuscator = payeSchemeObfuscator;
            _datetimeService = datetimeService;
            _logger = logger;
        }

        public async Task<Core.Models.Account> Get(string id, AccountFieldsSelection selection)
        {
            try
            {
                _logger.Debug($"IAccountApiClient.GetResource<AccountDetailViewModel>(\"/api/accounts/{id}\");");
                var response = await _accountApiClient.GetResource<AccountDetailViewModel>($"/api/accounts/{id}");

                return await GetAdditionalFields(response, selection);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Account with id {id} not found");
                return null;
            }
        }

        public async Task<IEnumerable<AccountDetailViewModel>> FindAllDetails()
        {
            var results = new List<AccountDetailViewModel>();
            foreach (var account in await FindAll())
                try
                {
                    var viewModel = await _accountApiClient.GetAccount(account.AccountHashId);
                    results.Add(viewModel);
                }
                catch (HttpRequestException e)
                {
                    _logger.Warn($"The Account API Http request threw an exception:\r\n{e}");
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"A general exception has been thrown while requesting employer account detals");
                }

            return results;
        }

        public async Task<decimal> GetAccountBalance(string id)
        {
            try
            {
                var response = await _accountApiClient.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}");

                _logger.Debug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetResource)}<{nameof(AccountWithBalanceViewModel)}>(\"/api/accounts/{id}\"); {response.Balance}");

                return response.Balance;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Account Balance with id {id} not found");
                return 0;
            }
        }

        private async Task<IEnumerable<AccountWithBalanceViewModel>> FindAll()
        {
            var results = new List<AccountWithBalanceViewModel>();


            var accounts = await _accountApiClient.GetPageOfAccounts(1, _accountsPerPage);

            results.AddRange(accounts.Data);

            for (var i = 2; i <= accounts.TotalPages; i++)
            {
                var page = await _accountApiClient.GetPageOfAccounts(i, _accountsPerPage);
                results.AddRange(page.Data);
            }
            return results;
        }

        private async Task<Core.Models.Account> GetAdditionalFields(
            AccountDetailViewModel response, AccountFieldsSelection selection)
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
                var transactions = await _accountApiClient.GetTransactions(accountId, financialYearIterator.Year,
                    financialYearIterator.Month);

                response.AddRange(transactions);
                financialYearIterator = financialYearIterator.AddMonths(1);
            }

            return GetFilteredTransactions(response);
        }

        private List<TransactionViewModel> GetFilteredTransactions(IEnumerable<TransactionViewModel> response)
        {
            return response.Where(x => x.Description != null && x.Amount != 0).OrderByDescending(x => x.DateCreated)
                .ToList();
        }

        private async Task<IEnumerable<PayeSchemeViewModel>> GetPayeSchemes(AccountDetailViewModel response)
        {
            var result = new List<PayeSchemeViewModel>();
            foreach (var payeScheme in response.PayeSchemes)
            {
                var obscured = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Id).Replace("/", "%252f");
                var paye = payeScheme.Id.Replace("/", "%252f");
                _logger.Debug(
                    $"IAccountApiClient.GetResource<PayeSchemeViewModel>(\"{payeScheme.Href.Replace(paye, obscured)}\");");
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
            return result.AddedDate <= DateTime.UtcNow &&
                   (result.RemovedDate == null || result.RemovedDate > DateTime.UtcNow);
        }

        private async Task<ICollection<TeamMemberViewModel>> GetAccountTeamMembers(string resultHashedAccountId)
        {
            try
            {
                _logger.Debug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetAccountUsers)}(\"{resultHashedAccountId}\");");
                var teamMembers = await _accountApiClient.GetAccountUsers(resultHashedAccountId);

                return teamMembers;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Account Team Member with id {resultHashedAccountId} not found");
                return new List<TeamMemberViewModel>();
            }
        }

        private async Task<List<LegalEntityViewModel>> GetLegalEntities(ResourceList responseLegalEntities)
        {
            var legalEntitiesList = new List<LegalEntityViewModel>();

            foreach (var legalEntity in responseLegalEntities)
            {
                _logger.Debug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetResource)}<{nameof(LegalEntityViewModel)}>(\"{legalEntity.Href}\");");
                var legalResponse = await _accountApiClient.GetResource<LegalEntityViewModel>(legalEntity.Href);

                if (legalResponse.AgreementStatus == EmployerAgreementStatus.Signed ||
                    legalResponse.AgreementStatus == EmployerAgreementStatus.Pending ||
                    legalResponse.AgreementStatus == EmployerAgreementStatus.Superseded)
                    legalEntitiesList.Add(legalResponse);
            }
            return legalEntitiesList;
        }

        private Core.Models.Account MapToAccount(AccountDetailViewModel accountDetailViewModel)
        {
            return new Core.Models.Account
            {
                AccountId = accountDetailViewModel.AccountId,
                DasAccountName = accountDetailViewModel.DasAccountName,
                HashedAccountId = accountDetailViewModel.HashedAccountId,
                DateRegistered = accountDetailViewModel.DateRegistered,
                OwnerEmail = accountDetailViewModel.OwnerEmail
            };
        }
    }
}