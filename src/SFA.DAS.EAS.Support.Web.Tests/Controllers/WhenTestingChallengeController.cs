using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers
{
    
    public abstract class WhenTestingChallengeController
    {
        protected ChallengeController Unit;
        protected Mock<IChallengeHandler> MockChallengeHandler;
        [SetUp]
        public void Setup()
        {
            MockChallengeHandler = new Mock<IChallengeHandler>();
            Unit = new ChallengeController(MockChallengeHandler.Object);
        }
       
    }
    [TestFixture]
    public class WhenCallingIndexGet : WhenTestingChallengeController
    {
        [Test]
        public async Task ItShouldReturnTheChallengeViewWithAModelWhenThereIsAMatch()
        {
            var challengeResponse = new ChallengeResponse()
            {
                Account = new Account() { AccountId = 123, HashedAccountId = "ERERER", DasAccountName = "Test Account"},
                StatusCode = SearchResponseCodes.Success
            };

            var id = "123";

            MockChallengeHandler.Setup(x => x.Get(id))
                .ReturnsAsync(challengeResponse);

            var actual = await Unit.Index(id, "http:/temprui.org/callback", false);

            Assert.IsInstanceOf<ViewResult>(actual);
            var viewResult = (ViewResult)actual;
            Assert.IsInstanceOf<ChallengeViewModel>( viewResult.Model);
        }
        [Test]
        public async Task ItShouldReturnHttpNoFoundWhenTheSearchFails()
        {
            var challengeResponse = new ChallengeResponse()
            {
                Account = null,
                StatusCode = SearchResponseCodes.SearchFailed
            };

            var id = "123";

            MockChallengeHandler.Setup(x => x.Get(id))
                .ReturnsAsync(challengeResponse);

            var actual = await Unit.Index(id, "http:/temprui.org/callback", false);

            Assert.IsInstanceOf<HttpNotFoundResult>(actual);

        }
        [Test]
        public async Task ItShouldReturnHttpNoFoundWhenThereIsNotAMatch()
        {
            var challengeResponse = new ChallengeResponse()
            {
                Account = null,
                StatusCode = SearchResponseCodes.NoSearchResultsFound
            };

            var id = "123";

            MockChallengeHandler.Setup(x => x.Get(id))
                .ReturnsAsync(challengeResponse);

            var actual = await Unit.Index(id, "http:/temprui.org/callback", false);

            Assert.IsInstanceOf<HttpNotFoundResult>(actual);
            
        }
    }
    [TestFixture]
    public class WhenCallingIndexPost : WhenTestingChallengeController
    {
        [Test]
        public void ItShould()
        {
            Assert.Fail();
        }
    }
}