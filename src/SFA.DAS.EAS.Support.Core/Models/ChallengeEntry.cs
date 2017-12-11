using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class ChallengeEntry
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public int Challenge1 { get; set; }

        public int Challenge2 { get; set; }

        public string Balance { get; set; }

        public string FirstCharacterPosition { get; set; }

        public string SecondCharacterPosition { get; set; }
    }
}