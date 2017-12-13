using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.Support.Shared;

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


            _urlHelper = new Mock<System.Web.Http.Routing.UrlHelper>();
            _unit.Url = _urlHelper.Object;
            _urlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns(@"~\");
        }

        private ManifestController _unit;
        private Mock<IAccountHandler> _accountHandler;
        private Mock<System.Web.Http.Routing.UrlHelper> _urlHelper;

        

        [Test]
        public async Task ItShouldReturnAllOfTheSearchItems()
        {
            _accountHandler.Setup(x => x.FindSearchItems()).ReturnsAsync(new List<SearchItem>
            {
                new SearchItem {Html = "", Keywords = new[] {"", ""}, SearchId = "123"}
            }.AsEnumerable());
            var actual = await _unit.Search();
            CollectionAssert.IsNotEmpty(actual);
        }
        [Test]
        public void ItShouldReturnTheSiteManifest()
        {
            var actual = _unit.Get();
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Challenges);
            Assert.IsNotNull(actual.Resources);
            Assert.IsNotNull(actual.BaseUrl);
            Assert.IsNotNull(actual.Version);
        }

        [Test]
        public void ItShouldReturnTheSiteManifestContaintResources()
        {
            var actual = _unit.Get();
            Assert.IsNotEmpty(actual.Resources);
            Assert.IsNotNull(actual.Resources.FirstOrDefault(x => x.ResourceKey == "account/finance"));
            Assert.IsNotNull(actual.Resources.FirstOrDefault(x => x.ResourceKey == "account/header"));
            Assert.IsNotNull(actual.Resources.FirstOrDefault(x => x.ResourceKey == "account"));

        }

        [Test]
        public void ItShouldReturnTheSiteManifestHavingABaseUrl()
        {
            var actual = _unit.Get();
            Assert.IsNotNull(actual.BaseUrl);
        }

        [Test]
        public void ItShouldReturnTheSiteManifestHAvingAVersion()
        {
            var actual = _unit.Get();
            Assert.IsNotNull(actual.Version);
        }
    }
}