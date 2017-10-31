﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using ESFA.DAS.Support.Shared;
using Sfa.Das.Console.ApplicationServices.Services;

namespace SubSite.Web.Controllers
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
        public SiteManifest Get()
        {
            return new SiteManifest
            {
                Version = GetVersion(),
                Resources = GetResources(),
                Challenges = GetChallenges(),
                BaseUrl = Url.Content("~/")
            };
        }

        private IEnumerable<SiteChallenge> GetChallenges()
        {
            yield return new SiteChallenge
            {
                ChallengeKey = "account/finance",
                ChallengeUrlFormat = "/account/challenge/{0}"
            };
        }

        [HttpGet]
        [Route("account")]
        public IEnumerable<SearchItem> Search()
        {
            return _handler.FindSearchItems();
        }

        private IEnumerable<SiteResource> GetResources()
        {
            yield return new SiteResource
            {
                ResourceTitle = "Organisations",
                ResourceKey = "account",
                ResourceUrlFormat = "/account/{0}",
                SearchItemsUrl = "/api/manifest/account"
            };

            yield return new SiteResource
            {
                ResourceTitle = "Finance",
                ResourceKey = "account/finance",
                ResourceUrlFormat = "/account/finance/{0}",
                Challenge = "account/finance"
            };

            yield return new SiteResource
            {
                ResourceKey = "account/header",
                ResourceUrlFormat = "/account/header/{0}"
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
