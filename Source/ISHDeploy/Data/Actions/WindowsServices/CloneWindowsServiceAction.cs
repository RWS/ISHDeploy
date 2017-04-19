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

using System.Linq;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Clones an windows service.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class CloneWindowsServiceAction : BaseAction
    {
        /// <summary>
        /// The deployment service.
        /// </summary>
        private readonly ISHWindowsService _service;

        /// <summary>
        /// The windows service manager
        /// </summary>
        private readonly IWindowsServiceManager _serviceManager;

        /// <summary>
        /// The registry manager
        /// </summary>
        private readonly ITrisoftRegistryManager _registryManager;

        /// <summary>
        /// The sequence of new service
        /// </summary>
        private readonly int _sequence;

        /// <summary>
        /// The windows service userName
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// The windows service password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartWindowsServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">The deployment service.</param>
        /// <param name="sequence">The sequence of new service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public CloneWindowsServiceAction(ILogger logger, ISHWindowsService service, int sequence, string userName, string password)
            : base(logger)
        {
            _service = service;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            _registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            _sequence = sequence;
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var newServiceName =_serviceManager.CloneWindowsService(_service, _sequence, _userName, _password);

            var namesOfValues = _registryManager.GetValueNames($@"SYSTEM\CurrentControlSet\Services\{_service.Name}").Where(x => x != "Description" && x != "DisplayName" && x != "ImagePath");
            _registryManager.CopyValues(namesOfValues, $@"SYSTEM\CurrentControlSet\Services\{_service.Name}", $@"SYSTEM\CurrentControlSet\Services\{newServiceName}");

            if (_service.Status == ISHWindowsServiceStatus.Running)
            {
                _serviceManager.StartWindowsService(newServiceName);
            }
        }
    }
}
