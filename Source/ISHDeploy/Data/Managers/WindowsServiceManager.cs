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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using System.ServiceProcess;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    /// <seealso cref="IWindowsServiceManager" />
    public class WindowsServiceManager : IWindowsServiceManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WindowsServiceManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Starts specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void StartWindowsService(string serviceName)
        {
            using (var service = new ServiceController(serviceName))
            {
                service.Start();
            }
        }
    }
}
