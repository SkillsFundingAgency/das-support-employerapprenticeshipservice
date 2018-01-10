using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.Support.Shared;
using SFA.DAS.Support.Shared.SearchIndexModel;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public interface IAccountHandler
    {
        Task<AccountDetailOrganisationsResponse> FindOrganisations(string id);
        Task<AccountPayeSchemesResponse> FindPayeSchemes(string id);
        Task<AccountFinanceResponse> FindFinance(string id);
        Task<IEnumerable<AccountSearchModel>> FindSearchItems();
        Task<AccountReponse> Find(string id);
    }
}