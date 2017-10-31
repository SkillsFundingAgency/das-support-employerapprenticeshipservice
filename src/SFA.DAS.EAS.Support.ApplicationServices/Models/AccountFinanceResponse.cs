using Sfa.Das.Console.Core.Domain.Model;

namespace Sfa.Das.Console.ApplicationServices.Responses
{
    public class AccountFinanceResponse
    {
        public Account Account { get; set; }

        public decimal Balance { get; set; }

        public SearchResponseCodes StatusCode { get; set; }
    }
}
