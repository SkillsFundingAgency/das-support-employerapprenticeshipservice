using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.Support.Shared.Discovery;
using SFA.DAS.Support.Shared.SearchIndexModel;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Manifest
{
    [TestFixture]
    public class WhenTestingManifestController
    {
        [SetUp]
        public void Setup()
        {
            _accountHandler = new Mock<IAccountHandler>();
            _unit = new ManifestController(_accountHandler.Object);


            _urlHelper = new Mock<UrlHelper>();
            _unit.Url = _urlHelper.Object;
            _urlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns(@"~\");
        }

        private ManifestController _unit;
        private Mock<IAccountHandler> _accountHandler;
        private Mock<UrlHelper> _urlHelper;

        [Test]
        public async Task ItShouldReturnAllOfTheSearchItems()
        {
            _accountHandler.Setup(x => x.FindSearchItems()).ReturnsAsync(new List<AccountSearchModel>
            {
                new AccountSearchModel {Account = "VALTECH", AccountID = "ABCDEF"}
            }.AsEnumerable());

            var actual = await _unit.Search();

            Assert.IsNotNull(actual);
            var result = (JsonResult<IEnumerable<AccountSearchModel>>) actual;
            CollectionAssert.IsNotEmpty(result.Content);
        }

        [Test]
        public void ItShouldReturnTheSiteManifest()
        {
            var actual = _unit.Get();

            Assert.IsNotNull(actual);

            var result = (JsonResult<SiteManifest>) actual;
            Assert.IsNotNull(result.Content.Challenges);
            Assert.IsNotNull(result.Content.Resources);
            Assert.IsNotNull(result.Content.BaseUrl);
            Assert.IsNotNull(result.Content.Version);
        }

        [Test]
        public void ItShouldReturnTheSiteManifestContaintResources()
        {
            var actual = _unit.Get();

            Assert.IsNotNull(actual);

            var result = (JsonResult<SiteManifest>) actual;

            Assert.IsNotEmpty(result.Content.Resources);
            Assert.IsNotNull(result.Content.Resources.FirstOrDefault(x => x.ResourceKey == "account/finance"));
            Assert.IsNotNull(result.Content.Resources.FirstOrDefault(x => x.ResourceKey == "account/header"));
            Assert.IsNotNull(result.Content.Resources.FirstOrDefault(x => x.ResourceKey == "account"));
        }

        [Test]
        public void ItShouldReturnTheSiteManifestHavingABaseUrl()
        {
            var actual = _unit.Get();
            Assert.IsNotNull(actual);
            var result = (JsonResult<SiteManifest>) actual;

            Assert.IsNotNull(result.Content.BaseUrl);
        }

        [Test]
        public void ItShouldReturnTheSiteManifestHAvingAVersion()
        {
            var actual = _unit.Get();
            Assert.IsNotNull(actual);
            var result = (JsonResult<SiteManifest>) actual;
            Assert.IsNotNull(result.Content.Version);
        }
    }
}