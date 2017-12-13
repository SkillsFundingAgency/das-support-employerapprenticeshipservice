using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

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
}