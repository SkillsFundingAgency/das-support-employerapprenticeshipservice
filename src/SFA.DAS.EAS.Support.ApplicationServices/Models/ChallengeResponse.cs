using System.Collections.Generic;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models
{
    public class ChallengeResponse
    {
        public Core.Models.Account Account { get; set; }

        public List<int> Characters { get; set; }

        public SearchResponseCodes StatusCode { get; set; }
    }
}
