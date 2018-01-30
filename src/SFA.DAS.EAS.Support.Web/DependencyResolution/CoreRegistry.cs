using System;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EAS.Support.Core.Services;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class CoreRegistry : Registry
    {
        public CoreRegistry()
        {
            try
            {
                For<IPayeSchemeObfuscator>().Use<PayeSchemeObfuscator>();
            }
            catch (Exception e)
            {
            }
        }
    }
}