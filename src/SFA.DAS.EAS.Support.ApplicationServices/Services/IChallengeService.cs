using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public interface IChallengeService
    {
        List<int> GetPayeSchemesCharacters(IEnumerable<PayeSchemeViewModel> payeSchemes);
    }
}