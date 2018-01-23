using SFA.DAS.EAS.Support.Infrastructure.Settings;

namespace SFA.DAS.EAS.Support.Web.Configuration
{
    public interface IWebConfiguration
    {
        AccountApiConfiguration AccountApi { get; set; }
        SiteConnectorSettings SiteValidator { get; set; }
    }
}