/**
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

using ISHDeploy.Data.Exceptions;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    public interface IWebAdministrationManager
    {
        /// <summary>
        /// Recycles specific application pool
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <param name="startIfNotRunning">if set to <c>true</c> then starts application pool if not running.</param>
        void RecycleApplicationPool(string applicationPoolName, bool startIfNotRunning = false);

        /// <summary>
        /// Stops specific application pool
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        void StopApplicationPool(string applicationPoolName);

        /// <summary>
        /// Enables the windows authentication.
        /// </summary>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <exception cref="WindowsAuthenticationModuleIsNotInstalledException"></exception>
        void EnableWindowsAuthentication(string webSiteName);

        /// <summary>
        /// Disables the windows authentication.
        /// </summary>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <exception cref="WindowsAuthenticationModuleIsNotInstalledException"></exception>
        void DisableWindowsAuthentication(string webSiteName);

        /// <summary>
        /// Sets identity type of specific application pool as ApplicationPoolIdentity
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        void SetApplicationPoolIdentityType(string applicationPoolName);

        /// <summary>
        /// Sets identity type of specific application pool as Custom account
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        void SetSpecificUserIdentityType(string applicationPoolName);
    }
}
