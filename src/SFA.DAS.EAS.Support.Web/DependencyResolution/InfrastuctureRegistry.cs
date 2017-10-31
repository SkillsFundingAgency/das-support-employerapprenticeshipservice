using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sfa.Das.Console.ApplicationServices;
using Sfa.Das.Console.Core.Services;
using Sfa.Das.Console.Infrastructure;
using Sfa.Das.Console.Infrastructure.Settings;
using SFA.DAS.EAS.Account.Api.Client;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    public class InfrastuctureRegistry : Registry
    {
        public InfrastuctureRegistry()
        {
            For<IAccountRepository>().Use<AccountRepository>();
            For<IChallengeRepository>().Use<ChallengeRepository>();
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings()));
            For<IAccountApiConfiguration>().Use<AccountApiConfiguration>();

            For<IAccountApiClient>().Use("", (ctx) =>
            {
                var empUserApiSettings = ctx.GetInstance<IAccountApiConfiguration>();
                return new AccountApiClient(empUserApiSettings);
            });
        }
    }
}