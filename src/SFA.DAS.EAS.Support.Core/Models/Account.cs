using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;

namespace Sfa.Das.Console.Core.Domain.Model
{
    public class Account
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public string DasAccountName { get; set; }
        public DateTime DateRegistered { get; set; }
        public string OwnerEmail { get; set; }
        public IEnumerable<LegalEntityViewModel> LegalEntities { get; set; }
        public IEnumerable<PayeSchemeViewModel> PayeSchemes { get; set; }
        public ICollection<TeamMemberViewModel> TeamMembers { get; set; }
        public IEnumerable<TransactionViewModel> Transactions { get; set; }
    }
}
