using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.TokenService.Api.Client;
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

            For<IAccountApiClient>().Use<AccountApiClient>();

            For<IAccountApiClient>().Use<AccountApiClient>();

            For<ITokenServiceApiClientConfiguration>().Use(string.Empty, (ctx) =>
            {
                var levySettings = ctx.GetInstance<IWebConfiguration>().LevySubmission.LevySubmissionsApiConfig;
                return levySettings;
            });

            For<IApprenticeshipLevyApiClient>().Use("", (ctx) =>
            {
                var levySubmissionsApiConfiguration = ctx.GetInstance<ITokenServiceApiClientConfiguration>();
                var hmrcApiBaseUrl = ctx.GetInstance<IWebConfiguration>().LevySubmission.HmrcApiBaseUrl;
                var httpClient = GetLevyHttpClient(levySubmissionsApiConfiguration, hmrcApiBaseUrl);

                return new ApprenticeshipLevyApiClient(httpClient);
            });

        }

        private HttpClient GetLevyHttpClient(ITokenServiceApiClientConfiguration levySubmissionsApiConfiguration, string hmrcApiBaseUrl)
        {
            var tokenService = new TokenServiceApiClient(levySubmissionsApiConfiguration);
            var tokenResult = tokenService.GetPrivilegedAccessTokenAsync().Result;
            return ApprenticeshipLevyApiClient.CreateHttpClient(tokenResult.AccessCode, hmrcApiBaseUrl);
        }

    }
}