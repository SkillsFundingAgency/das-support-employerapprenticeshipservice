using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DAS.Support.Shared;
using Sfa.Das.Console.ApplicationServices.Responses;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace Sfa.Das.Console.ApplicationServices.Services
{
    public interface IAccountHandler
    {
        Task<AccountDetailOrganisationsResponse> FindOrganisations(string id);
        Task<AccountPayeSchemesResponse> FindPayeSchemes(string id);

        Task<AccountFinanceResponse> FindFinance(string id);
        IEnumerable<SearchItem> FindSearchItems();
    }
}