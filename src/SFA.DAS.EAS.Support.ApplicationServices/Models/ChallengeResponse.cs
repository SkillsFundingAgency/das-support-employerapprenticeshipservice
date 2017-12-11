﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models
{
    [ExcludeFromCodeCoverage]
    public class ChallengeResponse
    {
        public Core.Models.Account Account { get; set; }

        public List<int> Characters { get; set; }

        public SearchResponseCodes StatusCode { get; set; }
    }
}