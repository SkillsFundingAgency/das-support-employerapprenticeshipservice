using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.Support.Shared;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public class AccountSearchMapper : IMapAccountSearch
    {
        public SearchItem Map(Core.Models.Account account)
        {
            var keywords = new List<string>
            {
                account.HashedAccountId,
                account.DasAccountName,
                account.OwnerEmail
            };

            keywords.AddRange(account.LegalEntities.Select(x =>x.Name));

            var searchModel = new SearchAccountModel
            {
                Account = account.DasAccountName,
                AccountID = account.HashedAccountId,
                Owner = account.OwnerEmail
            };
            
            return new SearchItem
            {
                SearchId = account.HashedAccountId,
                Keywords = keywords.Where(x => x != null).ToArray(),
                SearchResultJson = JsonConvert.SerializeObject(searchModel),
                SearchResultCategory = GlobalConstants.SearchResultCategory
            };


        }
    }
}
