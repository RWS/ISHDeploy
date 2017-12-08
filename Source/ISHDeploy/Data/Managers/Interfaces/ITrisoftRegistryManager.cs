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
﻿using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Common.Models.Backup;
﻿using Microsoft.Win32;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Manager that do all kinds of operations with registry.
    /// </summary>
    public interface ITrisoftRegistryManager
    {
        /// <summary>
        /// The path to of "SOFTWARE\Trisoft"
        /// </summary>
        string RelativeTrisoftRegPath { get; }

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
        /// Gets the specified value on the specified registry key.
        /// </summary>
        /// <param name="keyName">The registry key name.</param>
        /// <param name="valueName">The name of value.</param>
        object GetRegistryValue(string keyName, RegistryValueName valueName);

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

        /// <summary>
        /// Gets properties from one registry key
        /// </summary>
        /// <param name="namesOfValues">The list of names of values that need to be copied.</param>
        /// <param name="sourceLocalMachineSubKeyName">The registry path to source sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <returns>
        /// Properties from one registry key.
        /// </returns>
        PropertyCollection GetValues(IEnumerable<string> namesOfValues, string sourceLocalMachineSubKeyName);

        /// <summary>
        /// Gets value from one registry key
        /// </summary>
        /// <param name="keyName">The name of registry key.</param>
        /// <param name="valueName">The name of value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Value from one registry key.
        /// </returns>
        object GetValue(string keyName, string valueName, object defaultValue);

        /// <summary>
        /// Sets value in registry key
        /// </summary>
        /// <param name="keyName">The registry key name.</param>
        /// <param name="valueName">The name of value.</param>
        /// <param name="value">The value.</param>
        void SetValue(string keyName, RegistryValueName valueName, object value);

        /// <summary>
        /// Sets value in registry key
        /// </summary>
        /// <param name="keyName">The registry key name.</param>
        /// <param name="valueName">The name of value.</param>
        /// <param name="value">The value.</param>
        void SetValue(string keyName, string valueName, object value);
    }
}
