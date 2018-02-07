using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public interface IPayeLevySubmissionsHandler
    {
        Task<PayeLevySubmissionsResponse> Handle(string accountId, string payeId);
    }
}