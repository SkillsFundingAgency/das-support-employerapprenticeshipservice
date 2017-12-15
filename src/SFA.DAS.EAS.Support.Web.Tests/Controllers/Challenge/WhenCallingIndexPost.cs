using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge
{
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