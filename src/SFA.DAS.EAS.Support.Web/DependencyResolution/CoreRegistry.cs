using Sfa.Das.Console.Core.Helpers;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    public class CoreRegistry : Registry
    {
        public CoreRegistry()
        {
            For<IPayeSchemeObfuscator>().Use<PayeSchemeObfuscator>();
        }
    }
}