﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class AccountDetailViewModel
    {
        public Core.Models.Account Account { get; set; }
        public string SearchUrl { get; set; }
    }
}