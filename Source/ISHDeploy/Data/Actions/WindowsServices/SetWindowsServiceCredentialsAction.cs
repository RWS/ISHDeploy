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

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Sets windows service credentials.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class SetWindowsServiceCredentialsAction : BaseAction
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
        /// The windows service userName
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// The windows service password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetWindowsServiceCredentialsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">The deployment service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public SetWindowsServiceCredentialsAction(ILogger logger, ISHWindowsService service, string userName, string password)
            : base(logger)
        {
            _service = service;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _serviceManager.SetWindowsServiceCredentials(_service, _userName, _password);
        }
    }
}
