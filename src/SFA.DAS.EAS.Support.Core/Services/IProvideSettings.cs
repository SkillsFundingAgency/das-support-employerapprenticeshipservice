using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfa.Das.Console.Core.Services
{
    public interface IProvideSettings
    {
        string GetSetting(string settingKey);
        string GetNullableSetting(string settingKey);
    }
}
