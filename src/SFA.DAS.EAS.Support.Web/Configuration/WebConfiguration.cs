using Newtonsoft.Json;
using SFA.DAS.EAS.Support.Infrastructure.Settings;

namespace SFA.DAS.EAS.Support.Web.Configuration
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public AccountApiConfiguration AccountApi { get;set; }
        [JsonRequired]
        
        public SiteConnectorSettings SiteValidator { get; set; }
    }
}