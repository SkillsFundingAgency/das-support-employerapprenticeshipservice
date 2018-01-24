using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.Support.Shared;
using SFA.DAS.Support.Shared.Authentication;
using SFA.DAS.Support.Shared.Discovery;
using SFA.DAS.Support.Shared.SearchIndexModel;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [RoutePrefix("api/manifest")]
    public class ManifestController : ApiController
    {
        private readonly IAccountHandler _handler;

        public ManifestController(IAccountHandler handler)
        {
            _handler = handler;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            var manifest = new SiteManifest
            {
                Version = GetVersion(),
                Resources = GetResources(),
                Challenges = GetChallenges(),
                BaseUrl = Url.Content("~/")
            };

            return Json(manifest);
        }

        [HttpGet]
        [Route("account")]
        public async Task<IHttpActionResult> Search()
        {
            var accounts = await _handler.FindSearchItems();
            return Json(accounts);
        }

        private IEnumerable<SiteResource> GetResources()
        {
            return new List<SiteResource>()
            {
                new SiteResource
                {
                    ResourceTitle = "Organisations",
                    ResourceKey = "account",
                    ResourceUrlFormat = "/account/{0}",
                    SearchItemsUrl = "/api/manifest/account",
                    SearchCategory = SearchCategory.Account
                },
                new SiteResource
                {
                    ResourceTitle = "Finance",
                    ResourceKey = "account/finance",
                    ResourceUrlFormat = "/account/finance/{0}",
                    Challenge = "account/finance"
                },
                new SiteResource
                {
                    ResourceKey = "account/header",
                    ResourceUrlFormat = "/account/header/{0}"
                }
            };
        }

        private IEnumerable<SiteChallenge> GetChallenges()
        {
            return new List<SiteChallenge>(){new SiteChallenge
            {
                ChallengeKey = "account/finance",
                ChallengeUrlFormat = "/challenge/{0}"
            }};
        }
       
        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}