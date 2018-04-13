using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldsSelectionFinance : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnTheMatchingAccountWithTransaction()
        {
            var id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                AccountId = 123,
                Balance = 0m,
                PayeSchemes = new ResourceList(
                    new List<ResourceViewModel>
                    {
                        new ResourceViewModel
                        {
                            Id = "123/123456",
                            Href = "https://tempuri.org/payescheme/{1}"
                        }
                    }),
                LegalEntities = new ResourceList(
                    new List<ResourceViewModel>
                    {
                        new ResourceViewModel
                        {
                            Id = "TempUri Limited",
                            Href = "https://tempuri.org/organisation/{1}"
                        }
                    }),

                HashedAccountId = "DFGH",
                DateRegistered = DateTime.Today.AddYears(-2),
                OwnerEmail = "Owner@tempuri.org",
                DasAccountName = "Test Account 1"
            };

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            var obscuredPayePayeScheme = "123/123456";

            PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
                .Returns(obscuredPayePayeScheme);


            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                AddedDate = DateTime.Today.AddMonths(-4),
                Ref = "123/123456",
                Name = "123/123456",
                DasAccountId = "123",
                RemovedDate = null
            };

            AccountApiClient.Setup(x => x.GetResource<PayeSchemeViewModel>(It.IsAny<string>()))
                .ReturnsAsync(payeSchemeViewModel);


            /* 
             * This is a testing HACK to avoid using a concrete datetime service
             * because our collegues used DateTime.Now in the code! 
             * See ASCS-83 for a fix
             */
            var now = DateTime.Now.Date;
            var startOfFirstFinancialYear = new DateTime(2017, 4, 1);
            var monthsToQuery = (now.Year - startOfFirstFinancialYear.Year) * 12 +
                                            (now.Month - startOfFirstFinancialYear.Month) + 1;
            
            DatetimeService.Setup(x => x.GetBeginningFinancialYear(startOfFirstFinancialYear))
                .Returns(startOfFirstFinancialYear);

            
            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new TransactionsViewModel
            {
                new TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                },
                new TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };

            AccountApiClient.Setup(x => x.GetTransactions(accountDetailViewModel.HashedAccountId,
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(transactionsViewModel
                );

            var actual = await _sut.Get(id, AccountFieldsSelection.Finance);

            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));

            PayeSchemeObfuscator
                .Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

            AccountApiClient
                .Verify(x => x.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                    Times.Exactly(monthsToQuery));

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.PayeSchemes);
            Assert.AreEqual(1, actual.PayeSchemes.Count());
            Assert.IsNotNull(actual.Transactions);
            Assert.AreEqual(2 * monthsToQuery, actual.Transactions.Count());

            Assert.IsNull(actual.LegalEntities);
            Assert.IsNull(actual.TeamMembers);
        }


        [Test]
        public async Task ItShouldReturnZeroAccountTransactionsIfTheApiThrowsAnException()
        {
            var id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                AccountId = 123,
                Balance = 0m,
                PayeSchemes = new ResourceList(
                    new List<ResourceViewModel>
                    {
                        new ResourceViewModel
                        {
                            Id = "123/123456",
                            Href = "https://tempuri.org/payescheme/{1}"
                        }
                    }),
                LegalEntities = new ResourceList(
                    new List<ResourceViewModel>
                    {
                        new ResourceViewModel
                        {
                            Id = "TempUri Limited",
                            Href = "https://tempuri.org/organisation/{1}"
                        }
                    }),

                HashedAccountId = "DFGH",
                DateRegistered = DateTime.Today.AddYears(-2),
                OwnerEmail = "Owner@tempuri.org",
                DasAccountName = "Test Account 1"
            };

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            var obscuredPayePayeScheme = "123/123456";

            PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
                .Returns(obscuredPayePayeScheme);


            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                AddedDate = DateTime.Today.AddMonths(-4),
                Ref = "123/123456",
                Name = "123/123456",
                DasAccountId = "123",
                RemovedDate = null
            };

            AccountApiClient.Setup(x => x.GetResource<PayeSchemeViewModel>(It.IsAny<string>()))
                .ReturnsAsync(payeSchemeViewModel);


            /* 
             * This is a testing HACK to avoid using a concrete datetime service
             * because our collegues used DateTime.Now in the code! 
             * See ASCS-83 for a fix
             */
            var now = DateTime.Now.Date;
            var startOfFirstFinancialYear = new DateTime(2017, 4, 1);
            var monthsToQuery = (now.Year - startOfFirstFinancialYear.Year) * 12 +
                                            (now.Month - startOfFirstFinancialYear.Month) + 1;

            DatetimeService.Setup(x => x.GetBeginningFinancialYear(startOfFirstFinancialYear))
                .Returns(startOfFirstFinancialYear);


            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new TransactionsViewModel
            {
                new TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                },
                new TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };

            AccountApiClient.Setup(x => x.GetTransactions(accountDetailViewModel.HashedAccountId,
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ThrowsAsync(new Exception("Waaaaaaaah"));

            var actual = await _sut.Get(id, AccountFieldsSelection.Finance);

            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));

            PayeSchemeObfuscator
                .Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));

            AccountApiClient
                .Verify(x => x.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                    Times.Exactly(monthsToQuery));

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.PayeSchemes);
            Assert.AreEqual(1, actual.PayeSchemes.Count());
            Assert.IsNotNull(actual.Transactions);
            Assert.AreEqual(0, actual.Transactions.Count());

            Assert.IsNull(actual.LegalEntities);
            Assert.IsNull(actual.TeamMembers);
        }


    }
}