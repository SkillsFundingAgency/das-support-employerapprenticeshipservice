using Sfa.Das.Console.ApplicationServices.Responses;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models
{
    public class AccountPayeSchemesResponse
    {
        public Sfa.Das.Console.Core.Domain.Model.Account Account { get; set; }
        public SearchResponseCodes StatusCode { get; set; }
    }
}