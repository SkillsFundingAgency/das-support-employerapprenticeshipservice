using System.Diagnostics.CodeAnalysis;
using System.Web;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.NLog.Logger;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class InfrastuctureRegistry : Registry
    {
        public InfrastuctureRegistry()
        {
            For<ILoggingPropertyFactory>().Use<LoggingPropertyFactory>();

            HttpContextBase conTextBase = null;
            if (HttpContext.Current != null)
                conTextBase = new HttpContextWrapper(HttpContext.Current);

            For<IWebLoggingContext>().Use(x => new WebLoggingContext(conTextBase));

            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                x.GetInstance<IWebLoggingContext>(),
                x.GetInstance<ILoggingPropertyFactory>().GetProperties())).AlwaysUnique();


            For<IAccountRepository>().Use<AccountRepository>();
            For<IChallengeRepository>().Use<ChallengeRepository>();
            For<IProvideSettings>().Use(c => new AppConfigSettingsProvider(new MachineSettings()));
            For<IAccountApiConfiguration>().Use<AccountsApiConfiguration>();

            For<IAccountApiClient>().Use("", ctx =>
            {
                var empUserApiSettings = ctx.GetInstance<IAccountApiConfiguration>();
                return new AccountApiClient(empUserApiSettings);
            });
        }
    }
}