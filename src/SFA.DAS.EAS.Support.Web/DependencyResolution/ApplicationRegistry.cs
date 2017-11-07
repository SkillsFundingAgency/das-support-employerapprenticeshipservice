using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sfa.Das.Console.ApplicationServices.Services;
using SFA.DAS.EAS.Support.ApplicationServices;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry()
        {
            For<IAccountHandler>().Use<AccountHandler>();
            For<IChallengeRepository>().Use<ChallengeRepository>();
            For<IChallengeService>().Use<ChallengeService>();
            For<IDatetimeService>().Use<DatetimeService>();
            For<IChallengeHandler>().Use<ChallengeHandler>();
        }
    }
}