using System.Collections.Generic;

namespace SFA.DAS.EAS.Support.Web.Models
{
    public class ChallengeViewModel
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public List<int> Characters { get; set; }

        public bool HasError { get; set; }
    }
}