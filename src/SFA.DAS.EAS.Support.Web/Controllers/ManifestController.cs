﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.Support.Shared;

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
                SearchResultsMetadata = GetSearchResultsMetadata(),
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
                    SearchItemsUrl = "/api/manifest/account"
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
        private IEnumerable<SearchResultMetadata> GetSearchResultsMetadata()
        {
            var accountLink = new LinkDefinition
            {
                Format = "/resource/?key=account&id={0}",
                MapColumnName = nameof(SearchAccountModel.AccountID)
            };

            return new List<SearchResultMetadata>()
            {

               new SearchResultMetadata
                {
                  SearchResultCategory = GlobalConstants.SearchResultCategory,
                  ColumnDefinitions = new List<SearchColumnDefinition>
                  {
                      new SearchColumnDefinition
                      {
                          Name = nameof(SearchAccountModel.Account),
                          Link = accountLink
                      },
                      new SearchColumnDefinition
                      {
                          Name = nameof(SearchAccountModel.Owner),
                      },
                      new SearchColumnDefinition
                      {
                           Name = nameof(SearchAccountModel.AccountID),
                           DisplayName = "Account ID",
                           Link = accountLink
                      }
                  }

                }
            };

        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}