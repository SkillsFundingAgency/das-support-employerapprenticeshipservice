﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public interface IAccountRepository
    {
        Task<Core.Models.Account> Get(string id, AccountFieldsSelection selection);
        Task<decimal> GetAccountBalance(string id);
        Task<IEnumerable<Core.Models.Account>> FindAllDetails();
    }
}