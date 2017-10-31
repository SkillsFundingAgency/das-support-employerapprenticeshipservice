using System;

namespace Sfa.Das.Console.ApplicationServices.Services
{
    public interface IDatetimeService
    {
        int GetYear(DateTime endDate);
        DateTime GetBeginningFinancialYear(DateTime endDate);
    }
}