namespace SFA.DAS.EAS.Support.Web.Models
{
    public class FinanceViewModel
    {
        public Core.Models.Account Account { get; set; }

        public decimal Balance { get; set; }

        public string SearchUrl { get; set; }
    }
}