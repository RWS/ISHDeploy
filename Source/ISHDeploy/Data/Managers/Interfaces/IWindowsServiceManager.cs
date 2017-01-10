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
using System.ServiceProcess;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Implements windows service management.
    /// </summary>
    public interface IWindowsServiceManager
    {
        /// <summary>
        /// Starts specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        void StartWindowsService(string serviceName);

        /// <summary>
        /// Stops specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        void StopWindowsService(string serviceName);

        /// <summary>
        /// Gets all windows services of specified type.
        /// </summary>
        /// <param name="types">Types of deployment service.</param>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <returns>
        /// The windows services of deployment of specified type.
        /// </returns>
        IEnumerable<ISHWindowsService> GetServices(string deploymentName, params ISHWindowsServiceType[] types);
    }
}
