using Newtonsoft.Json;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EAS.Support.Web.Configuration
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AccountApiConfiguration AccountApi { get; set; }

        [JsonRequired] public SiteValidatorSettings SiteValidator { get; set; }
    }
}