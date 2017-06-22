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

using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.Backup;

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Sets windows service credentials.
    /// </summary>
    public class InstallWindowsServiceAction : BaseAction
    {
        /// <summary>
        /// The name of deployment service.
        /// </summary>
        private readonly ISHWindowsServiceBackup _service;

        /// <summary>
        /// The windows service manager
        /// </summary>
        private readonly IWindowsServiceManager _serviceManager;

        /// <summary>
        /// The registry manager
        /// </summary>
        private readonly ITrisoftRegistryManager _registryManager;

        /// <summary>
        /// The pattern of registry path to the "HKEY_LOCAL_MACHINESYSTEM\CurrentControlSet\Services\{0}"
        /// </summary>
        private readonly string _regPathPattern;

        /// <summary>
        /// The windows service userName
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// The windows service password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallWindowsServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">The service.</param>
        /// <param name="regPathPattern">The pattern of registry path to the "HKEY_LOCAL_MACHINESYSTEM\CurrentControlSet\Services\{0}".</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public InstallWindowsServiceAction(ILogger logger, ISHWindowsServiceBackup service, string regPathPattern, string userName, string password)
            : base(logger)
        {
            _service = service;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            _registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            _regPathPattern = regPathPattern;
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _serviceManager.InstallWindowsService(_service, _userName, _password);

            foreach (var prop in _service.RegistryManagerProperties.Properties)
            {
                _registryManager.SetValue(string.Format(_regPathPattern, _service.Name), prop.Name, prop.Value);
            }
        }
    }
}
