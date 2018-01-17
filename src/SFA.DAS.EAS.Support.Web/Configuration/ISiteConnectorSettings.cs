namespace SFA.DAS.EAS.Support.Web
{
    public interface ISiteConnectorSettings
    {
        string Tenant { get; set; }
        string Audience { get; set; }
        string Scope { get; set; }
    }
}