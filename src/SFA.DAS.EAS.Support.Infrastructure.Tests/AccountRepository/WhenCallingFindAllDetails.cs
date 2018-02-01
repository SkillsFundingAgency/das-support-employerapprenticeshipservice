using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingFindAllDetails : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnAnEmptyListIfGetAccountsThrowsAnException()
        {
            var id = "123";
            var accountWithBalanceViewModels = new List<AccountWithBalanceViewModel>
            {
                new AccountWithBalanceViewModel
                {
                    AccountId = 123,
                    AccountHashId = "ERERE",
                    Balance = 1000m,
                    Href = "http://tempuri.org/account/ERERE",
                    AccountName = "Test Account",
                    IsLevyPayer = true
                }
            };
            var pagedApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                Data = accountWithBalanceViewModels,
                Page = 1,
                TotalPages = 2
            };

            AccountApiClient.Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
                .ReturnsAsync(pagedApiResponseViewModel);

            var e = new Exception("Some exception message");
            AccountApiClient.Setup(x => x.GetAccount(It.IsAny<string>()))
                .ThrowsAsync(e);


            var actual = await Unit.FindAllDetails();


            AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.Exactly(2));
            AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Exactly(2));

            Logger.Verify(x => x.Error(e, $"Exception while retrieving details for account ID {accountWithBalanceViewModels.First().AccountHashId}"));

            Assert.IsNotNull(actual);
            var list = actual.ToList();
            CollectionAssert.IsEmpty(list);
        }

        [Test]
        public async Task ItShouldReturnAnEmptyListIfGetAccountsThrowsAnHttpRequestException()
        {
            var id = "123";
            var accountWithBalanceViewModels = new List<AccountWithBalanceViewModel>
            {
                new AccountWithBalanceViewModel
                {
                    AccountId = 123,
                    AccountHashId = "ERERE",
                    Balance = 1000m,
                    Href = "http://tempuri.org/account/ERERE",
                    AccountName = "Test Account",
                    IsLevyPayer = true
                }
            };
            var pagedApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                Data = accountWithBalanceViewModels,
                Page = 1,
                TotalPages = 2
            };

            AccountApiClient.Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
                .ReturnsAsync(pagedApiResponseViewModel);

            var e = new HttpRequestException("Some exception message");
            AccountApiClient.Setup(x => x.GetAccount(It.IsAny<string>()))
                .ThrowsAsync(e);


            var actual = await Unit.FindAllDetails();


            AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.Exactly(2));
            AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Exactly(2));

            Logger.Verify(x => x.Error(e, $"Exception while retrieving details for account ID {accountWithBalanceViewModels.First().AccountHashId}"));

            Assert.IsNotNull(actual);
            var list = actual.ToList();
            CollectionAssert.IsEmpty(list);
        }


        [Test]
        public async Task ItShouldReturnTheEntireListOfAccounts()
        {
            var id = "123";
            var accountWithBalanceViewModels = new List<AccountWithBalanceViewModel>
            {
                new AccountWithBalanceViewModel
                {
                    AccountId = 123,
                    AccountHashId = "ERERE",
                    Balance = 1000m,
                    Href = "http://tempuri.org/account/ERERE",
                    AccountName = "Test Account",
                    IsLevyPayer = true
                }
            };
            var pagedApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                Data = accountWithBalanceViewModels,
                Page = 1,
                TotalPages = 2
            };

            AccountApiClient.Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), 10, null))
                .ReturnsAsync(pagedApiResponseViewModel);

            AccountApiClient.Setup(x => x.GetAccount(It.IsAny<string>()))
                .ReturnsAsync(new AccountDetailViewModel());


            var actual = await Unit.FindAllDetails();


            AccountApiClient.Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), null), Times.Exactly(2));
            AccountApiClient.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Exactly(2));

            Assert.IsNotNull(actual);
            var list = actual.ToList();
            CollectionAssert.IsNotEmpty(list);
            Assert.AreEqual(2, list.Count());
        }
    }
}