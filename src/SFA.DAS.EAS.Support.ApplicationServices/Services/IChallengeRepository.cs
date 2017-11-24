using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public interface IChallengeRepository
    {
        Task<bool> CheckData(Core.Models.Account record, ChallengePermissionQuery message);
    }
}