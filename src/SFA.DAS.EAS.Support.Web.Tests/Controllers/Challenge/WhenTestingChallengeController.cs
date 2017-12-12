using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge
{
    
    public abstract class WhenTestingChallengeController
    {
        protected ChallengeController Unit;
        protected Mock<IChallengeHandler> MockChallengeHandler;
        protected RouteData RouteData;
        protected Mock<HttpContextBase> MockContextBase;
        protected Mock<HttpRequestBase> MockRequestBase;
        protected Mock<HttpResponseBase> MockResponseBase;
        protected Mock<IPrincipal> MockUser;
        protected ControllerContext UnitControllerContext;

        [SetUp]
        public void Setup()
        {
            MockChallengeHandler = new Mock<IChallengeHandler>();
            Unit = new ChallengeController(MockChallengeHandler.Object);

            RouteData = new RouteData();
            MockContextBase = new Mock<HttpContextBase>();

            MockRequestBase = new Mock<HttpRequestBase>();
            MockResponseBase = new Mock<HttpResponseBase>();
            MockUser = new Mock<IPrincipal>();

            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
            MockContextBase.Setup(x => x.User).Returns(MockUser.Object);
            UnitControllerContext = new ControllerContext(MockContextBase.Object, RouteData, Unit);

            Unit.ControllerContext = UnitControllerContext;
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
                Account = new Core.Models.Account() { AccountId = 123, HashedAccountId = "ERERER", DasAccountName = "Test Account"},
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
        public async Task ItShouldReturnEmptyStringContentWhenTheChallengeEntryIsValid()
        {
            var challengeEntry = new ChallengeEntry()
            {
                Id = "123", Balance = "£1000",
                Challenge1 = 1, Challenge2 = 2,
                FirstCharacterPosition = "0", SecondCharacterPosition = "1",
                Url = "https://tempuri.org/challenge/me/to/a/deul/any/time"
            };

            var query = new ChallengePermissionQuery()
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "2",
                FirstCharacterPosition = "0",
                SecondCharacterPosition = "1",
                Url = "https://tempuri.org/challenge/me/to/a/deul/any/time"
            };

            ChallengePermissionResponse response = new ChallengePermissionResponse()
            {
                Id = challengeEntry.Id, Url = challengeEntry.Url, IsValid = true, 
                
            };

            MockChallengeHandler.Setup(x => x.Handle(It.IsAny<ChallengePermissionQuery>()))
                .ReturnsAsync(response);

            var actual = await Unit.Index(challengeEntry);

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ContentResult>(actual);
            Assert.IsInstanceOf<string>(((ContentResult)actual).Content);
            Assert.AreEqual(string.Empty, ((ContentResult)actual).Content);

        }

        /// <summary>
        /// Note that this Controler method scenario sets HttpResponse.StatusCode = 403 (Forbidden), this result is not testable from a unit test
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ItShouldReturnAViewModelWhenTheChallengeEntryIsInvalid()
        {
            var challengeEntry = new ChallengeEntry()
            {
                Id = "123",
                Balance = "£1000",
                Challenge1 = 1,
                Challenge2 = 2,
                FirstCharacterPosition = "0",
                SecondCharacterPosition = "1",
                Url = "https://tempuri.org/challenge/me/to/a/deul/any/time"
            };

            var query = new ChallengePermissionQuery()
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "2",
                FirstCharacterPosition = "0",
                SecondCharacterPosition = "1",
                Url = "https://tempuri.org/challenge/me/to/a/deul/any/time"
            };

            ChallengePermissionResponse response = new ChallengePermissionResponse()
            {
                Id = challengeEntry.Id,
                Url = challengeEntry.Url,
                IsValid = false,

            };

            MockChallengeHandler.Setup(x => x.Handle(It.IsAny<ChallengePermissionQuery>()))
                .ReturnsAsync(response);

            var actual = await Unit.Index(challengeEntry);

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.IsInstanceOf<ChallengeViewModel>(((ViewResult)actual).Model);
            Assert.AreEqual(true, ((ChallengeViewModel)((ViewResult)actual).Model).HasError);

        }

    }
}