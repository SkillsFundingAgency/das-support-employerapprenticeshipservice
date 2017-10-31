using Sfa.Das.Console.Core.Domain.Model;

namespace Sfa.Das.Console.Web.ViewModels
{
    public class FinanceViewModel
    {
        public Account Account { get; set; }

        public decimal Balance { get; set; }

        public string SearchUrl { get; set; }
    }
}