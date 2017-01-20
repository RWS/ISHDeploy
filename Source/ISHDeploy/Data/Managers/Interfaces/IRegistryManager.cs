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
﻿using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Manager that do all kinds of operations with registry.
    /// </summary>
    public interface IRegistryManager
    {
        /// <summary>
        /// Gets the installed deployments keys.
        /// </summary>
        /// <param name="expectedSuffix">The deployment suffix. If not specified method will return all found deployments.</param>
        /// <returns>List of found deployments</returns>
        IEnumerable<RegistryKey> GetInstalledProjectsKeys(string expectedSuffix = null);

                /// <summary>
        /// Gets the inputparameters.xml file path.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Path to inputparameters.xml file</returns>
        string GetInstallParamFilePath(RegistryKey projectRegKey);

        /// <summary>
        /// Gets the installed deployment version.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Deployment version.</returns>
        Version GetInstalledProjectVersion(RegistryKey projectRegKey);

        /// <summary>
        ///  Sets the specified name/value pair on the specified registry key. If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="keyName">The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".</param>
        /// <param name="valueName">The name of the name/value pair.</param>
        /// <param name="value">The value to be stored.</param>
        void SetRegistryValue(string keyName, string valueName, object value);

        /// <summary>
        /// Gets names of all values of registry key.
        /// </summary>
        /// <param name="localMachineSubKeyName">The registry path of the sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <returns>
        /// The array of names of all values of specified registry key.
        /// </returns>
        string[] GetValueNames(string localMachineSubKeyName);

        /// <summary>
        /// Copies values from one registry key to another.
        /// </summary>
        /// <param name="namesOfValues">The list of names of values that need to be copied.</param>
        /// <param name="sourceLocalMachineSubKeyName">The registry path to source sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <param name="destLocalMachineSubKeyName">The registry path to destination sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        void CopyValues(IEnumerable<string> namesOfValues, string sourceLocalMachineSubKeyName, string destLocalMachineSubKeyName);
    }
}
