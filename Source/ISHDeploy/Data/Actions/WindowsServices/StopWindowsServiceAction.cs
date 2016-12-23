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

using System.ServiceProcess;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
﻿using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Stops an windows service.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class StopWindowsServiceAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The windows service name.
        /// </summary>
        private readonly string _serviceName;

        /// <summary>
        /// The status of the service.
        /// </summary>
        private ServiceControllerStatus _previousServiceStatus;

        /// <summary>
        /// The windows service manager
        /// </summary>
        private readonly IWindowsServiceManager _serviceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartWindowsServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceName">Name of the windows service.</param>
        public StopWindowsServiceAction(ILogger logger, string serviceName)
            : base(logger)
        {
            _serviceName = serviceName;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _serviceManager.StopWindowsService(_serviceName);
        }

        /// <summary>
        /// Creates backup of the asset.
        /// </summary>
        public void Backup()
        {
            _previousServiceStatus = _serviceManager.GetWindowsServiceStatus(_serviceName);
        }

        /// <summary>
        /// Reverts an asset to initial state.
        /// </summary>
        public void Rollback()
        {
            if (_previousServiceStatus == ServiceControllerStatus.Running)
            {
                _serviceManager.StartWindowsService(_serviceName);
            }
        }
    }
}
