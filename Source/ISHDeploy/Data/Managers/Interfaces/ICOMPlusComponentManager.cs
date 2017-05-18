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
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Implements COM+ component management.
    /// </summary>
    public interface ICOMPlusComponentManager
    {
        /// <summary>
        /// Set COM+ component credentials
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>

        void SetCOMPlusComponentCredentials(string comPlusComponentName, string userName, string password);

        /// <summary>
        /// Check COM+ component is enabled or not
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <returns>State of COM+ component</returns>
        bool CheckCOMPlusComponentEnabled(string comPlusComponentName);

        /// <summary>
        /// Enable COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        void EnableCOMPlusComponents(string comPlusComponentName);

        /// <summary>
        /// Disable COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        void DisableCOMPlusComponents(string comPlusComponentName);

        /// <summary>
        /// Gets all COM+ components.
        /// </summary>
        /// <returns>
        /// The list of COM+ components.
        /// </returns>
        IEnumerable<ISHCOMPlusComponent> GetCOMPlusComponents();
    }
}
