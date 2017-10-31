using Sfa.Das.Console.ApplicationServices.Responses;
using Sfa.Das.Console.Core.Domain.Model;

public class AccountDetailOrganisationsResponse
{
    public Account Account { get; set; }
    public SearchResponseCodes StatusCode { get; set; }
}