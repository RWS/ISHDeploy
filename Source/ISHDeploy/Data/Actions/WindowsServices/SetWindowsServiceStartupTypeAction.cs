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
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Interfaces.Actions;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Set windows service startyp type (Manual, Automatic (Delayed start), Automatic,...)
    /// </summary>
    class SetWindowsServiceStartupTypeAction : BaseAction
    {
        /// <summary>
        /// The name of deployment service.
        /// </summary>
        private readonly string _serviceName;

        /// <summary>
        /// The windows service manager
        /// </summary>
        private readonly IWindowsServiceManager _serviceManager;

        /// <summary>
        /// The startup type of the service
        /// </summary>
        private readonly ISHWindowsServiceStartupType _startupType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetWindowsServiceCredentialsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">The deployment service</param>
        /// <param name="startupType">The new startup type of the service</param>
        public SetWindowsServiceStartupTypeAction(ILogger logger, ISHWindowsService service, ISHWindowsServiceStartupType startupType)
            : base(logger)
        {
            _serviceName = service.Name;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            _startupType = startupType;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _serviceManager.SetWindowsServiceStartupType(_serviceName, _startupType);

        }
    }
}
