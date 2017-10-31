using System.Threading.Tasks;
using Sfa.Das.Console.ApplicationServices.Queries;
using Sfa.Das.Console.ApplicationServices.Responses;

namespace Sfa.Das.Console.ApplicationServices.Handlers
{
    public interface IChallengeHandler
    {
        Task<ChallengeResponse> Get(string id);
        Task<ChallengePermissionResponse> Handle(ChallengePermissionQuery message);
    }
}