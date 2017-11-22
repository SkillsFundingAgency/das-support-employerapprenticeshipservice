using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests
{
    [TestFixture]
    public class WhenTestingAccountHandler
    {
        private AccountHandler _unit;

        private Mock<IAccountRepository> _mockAccountRepository;
        [SetUp]
        public void Setup()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _unit = new AccountHandler(_mockAccountRepository.Object);
        }

        [Ignore("Placeholder")] [Test] public void ItShouldTestTheBehaviour() { Assert.Fail(); }


        [Test]
        public async Task ItShouldFindOrganisations()
        {
            var accountId = 123L;
            var orgid = accountId.ToString();
            var account = new Account() { AccountId = accountId };


            _mockAccountRepository.Setup(r => r.Get(orgid, AccountFieldsSelection.Organisations)).ReturnsAsync(account);

            var actual = await _unit.FindOrganisations(orgid);
            Assert.IsNotNull(actual);
            Assert.AreEqual(SearchResponseCodes.Success, actual.StatusCode);
            Assert.IsNotNull(actual.Account);

        }

        [Test]
        public async Task ItShouldNotFindOrganisations()
        {
            var accountId = 123L;
            var orgid = accountId.ToString();
            var account = new Account() { AccountId = accountId };


            _mockAccountRepository.Setup(r => r.Get(orgid, AccountFieldsSelection.Organisations)).ReturnsAsync(null as Account);

            var actual = await _unit.FindOrganisations(orgid);
            Assert.IsNotNull(actual);
            Assert.AreEqual(new AccountDetailOrganisationsResponse().StatusCode, actual.StatusCode);
            Assert.IsNull(actual.Account);
        }
        

    }
}