using Sfa.Das.Console.ApplicationServices.Responses;
using Sfa.Das.Console.Core.Domain.Model;

namespace Sfa.Das.Console.ApplicationServices.Queries
{
    public class ChallengePermissionQuery
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string ChallengeElement1 { get; set; }

        public string ChallengeElement2 { get; set; }

        public string Balance { get; set; }

        public string FirstCharacterPosition { get; set; }

        public string SecondCharacterPosition { get; set; }
    }
}