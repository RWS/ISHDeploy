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

using System.Collections.Generic;
using ISHDeploy.Common;
using ISHDeploy.Common.Interfaces;
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

            _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Set-COMPlusComponentCredentials.ps1",
                new Dictionary<string, string>
                {
                    { "$name", comPlusComponentName },
                    { "$username", userName },
                    { "$password", password }
                },
                "Setting of COM+ component credentials");

            _logger.WriteVerbose($"Credentials for the COM+ component `{comPlusComponentName}` has been chenged");
        }
    }
}
