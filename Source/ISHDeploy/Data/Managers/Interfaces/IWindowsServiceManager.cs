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
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.Backup;

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

        /// <summary>
        /// Gets all BackgroundTask windows services.
        /// </summary>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <returns>
        /// The BackgroundTask windows services of deployment of specified type.
        /// </returns>
        IEnumerable<ISHBackgroundTaskWindowsService> GetISHBackgroundTaskWindowsServices(string deploymentName);

        /// <summary>
        /// Removes specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        void RemoveWindowsService(string serviceName);

        /// <summary>
        /// Clones specific windows service
        /// </summary>
        /// <param name="service">The windows service to be cloned.</param>
        /// <param name="sequence">The sequence of new service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="role">The role of BackgroundTask service.</param>
        string CloneWindowsService(ISHWindowsService service, int sequence, string userName, string password, string role);

        /// <summary>
        /// Creates windows service
        /// </summary>
        /// <param name="service">The windows service to be created.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The name of new service that have been created.
        /// </returns>
        void InstallWindowsService(ISHWindowsServiceBackup service, string userName, string password);

        /// <summary>
        /// Set windows service credentials
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>

        void SetWindowsServiceCredentials(string serviceName, string userName, string password);

        /// <summary>
        /// Check windows service is Started or not
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <returns>
        /// True if the state of windows service is Manual or Auto.
        /// </returns>
        bool IsWindowsServiceStarted(string serviceName);

        /// <summary>
        /// Gets properties of windows service
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <returns>
        /// Properties of windows service.
        /// </returns>
        PropertyCollection GetWindowsServiceProperties(string serviceName);
    }
}
