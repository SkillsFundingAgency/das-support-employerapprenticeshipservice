using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldSelectionNone : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnJustTheAccount()
        {
            string id = "123";

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(new AccountDetailViewModel());

            var actual = await Unit.Get(id, AccountFieldsSelection.None);

            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);

            Assert.IsNotNull(actual);
            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.LegalEntities);
            Assert.IsNull(actual.TeamMembers);
            Assert.IsNull(actual.Transactions);

        }
        [Test]
        public async Task ItShouldReturnNullOnException()
        {
            string id = "123";

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ThrowsAsync(new Exception());

            var actual = await Unit.Get(id, AccountFieldsSelection.None);

            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);
            Logger.Verify(x => x.Error(It.IsAny<Exception>(), $"Account with id {id} not found"));

            Assert.IsNull(actual);

        }
    }
}