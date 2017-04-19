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
using System.Linq;
﻿using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using Microsoft.Win32;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Manager that do all kinds of operations with registry.
    /// </summary>
    /// <seealso cref="ITrisoftRegistryManager" />
    public class TrisoftRegistryManager : ITrisoftRegistryManager
    {
        #region Private constants

        /// <summary>
        /// The project base registry name.
        /// </summary>
        private const string ProjectBaseRegName = "InfoShare";

        /// <summary>
        /// The core registry key name.
        /// </summary>
        private const string CoreRegName = "Core";

        /// <summary>
        /// The current registry key name.
        /// </summary>
        private const string CurrentRegName = "Current";

        /// <summary>
        /// The history registry key name.
        /// </summary>
        private const string HistoryRegName = "History";

        /// <summary>
        /// The install history path registry value name.
        /// </summary>
        private const string InstallHistoryPathRegValue = "InstallHistoryPath";

        /// <summary>
        /// The version registry value name
        /// </summary>
        private const string VersionRegValue = "Version";

        #endregion
        
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The path to of "SOFTWARE\Trisoft"
        /// </summary>
        public string RelativeTrisoftRegPath { get; }

        /// <summary>
        /// The path to of "SOFTWARE\Trisoft\InstallTool"
        /// </summary>
        private readonly string _relativeInstallToolTrisoftRegPath;

        /// <summary>
        /// The path to of "SOFTWARE\Trisoft\InstallTool"
        /// </summary>
        private readonly Dictionary<RegistryValueName, string[]> _pathsToRegistryValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrisoftRegistryManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TrisoftRegistryManager(ILogger logger)
        {
            _logger = logger;
            RelativeTrisoftRegPath = Environment.Is64BitOperatingSystem ? "SOFTWARE\\Wow6432Node\\Trisoft" : "SOFTWARE\\Trisoft";
            _pathsToRegistryValues = new Dictionary<RegistryValueName, string[]>
            {
                { RegistryValueName.DbConnectionString, new []{ $"HKEY_LOCAL_MACHINE\\{RelativeTrisoftRegPath}\\Tridk\\TridkApp\\{{0}}", "Connect" } }
            };
            _relativeInstallToolTrisoftRegPath = $"{RelativeTrisoftRegPath}\\InstallTool";

        }

        /// <summary>
        /// Gets the installed deployments keys.
        /// </summary>
        /// <param name="projectName">The deployment name. If not specified method will return all found deployments.</param>
        /// <returns>List of found deployments</returns>
        public IEnumerable<RegistryKey> GetInstalledProjectsKeys(string projectName = null)
        {
            _logger.WriteDebug("Retrieve registry keys", (string.IsNullOrEmpty(projectName) ? "for all installed projects" : projectName));

            var installedProjectsKeys = new List<RegistryKey>();
            var installToolRegKey = Registry.LocalMachine.OpenSubKey(_relativeInstallToolTrisoftRegPath);

            var projectBaseRegKey = installToolRegKey?.OpenSubKey(ProjectBaseRegName);

            if (projectBaseRegKey == null || projectBaseRegKey.SubKeyCount == 0)
            {
                _logger.WriteWarning("None project base registry keys were found on the system.");
                return installedProjectsKeys;
            }

            string[] projectsKeyNames = projectBaseRegKey.GetSubKeyNames();

            foreach (var name in projectsKeyNames)
            {
                if (name == CoreRegName || (!string.IsNullOrEmpty(projectName) && name != projectName))
                {
                    continue;
                }

                var projRegKey = projectBaseRegKey.OpenSubKey(name);

                var currentValue = projRegKey?.GetValue(CurrentRegName, string.Empty).ToString();

                if (!string.IsNullOrWhiteSpace(currentValue))
                {
                    installedProjectsKeys.Add(projRegKey);
                }
            }

            _logger.WriteVerbose($"Registry keys for {(string.IsNullOrEmpty(projectName) ? "installed projects" : $"project `{projectName}`")} have been retrieved");
            return installedProjectsKeys;
        }

        /// <summary>
        /// Gets the inputparameters.xml file path.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Path to inputparameters.xml file</returns>
        public string GetInstallParamFilePath(RegistryKey projectRegKey)
        {
            _logger.WriteDebug("[Get path to inputparameters.xml]");

            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            var path = historyItem?.GetValue(InstallHistoryPathRegValue).ToString();
            _logger.WriteVerbose($"The path to file inputparameters.xml for {projectRegKey.Name} has been retrieved");
            return path;
        }

        /// <summary>
        /// Gets the installed deployment version.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Deployment version.</returns>
        public Version GetInstalledProjectVersion(RegistryKey projectRegKey)
        {
            _logger.WriteDebug("[Get deployment version]");

            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            var versionStr = historyItem?.GetValue(VersionRegValue).ToString();
            Version version;

            if (string.IsNullOrWhiteSpace(versionStr) || !Version.TryParse(versionStr, out version))
            {
                _logger.WriteWarning($"`{projectRegKey}` registry key does not contain correct `{VersionRegValue}` value");
                return null;
            }

            _logger.WriteVerbose($"The version for installed deployment {projectRegKey.Name} has been retrieved");
            return version;
        }

        /// <summary>
        /// Gets the history folder registry key.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Registry history folder key.</returns>
        private RegistryKey GetHistoryFolderRegKey(RegistryKey projectRegKey)
        {
            var currentInstallvalue = projectRegKey?.GetValue(CurrentRegName).ToString();

            if (string.IsNullOrWhiteSpace(currentInstallvalue))
            {
                _logger.WriteWarning($"`{projectRegKey}` does not contain `{CurrentRegName}` key");
                return null;
            }

            var historyRegKey = projectRegKey.OpenSubKey(HistoryRegName);

            var installFolderRegKey = historyRegKey?.GetSubKeyNames().FirstOrDefault(keyName => keyName == currentInstallvalue);

            if (installFolderRegKey == null)
            {
                _logger.WriteWarning($"`{projectRegKey}` does not contain `{HistoryRegName}` key that named `{currentInstallvalue}`");
                return null;
            }

            return historyRegKey.OpenSubKey(installFolderRegKey);
        }

        /// <summary>
        ///  Sets the specified name/value pair on the specified registry key. If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="registryValueName">The name of the name/value pair</param>
        /// <param name="value">The value to be stored</param>
        /// <param name="parameters">Additional parameters</param>
        public void SetRegistryValue(RegistryValueName registryValueName, object value, params object[] parameters)
        {
            var keyName = string.Format(_pathsToRegistryValues[registryValueName][0], parameters);
            var valueName = _pathsToRegistryValues[registryValueName][1];
            _logger.WriteDebug("Set registry value", keyName, valueName, value);
            Registry.SetValue(keyName, valueName, value);
            _logger.WriteVerbose($"The registry value `{keyName}\\{valueName}` has been set to `{value}`");
        }

        /// <summary>
        ///  Gets the specified value on the specified registry key.
        /// </summary>
        /// <param name="registryValueName">The name of the name/value pair</param>
        /// <param name="parameters">Additional parameters</param>
        public object GetRegistryValue(RegistryValueName registryValueName, params object[] parameters)
        {
            var keyName = string.Format(_pathsToRegistryValues[registryValueName][0], parameters);
            var valueName = _pathsToRegistryValues[registryValueName][1];
            _logger.WriteDebug("Get registry value", keyName, valueName);
            var value = Registry.GetValue(keyName, valueName, null);
            _logger.WriteVerbose($"The registry value `{keyName}\\{valueName}` is `{value}`");
            return value;
        }

        /// <summary>
        /// Gets names of all values of registry key.
        /// </summary>
        /// <param name="localMachineSubKeyName">The registry path of the sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <returns>
        /// The array of names of all values of specified registry key.
        /// </returns>
        public string[] GetValueNames(string localMachineSubKeyName)
        {
            _logger.WriteDebug("Get names of all values of registry key", localMachineSubKeyName);
            var result = Registry.LocalMachine.OpenSubKey(localMachineSubKeyName).GetValueNames();
            _logger.WriteVerbose($"The names of values for registry key `HKEY_LOCAL_MACHINE\\{localMachineSubKeyName}` has been loaded`");
            return result;
        }

        /// <summary>
        /// Copies values from one registry key to another.
        /// </summary>
        /// <param name="namesOfValues">The list of names of values that need to be copied.</param>
        /// <param name="sourceLocalMachineSubKeyName">The registry path to source sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <param name="destLocalMachineSubKeyName">The registry path to destination sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        public void CopyValues(IEnumerable<string> namesOfValues, string sourceLocalMachineSubKeyName, string destLocalMachineSubKeyName)
        {
            _logger.WriteDebug($"Copies values from `{sourceLocalMachineSubKeyName}` registry key to `{destLocalMachineSubKeyName}`");

            var sourceKey = Registry.LocalMachine.OpenSubKey(sourceLocalMachineSubKeyName);
            var destKey = Registry.LocalMachine.OpenSubKey(destLocalMachineSubKeyName, true);
            foreach (var nameOfValue in namesOfValues)
            {
                destKey.SetValue(nameOfValue, sourceKey.GetValue(nameOfValue));
            }
            _logger.WriteVerbose($"The values from `{sourceLocalMachineSubKeyName}` registry has been copied to `{destLocalMachineSubKeyName}``");
        }
    }
}
