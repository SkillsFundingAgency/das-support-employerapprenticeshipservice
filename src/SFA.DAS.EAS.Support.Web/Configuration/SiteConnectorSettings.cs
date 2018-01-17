namespace SFA.DAS.EAS.Support.Web
{
    public class SiteConnectorSettings : ISiteConnectorSettings
    {
        public string Tenant { get; set; }
        public string Audience { get; set; }
        public string Scope { get; set; }
    }
}