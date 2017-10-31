using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfa.Das.Console.Core.Services;

namespace Sfa.Das.Console.Infrastructure.Settings
{
    public sealed class MachineSettings : IProvideSettings
    {
        public string GetSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable($"DAS_{settingKey.ToUpper(CultureInfo.InvariantCulture)}", EnvironmentVariableTarget.User);
        }

        public string GetNullableSetting(string settingKey)
        {
            return GetSetting(settingKey);
        }
    }
}
