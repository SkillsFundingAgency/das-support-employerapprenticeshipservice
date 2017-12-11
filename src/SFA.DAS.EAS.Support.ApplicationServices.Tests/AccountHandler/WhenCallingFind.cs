using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    [TestFixture]
    public class WhenCallingFindSearchItems : WhenTestingAccountHandler
    {
        [Test]
        public async Task ItShouldReturnAnAccountForEachItemInResponseIfFound()
        {
            var accountDetailViewModels = new List<AccountDetailViewModel>
            {
                new AccountDetailViewModel
                {
                    AccountId = 123,
                    PayeSchemes = new ResourceList(new List<ResourceViewModel>
                    {
                        new ResourceViewModel {Id = Guid.NewGuid().ToString()}
                    }),
                    Balance = 0m,
                    DasAccountName = "Account 1",
                    DateRegistered = DateTime.MaxValue,
                    HashedAccountId = "ASDASD",
                    LegalEntities = new ResourceList(new List<ResourceViewModel>
                    {
                        new ResourceViewModel {Id = Guid.NewGuid().ToString()}
                    }),
                    OwnerEmail = "owner1@tempuri.org"
                },
                new AccountDetailViewModel
                {
                    AccountId = 124,
                    PayeSchemes = new ResourceList(new List<ResourceViewModel>
                    {
                        new ResourceViewModel {Id = Guid.NewGuid().ToString()}
                    }),
                    Balance = 0m,
                    DasAccountName = "Account 2",
                    DateRegistered = DateTime.MaxValue,
                    HashedAccountId = "DFDFD",
                    LegalEntities = new ResourceList(new List<ResourceViewModel>
                    {
                        new ResourceViewModel {Id = Guid.NewGuid().ToString()}
                    }),
                    OwnerEmail = "owner2@tempuri.org"
                }
            };

            MockAccountRepository.Setup(r => r.FindAllDetails()).ReturnsAsync(accountDetailViewModels);

            var actual = await Unit.FindSearchItems();
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public async Task ItShouldReturnAnEmptyCollectionIfNotFound()
        {
            var accountDetailViewModels = new List<AccountDetailViewModel>();

            MockAccountRepository.Setup(r => r.FindAllDetails()).ReturnsAsync(accountDetailViewModels);

            var actual = await Unit.FindSearchItems();
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }
    }

    [TestFixture]
    public class WhenCallingFind : WhenTestingAccountHandler
    {
        [Test]
        public async Task ItShouldReturnAccountInResponseIfFound()
        {
            var accountId = 123L;
            var orgid = accountId.ToString();
            var account = new Core.Models.Account {AccountId = accountId};


            MockAccountRepository.Setup(r => r.Get(orgid, AccountFieldsSelection.None)).ReturnsAsync(account);

            var actual = await Unit.Find(orgid);
            Assert.IsNotNull(actual);
            Assert.AreEqual(SearchResponseCodes.Success, actual.StatusCode);
            Assert.IsNotNull(actual.Account);
        }

        [Test]
        public async Task ItShouldReturnNoAccountInTheResponseIfNotFound()
        {
            var accountId = 123L;
            var orgid = accountId.ToString();
            var account = new Core.Models.Account {AccountId = accountId};


            MockAccountRepository.Setup(r => r.Get(orgid, AccountFieldsSelection.None))
                .ReturnsAsync(null as Core.Models.Account);

            var actual = await Unit.Find(orgid);
            Assert.IsNotNull(actual);
            Assert.AreEqual(new AccountDetailOrganisationsResponse().StatusCode, actual.StatusCode);
            Assert.IsNull(actual.Account);
        }
    }
}