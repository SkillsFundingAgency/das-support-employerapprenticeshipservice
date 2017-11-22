using System;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services
{
    public interface IDatetimeService
    {
        int GetYear(DateTime endDate);
        DateTime GetBeginningFinancialYear(DateTime endDate);
    }
}