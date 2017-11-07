using System.Collections.Generic;
using Sfa.Das.Console.Core.Domain.Model;

namespace Sfa.Das.Console.Web.Models
{
    public class ChallengeViewModel
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public List<int> Characters { get; set; }

        public bool HasError { get; set; }
    }
}