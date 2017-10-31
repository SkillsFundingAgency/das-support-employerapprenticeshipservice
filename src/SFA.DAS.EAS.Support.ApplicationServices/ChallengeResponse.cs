using System.Collections.Generic;
using Sfa.Das.Console.Core.Domain.Model;

namespace Sfa.Das.Console.ApplicationServices.Responses
{
    public class ChallengeResponse
    {
        public Account Account { get; set; }

        public List<int> Characters { get; set; }

        public SearchResponseCodes StatusCode { get; set; }
    }
}
