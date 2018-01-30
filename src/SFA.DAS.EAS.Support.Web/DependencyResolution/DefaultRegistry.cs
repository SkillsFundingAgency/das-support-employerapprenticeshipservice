// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    using Microsoft.Azure;
    using SFA.DAS.Configuration;
    using SFA.DAS.Configuration.AzureTableStorage;
    using SFA.DAS.EAS.Account.Api.Client;
    using SFA.DAS.EAS.Support.Infrastructure.DependencyResolution;
    using SFA.DAS.EAS.Support.Web.Configuration;
    using SFA.DAS.NLog.Logger;
    using SFA.DAS.Support.Shared.SiteConnection;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Mvc;

    [ExcludeFromCodeCoverage]
    public class DefaultRegistry : Registry {

        private const string ServiceName = "SFA.DAS.Support.EAS";
        private const string Version = "1.0";
      
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });

            
            For<ILoggingPropertyFactory>().Use<LoggingPropertyFactory>();

            HttpContextBase conTextBase = null;
            if (HttpContext.Current != null)
                conTextBase = new HttpContextWrapper(HttpContext.Current);

            For<IWebLoggingContext>().Use(x => new WebLoggingContext(conTextBase));

            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                x.GetInstance<IWebLoggingContext>(),
                x.GetInstance<ILoggingPropertyFactory>().GetProperties())).AlwaysUnique();

            var logger = DependencyResolver.Current.GetService<ILog>();

            try
            {
                logger.Debug($"Starting Configuration");
            
                WebConfiguration configuration = GetConfiguration();
           

                For<IWebConfiguration>().Use(configuration);
                For<IAccountApiConfiguration>().Use(configuration.AccountApi);
                For<ISiteValidatorSettings>().Use(configuration.SiteValidator);
            }
            catch (Exception ex)
            {
                logger.Error(ex,$"Configuration exception occured");
            }


        }

        private WebConfiguration GetConfiguration()
        {
            var environment = CloudConfigurationManager.GetSetting("EnvironmentName") ?? 
                              "LOCAL";
            var storageConnectionString = CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString") ??
                                          "UseDevelopmentStorage=true";

            var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString); ;

            var configurationOptions = new ConfigurationOptions(ServiceName, environment, Version);

            var configurationService = new ConfigurationService(configurationRepository, configurationOptions);

            var webConfiguration = configurationService.Get<WebConfiguration>();

            return webConfiguration;
        }

        #endregion
    }
}