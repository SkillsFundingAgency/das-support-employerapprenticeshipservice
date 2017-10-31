using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Das.Console.Core.Domain.Model;
using SFA.DAS.EAS.Account.Api.Types;

namespace Sfa.Das.Console.ApplicationServices
{
    public interface IAccountRepository
    {
        Task<Account> Get(string id, AccountFieldsSelection selection);
        Task<decimal> GetAccountBalance(string id);
        IEnumerable<AccountDetailViewModel> FindAllDetails();
    }
}
