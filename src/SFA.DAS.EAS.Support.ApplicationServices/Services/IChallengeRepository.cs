using System.Threading.Tasks;
using Sfa.Das.Console.ApplicationServices.Queries;
using Sfa.Das.Console.Core.Domain.Model;

public interface IChallengeRepository
{
    Task<bool> CheckData(Account record, ChallengePermissionQuery message);
}