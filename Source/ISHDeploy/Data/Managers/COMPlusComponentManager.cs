/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using COMAdmin;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    /// <seealso cref="ICOMPlusComponentManager" />
    public class COMPlusComponentManager : ICOMPlusComponentManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The powershell manager.
        /// </summary>
        private readonly IPowerShellManager _psManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public COMPlusComponentManager(ILogger logger)
        {
            _logger = logger;
            _psManager = ObjectFactory.GetInstance<IPowerShellManager>();
        }
        
        /// <summary>
        /// Set COM+ component credentials
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public void SetCOMPlusComponentCredentials(string comPlusComponentName, string userName, string password)
        {
            _logger.WriteDebug("Set COM+ component credentials", comPlusComponentName);

            var comAdminCatalog = (COMAdminCatalog)Activator.CreateInstance(Type.GetTypeFromProgID("COMAdmin.COMAdminCatalog.1"));

            var applications = comAdminCatalog.GetCollection("Applications");
            applications.Populate();
            foreach (ICatalogObject applicationInstance in applications)
            {
                if (string.Equals(applicationInstance.Name, comPlusComponentName,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    applicationInstance.set_Value("Identity", userName);
                    applicationInstance.set_Value("Password", password);
                }
                
            }
            applications.SaveChanges();

            _logger.WriteVerbose($"Credentials for the COM+ component `{comPlusComponentName}` has been chenged");
        }

        /// <summary>
        /// Check COM+ component is enabled or not
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <returns>State of COM+ component</returns>
        public bool CheckCOMPlusComponentEnabled(string comPlusComponentName)
        {
            _logger.WriteDebug("Check COM+ component is enabled or not", comPlusComponentName);
            var result = _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Check-COMPlusComponentEnabled.ps1",
                new Dictionary<string, string>
                {
                    { "$name", comPlusComponentName },
                },
                "Checking of COM+ component is enabled or not");

            var isEnabled = bool.Parse(result.ToString());

            _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` is {(isEnabled ? "Enabled" : "Disabled")}");

            return isEnabled;
        }

        /// <summary>
        /// Enable COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void EnableCOMPlusComponents(string comPlusComponentName)
        {
            _logger.WriteDebug("Enable COM+ component");
            var result = _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Enable-COMPlusComponent.ps1",
                new Dictionary<string, string>
                {
                    { "$name", comPlusComponentName },
                },
                $"Enabling of COM+ component `{comPlusComponentName}`");

            var count = int.Parse(result.ToString());

            if (count == 1)
            {
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` has been enabled");
            }
        }

        /// <summary>
        /// Disable COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void DisableCOMPlusComponents(string comPlusComponentName)
        {
            _logger.WriteDebug("Disable COM+ component");
            var result = _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Disable-COMPlusComponent.ps1",
                new Dictionary<string, string>
                {
                    { "$name", comPlusComponentName },
                },
                $"Disabling of COM+ component `{comPlusComponentName}`");

            var count = int.Parse(result.ToString());

            if (count == 1)
            {
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` has been disabled");
            }
        }

        /// <summary>
        /// Gets all COM+ components.
        /// </summary>
        /// <returns>
        /// The list of COM+ components.
        /// </returns>
        public IEnumerable<ISHCOMPlusComponent> GetCOMPlusComponents()
        {
            _logger.WriteDebug("Get COM+ components");
            var results = _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Get-COMPlusComponents.ps1");

            var components = (from result in (List<object>) results select new ISHCOMPlusComponent {Name = result.ToString()}).ToList();

            foreach (var component in components)
            {
                component.Status = CheckCOMPlusComponentEnabled(component.Name)
                    ? ISHCOMPlusComponentStatus.Enabled
                    : ISHCOMPlusComponentStatus.Disabled;
            }

            return components;
        }
    }
}
