using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Support.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public interface IMapAccountSearch
    {
        SearchItem Map(Core.Models.Account account);
    }
}
