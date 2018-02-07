using HMRC.ESFA.Levy.Api.Types;
using SFA.DAS.EAS.Account.Api.Types;
using System;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models
{
    public class PayeLevySubmissionsResponse
    {
        public PayeSchemeViewModel PayeScheme { get; set; }
        public PayeLevySubmissionsResponseCodes StatusCode { get; set; }
        public LevyDeclarations LevySubmissions { get; set; }
        public Exception ResponseException  {get; set;}
    }
}
