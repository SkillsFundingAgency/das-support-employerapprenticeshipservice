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
using SFA.DAS.EAS.Support.Infrastructure.DependencyResolution;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    public class InfrastuctureRegistry : Registry
    {
        public InfrastuctureRegistry()
        {

            For<ILoggingPropertyFactory>().Use<LoggingPropertyFactory>();

            HttpContextBase conTextBase = null;
            if (HttpContext.Current != null)
            {
                conTextBase = new HttpContextWrapper(HttpContext.Current);
            }
            
            For<IWebLoggingContext>().Use(x => new WebLoggingContext(conTextBase));

            For<ILog>().Use(x => new NLogLogger(
                 x.ParentType,
                 x.GetInstance<SFA.DAS.NLog.Logger.IWebLoggingContext>(),
                 x.GetInstance<ILoggingPropertyFactory>().GetProperties())).AlwaysUnique();


            For<IAccountRepository>().Use<AccountRepository>();
            For<IChallengeRepository>().Use<ChallengeRepository>();
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings()));
            For<IAccountApiConfiguration>().Use<Infrastructure.Settings.AccountsApiConfiguration>();

            For<IAccountApiClient>().Use("", (ctx) =>
            {
                var empUserApiSettings = ctx.GetInstance<IAccountApiConfiguration>();
                return new AccountApiClient(empUserApiSettings);
            });
        }
    }
}